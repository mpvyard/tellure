using System.Collections.Generic;
using Domain.Entity;

namespace Clusterizing.ClusteringMethods
{
	public interface IClusterizeAlgor
	{
		List<InitialCluster> Clusterize(List<ZVector> zVectors);
	}
}
