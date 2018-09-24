using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Resolute.ChatHub.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using System.Reflection;
using Resolute.ChatHub.Models;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Newtonsoft.Json.Converters;


namespace Resolute.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolutionController: ControllerBase
    {

        public ISolutionService _service;
        public SolutionController(ISolutionService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("{intent}")]
        public IActionResult GetSolutionTemplatesByIntent(string intent)
        {
            Console.WriteLine(intent);
            var solutions = _service.GetSolutionsByIntentAsync(intent);
            return Ok(solutions);
        }

        [HttpPost]
        public async Task<IActionResult> PostSolutionTemplate([FromBody] SolutionTemplateViewModel solutionTemplateViewModel)
        {
            try
            {
                var deserializer = new Deserializer();
                var solutionTemplate = new SolutionTemplate();
                solutionTemplate.Intent = solutionTemplateViewModel.Intent;
                solutionTemplate.Tasks = deserializer.Deserialize(new StringReader(solutionTemplateViewModel.Tasks.ToString()));

                List<dynamic> actions = new List<dynamic>();

                foreach(var e in solutionTemplate.Tasks as List<dynamic>)
                {
                    Guid g = Guid.NewGuid();
                    string GuidString = Convert.ToBase64String(g.ToByteArray());
                    GuidString = GuidString.Replace("=", "");
                    GuidString = GuidString.Replace("+", "");

                    if (e["stage"] == "action")
                        e["tags"] = new List<string> { GuidString };

                    var data = JObject.Parse(JsonConvert.SerializeObject(e));
                    //Console.WriteLine(data);

                    if (data["stage"].ToString() == "action")
                    {
                        var values = data.ToObject<Dictionary<string, object>>();
                        values.Remove("stage");
                        actions.Add(values);

                        try {
                            if (values["register"] != null)
                            {
                                object redisobj = new { name = "Store gitdata in redis", shell = "redis-cli HSET ${{threadId}} gitdata {{" + values["register"] + ".content}}", tags = values["tags"] };
                                actions.Add(redisobj);
                            }
                        }
                        catch(Exception ex){}
                    }
                }
                Console.WriteLine(JsonConvert.SerializeObject(solutionTemplate.Tasks));

                object scriptobj = new { hosts = "localhost", gather_facts = false, tasks = actions };
                List<object> final = new List<object>() { scriptobj };
                var finalJson = JsonConvert.SerializeObject(final);
                
                var expConverter = new ExpandoObjectConverter();
                dynamic desiralizeObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(finalJson, expConverter);

                var serializer = new YamlDotNet.Serialization.Serializer();
                string yaml = serializer.Serialize(desiralizeObject);
                //Console.Write(solutionTemplate.Intent);
                solutionTemplate.Actions = yaml;

                var solutionTemplateAsJsonString = JsonConvert.SerializeObject(solutionTemplate);
                var solutionTemplateAsBsonDocument = BsonDocument.Parse(solutionTemplateAsJsonString);
                Console.WriteLine(solutionTemplateAsBsonDocument);
                await _service.CreateSolution(solutionTemplateAsBsonDocument);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
