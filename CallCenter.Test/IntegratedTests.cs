using CallCenter.Data;
using CallCenter.Model;
using CallCenter.Model.Services;
using CallCenter.Model.Services.DTO;
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
            // чтение по id
            Guid pId = pers[0].Id;
            var p = callCenter.GetPerson(pId);
            Assert.IsNotNull(p, "Данные Person по id не получены");            
            // добавление отчета 
            var c = new CallDetails() { CallDate = DateTime.Now, CallReport = "Empty", OrderCost=0 };
            callCenter.AddCallAsync(c, pId).Wait();
            var calls = callCenter.GetCalls(pId);
            Assert.AreEqual(1, calls.Count, "Отчет не добавлен");
            // обновление
            p.FirstName = "NewFirstName";
            p.LastName = "NewLastName";
            p.Patronymic = "NewPatronymic";
            DateTime newBirthDate = DateTime.Now.AddYears(-20);
            p.BirthDate = newBirthDate;
            p.Gender = Gender.All;
            p.PhoneNumber = "00000000";
            callCenter.UpdatePersonAsync(p).Wait();
            p = callCenter.GetPerson(pId);
            Assert.AreEqual(p.FirstName, "NewFirstName", "Данные не обновились (FirstName)");
            Assert.AreEqual(p.LastName, "NewLastName", "Данные не обновились (LastName)");
            Assert.AreEqual(p.Patronymic, "NewPatronymic", "Данные не обновились (Patronymic)");
            Assert.AreEqual(p.BirthDate, newBirthDate, "Данные не обновились (BirthDate)");
            Assert.AreEqual(p.Gender, Gender.All, "Данные не обновились (Gender)");
            Assert.AreEqual(p.PhoneNumber, "00000000", "Данные не обновились (PhoneNumber)");
            // удаление
            callCenter.DeletePersonAsync(p.Id).Wait();
            p = callCenter.GetPerson(p.Id);
            Assert.IsNull(p, "Запись не удалилась");
            //
            callCenter.AddPersonAsync(pList[1]).Wait();
            var pd = callCenter.GetPersonsAsync(new PersonsFilters()).Result[0];
            Assert.IsNotNull(pd);
            pId = pd.Id; 
            callCenter.DeletePersonAsync(pId).Wait();
            p = callCenter.GetPerson(pId);
            Assert.IsNull(p);            
        }
        
        [TestMethod]
        public void Domain_WithTestDB_CallsCRUD()
        {
            // для тестирования указываем название тестовой бд, которая будет пересоздаваться
            var storage = new CallCenterStorage("TestsDB");
            var callCenter = new CallCenterService(storage);
            
            var person = TestData.GetTestPersons()[0];
            var call = TestData.GetTestCalls()[0];
            callCenter.AddPersonAsync(person).Wait();

            var pListItem = callCenter.GetPersonsAsync(new PersonsFilters() { NameFilter = person.FirstName}).Result[0];
            Assert.IsNotNull(pListItem);
            var pCalls = callCenter.GetCalls(pListItem.Id);
            Assert.AreEqual(0, pCalls.Count);
            // Добавление отчета
            callCenter.AddCallAsync(call, pListItem.Id).Wait();
            pCalls = callCenter.GetCalls(pListItem.Id);
            Assert.AreEqual(1, pCalls.Count, "Отчет не добавлен");
            Assert.AreEqual(call.CallDate, pCalls[0].CallDate, "Данные не сохранились (CallDate)");
            Assert.AreEqual(call.CallReport, pCalls[0].CallReport, "Данные не сохранились (CallReport)");
            Assert.AreEqual(call.OrderCost, pCalls[0].OrderCost, "Данные не сохранились (OrderCost)");
            // Обновление отчета
            call = pCalls[0];
            var newCall = new CallDetails() { Id = call.Id, CallDate = DateTime.Now.AddDays(-1), CallReport = "call report", OrderCost = 999.99};
            callCenter.UpdateCallAsync(newCall).Wait();
            pCalls = callCenter.GetCalls(pListItem.Id);
            Assert.AreEqual(1, pCalls.Count, "Неправильное количество отчетов");
            Assert.AreEqual(newCall.CallDate, pCalls[0].CallDate, "Данные не сохранились (CallDate)");
            Assert.AreEqual(newCall.CallReport, pCalls[0].CallReport, "Данные не сохранились (CallReport)");
            Assert.AreEqual(newCall.OrderCost, pCalls[0].OrderCost, "Данные не сохранились (OrderCost)");
            // удаление отчета
            callCenter.DeleteCallAsync(pCalls[0].Id).Wait();
            pCalls = callCenter.GetCalls(person.Id);
            Assert.AreEqual(0, pCalls.Count, "Отчет не удален");
            //
            newCall = new CallDetails() { CallDate = DateTime.Now.AddDays(-1), CallReport = "call report", OrderCost = 999.99};
            callCenter.AddCallAsync(newCall, pListItem.Id).Wait();
            pCalls = callCenter.GetCalls(pListItem.Id);
            callCenter.DeleteCallAsync(pCalls[0].Id).Wait();
            pCalls = callCenter.GetCalls(pListItem.Id);
            Assert.AreEqual(0, pCalls.Count, "Отчет не удален");
        }
        
        [TestMethod]
        public void Domain_WithTestDB_PersonsFiltering()
        {
            // создание тестовой бд и наполнение ее тестовыми данными
            var storage = new CallCenterStorage("TestsDB");
            var callCenter = new CallCenterService(storage);

            var resList = callCenter.GetPersonsAsync(new PersonsFilters()).Result;
            foreach(PersonsListItem p in resList)
            {
                callCenter.DeletePersonAsync(p.Id).Wait();
            }

            var pList = TestData.GetTestPersons(false, false);
            var pCalls = TestData.GetTestCalls();
            foreach(var p in pList)
            {
                callCenter.AddPersonAsync(p).Wait();
            }
            resList = callCenter.GetPersonsAsync(new PersonsFilters()).Result;
            for(int i = 0; i < 3; i++)
            {
                callCenter.AddCallAsync(pCalls[3 * i], resList[i].Id).Wait();
                callCenter.AddCallAsync(pCalls[3 * i + 1], resList[i].Id).Wait();
                callCenter.AddCallAsync(pCalls[3 * i + 2], resList[i].Id).Wait();
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
