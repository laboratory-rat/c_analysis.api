using Infrastructure.Interface.Manager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MRCryptoCurrencyAnalysis.Controllers
{
    [Route("coin")]
    public class CoinController : Controller
    {
        protected readonly ICoinManager _coinManager;

        public CoinController(ICoinManager coinManager)
        {
            _coinManager = coinManager;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> All()
        {
            return Ok(await _coinManager.GetAll());
        }
    }
}
