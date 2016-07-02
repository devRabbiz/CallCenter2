using CallCenter.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CallCenter.Model.Services
{
    public class Filter
    {
        // фильтр по имени, фамилии и отчеству
        public string NameFilter { get; set; } = string.Empty;
        // фильтр по полу
        public Gender GenderFilter { get; set; } = Gender.All;
        // фильтр по возрастной категории
        public int? AgeMin { get; set; } = null;
        public int? AgeMax { get; set; } = null;
        // фильтр по дате последнего звонка
        public int MinDaysAfterLastCall { get; set; } = 0;

        public int PageSize { get; set; } = 25;
        public int PageNo { get; set; } = 1;

        private IQueryable<Person> GetPersonQuery(ICallCenterStorage storage)
        {
            var defaultCall = new Call() { CallDate = DateTime.Now.AddDays((2 + MinDaysAfterLastCall) * (-1)) };
            var persons = storage.Persons.Include(p => p.Calls).ToList();
            persons = persons.Where(p => (DateTime.Now - p.Calls.DefaultIfEmpty(defaultCall).Max(c => c.CallDate)).TotalDays >= MinDaysAfterLastCall).ToList();

            if (GenderFilter != Gender.All)
            {
                persons = persons.Where(p => p.Gender == GenderFilter).ToList();
            }
            if (AgeMin != null)
            {
                persons = persons.Where(p => p.BirthDate != null).Where(p => DateTime.Now.Year - (p.BirthDate?.Year ?? 0) >= AgeMin.Value).ToList();
            }
            if (AgeMax != null)
            {
                persons = persons.Where(p => p.BirthDate != null).Where(p => DateTime.Now.Year - (p.BirthDate?.Year) <= AgeMax.Value).ToList();
            }
            if (NameFilter != string.Empty)
            {
                persons = persons.FindAll(
                    p => p.FirstName.ToLower().Contains(NameFilter.ToLower()) ||
                    (p.LastName == null ? false : p.LastName.ToLower().Contains(NameFilter.ToLower())) ||
                    (p.Patronymic == null ? false : p.Patronymic.ToLower().Contains(NameFilter.ToLower()))).ToList();
            }
            var Ids = persons.Select(p => p.Id).ToList();
            //return persons.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();     
            return storage.Persons.AsNoTracking().Where(p => Ids.Contains(p.Id)).OrderBy(p => p.Id);
        }

        public async Task<List<Person>> GetPersonsAsync(ICallCenterStorage storage)
        {               
            return await GetPersonQuery(storage).Skip((PageNo - 1) * PageSize).Take(PageSize).ToListAsync();
        }
        public async Task<int> GetCountAsync(ICallCenterStorage storage)
        {                
            return await GetPersonQuery(storage).CountAsync();
        }
    }
}
