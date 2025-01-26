using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using SlaveBackend.Models;
using SlaveBackend.Services;

namespace SlaveBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHostInfo()
        {
            OSHelper _os = new OSHelper();
            Hostinfo hi;
     
                hi = new Hostinfo
                {
                    MachineName = _os.GetMachineName(),
                    OSDescription = _os.GetOSDescription(),
                    OSArchitecture = _os.GetArchitecture(),
                    ProcessorCount = _os.GetProcCount(),
                    Framework = _os.GetFramework(),
                    DriveCount = _os.GetDriveCount(),
                    TotalMemory = _os.GetTotalMemory()
                };

            if (hi != null)
            {
                return Ok(hi);
            }
            else
            {
                return BadRequest("");
            }
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new { status = "Healthy" });
        }
    }
}
