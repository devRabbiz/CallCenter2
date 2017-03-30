using CallCenter.Model;
using CallCenter.Model.Abstract;
using CallCenter.Model.Services.DTO;
using CallCenter.UI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CallCenter.UI.Controllers
{
    public class HomeController : Controller
    {
        private ICallCenterService service = null;
        public HomeController(ICallCenterService service)
        {
            this.service = service;
        }       
        public async Task<ActionResult> Index(IndexFilterViewModel filtersViewModel)
        {            
            var tmp = filtersViewModel ?? new IndexFilterViewModel {
                    NameFilter = string.Empty,
                    PageNo = 1,
                    PageSize = 25
                };
            
            var filters = new PersonsFilters
            {
                Gender = (Gender)tmp.GenderFilter,
                MaxAge = tmp.AgeMax == 0 ? null : (int?)tmp.AgeMax,
                MinAge = tmp.AgeMin == 0 ? null : (int?)tmp.AgeMin,
                MinDaysAfterLastCall = tmp.MinDaysAfterLastCall,
                NameFilter = tmp.NameFilter ?? string.Empty,
                PageNo = tmp.PageNo > 0 ? tmp.PageNo : 1,
                PageSize = tmp.PageSize
            };

            tmp.RecordsCount = await service.PersonsCountAsync(filters);
            ViewBag.PersonsList = await service.GetPersonsAsync(filters);
            return View(tmp);
        }        
    }
}