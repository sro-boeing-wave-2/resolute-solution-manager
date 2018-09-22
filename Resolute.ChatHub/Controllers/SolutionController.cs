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
