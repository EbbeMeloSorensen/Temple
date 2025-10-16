using Microsoft.AspNetCore.Mvc;
using Temple.Application.Smurfs;

namespace Temple.API.Controllers
{
    public class SmurfsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetSmurfs(
            [FromQuery] SmurfParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }
    }
}