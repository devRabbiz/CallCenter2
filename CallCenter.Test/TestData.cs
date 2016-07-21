using CallCenter.Model;
using CallCenter.Model.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CallCenter.Test
{
    public static class TestData
    {
        public static List<PersonDetails> GetTestPersons(bool generateId = true, bool uniqNames = true)
        {
            var persons = new List<PersonDetails>();
            //
            var calls = new List<Call>();
            var person = new PersonDetails() {
                    Id = generateId ? Guid.NewGuid() : Guid.Empty,
                    BirthDate = DateTime.Parse("20.05.1990"),
                    FirstName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Алена",
                    Patronymic = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Викторовна",
                    LastName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Михайлюк",
                    Gender = Gender.Femaile,
                    PhoneNumber = "+380964521256",
                };            
            persons.Add(person);
            //---------------------
            person = new PersonDetails()
            {
                Id = generateId ? Guid.NewGuid() : Guid.Empty,
                BirthDate = DateTime.Parse("18.03.1998"),
                FirstName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Евгений",
                Patronymic = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Павлович",
                LastName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Мостовой",
                Gender = Gender.Male,
                PhoneNumber = "+380502486363",               
            };            
            persons.Add(person);
            //---------------------
            person = new PersonDetails()
            {
                Id = generateId ? Guid.NewGuid() : Guid.Empty,
                BirthDate = DateTime.Parse("7.11.2004"),
                FirstName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Ольга",
                Patronymic = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Владимировна",
                LastName = uniqNames ? Guid.NewGuid().ToString().Substring(0, 8) : "Кожемякина",
                Gender = Gender.All,
                PhoneNumber = "+380954562312",                
            };            
            persons.Add(person);

            return persons;
        }

        public static List<CallDetails> GetTestCalls()
        {
            var calls = new List<CallDetails>() ;
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,                
                CallDate = DateTime.Now.AddDays(-1),
                OrderCost = 0,
                CallReport = "Звонок прошел хорошо"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-7),
                OrderCost = 0,
                CallReport = "Опреатор вне зоны доступа"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-5),
                OrderCost = 199.55,
                CallReport = "Был сделан заказ"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-15),
                OrderCost = 55.12,
                CallReport = "Часть заказа была с браком"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-10),
                OrderCost = 0,
                CallReport = "С 12 до 16 - неудобное время"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-8),
                OrderCost = 230.00,
                CallReport = "Повторный заказ 55"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-16),
                OrderCost = 0,
                CallReport = "Отмена повторного заказа"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-20),
                OrderCost = 330.85,
                CallReport = "Заказ из новой коллекции"
            });
            calls.Add(new CallDetails()
            {
                Id = Guid.Empty,
                CallDate = DateTime.Now.AddDays(-18),
                OrderCost = 5,
                CallReport = "Уточнение заказа и замена товара"
            });

            return calls;
        }
    }
}
