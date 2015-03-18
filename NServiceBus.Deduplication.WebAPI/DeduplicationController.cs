using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NServiceBus.Deduplication.WebAPI
{
	public class DeduplicationController : ApiController
	{
		[HttpGet]
		public IEnumerable<String> GetNextIdentifiersChunk( Int32 qty = 50 )
		{
			var temp = new List<String>();

			for( int i = 0; i < qty; i++ )
			{
				temp.Add( Guid.NewGuid().ToString() );
			}

			return temp;
		}
	}
}
