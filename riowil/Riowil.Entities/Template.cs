using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riowil.Entities.Clusters;

namespace Riowil.Entities
{
	public class Template<T>
	{
		public int Id { get; set; }
		public IEnumerable<Cluster<T>> Clusters { get; set; }
		public int[] Value { get; set; }
	}
}
