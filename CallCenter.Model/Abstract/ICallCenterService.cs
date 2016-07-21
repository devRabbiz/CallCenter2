using CallCenter.Model.Services.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallCenter.Model.Abstract
{
    public interface ICallCenterService : IDisposable
    {
        Task AddPersonAsync(PersonDetails person);
        Task UpdatePersonAsync(PersonDetails person);
        Task<List<PersonsListItem>> GetPersonsAsync(PersonsFilters filters);
        PersonDetails GetPerson(Guid personId);
        Task DeletePersonAsync(Guid personId);
        Task AddCallAsync(CallDetails call, Guid personId);
        Task DeleteCallAsync(Guid callId);
        Task UpdateCallAsync(CallDetails call);
        List<CallDetails> GetCalls(Guid personId);        
        Task<int> PersonsCountAsync();
        Task<int> PersonsCountAsync(PersonsFilters filters);
    }
}
