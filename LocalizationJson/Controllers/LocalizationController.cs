using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Localization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocalizationController : ControllerBase
    {
        private readonly ILogger<LocalizationController> _logger;
        private readonly IStringLocalizer<LocalizationController> _localizer;

        public LocalizationController(ILogger<LocalizationController> logger, IStringLocalizer<LocalizationController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var message = _localizer["hi"].ToString();
            return Ok(message);
        }

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            var message = string.Format(_localizer["welcome"], name);
            return Ok(message);
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var message = _localizer.GetAllStrings();
            return Ok(message);
        }
    }
}