using Infrastructure.Interface.Manager;
using Infrastructure.Model.CoinHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MRCryptoCurrencyAnalysis.Controllers
{
    [Authorize]
    [Route("history")]
    public class CoinHistoryController : Controller
    {
        protected readonly ICoinHistoryManager _coinHistoryManager;

        public CoinHistoryController(ICoinHistoryManager coinHistoryManager)
        {
            _coinHistoryManager = coinHistoryManager;
        }

        [HttpPut]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] CoinHistorySearchModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(await _coinHistoryManager.Search(model));
        }
    }
}
