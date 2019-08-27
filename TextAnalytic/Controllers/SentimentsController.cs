using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TextAnalytic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentsController : Controller
    {
        private string _apikey;
        private string _endpoint;

        public SentimentsController(IOptions<CognitiveService> settings)
        {
            _apikey = settings.Value.Apikey;
            _endpoint = settings.Value.Endpoint;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            TextAnalytic textAnalytic = new TextAnalytic(_apikey, _endpoint);
            textAnalytic.Run();
            return new string[] { "value1", "value2" };
        }

        public IActionResult Index()
        {
            TextAnalytic textAnalytic = new TextAnalytic(_apikey,_endpoint);
            return View();
        }
    }
}