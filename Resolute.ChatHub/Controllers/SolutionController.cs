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

                foreach(var e in solutionTemplate.Tasks as List<object>)
                {
                    var data = JObject.Parse(JsonConvert.SerializeObject(e));

                    if (data["stage"].ToString() == "action")
                    {
                        var values = data.ToObject<Dictionary<string, object>>();
                        values.Remove("stage");
                        actions.Add(values);
                    }
                }

                dynamic scriptobj = new { hosts = "localhost", gather_facts = false, tasks = actions };
                List<dynamic> final = new List<dynamic>() { scriptobj };
                Console.WriteLine(JsonConvert.SerializeObject(final));

                
                var serializer = new YamlDotNet.Serialization.Serializer();
                //using (var sw = new StringWriter())
                //{
                //    serializer.Serialize(sw, actions);
                //    var yaml = sw.ToString();
                //    Console.WriteLine(sw);
                //}
                string yaml = serializer.Serialize(final);
                Console.Write(yaml);


                var solutionTemplateAsJsonString = JsonConvert.SerializeObject(solutionTemplate);
                var solutionTemplateAsBsonDocument = BsonDocument.Parse(solutionTemplateAsJsonString);
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
