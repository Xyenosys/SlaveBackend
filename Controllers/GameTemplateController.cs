using Microsoft.AspNetCore.Mvc;
using SlaveBackend.Models;
using SlaveBackend.Services;

namespace SlaveBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameTemplateController : ControllerBase
    {
        private readonly GameTemplateService _service;

        public GameTemplateController(GameTemplateService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllGameTemplates()
        {
            var templates = _service.GetAllGameTemplates();
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public IActionResult GetGameTemplateById(int id)
        {
            var template = _service.GetGameTemplateById(id);
            if (template == null)
                return NotFound("Game template not found.");

            return Ok(template);
        }

        [HttpPost]
        public IActionResult AddGameTemplate([FromBody] GameTemplate gameTemplate)
        {
            _service.AddGameTemplate(gameTemplate);
            return CreatedAtAction(nameof(GetGameTemplateById), new { id = gameTemplate.Id }, gameTemplate);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGameTemplate(int id, [FromBody] GameTemplate updatedTemplate)
        {
            var existingTemplate = _service.GetGameTemplateById(id);
            if (existingTemplate == null)
                return NotFound("Game template not found.");

            _service.UpdateGameTemplate(id, updatedTemplate);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGameTemplate(int id)
        {
            var existingTemplate = _service.GetGameTemplateById(id);
            if (existingTemplate == null)
                return NotFound("Game template not found.");

            _service.DeleteGameTemplate(id);
            return NoContent();
        }
    }
}
