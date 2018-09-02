using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BotHost.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class VkController : ControllerBase
    {
        [HttpPost("vkapi")]
        public async Task<IActionResult> Validation([FromBody] object content)
        {
            var request = content.ToString();
            Console.WriteLine(request);
            return Ok("ok");
        }
    }
}