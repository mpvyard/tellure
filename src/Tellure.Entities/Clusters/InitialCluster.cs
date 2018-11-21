using System.Collections.Generic;
using System.Linq;
using Tellure.Entities;
using System.Numerics;

namespace Tellure.Entities.Clusters
{
    public class InitialCluster : GenericInitialCluster<ZVector, Vector4>
    {
        public InitialCluster()
            :base(new ZVector(null)) { }

        public InitialCluster(ZVector zVector)
            : base(zVector)
        {
        }

        protected override IReadOnlyList<Vector4> FindCentr()
        {
            List<Vector4> res = ZVectors[0].List.ToList();
            //var res = ZVectors[0];

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
}