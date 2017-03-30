using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Routing.Constraints;

namespace CallCenter.UI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "GetPersonsCount",
               routeTemplate: "api/persons/count",
               defaults: new { controller = "Persons", action = "GetPersonsCount" }               
           );

            config.Routes.MapHttpRoute(
                name: "GetFilteredPersonsList",
                routeTemplate: "api/persons",                
                defaults: new { controller = "Persons", action = "GetPersons" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            config.Routes.MapHttpRoute(
                name: "AddPerson",
                routeTemplate: "api/persons",
                defaults: new { controller="Persons", action="AddPerson"}, 
                constraints : new { httpMethod= new HttpMethodConstraint(HttpMethod.Post) }                
            );

            config.Routes.MapHttpRoute(
                name: "GetPersonById",
                routeTemplate: "api/persons/{id}",
                defaults: new { controller = "Persons", action = "GetPerson" },
                constraints: new {
                    id = new GuidRouteConstraint(),
                    httpMethod = new HttpMethodConstraint(HttpMethod.Get)
                }
            );
           
            config.Routes.MapHttpRoute(
                name: "DeletePerson",
                routeTemplate: "api/persons/{id}",
                defaults: new { controller = "Persons", action = "DeletePerson" },
                constraints: new
                {
                    id = new GuidRouteConstraint(),
                    httpMethod = new HttpMethodConstraint(HttpMethod.Delete)
                }
            );
            
            config.Routes.MapHttpRoute(
                name: "UpdatePerson",
                routeTemplate: "api/persons",
                defaults: new { controller = "Persons", action = "UpdatePerson" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) }
            );

            config.Routes.MapHttpRoute(
                name: "AddCall",
                routeTemplate: "api/persons/{pid}/calls",
                defaults: new { controller = "Calls", action = "AddCall" },
                constraints: new {
                    pid = new GuidRouteConstraint(),
                    httpMethod = new HttpMethodConstraint(HttpMethod.Post)
                }
            );

            config.Routes.MapHttpRoute(
                name: "UpdateCall",
                routeTemplate: "api/persons/{pid}/calls",
                defaults: new { controller = "Calls", action = "UpdateCall" },
                constraints: new {
                    pid = new GuidRouteConstraint(),
                    httpMethod = new HttpMethodConstraint(HttpMethod.Put)
                }
            );

            config.Routes.MapHttpRoute(
                name: "DeleteCall",
                routeTemplate: "api/persons/{pid}/calls/{cid}",
                defaults: new { controller = "Calls", action = "DeleteCall" },
                constraints: new {
                    cid = new GuidRouteConstraint(),
                    pid = new GuidRouteConstraint(),
                    httpMethod = new HttpMethodConstraint(HttpMethod.Delete)
                }
            );

            config.Routes.MapHttpRoute(
               name: "GetCalls",
               routeTemplate: "api/persons/{pid}/calls",
               defaults: new { controller = "Calls", action = "GetCalls" },
               constraints: new {
                   pid = new GuidRouteConstraint(),
                   httpMethod = new HttpMethodConstraint(HttpMethod.Get)
               }
           );
        }
    }
}
