using Microsoft.VisualStudio.TestTools.UnitTesting;
using CallCenter.UI.Controllers;
using Moq;
using CallCenter.Model.Abstract;
using System.Threading.Tasks;
using CallCenter.Model;
using System.Threading;
using System;
using System.Net.Http;

namespace CallCenter.UI.Tests
{
    [TestClass]
    public class ControllersTest
    {
        [TestMethod]
        public void PersonsCount_ReturnInt()
        {
            var mock = new Mock<ICallCenterService>();
            mock.Setup(s => s.PersonsCountAsync(It.IsAny<PersonsFilters>())).Returns(Task.Run(() => 7));

            var controller = new PersonsController(mock.Object);
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new HttpRequestMessage { Method = HttpMethod.Get };

            var httpResp = controller.GetPersonsCount(new PersonsFilters()).Result;
            var resp = httpResp.ExecuteAsync(new CancellationToken()).Result;

            Assert.IsTrue(resp.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.AreEqual(7, Convert.ToInt32(resp.Content.ReadAsStringAsync().Result));
        }
    }
}
