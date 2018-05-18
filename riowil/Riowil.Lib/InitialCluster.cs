using System.Collections.Generic;
using System.Linq;
using Riowil.Entities;

namespace Riowil.Lib
{
	public class InitialCluster
	{
		private List<ZVector> zVectors;
		private ZVector centr;
		private bool isFormed;

		private bool actualCentr;

		public List<ZVector> ZVectors
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

		public ZVector Centr
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

		public InitialCluster(ZVector zVector = null)
		{
			zVectors = new List<ZVector>();
			if (zVector != null)
			{
				zVectors.Add(zVector);
			}

			this.isFormed = false;
			this.actualCentr = false;
		}

		public void Add(InitialCluster c)
		{
			ZVectors.AddRange(c.ZVectors);
		}

		public void Add(ZVector x)
		{
			ZVectors.Add(x);
		}

		public Cluster ToCluster(int id)
		{
			return new Cluster
			{
				Id = id,
				Centr = Centr.List.ToArray()
			};
		}

		private ZVector FindCentr()
		{
			List<double> res = zVectors[0].List.ToList();

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

			return new ZVector(res, zVectors[0].Pattern);
		}

		public override string ToString()
		{
			IEnumerable<string> valuesStr = zVectors.Select(x => x.ToString());
			return string.Join(ClusterFormat.ValueSeparator.ToString(), valuesStr);
		}

		public static InitialCluster Parse(string str, int[] pattern)
		{
			InitialCluster cluster = new InitialCluster();
			string[] zVectorsStrings = str.Split(ClusterFormat.ValueSeparator);
			foreach (string zVectorString in zVectorsStrings)
			{
				if (str != "")
				{
					cluster.Add(ZVector.Parse(zVectorString, pattern));
				}
			}

			return cluster;
		}
	}

	internal static class ClusterFormat
	{
		public const char ValueSeparator = ';';
	}
}