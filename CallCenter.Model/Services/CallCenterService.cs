using CallCenter.Model.Abstract;
using CallCenter.Model.Services.DTO;
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
        public async Task AddPersonAsync(PersonDetails person)
        {
            _storage.Persons.Add(new Person
            {
                BirthDate = person.BirthDate,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Patronymic = person.Patronymic,
                PhoneNumber = person.PhoneNumber,
                Gender = person.Gender                
            });
            await _storage.SaveChangesAsync();
        }
        public async Task UpdatePersonAsync(PersonDetails person)
        {
            var pers = _storage.Persons.Find(person.Id);
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
        public async Task<List<PersonsListItem>> GetPersonsAsync(PersonsFilters filters)
        {
            _filter.NameFilter = filters.NameFilter;
            _filter.GenderFilter = filters.Gender;
            _filter.AgeMin = filters.MinAge;
            _filter.AgeMax = filters.MaxAge;
            _filter.MinDaysAfterLastCall = filters.MinDaysAfterLastCall;
            _filter.PageSize = filters.PageSize;
            _filter.PageNo = filters.PageNo;

            var pList = await _filter.GetPersonsAsync(_storage);
            return pList.AsParallel().Select(p => new PersonsListItem { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, Patronymic = p.Patronymic }).ToList();
        } 
        public PersonDetails GetPerson(Guid personId)
        {            
            var pers = _storage.Persons.Find(personId);
            return pers != null ?
            new PersonDetails {
                Id = pers.Id,
                BirthDate = pers.BirthDate,
                FirstName = pers.FirstName,
                LastName = pers.LastName,
                Patronymic = pers.Patronymic,
                Gender = pers.Gender,
                PhoneNumber = pers.PhoneNumber
            } : null;
        }            
        public async Task AddCallAsync(CallDetails call, Guid personId)
        {
            var pers = _storage.Persons.Find(personId);
            if (pers != null)
            {
                _storage.Calls.Add(new Call { CallDate = call.CallDate, OrderCost = call.OrderCost, CallReport = call.CallReport, PersonId = personId});
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
            var pers = _storage.Persons.Find(personId);
            if(pers != null)
            {
                _storage.Persons.Remove(pers);
                await _storage.SaveChangesAsync();
            }
        }        
        public async Task DeleteCallAsync(Guid callId)
        {
            var dbCall = _storage.Calls.Find(callId);
            if (dbCall != null)
            {
                _storage.Calls.Remove(dbCall);
                await _storage.SaveChangesAsync();
            }
        }
        public async Task UpdateCallAsync(CallDetails call)
        {
            var dbCall = _storage.Calls.Find(call.Id);
            if (dbCall != null)
            {
                dbCall.OrderCost = call.OrderCost;
                dbCall.CallDate = call.CallDate;
                dbCall.CallReport = call.CallReport;

                await _storage.SaveChangesAsync();
            }
            else throw new Exception($"Отчет с Id = {call.Id} не найден в базе данных");
        }
        public async Task<int> PersonsCountAsync()
        {
            return await _storage.Persons.CountAsync();
        }       
        public List<CallDetails> GetCalls(Guid personId)
        {
            var calls = _storage.Persons.Where(p => p.Id == personId).SelectMany(p => p.Calls);
            return calls?.AsParallel().Select(c => new CallDetails { Id = c.Id, CallDate = c.CallDate, CallReport = c.CallReport, OrderCost = c.OrderCost }).ToList();
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
