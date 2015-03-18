using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Deduplication.WebAPI
{
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false )]
	public class DeduplicateAttribute : Attribute
	{
		public String DeduplicationRequestHeader { get; set; }

		public string DeduplicationResponseHeader { get; set; }
	}
}
