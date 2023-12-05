using Microsoft.AspNetCore.Mvc;

namespace RegnalHome.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CAController : ControllerBase
    {
        [HttpGet]
        public FileContentResult Get()
        {
            var myfile = System.IO.File.ReadAllBytes("regnalCA.cer");
            return new FileContentResult(myfile, "application/x-x509-user-cert");
        }
    }
}
