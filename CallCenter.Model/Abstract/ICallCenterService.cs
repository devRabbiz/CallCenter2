using CallCenter.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallCenter.Model.Abstract
{
    public interface ICallCenterService : IDisposable
    {
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task<List<Person>> GetPersonsAsync(PersonsFilters filters);
        Task<Person> GetPersonAsync(Guid personId);
        Task DeletePersonAsync(Guid personId);
        Task AddCallAsync(Call call);
        Task DeleteCallAsync(Guid callId);
        Task UpdateCallAsync(Call call);
        Task<List<Call>> GetCallsAsync(Guid personId);        
        Task<int> PersonsCountAsync();
        Task<int> PersonsCountAsync(PersonsFilters filters);
    }
}
