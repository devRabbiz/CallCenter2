using CallCenter.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CallCenter.Model.Services
{
    public class CallCenterService : ICallCenterService
    {
        private Filter _filter { get; set; } = new Filter();
        private ICallCenterStorage _storage { get; set; }        
        public CallCenterService(ICallCenterStorage storage)
        {
            _storage = storage;
        }       
        public async Task AddPersonAsync(Person person)
        {
            _storage.Persons.Add(person);
            await _storage.SaveChangesAsync();
        }
        public async Task UpdatePersonAsync(Person person)
        {
            var pers = _storage.Persons.FirstOrDefault(p => p.Id == person.Id);
            if (pers != null)
            {
                pers.BirthDate = person.BirthDate;
                pers.FirstName = person.FirstName;
                pers.LastName = person.LastName;
                pers.Patronymic = person.Patronymic;
                pers.Gender = person.Gender;
                pers.PhoneNumber = person.PhoneNumber;

                await _storage.SaveChangesAsync();
            }
        }
        public async Task<List<Person>> GetPersonsAsync(PersonsFilters filters)
        {
            _filter.NameFilter = filters.NameFilter;
            _filter.GenderFilter = filters.Gender;
            _filter.AgeMin = filters.MinAge;
            _filter.AgeMax = filters.MaxAge;
            _filter.MinDaysAfterLastCall = filters.MinDaysAfterLastCall;
            _filter.PageSize = filters.PageSize;
            _filter.PageNo = filters.PageNo;

            return await _filter.GetPersonsAsync(_storage);
        } 
        public async Task<Person> GetPersonAsync(Guid personId)
        {            
            return await _storage.Persons.FirstOrDefaultAsync(p => p.Id == personId);            
        }            
        public async Task AddCallAsync(Call call)
        {
            var pers = await _storage.Persons.FirstOrDefaultAsync(p => p.Id == call.PersonId);
            if (pers != null)
            {
                _storage.Calls.Add(call);
                await _storage.SaveChangesAsync();
            }
        }
        public void Dispose()
        {
            if(_storage != null)
            {
                _storage.Dispose();
                _storage = null;
            }
        }
        public async Task DeletePersonAsync(Guid personId)
        {
            var pers = await _storage.Persons.FirstOrDefaultAsync(p => p.Id == personId);
            if(pers != null)
            {
                _storage.Persons.Remove(pers);
                await _storage.SaveChangesAsync();
            }
        }        
        public async Task DeleteCallAsync(Guid callId)
        {
            var dbCall = await _storage.Calls.FirstOrDefaultAsync(c => c.Id == callId);
            if (dbCall != null)
            {
                _storage.Calls.Remove(dbCall);
                await _storage.SaveChangesAsync();
            }
        }
        public async Task UpdateCallAsync(Call call)
        {
            var dbCall = await _storage.Calls.FirstOrDefaultAsync(c => c.Id == call.Id);
            if (dbCall != null)
            {
                dbCall.OrderCost = call.OrderCost;
                dbCall.CallDate = call.CallDate;
                dbCall.CallReport = call.CallReport;               

                await _storage.SaveChangesAsync();
            }
        }
        public async Task<int> PersonsCountAsync()
        {
            return await _storage.Persons.CountAsync();
        }       
        public async Task<List<Call>> GetCallsAsync(Guid personId)
        {
            var person = await _storage.Persons.Include(p => p.Calls).FirstOrDefaultAsync(p => p.Id == personId);
            if (person == null) return null;
            return person.Calls.ToList();
        }

        public Task<int> PersonsCountAsync(PersonsFilters filters)
        {
            _filter.NameFilter = filters.NameFilter;
            _filter.GenderFilter = filters.Gender;
            _filter.AgeMin = filters.MinAge;
            _filter.AgeMax = filters.MaxAge;
            _filter.MinDaysAfterLastCall = filters.MinDaysAfterLastCall;
            _filter.PageSize = filters.PageSize;
            _filter.PageNo = filters.PageNo;

            return _filter.GetCountAsync(_storage);            
        }
    }
}
