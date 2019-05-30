using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetResponse(
            [FromQuery] string name)
        {
            return await Task.Run(() => Ok("Hello World!"));
        }
    }
}