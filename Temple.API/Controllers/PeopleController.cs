using Microsoft.AspNetCore.Mvc;
using Temple.Domain.Entities.PR;
using Temple.Application.People;
using Temple.Application.Core;

namespace Temple.API.Controllers
{
    public class PeopleController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetPeople(
            [FromQuery] PersonParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(
            Guid id,
            [FromQuery] VersioningParams param)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id, Params = param }));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson(
            Person person)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Person = person }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditPerson(
            Guid id,
            Person person)
        {
            person.ID = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Person = person }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(
            Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
