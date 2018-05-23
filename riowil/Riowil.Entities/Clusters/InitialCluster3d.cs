using System.Collections.Generic;
using System.Linq;
using Riowil.Entities;
using System.Numerics;

namespace Riowil.Entities.Clusters
{
    public class InitialCluster3d : GenericInitialCluster<ZVector3d, Vector3>
    {
        public InitialCluster3d(ZVector3d zVector = null)
            : base(zVector)
        {
        }

        protected override List<Vector3> FindCentr()
        {
            List<Vector3> res = ZVectors[0].List.ToList();

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
