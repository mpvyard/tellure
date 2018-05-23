using System.Collections.Generic;
using System.Linq;
using Riowil.Entities;
using System.Numerics;

namespace Riowil.Entities.Clusters
{
    public class InitialCluster : GenericInitialCluster<ZVector, double>
    {
        public InitialCluster(ZVector zVector)
            : base(zVector)
        {
        }

        private protected override List<double> FindCentr()
        {
            List<double> res = ZVectors[0].List.ToList();

            for (int i = 1; i < ZVectors.Count; i++)
            {
                for (int j = 0; j < ZVectors[i].List.Count; j++)
                {
                    res[j] += ZVectors[i].List[j];
                }
            }

            for (int j = 0; j < ZVectors[0].List.Count; j++)
            {
                res[j] /= ZVectors.Count;
            }

            return res;
        }
    }

	internal static class ClusterFormat
	{
		public const char ValueSeparator = ';';
	}
}