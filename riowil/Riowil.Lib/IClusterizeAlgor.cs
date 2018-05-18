using System.Collections.Generic;
using Riowil.Entities;

namespace Riowil.Lib
{
	public interface IClusterizeAlgor
	{
		List<InitialCluster> Clusterize(List<ZVector> zVectors);
	}
}
