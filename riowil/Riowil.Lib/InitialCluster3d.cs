using System.Collections.Generic;
using System.Linq;
using Riowil.Entities;
using System.Numerics;

namespace Riowil.Lib
{
    public class InitialCluster3d
    {
        private List<ZVector3d> zVectors;
        private ZVector3d centr;
        private bool isFormed;

        private bool actualCentr;

        public List<ZVector3d> ZVectors
        {
            get { return zVectors; }
            set
            {
                zVectors = value;
                actualCentr = false;
            }
        }

        public bool IsFormed
        {
            get { return isFormed; }
            set { isFormed = value; }
        }

        public ZVector3d Centr
        {
            get
            {
                if (!actualCentr)
                {
                    centr = FindCentr();
                    actualCentr = true;
                }
                return centr;
            }
        }

        public InitialCluster3d(ZVector3d zVector = null)
        {
            zVectors = new List<ZVector3d>();
            if (zVector != null)
            {
                zVectors.Add(zVector);
            }

            this.isFormed = false;
            this.actualCentr = false;
        }

        public void Add(InitialCluster3d c)
        {
            ZVectors.AddRange(c.ZVectors);
        }

        public void Add(ZVector3d x)
        {
            ZVectors.Add(x);
        }

        public Cluster3d ToCluster(int id)
        {
            return new Cluster3d
            {
                Id = id,
                Centr = Centr.List.ToArray()
            };
        }

        public override string ToString()
        {
            IEnumerable<string> valuesStr = zVectors.Select(x => x.ToString());
            return string.Join(ClusterFormat.ValueSeparator.ToString(), valuesStr);
        }

        public static InitialCluster3d Parse(string str, int[] pattern)
        {
            InitialCluster3d cluster = new InitialCluster3d();
            string[] zVectorsStrings = str.Split(ClusterFormat.ValueSeparator);
            foreach (string zVectorString in zVectorsStrings)
            {
                if (str != "")
                {
                    cluster.Add(ZVector3d.Parse(zVectorString, pattern));
                }
            }

            return cluster;
        }

        private ZVector3d FindCentr()
        {
            List<Vector3> res = zVectors[0].List.ToList();

            for (int i = 1; i < zVectors.Count; i++)
            {
                for (int j = 0; j < zVectors[i].List.Count; j++)
                {
                    res[j] += zVectors[i].List[j];
                }
            }

            for (int j = 0; j < zVectors[0].List.Count; j++)
            {
                res[j] /= zVectors.Count;
            }

            return new ZVector3d(res, zVectors[0].Pattern);
        }
    }
}
