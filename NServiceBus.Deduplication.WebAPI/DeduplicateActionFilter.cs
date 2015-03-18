using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace NServiceBus.Deduplication.WebAPI
{
	public class DeduplicateActionFilter : IActionFilter
	{
		readonly IDeduplicateIncomingRequests deduplicator;

		public DeduplicateActionFilter( IDeduplicateIncomingRequests deduplicator )
		{
			if( deduplicator == null )
			{
				throw new ArgumentNullException( "deduplicator", "A IDeduplicateIncomingRequests instance is mandatory." );
			}

			this.deduplicator = deduplicator;
			this.DeduplicationRequestHeader = "x-nservicebus-request-id";
			this.DeduplicationResponseHeader = "x-nservicebus-deduplication-status";
		}

		public async Task<System.Net.Http.HttpResponseMessage> ExecuteActionFilterAsync( System.Web.Http.Controllers.HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken, Func<Task<System.Net.Http.HttpResponseMessage>> continuation )
		{
			var deduplicateAttribute = actionContext.ActionDescriptor.GetCustomAttributes<DeduplicateAttribute>().SingleOrDefault();
			
			var shouldDeduplicate = deduplicateAttribute != null;
			if( shouldDeduplicate )
			{
				var deduplicationRequestHeader = deduplicateAttribute.DeduplicationRequestHeader ?? this.DeduplicationRequestHeader;

				if( !actionContext.Request.Headers.Contains( deduplicationRequestHeader ) )
				{
					throw new InvalidOperationException( "Action is enlisted for deduplication but no deduplication header can be found in the request" );
				}

				var requestId = actionContext.Request.Headers.GetValues( deduplicationRequestHeader ).Single();
				Object response;
				if( this.deduplicator.IsRequestHandled( requestId, out response ) )
				{
					var responseMessage = actionContext.Request.CreateResponse( response );
					var deduplicationResponseHeader = deduplicateAttribute.DeduplicationResponseHeader ?? this.DeduplicationResponseHeader;
					responseMessage.Headers.Add( deduplicationResponseHeader, "deduplicated" );

					return responseMessage;
				}
				else 
				{
					return await continuation()
						.ContinueWith( async t => 
						{
							if( !t.IsFaulted ) 
							{
								var actualResponse = t.Result;
								var data = await actualResponse.Content.ReadAsAsync( actionContext.ActionDescriptor.ReturnType );
								this.deduplicator.MarkRequestAsHandled( requestId, data );
							}

							return t;
						} )
						.Unwrap()
						.Unwrap();
				}
			}

			return await continuation();
		}

		public bool AllowMultiple
		{
			get { return false; }
		}

		public String DeduplicationRequestHeader { get; set; }

		public string DeduplicationResponseHeader { get; set; }
	}
}
