using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using NServiceBus.Deduplication.WebAPI;
using NServiceBus.Deduplication;

namespace WebApplication
{
	public static class WebApiConfig
	{
		public static void Register( HttpConfiguration config )
		{
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.Filters.Add( new DeduplicateActionFilter( new InMemoryDeduplicator() ) );
		}
	}
}
