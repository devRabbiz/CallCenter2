using CallCenter.Model;
using CallCenter.Model.Abstract;
using CallCenter.Model.Services.DTO;
using CallCenter.UI.Validators;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace CallCenter.UI.Controllers
{
    public class PersonsController : ApiController
    {
        private ICallCenterService ccService;
        public PersonsController(ICallCenterService cCenter)
        {
            ccService = cCenter;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPersonsCount([FromUri] PersonsFilters filters)
        {
            var validator = new PersonsFilterValidator();
            var valRes = validator.Validate(filters);
            if (valRes.IsValid)
            {
                return Ok(await ccService.PersonsCountAsync(filters));
            }
            return BadRequest();

        }
                
        [HttpGet]
        public async Task<IHttpActionResult> GetPersons([FromUri] PersonsFilters filters)
        {
            var validator = new PersonsFilterValidator();
            var valRes = validator.Validate(filters);
            if (valRes.IsValid)
            {
                var pList = await ccService.GetPersonsAsync(filters);
                return Ok(pList);
            }           
            return BadRequest();
        }
                
        [HttpGet]
        public async Task<IHttpActionResult> GetPerson(Guid id)
        {
            var p = await Task.Run(() => ccService.GetPerson(id));
            if (p == null) return NotFound();
            return Ok(p);
        }
                
        [HttpDelete]
        public async Task<IHttpActionResult> DeletePerson(Guid id)
        {
            var p = await Task.Run(() => ccService.GetPerson(id));
            if (p == null) return NotFound();
            await ccService.DeletePersonAsync(id);
            return Ok();
        }
       
        [HttpPost]
        public async Task<IHttpActionResult> AddPerson([FromBody] PersonDetails person)
        {
            var validator = new PersonValidator();
            var valRes = validator.Validate(person);
            if (valRes.IsValid)
            {
                await ccService.AddPersonAsync(person);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdatePerson([FromBody] PersonDetails person)
        {
            var validator = new PersonValidator();
            var valRes = validator.Validate(person);
            if (valRes.IsValid)
            {
                var p = await Task.Run(() => ccService.GetPerson(person.Id));
                if (p == null) return NotFound();
                await ccService.UpdatePersonAsync(person);
                return Ok();
            }           
            return BadRequest();
        }        
    }
}
