using NServiceBus.Deduplication.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication.Controllers.Api
{
    public class SampleController : ApiController
    {
		[HttpGet, Deduplicate]
		public dynamic WhenCalled() 
		{
			var response = new 
			{
				When = DateTime.Now
			};

			return response;
		}
    }
}
