using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NServiceBus.Deduplication.WebAPI
{
	public class InMemoryDeduplicator : IDeduplicateIncomingRequests
	{
		class Data
		{
			public String Content { get; set; }
			public Type ContentType { get; set; }
		}
		Dictionary<String, Data> requests = new Dictionary<string, Data>();

		public bool IsRequestHandled( string requestId, out Object previousResponse )
		{
			lock( this.requests )
			{
				Data data;
				var handled = this.requests.TryGetValue( requestId, out data );
				if( handled )
				{
					previousResponse = JsonConvert.DeserializeObject( data.Content, data.ContentType );
				}
				else
				{
					previousResponse = null;
				}
				return handled;
			}
		}

		public void MarkRequestAsHandled( string requestId, Object response )
		{
			lock( this.requests )
			{
				var data = new Data
				{
					Content = JsonConvert.SerializeObject( response ),
					ContentType = response.GetType()
				};

				this.requests.Add( requestId, data );
			}
		}
	}
}
