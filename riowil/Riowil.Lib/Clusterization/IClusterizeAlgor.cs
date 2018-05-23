using System.Collections.Generic;
using Riowil.Entities;
using Riowil.Entities.Clusters;

namespace Riowil.Lib
{
	public interface IClusterizeAlgor<T>
	{
        List<GenericInitialCluster<IZVector<T>, T>> Clusterize(List<T> zVectors);
    }
}
