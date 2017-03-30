using System;
using System.Web.Http;
using System.Linq;
using System.Threading.Tasks;
using CallCenter.Model.Abstract;
using CallCenter.Model.Services.DTO;
using CallCenter.UI.Validators;

namespace CallCenter.UI
{
    public class CallsController : ApiController
    {
        private ICallCenterService ccService;
        public CallsController(ICallCenterService cCenter)
        {
            ccService = cCenter;
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCall(Guid pid, Guid cid)
        {
            var pers = ccService.GetPerson(pid);
            if (pers == null) return BadRequest();
            var calls = ccService.GetCalls(pid);
            if (calls.FirstOrDefault(c => c.Id == cid) == null) return BadRequest();
            await ccService.DeleteCallAsync(cid);
            return Ok();
        }       
         
        [HttpPost]
        public async Task<IHttpActionResult> AddCall(Guid pid, [FromBody] CallDetails call)
        {
            var validator = new CallValidator();
            var valRes = validator.Validate(call);
            if (valRes.IsValid)
            {
                await ccService.AddCallAsync(call, pid);
                return Ok();
            }          
            return BadRequest();
        }
        
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCall(Guid pid, [FromBody] CallDetails call)
        {              
            var pers = ccService.GetPerson(pid);
            if (pers == null) return BadRequest();

            if (!ccService.GetCalls(pid).Exists(c => c.Id == call.Id)) return BadRequest();

            var validator = new CallValidator();
            var valRes = validator.Validate(call);
            if (valRes.IsValid)
            {
                await ccService.UpdateCallAsync(call);
                return Ok();
            }           
            return BadRequest();
        }
        
        [HttpGet]
        public async Task<IHttpActionResult> GetCalls(Guid pid)
        {
            if (ccService.GetPerson(pid) == null) return BadRequest();
            return Ok(await Task.Run(() => ccService.GetCalls(pid)));
        }
    }
}
