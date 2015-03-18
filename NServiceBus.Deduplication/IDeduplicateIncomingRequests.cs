using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Deduplication
{
	public interface IDeduplicateIncomingRequests
	{
		Boolean IsRequestHandled( String requestId, out Object previousResponse );

		void MarkRequestAsHandled( String requestId, Object response );
	}
}
