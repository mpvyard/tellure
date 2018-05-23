using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riowil.Entities.Clusters
{
    public abstract class GenericInitialCluster<TZVector, TCentr>
        where TZVector : IZVector<TCentr>
    {
        private List<TZVector> zVectors;
        private List<TCentr> centr;
        private bool isFormed;

        private bool actualCentr;

        public List<TZVector> ZVectors
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

        //public GenericInitialCluster(TZVector zVector = null)
        public GenericInitialCluster(TZVector zVector )
        {
            zVectors = new List<TZVector>();
            if (zVector != null)
            {
                zVectors.Add(zVector);
            }

            this.isFormed = false;
            this.actualCentr = false;
        }

        public void Add(IEnumerable<TZVector> c)
        {
            ZVectors.AddRange(c);
        }

        public void Add(TZVector x)
        {
            ZVectors.Add(x);
        }

        //public void Add(InitialCluster x)
        //{
        //    ZVectors.Add(x);
        //}


        public List<TCentr> Centr
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

        protected abstract List<TCentr> FindCentr();

        public Cluster<TCentr> ToCluster(int id)
        {
            return new Cluster<TCentr>
            {
                Id = id,
                Centr = Centr.ToArray()
            };
        }

        public override string ToString()
        {
            IEnumerable<string> valuesStr = zVectors.Select(x => x.ToString());
            return string.Join(ClusterFormat.ValueSeparator.ToString(), valuesStr);
        }
    }
}
