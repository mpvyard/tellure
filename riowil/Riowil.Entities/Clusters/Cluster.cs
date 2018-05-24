using System.Collections.Generic;

namespace Riowil.Entities.Clusters
{
	public class Cluster<T>
	{
		public IReadOnlyList<T> Centr { get; set; }
		public int Id { get; set; }
	}
}