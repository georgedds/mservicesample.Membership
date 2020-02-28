using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace mservicesample.Membership.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET api/values
        [HttpGet("Check")]
        public ActionResult<IEnumerable<string>> Check()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
