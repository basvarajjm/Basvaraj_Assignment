using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Exceptions;
using WebApp.Interfaces;

namespace WebApp.Controllers
{
    /// <summary>
    /// Currency Conversion
    /// </summary>
    public class CurrencyController : Controller
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrenyConversionService _currenyService;
        /// <summary>
        /// Currency Conversion
        /// </summary>
        public CurrencyController(ILogger<CurrencyController> logger, ICurrenyConversionService currenyService)
        {
            _logger = logger;
            _currenyService = currenyService;
        }

        [Authorize]
        [HttpGet("/Currency/GetDKKEquivalent/{currency}")]
        public async Task<ActionResult> GetDKKEquivalent([FromRoute]string currency, [FromQuery]long value)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return new BadRequestObjectResult("Currency parameter is required.");
            }
            double result;
            try
            {
                // service call
                result = await _currenyService.GetDKKEquivalentOf(currency, value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching DKK equivalent data");
                return HandleException(e);
            }
            return Ok(result);
        }

        private ActionResult HandleException(Exception exception)
        {
            return exception switch
            {
                ItemNotFoundException ex => StatusCode(404, "Invalid Currency."),
                OverflowException ex => ReturnInternalError(),
                Exception ex => ReturnInternalError()
            };
        }

        private ObjectResult ReturnInternalError()
            => StatusCode(500, "Something went wrong.");

    }
}
