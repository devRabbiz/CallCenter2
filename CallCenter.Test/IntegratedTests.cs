using CallCenter.Data;
using CallCenter.Model;
using CallCenter.Model.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CallCenter.Test
{
    [TestClass]
    public class IntegratedTests
    {
        [TestMethod]
        public void Domain_WithTestDB_PersonsCRUD()
        {
            // для тестирования указываем название тестовой бд, которая будет пересоздаваться
            var storage = new CallCenterStorage("TestsDB");
            var callCenter = new CallCenterService(storage);
            var pList = TestData.GetTestPersons(false);
            // запись и чтение
            int recCount = callCenter.PersonsCountAsync().Result;
            callCenter.AddPersonAsync(pList[0]).Wait();
            Assert.AreEqual(recCount + 1, callCenter.PersonsCountAsync().Result, "Запись не добавлена");
            var fltr = new PersonsFilters() { NameFilter = pList[0].LastName };
            var pers = callCenter.GetPersonsAsync(fltr).Result;
            Assert.AreEqual(pers.Count, 1, "Данные не получены");
            Assert.AreEqual(pers[0].Calls, null, "Получны незапрашиваемые данные (Calls for person)");
            // чтение по id
            Guid pId = pers[0].Id;
            var p = callCenter.GetPersonAsync(pId).Result;
            Assert.IsNotNull(p, "Данные Person по id не получены");            
            // добавление отчета 
            var c = new Call() { CallDate = DateTime.Now, PersonId = pId, CallReport = "Empty", OrderCost=0 };
            callCenter.AddCallAsync(c).Wait();
            var calls = callCenter.GetCallsAsync(pId).Result;
            Assert.AreEqual(calls.Count, 4, "Отчет не добавлен");
            // обновление
            p.FirstName = "NewFirstName";
            p.LastName = "NewLastName";
            p.Patronymic = "NewPatronymic";
            DateTime newBirthDate = DateTime.Now.AddYears(-20);
            p.BirthDate = newBirthDate;
            p.Gender = Gender.All;
            p.PhoneNumber = "00000000";
            callCenter.UpdatePersonAsync(p).Wait();
            p = callCenter.GetPersonAsync(pId).Result;
            Assert.AreEqual(p.FirstName, "NewFirstName", "Данные не обновились (FirstName)");
            Assert.AreEqual(p.LastName, "NewLastName", "Данные не обновились (LastName)");
            Assert.AreEqual(p.Patronymic, "NewPatronymic", "Данные не обновились (Patronymic)");
            Assert.AreEqual(p.BirthDate, newBirthDate, "Данные не обновились (BirthDate)");
            Assert.AreEqual(p.Gender, Gender.All, "Данные не обновились (Gender)");
            Assert.AreEqual(p.PhoneNumber, "00000000", "Данные не обновились (PhoneNumber)");
            // удаление
            callCenter.DeletePersonAsync(p.Id).Wait();
            p = callCenter.GetPersonAsync(p.Id).Result;
            Assert.IsNull(p, "Запись не удалилась");
            //
            callCenter.AddPersonAsync(pList[1]).Wait();
            p = callCenter.GetPersonsAsync(new PersonsFilters()).Result[0];
            Assert.IsNotNull(p);
            pId = p.Id; 
            callCenter.DeletePersonAsync(pId).Wait();
            p = callCenter.GetPersonAsync(pId).Result;
            Assert.IsNull(p);            
        }
        [TestMethod]
        public void Domain_WithTestDB_CallsCRUD()
        {
            // для тестирования указываем название тестовой бд, которая будет пересоздаваться
            var storage = new CallCenterStorage("TestsDB");
            var callCenter = new CallCenterService(storage);
            var person = TestData.GetTestPersons(false)[0];
            person.FirstName = Guid.NewGuid().ToString().Substring(0, 20);
            var call = person.Calls[0];
            person.Calls.Clear();
            callCenter.AddPersonAsync(person).Wait();

            person = callCenter.GetPersonsAsync(new PersonsFilters() { NameFilter = person.FirstName}).Result[0];
            Assert.IsNotNull(person);
            Assert.AreEqual(null, person.Calls);
            var calls = callCenter.GetCallsAsync(person.Id).Result;
            Assert.IsNull(person.Calls);
            call.PersonId = person.Id; // для связи отчет должен содержать Id человека
            // Добавление отчета
            callCenter.AddCallAsync(call).Wait();
            calls = callCenter.GetCallsAsync(person.Id).Result;
            Assert.AreEqual(1, calls.Count, "Отчет не добавлен");
            Assert.AreEqual(call.CallDate, calls[0].CallDate, "Данные не сохранились (CallDate)");
            Assert.AreEqual(call.CallReport, calls[0].CallReport, "Данные не сохранились (CallReport)");
            Assert.AreEqual(call.OrderCost, calls[0].OrderCost, "Данные не сохранились (OrderCost)");
            // Обновление отчета
            var newCall = new Call() { Id=call.Id, CallDate = DateTime.Now.AddDays(-1), CallReport = "call report", OrderCost = 999.99, PersonId = call.PersonId };
            callCenter.UpdateCallAsync(newCall).Wait();
            calls = callCenter.GetCallsAsync(person.Id).Result;
            Assert.AreEqual(1, calls.Count, "Неправильное количество отчетов");
            Assert.AreEqual(newCall.CallDate, calls[0].CallDate, "Данные не сохранились (CallDate)");
            Assert.AreEqual(newCall.CallReport, calls[0].CallReport, "Данные не сохранились (CallReport)");
            Assert.AreEqual(newCall.OrderCost, calls[0].OrderCost, "Данные не сохранились (OrderCost)");
            // удаление отчета
            callCenter.DeleteCallAsync(calls[0].Id).Wait();
            calls = callCenter.GetCallsAsync(person.Id).Result;
            Assert.AreEqual(0, calls.Count, "Отчет не удален");
            //
            newCall = new Call() { CallDate = DateTime.Now.AddDays(-1), CallReport = "call report", OrderCost = 999.99, PersonId = person.Id };
            callCenter.AddCallAsync(newCall).Wait();
            calls = callCenter.GetCallsAsync(person.Id).Result;
            callCenter.DeleteCallAsync(calls[0].Id).Wait();
            calls = callCenter.GetCallsAsync(person.Id).Result;
            Assert.AreEqual(0, calls.Count, "Отчет не удален");
        }

        [TestMethod]
        public void Domain_WithTestDB_PersonsFiltering()
        {
            // создание тестовой бд и наполнение ее тестовыми данными
            var storage = new CallCenterStorage("TestsDB");
            var callCenter = new CallCenterService(storage);

            var resList = callCenter.GetPersonsAsync(new PersonsFilters()).Result;
            foreach(Person p in resList)
            {
                callCenter.DeletePersonAsync(p.Id).Wait();
            }

            var pList = TestData.GetTestPersons(false, false);
            foreach(var p in pList)
            {
                callCenter.AddPersonAsync(p).Wait();
            }
            // вывод без фильтров
            resList = callCenter.GetPersonsAsync(new PersonsFilters()).Result;
            Assert.IsTrue(resList.Count == 3, "Не все элементы прошли фильтр без условий");
            // Фильтрация по имени (имени, фамилии или отчеству)
            var fltr = new PersonsFilters() { NameFilter = pList[0].FirstName };
            resList = callCenter.GetPersonsAsync(fltr).Result;
            Assert.AreEqual(1, resList.Count, "Ошибка при фильтрации по имени");
            fltr.NameFilter = pList[0].LastName;
            resList = callCenter.GetPersonsAsync(fltr).Result;
            Assert.AreEqual(1, resList.Count, "Ошибка при фильтрации по фамилии");
            fltr.NameFilter = pList[1].Patronymic;
            resList = callCenter.GetPersonsAsync(fltr).Result;
            Assert.AreEqual(1, resList.Count, "Ошибка при фильтрации по отчеству");
            // Фильтр по дате последнего звонка (отбор тех, кому не звонили N-е количество дней)
            resList = callCenter.GetPersonsAsync(new PersonsFilters() { MinDaysAfterLastCall = 8 }).Result;
            Assert.IsTrue(resList.Count == 2, "Фильтр по давности последнего звонка не сработал");
            Assert.IsTrue(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по дате последнего звонка");
            Assert.IsTrue(resList.Find(p => p.FirstName == "Ольга") != null, "Неправильный результат фильтрации по дате последнего звонка");
            // Фильтрация по полу
            resList = callCenter.GetPersonsAsync(new PersonsFilters() { Gender = Gender.Male }).Result;
            Assert.IsTrue(resList.Count == 1, "Не работает фильтр по полу");
            Assert.IsTrue(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по полу");                        
            // фильтрация по возрасту
            resList = callCenter.GetPersonsAsync(new PersonsFilters() { MaxAge = 20, MinAge = 15 }).Result;
            Assert.IsTrue(resList.Count == 1, "Не сработал фильтр по возрасту");
            Assert.IsTrue(resList.Find(p => p.FirstName == "Евгений") != null, "Неправильный результат фильтрации по возрасту");           
            // проверка пейджинга           
            resList = callCenter.GetPersonsAsync(new PersonsFilters() { PageSize = 2, PageNo = 2 }).Result;
            Assert.IsTrue(resList.Count == 1, "Не работает разбиение на страницы");          
        }
    }
}
