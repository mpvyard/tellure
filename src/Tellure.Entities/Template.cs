using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tellure.Entities.Clusters;

namespace Tellure.Entities
{
	public class Template<T>
	{
		public int Id { get; set; }
		public IEnumerable<Cluster<T>> Clusters { get; set; }
		public int[] Value { get; set; }
	}
}
