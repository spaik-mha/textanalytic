using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace TextAnalytic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentsController : Controller
    {
        private readonly string _apikey;
        private readonly string _endpoint;
        private readonly string connection;

        public SentimentsController(IOptions<CognitiveService> settings)
        {
            _apikey = settings.Value.Apikey;
            _endpoint = settings.Value.Endpoint;
            connection = settings.Value.Connection;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            TextAnalytic textAnalytic = new TextAnalytic(_apikey, _endpoint, connection);
            return textAnalytic.Run();
        }

        public IActionResult Index()
        {
            TextAnalytic textAnalytic = new TextAnalytic(_apikey, _endpoint, connection);
            return View();
        }
    }
}