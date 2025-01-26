using Microsoft.AspNetCore.Mvc;

namespace SlaveBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController : ControllerBase
    {
        private readonly GameController _registry;

        public ServerController(GameController registry)
        {
            _registry = registry;
        }

        [HttpPost("/start")]
        public IActionResult StartServer(Guid id)
        {
            // Check if the instance exists
            //if (!_registry.TryGetValue(id, out var instance))
            //    return NotFound($"Game instance with ID '{id}' not found.");

            // Check if the server is already running
            //if (instance.IsRunning)
            //    return Conflict($"Game server with ID '{id}' is already running.");

            // Mark the instance as running (simulated start)
            //instance.IsRunning = true;

            // Simulate starting the game server
            Console.WriteLine($"Starting server for instance: {id}");
            return Ok($"Game server '{id}' started successfully.");
        }

        [HttpPost("{gameName}/stop")]
        public IActionResult StopServer(Guid id)
        {
            Console.WriteLine("bye");
            //if (!_registry.IsRunning(id))
            //    return NotFound($"Game server '{id}' is not running.");

            _registry.StopInstance(id);
            return Ok($"Game server '{id}' stopped.");
        }

        [HttpPost("{gameName}/restart")]
        public IActionResult RestartServer(Guid id)
        {
            //if (!_registry.IsRunning(id))
            //    return NotFound($"Game server '{id}' is not running.");

            _registry.StopInstance(id);
            _registry.StartInstance(id);
            return Ok($"Game server '{id}' restarted.");
        }
    }
}
