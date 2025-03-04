using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Constants;
using WebApp.Exceptions;
using WebApp.Interfaces;

namespace WebApp.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrenyConversionService _currenyService;
        public CurrencyController(ILogger<CurrencyController> logger, ICurrenyConversionService currenyService)
        {
            _logger = logger;
            _currenyService = currenyService;
        }

        [Authorize]
        [HttpGet("/Currency/GetDKKEquivalent/{currency}")]
        public async Task<ActionResult> GetDKKEquivalent([FromRoute]string currency, [FromQuery]long value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return new BadRequestObjectResult("Currency parameter is required.");
            }
            double result;
            try
            {
                // service call
                result = await _currenyService.GetDKKEquivalentOf(currency, value, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching DKK equivalent data");
                return HandleException(e);
            }
            return Ok(
                new
                {
                    Currency = currency,
                    DKKRate = result
                }
            );
        }

        [Authorize(AuthorizationPolicyConstants.AdminPolicyName)]
        [HttpGet("/Currency/GetCurrencyRates")]
        public async Task<ActionResult> GetAllCurrencyRates(CancellationToken cancellationToken = default)
        {
            try
            {
                var list = await _currenyService.GetAllCurrenciesAsync(cancellationToken);
                return Ok(list);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching Currency Rates data");
                return HandleException(e);
            }
        }

        private ActionResult HandleException(Exception exception)
        {
            return exception switch
            {
                ItemNotFoundException ex => StatusCode(400, "Currency conversion not supported."),
                OverflowException ex => ReturnInternalError(),
                Exception ex => ReturnInternalError()
            };
        }

        private ObjectResult ReturnInternalError()
            => StatusCode(500, "Something went wrong.");

    }
}
