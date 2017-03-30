using CallCenter.Model.Services.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace CallCenter.UI.Tests
{
    [TestClass]
    public class WebApiTests
    {
        // !!! Тесты выполняются на рабочей базе данных (НЕ ТЕСТОВОЙ)
        private string BaseAddress { get { return "http://localhost:30333/"; } }
        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        private void DeletePersonFromBaseIfExists(string personName)
        {
            using (var client = GetClient())
            {
                using (var response = client.GetAsync($"api/persons?NameFilter={personName}").Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var pList = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                        foreach(var pers in pList)
                        {
                            var r = client.DeleteAsync($"api/persons/{pers.Id}").Result;
                        }
                    }
                }
            }
        }
        private PersonDetails AddPersonAndGetItWithId()
        {
            var person = new PersonDetails()
            {
                FirstName = "Арнольд_" + Guid.NewGuid().ToString().Substring(0, 8),
                LastName = "Шварценеггер_" + Guid.NewGuid().ToString().Substring(0, 8),
                Patronymic = "Георгиевич_" + Guid.NewGuid().ToString().Substring(0, 8),
                Gender = Model.Gender.Male,
                BirthDate = DateTime.Now.AddYears((DateTime.Now.Second * (-1)) - 18),
                PhoneNumber = "0984562121"
            };
            DeletePersonFromBaseIfExists(person.FirstName);

            using (var client = GetClient())
            {
                using(var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    if (!response.IsSuccessStatusCode) return null;
                }
                using(var response = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    if (!response.IsSuccessStatusCode) return null;
                    var list = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    if (list == null) return null;
                    person.Id = list.First().Id;
                }
            }
            return person;
        }

        [TestMethod]
        public void WebApi_GetCount()
        {
            using (var client = GetClient())
            {
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    int recCount = response.Content.ReadAsAsync<int>().Result;
                    Assert.IsTrue(recCount >= 0, "Отрицательное значение Count");
                }
            }            
        }
        
        [TestMethod]
        public void WebApi_AddPerson()
        {
            using (var client = GetClient())
            {
                int pCount, newCount; 
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;                    
                }

                var person = new PersonDetails()
                {
                    Id = Guid.Empty,
                    BirthDate = DateTime.Parse("20.05.1990"),
                    FirstName = "Евгения",
                    Patronymic = "Федоровна",
                    LastName = "Полкина",
                    Gender = Model.Gender.Femaile,
                    PhoneNumber = "+380997586636"                    
                };
                using (var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код: {response.StatusCode}");                    
                }

                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    newCount = response.Content.ReadAsAsync<int>().Result;
                }
                Assert.AreEqual(pCount + 1, newCount, $"Запись не добавлена: До добавления записей - {pCount},  после добавления - {newCount}");
            }                     
        }
        
        [TestMethod]
        public void WebApi_GetLastPageWithOnePerson()
        {
            using(var client = GetClient())
            {
                int pCount;
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;
                }

                Assert.IsTrue(pCount != 0, "Для выполнения теста недостаточно записей в базе");

                using(var response = client.GetAsync($"api/persons?PageSize=1&PageNo=/{pCount}").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsNotNull(page, "Пустй ответ при запросе страницы");
                    Assert.IsTrue(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                }
            }
        }
        [TestMethod]
        public void WebApi_GetFirstPageWithThreePersons()
        {
            using (var client = GetClient())
            {
                int pCount;
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;
                }

                Assert.IsTrue(pCount >=3, "Для выполнения теста недостаточно записей в базе");

                using (var response = client.GetAsync("api/persons?PageSize=3&PageNo=1").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsNotNull(page, "Пустй ответ при запросе страницы");
                    Assert.IsTrue(page.Count() == 3, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                }
            }
        }
        [TestMethod]
        public void WebApi_GetPersonById()
        {
            using(var client = GetClient())
            {
                var person = new PersonDetails() { FirstName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}", PhoneNumber="test number" };                
                using (var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при добавлении записи: {response.StatusCode}");
                }

                using (var response = client.GetAsync($"api/persons?NameFilter={Uri.EscapeDataString(person.FirstName)}").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsNotNull(page, "Пустой ответ при запросе страницы");
                    Assert.IsTrue(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    person.Id = page.First().Id;
                }

                using (var response = client.GetAsync($"api/persons/{Uri.EscapeDataString(person.Id.ToString())}").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var pers = response.Content.ReadAsAsync<PersonDetails>().Result;
                    Assert.IsNotNull(pers, $"Запись не найдена по id - {person.Id}");
                }
            }
        }
        [TestMethod]
        public void WebApi_GetPageFilteredByName()
        {
            using (var client = GetClient())
            {                
                var person = new PersonDetails() {
                    FirstName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    LastName = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    Patronymic = $"Name_{Guid.NewGuid().ToString().Substring(0, 8)}",
                    PhoneNumber = "phone number" };
                using (var response = client.PostAsJsonAsync("api/persons", person).Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при добавлении записи: {response.StatusCode}");
                }
                
                using (var response = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsNotNull(page, "Пустй ответ при запросе страницы");
                    Assert.IsTrue(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    Assert.AreEqual(person.FirstName, page.First().FirstName, "Получены некорректные данные (FirstName)");
                }

                using (var response = client.GetAsync($"api/persons?NameFilter={person.LastName}").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsNotNull(page, "Пустй ответ при запросе страницы");
                    Assert.IsTrue(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    Assert.AreEqual(person.LastName, page.First().LastName, "Получены некорректные данные (LastName)");
                }

                using (var response = client.GetAsync($"api/persons?NameFilter={person.Patronymic}").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный код ответа при запросе страницы - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsNotNull(page, "Пустй ответ при запросе страницы");
                    Assert.IsTrue(page.Count() == 1, $"Страница не содержит ожидаемого количества данных: {page.Count()}");
                    Assert.AreEqual(person.Patronymic, page.First().Patronymic, "Получены некорректные данные (Patronymic)");
                }
            }
        }
        [TestMethod]
        public void WebApi_GetPageFilteredByGender()
        {
            using(var client = GetClient())
            {
                int allCount, maleCount, femaleCount, pCount;
                using (var response = client.GetAsync($"api/persons?Gender={Model.Gender.All}&pagesize=500").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.All) - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    allCount = page.Count();
                    Assert.IsTrue(allCount > 0, "Недостаточно записей в базе для тестирования");
                }
                using (var response = client.GetAsync($"api/persons?Gender={Model.Gender.Femaile}&pagesize=500").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.Female) - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    femaleCount = page.Count();                    
                }
                using (var response = client.GetAsync($"api/persons?Gender={Model.Gender.Male}&pagesize=500").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Код запроса отфильрованного списка (Gender.Male) - {response.StatusCode}");
                    var page = response.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    maleCount = page.Count();                    
                }
                using (var response = client.GetAsync("api/persons/count").Result)
                {
                    Assert.IsTrue(response.IsSuccessStatusCode, $"Неправильный статусный код при запросе количества записей: {response.StatusCode}");
                    pCount = response.Content.ReadAsAsync<int>().Result;
                }
                Assert.AreEqual(allCount, pCount, $"Ошибка в определении пола. All:{allCount}; Records Count:{pCount};");
            }
        }
        [TestMethod]
        public void WebApi_GetPageFilteredByMaxAge()
        {
            using (var client = GetClient())
            {
                var person = new PersonDetails() { FirstName = "MaxAgeTestUser", PhoneNumber = "444-444", BirthDate = DateTime.Now.AddYears(-1) };
                DeletePersonFromBaseIfExists(person.FirstName);
                Thread.Sleep(1000);
                var res = client.PostAsJsonAsync("api/persons", person).Result;
                
                using(res = client.GetAsync("api/persons?MaxAge=1").Result)
                {
                    Assert.IsTrue(res.IsSuccessStatusCode, $"Код ответа не положительный- {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.IsTrue(pList.Count() == 1, $"Страница не содержит ожидаемого количества данных: {pList.Count()}");
                    Assert.AreEqual(person.FirstName, pList.First().FirstName, "Получены некорректные данные");
                }
            }
        }
        [TestMethod]
        public void WebApi_GetPageFilteredByMinAge()
        {
            using (var client = GetClient())
            {
                var person = new PersonDetails() { FirstName = "MinAgeTestUser", PhoneNumber = "444-444", BirthDate = DateTime.Now.AddYears(-100) };
                DeletePersonFromBaseIfExists(person.FirstName);

                using (var res = client.PostAsJsonAsync("api/persons", person).Result) { }

                using (var res = client.GetAsync("api/persons?MinAge=100&MaxAge=101").Result)
                {
                    Assert.IsTrue(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.AreEqual(pList.Count(), 1, "Страница не содержит ожидаемого количества данных");
                    Assert.AreEqual(person.FirstName, pList.First().FirstName, "Получены некорректные данные");
                }
            }
        }       
        [TestMethod]
        public void WebApi_UpdatePerson()
        {
            using (var client = GetClient())
            {
                var person = new PersonDetails()
                {
                    FirstName = "UpdateName",
                    LastName = "UpdateName2",
                    Patronymic = "UpdatePatronymic",
                    BirthDate = DateTime.Now.AddYears(-30),
                    Gender = Model.Gender.All,
                    PhoneNumber = "1111111111"
                };

                DeletePersonFromBaseIfExists(person.FirstName);
                using (var res = client.PostAsJsonAsync("api/persons", person).Result) { }

                using (var res = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    Assert.IsTrue(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.AreEqual(1, pList.Count(), "Страница не содержит ожидаемого количества данных");
                    Assert.AreEqual(person.FirstName, pList.First().FirstName, "Получены некорректные данные");
                    person.Id = pList.First().Id;
                }

                person.FirstName = "NewName";
                person.LastName = "NewName2";
                person.Patronymic = "NewPatronymic";
                person.PhoneNumber = "2222222222";
                person.BirthDate = person.BirthDate?.AddYears(10);
                person.Gender = Model.Gender.Male;
                DeletePersonFromBaseIfExists(person.FirstName);
                using (var res = client.PutAsJsonAsync($"api/persons", person).Result) { }

                PersonDetails personById, personByName;
                personByName = new PersonDetails();
                using (var res = client.GetAsync($"api/persons/{person.Id}").Result)
                {
                    personById = res.Content.ReadAsAsync<PersonDetails>().Result;                    
                }
                using (var res = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    var list = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.AreEqual(1, list.Count(), "Страница не содержит ожидаемого количества данных");
                    personByName.Id = list.First().Id;                    
                }

                using (var res = client.GetAsync($"api/persons/{personByName.Id}").Result)
                {
                    personByName = res.Content.ReadAsAsync<PersonDetails>().Result;                    
                }

                Assert.AreEqual(person.FirstName, personById.FirstName, "Получены некорректные данные по Id");
                Assert.AreEqual(person.LastName, personById.LastName, "Получены некорректные данные по Id");
                Assert.AreEqual(person.Patronymic, personById.Patronymic, "Получены некорректные данные по Id");
                Assert.AreEqual(person.PhoneNumber, personById.PhoneNumber, "Получены некорректные данные по Id");
                Assert.AreEqual(person.BirthDate?.Date, personById.BirthDate?.Date, "Получены некорректные данные по Id");
                Assert.AreEqual(person.Gender, personById.Gender, "Получены некорректные данные по Id");

                Assert.AreEqual(person.Id, personByName.Id, "Получены некорректные данные по FirstName");
                Assert.AreEqual(person.FirstName, personByName.FirstName, "Получены некорректные данные по FirstName");
                Assert.AreEqual(person.LastName, personByName.LastName, "Получены некорректные данные по FirstName");
                Assert.AreEqual(person.Patronymic, personByName.Patronymic, "Получены некорректные данные по FirstName");
                Assert.AreEqual(person.PhoneNumber, personByName.PhoneNumber, "Получены некорректные данные по FirstName");
                Assert.AreEqual(person.Gender, personByName.Gender, "Получены некорректные данные по FirstName");
                Assert.AreEqual(person.BirthDate?.Date, personByName.BirthDate?.Date, "Получены некорректные данные по FirstName");
            }
        }

        [TestMethod]
        public void WebApi_DeletePerson()
        {
            using (var client = GetClient())
            {
                var person = new PersonDetails() { FirstName="DeletingTest", PhoneNumber="77777777"};

                using (var res = client.PostAsJsonAsync("api/persons", person).Result) { }

                using (var res = client.GetAsync($"api/persons?NameFilter={person.FirstName}").Result)
                {
                    Assert.IsTrue(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                    var resList = res.Content.ReadAsAsync<IEnumerable<PersonsListItem>>().Result;
                    Assert.AreEqual(resList.Count(), 1, "Страница не содержит ожидаемого количества данных");
                    person.Id = resList.First().Id;
                }

                using (var res = client.DeleteAsync($"api/persons/{person.Id}").Result)
                {
                    Assert.IsTrue(res.IsSuccessStatusCode, $"Код ответа не положительный - {res.StatusCode}");
                }
                using (var res = client.GetAsync($"api/persons/{person.Id}").Result)
                {
                    Assert.AreEqual(res.StatusCode, HttpStatusCode.NotFound, "Запись не удалена");
                }
            }
        }
              
        [TestMethod]
        public void WebApi_AddCall()
        {
            using (var client = GetClient())
            {
                var person = AddPersonAndGetItWithId();
                var call = new CallDetails() {
                    CallDate = DateTime.Now.AddDays(-10),
                    CallReport = "Звонок прошел на удивление хорошо",
                    OrderCost = 489.54                    
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.Id}/calls", call).Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                Guid fcId;
                using (var resp = client.GetAsync($"api/persons/{person.Id}/calls").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var c = resp.Content.ReadAsAsync<IEnumerable<CallDetails>>().Result;
                    Assert.IsNotNull(c, "После обновления записи не удалось загрузить данные");
                    Assert.AreEqual(1, c.Count(), $"Неожиданное количество отчетов после добавления: {c.Count()}");

                    Assert.AreEqual(call.CallDate.Date, c.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                    Assert.AreEqual(call.CallReport, c.First().CallReport, "Получены некорректные данные (CallReport)");
                    Assert.AreEqual(call.OrderCost, c.First().OrderCost, "Получены некорректные данные (OrderCost)");                    

                    fcId = c.First().Id;
                }
                 
                call = new CallDetails()
                {
                    CallDate = DateTime.Now.AddDays(-5),
                    CallReport = "Звонок прошел нормально",
                    OrderCost = 0                    
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.Id}/calls", call).Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.Id}/calls").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var c = resp.Content.ReadAsAsync<IEnumerable<CallDetails>>().Result;
                    Assert.IsNotNull(c, "После обновления записи не удалось загрузить данные");
                    Assert.AreEqual(2, c.Count(), $"Неожиданное количество отчетов после добавления: {c.Count()}");

                    Assert.AreEqual(call.CallDate.Date, c.FirstOrDefault(cl=>cl.Id != fcId).CallDate.Date, "Получены некорректные данные (CallDate)");
                    Assert.AreEqual(call.CallReport, c.FirstOrDefault(cl => cl.Id != fcId).CallReport, "Получены некорректные данные (CallReport)");
                    Assert.AreEqual(call.OrderCost, c.FirstOrDefault(cl => cl.Id != fcId).OrderCost, "Получены некорректные данные (OrderCost)");                    
                }
            }
        }
        [TestMethod]
        public void WebApi_UpdateCall()
        {
            using (var client = GetClient())
            {
                var person = AddPersonAndGetItWithId();
                var call = new CallDetails()
                {
                    CallDate = DateTime.Now.AddDays(-7),
                    CallReport = "Предложение по новой коллекции понравилось клиенту, но он решил заказать предыдущий товар",
                    OrderCost = 229.99                                       
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.Id}/calls", call).Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.Id}/calls").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<CallDetails>>().Result;
                    Assert.IsNotNull(calls, "После обновления записи не удалось загрузить данные");
                    Assert.AreEqual(1, calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");

                    Assert.AreEqual(call.CallDate.Date, calls.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                    Assert.AreEqual(call.CallReport, calls.First().CallReport, "Получены некорректные данные (CallReport)");
                    Assert.AreEqual(call.OrderCost, calls.First().OrderCost, "Получены некорректные данные (OrderCost)");                   

                    call.Id = calls.First().Id;
                }

                call.CallDate = DateTime.Now.AddDays(-3);
                call.CallReport = "Был сделан заказ";
                call.OrderCost = 300;               
                using (var resp = client.PutAsJsonAsync($"api/persons/{person.Id}/calls", call).Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При обновлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.Id}/calls").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<CallDetails>>().Result;
                    Assert.IsNotNull(calls, "После обновления записи не удалось загрузить данные");
                    Assert.AreEqual(1, calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");

                    Assert.AreEqual(call.CallDate.Date, calls.First().CallDate.Date, "Получены некорректные данные (CallDate)");
                    Assert.AreEqual(call.CallReport, calls.First().CallReport, "Получены некорректные данные (CallReport)");
                    Assert.AreEqual(call.OrderCost, calls.First().OrderCost, "Получены некорректные данные (OrderCost)");                                   
                }
            }
        }
        [TestMethod]
        public void WebApi_DeleteCall()
        {
            using (var client = GetClient())
            {
                var person = AddPersonAndGetItWithId();
                var call = new CallDetails()
                {
                    CallDate = DateTime.Now.AddDays(-7),
                    CallReport = "Предложение по новой коллекции понравилось клиенту, но он решил заказать предыдущий товар"                   
                };
                using (var resp = client.PostAsJsonAsync($"api/persons/{person.Id}/calls", call).Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.Id}/calls").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<CallDetails>>().Result;
                    Assert.IsNotNull(calls, "После обновления записи не удалось загрузить данные");
                    Assert.AreEqual(1, calls.Count(), $"Неожиданное количество отчетов после добавления: {calls.Count()}");
                                        
                    call.Id = calls.First().Id;
                }
                using (var resp = client.DeleteAsync($"api/persons/{person.Id}/calls/{call.Id}").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При удалении отчета получен неожиданный ответ - {resp.StatusCode}");
                }
                using (var resp = client.GetAsync($"api/persons/{person.Id}/calls").Result)
                {
                    Assert.IsTrue(resp.IsSuccessStatusCode, $"При добавлении отчета получен неожиданный ответ - {resp.StatusCode}");
                    var calls = resp.Content.ReadAsAsync<IEnumerable<CallDetails>>().Result;
                    Assert.IsNotNull(calls, "После обновления записи не удалось загрузить данные");
                    Assert.AreEqual(0, calls.Count(), $"Неожиданное количество отчетов после удаления: {calls.Count()}");
                }
            }
        }
        [TestMethod]
        public void WebApi_GetPageFilteredByMinDays()
        {
            using (var client = GetClient())
            {
                var person1 = AddPersonAndGetItWithId();
                var call1 = new CallDetails()
                {
                    CallDate = DateTime.Now.AddDays(-30),
                    CallReport = "Отчет для фильтрации"                   
                };
                var person2 = AddPersonAndGetItWithId();
                person2.FirstName = person1.FirstName;
                using (var res = client.PutAsJsonAsync("api/persons", person2).Result) { }
                var call2 = new CallDetails()
                {
                    CallDate = DateTime.Now.AddDays(-10),
                    CallReport = "Отчет для фильтрации"                    
                };

                using (var res = client.PostAsJsonAsync($"api/persons/{person1.Id}/calls", call1).Result) { }
                using (var res = client.PostAsJsonAsync($"api/persons/{person2.Id}/calls", call2).Result) { }
                
                using (var res = client.GetAsync($"api/persons?MinDaysAfterLastCall=30&PageSize=100&NameFilter={person1.FirstName}").Result)
                {
                    Assert.IsTrue(res.IsSuccessStatusCode, $"Неожиданный код ответа {res.StatusCode}");
                    var pList = res.Content.ReadAsAsync<IEnumerable<PersonDetails>>().Result;
                    Assert.AreEqual(1, pList.Count(), $"Страница не содержит ожидаемого количества данных {pList.Count()}");
                    Assert.AreEqual(person1.FirstName, pList.First().FirstName, "Получены некорректные данные");
                    Assert.AreEqual(person1.LastName, pList.First().LastName, "Получены некорректные данные");
                }
            }            
        }        
    }
}
