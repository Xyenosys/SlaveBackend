using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SlaveBackend.Models;
using System.Collections.Concurrent;
using YamlDotNet.Core.Tokens;

namespace SlaveBackend.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, GameTemplate> _templates = new();
        private static readonly ConcurrentDictionary<Guid, GameInstance> _instances = new();

        #region Game Templates

        [HttpPost("templates/add")]
        public IActionResult AddGameTemplate([FromBody] GameTemplate template)
        {
            if (_templates.ContainsKey(template.Game.Name!))
                return BadRequest("Template with this name already exists.");

            _templates[template.Game.Name!] = template;
            return Ok("Template added successfully.");
        }

        [HttpGet("templates")]
        public IActionResult ListTemplates()
        {
            return Ok(_templates.Values);
        }

        [HttpDelete("templates/remove/{name}")]
        public IActionResult RemoveTemplate(string name)
        {
            if (_templates.TryRemove(name, out _))
                return Ok("Template removed successfully.");
            return NotFound("Template not found.");
        }

        #endregion

        #region Game Instances

        [HttpPost("instances/start/{id}")]
        public IActionResult StartInstance(Guid id, [FromBody] Dictionary<string, object>? overrides = null)
        {
            // Check if the instance exists
            if (!_instances.TryGetValue(id, out var instance))
                return NotFound("Instance not found.");

            // Update the instance arguments based on overrides
            foreach (var update in overrides ?? new Dictionary<string, object>())
            {
                if (instance.Arguments.ContainsKey(update.Key))
                {
                    instance.Arguments[update.Key].Default = update.Value; // Update the argument value
                }
            }

            // Here you would normally start the game server process using the updated arguments.
            // Simulate starting the instance
            return Ok(new { instance.Id, Message = "Game instance started successfully with updated configuration." });
        }

        [HttpGet("instances")]
        public IActionResult ListInstances()
        {
            return Ok(_instances.Values);
        }

        [HttpGet("instances/{id}")]
        public IActionResult GetInstance(Guid id)
        {
            if (_instances.TryGetValue(id, out var instance))
                return Ok(instance);
            return NotFound("Instance not found.");
        }

        [HttpGet("instances/status/{id}")]
        public IActionResult isrunning(Guid id)
        {
            bool isrunning = _instances.TryGetValue(id, out var instance);
            if (isrunning)
                return Ok("The server is running.");
            return Problem("Server is not running!");               
        }

        [HttpPut("instances/{id}/update")]
        public IActionResult UpdateInstanceArguments(Guid id, [FromBody] Dictionary<string, object> updates)
        {
            if (!_instances.TryGetValue(id, out var instance))
                return NotFound("Instance not found.");

            foreach (var update in updates)
            {
                if (instance.Arguments.ContainsKey(update.Key))
                {
                    instance.Arguments[update.Key].Default = update.Value;
                }
            }

            return Ok("Instance arguments updated successfully.");
        }

        [HttpPost("instances/stop/{id}")]
        public IActionResult StopInstance(Guid id)
        {
            if (!_instances.ContainsKey(id))
                return NotFound("Instance not found.");

            _instances.TryRemove(id, out _);
            return Ok("Game instance stopped successfully.");
        }

        [HttpDelete("instances/remove/{id}")]
        public IActionResult RemoveInstance(Guid id)
        {
            if (_instances.TryRemove(id, out _))
                return Ok("Instance removed successfully.");
            return NotFound("Instance not found.");
        }

        #endregion
    }
}
