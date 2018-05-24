using System;
using System.Collections.Generic;
using System.Linq;
using Riowil.Entities;
using Riowil.Entities.Clusters;

namespace Riowil.Lib
{
	public class WishartAlgor
	{
		private readonly int k;
		private readonly double h;

		private List<ZVector> x;
		private List<InitialCluster> clusters;

		//для оптимизации
		private List<double> distance;
		private List<double> px;

		//параметры ZVector'ов
		private int n;
		private int dimension;

		public WishartAlgor(WishartParams param)
		{
			this.k = param.K;
			this.h = param.H;
		}

        public IReadOnlyList<InitialCluster> Clusterize(IReadOnlyList<ZVector> zVectors)//
        {
			Prepare(zVectors);

			double e = 0.000001;
			clusters.Add(new InitialCluster());
			for (int i = 0; i < n; i++)
			{
				ZVector xi = x[i];

				List<double> Ui = BuildU(i);

				//находим номера l классов, которые содержат точки, с которыми связана xi
				List<int> l = new List<int>();
				for (int j = 0; j < Ui.Count; j++)
				{
					if (Ui[j] > e) //xi связанна с xj
					{
						int wxj = w(x[j]);
						if (wxj != -1 && (!l.Contains(wxj)) && IsFar(xi, clusters[wxj]))
						{
							l.Add(wxj);
						}
					}
				}

				//(3.1)
				if (l.Count == 0)
				{
					clusters.Add(new InitialCluster(xi));
					continue;
				}
				//(3.2)
				if (l.Count == 1)
				{
					if (clusters[l[0]].IsFormed)
					{
						clusters[0].Add(xi);
					}
					else
					{
						clusters[l[0]].Add(xi);
					}
					continue;
				}
				//(3.3)
				//проверяем сколько классов сформировано
				int tmp = 1;
				for (; tmp < clusters.Count; tmp++)
				{
					if (!clusters[tmp].IsFormed)
					{ break; }
				}
				//(3.3.1)
				if (tmp == clusters.Count) //все классы сформированы
				{
					clusters[0].Add(xi);
					continue;
				}
				//(3.3.2)                
				int zh = z(l);
				l.Sort();
				//a)
				if (zh > 1 || l[0] == 0)
				{
					clusters[0].Add(xi);

					for (int j = clusters.Count - 1; j >= 1; j--)
					{
						InitialCluster c = clusters[j];
						if (Check(c))
						{
							c.IsFormed = true;
						}
						else
						{
							clusters[0].Add(c.ZVectors);
							clusters.Remove(c);
						}
					}
				}
				else //b)
				{
					for (int j = l.Count - 1; j > 0; j--)
					{
						clusters[l[0]].Add(clusters[l[j]].ZVectors);
						clusters.Remove(clusters[l[j]]);
					}
					clusters[l[0]].Add(xi);
				}

			}

			clusters.Remove(clusters[0]);
            //foreach (var cluster in clusters)
            //{
            //    cluster.SetCentr();
            //}
            return clusters;
		}

		private int CompareTupleByItem2((ZVector, double) t1, (ZVector, double) t2)
		{
			return t1.Item2.CompareTo(t2.Item2);
		}

		private void Prepare(IReadOnlyList<ZVector> zVectors)//+
		{
			if (zVectors == null || zVectors.Count == 0)
			{
				throw new ArgumentNullException("zVectors");
			}

			this.n = zVectors.Count;
			this.x = new List<ZVector>();
			this.distance = new List<double>();
			this.px = new List<double>();
			this.clusters = new List<InitialCluster>();
			this.dimension = zVectors[0].List.Count;

			x.AddRange(zVectors);
			var list = new List<(ZVector, double)>();
			for (int i = 0; i < zVectors.Count; i++)
			{
				(ZVector, double) t = (zVectors[i], dk(zVectors[i]));
				list.Add(t);
			}

			list.Sort(CompareTupleByItem2);
			x.Clear();

			for (int i = 0; i < zVectors.Count; i++)
			{
				x.Add(list[i].Item1);
				distance.Add(list[i].Item2);
				px.Add(p(i));
			}
		}

		private double dk(ZVector xi)//+
		{
			int index = x.IndexOf(xi);
			List<double> dictance = new List<double>();
			for (int i = 0; i < n; i++)
			{
				if (i != index)
				{
					dictance.Add(MathExtended.Distance(xi.List, x[i].List));
				}
			}
			dictance.Sort();
			return dictance[k];
		}

		private double p(int i)//+
		{
			return k / (Vk(i) * n);
		}

		private double Vk(int i)//+
		{
			return V(distance[i], dimension);
		}

		private double V(double R, int dimension)//+
        {
            switch (dimension)
            {
                case 1:
                    return 2 * R;
                case 2:
                    return Math.PI * R * R;
                default:
                    return 2 * Math.PI * R * R * V(R, dimension - 2) / dimension;
            }
        }

		private List<double> BuildU(int i)
		{
			List<double> U = new List<double>();
			double dK = distance[i];
			for (int j = 0; j < i; j++)
			{
				double dij = MathExtended.Distance(x[i].List, x[j].List);
				double uj = dij <= dK ? dij : 0.0;
				U.Add(uj);
			}
			return U;
		}

		private int w(ZVector zVector)
		{
			for (int i = 0; i < clusters.Count; i++)
			{
				if (clusters[i].ZVectors.Contains(zVector))
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsFar(ZVector zVector, InitialCluster c)
		{
			double dif;
			for (int j = 0; j < c.ZVectors.Count; j++)
			{
				dif = Math.Abs(zVector.Num - c.ZVectors[j].Num);
				if (dif < dimension)
				{
					return false;
				}
			}
			return true;
		}

		private int z(IEnumerable<int> l)
		{
			int z = 0;
			foreach (int i in l)
			{
				if (Check(clusters[i]))
				{
					z++;
				}
			}
			return z;
		}

		private bool Check(InitialCluster c)//Проверка на значимость
		{
			int count = c.ZVectors.Count;
			double cur;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < count; j++)
				{
					if (i == j) continue;

					cur = Math.Abs(px[i] - px[j]);
					if (cur >= h)
					{
						return true;
					}
				}
			}
			return false;
		}

    }

    public class WishartParams
	{
		public int K { get; set; }
		public double H { get; set; }
	}

}
