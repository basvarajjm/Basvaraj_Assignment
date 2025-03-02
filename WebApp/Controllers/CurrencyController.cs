using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;

namespace WebApp.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrenyService _currenyService;
        public CurrencyController(ILogger<CurrencyController> logger, ICurrenyService currenyService)
        {
            _logger = logger;
            _currenyService = currenyService;
        }
        // GET: CurrencyController/Get/USD
        public ActionResult GetDKKEquivalent([FromRoute]string currency, [FromQuery]long value)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return new BadRequestObjectResult("Currency parameter is required.");
            }
            long result;
            try
            {
                // service call
                result = _currenyService.GetDKKEquivalentOf(currency, value);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return HandleException(e);
            }
            return Ok(result);
        }

        private ActionResult HandleException(Exception exception)
        {
            return exception switch
            {
                OverflowException ex => ReturnInternalError(),
                Exception ex => ReturnInternalError()
            };
        }

        private ObjectResult ReturnInternalError()
            => StatusCode(500, "Something went wrong.");

    }
}
