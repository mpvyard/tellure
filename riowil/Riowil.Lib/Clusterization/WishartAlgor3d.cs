using System;
using System.Collections.Generic;
using System.Text;
using Riowil.Entities;
using Riowil.Entities.Clusters;

namespace Riowil.Lib
{
    public class WishartAlgor3d : IClusterizeAlgor<ZVector3d>
    {
        private readonly int k;
        private readonly double h;

        private List<ZVector3d> x;
        private List<InitialCluster3d> clusters;

        //для оптимизации
        private List<double> distance;
        private List<double> px;

        //параметры ZVector'ов
        private int n;
        private int dimension;

        public WishartAlgor3d(WishartParams param)
        {
            this.k = param.K;
            this.h = param.H;
        }
        //For Vector3
        public List<InitialCluster3d> Clusterize(List<ZVector3d> zVectors)
        {
            Prepare3(zVectors);

            double e = 0.000001;
            clusters.Add(new InitialCluster3d());
            for (int i = 0; i < n; i++)
            {
                ZVector3d xi = x[i];

                List<double> Ui = BuildU3(i);//Строим вершины графа 

                //находим номера l классов, которые содержат точки, с которыми связана xi
                List<int> l = new List<int>();
                for (int j = 0; j < Ui.Count; j++)
                {
                    if (Ui[j] > e) //xi связанна с xj
                    {
                        int wxj = w3(x[j]);
                        if (wxj != -1 && (!l.Contains(wxj)) && IsFar(xi, clusters[wxj]))
                        {
                            l.Add(wxj);
                        }
                    }
                }

                //(3.1)
                if (l.Count == 0)
                {
                    clusters.Add(new InitialCluster3d(xi));
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
                int zh = z3(l);
                l.Sort();
                //a)
                if (zh > 1 || l[0] == 0)
                {
                    clusters[0].Add(xi);

                    for (int j = clusters.Count - 1; j >= 1; j--)
                    {
                        InitialCluster3d c = clusters[j];
                        if (Check3(c))
                        {
                            c.IsFormed = true;
                        }
                        else
                        {
                            clusters[0].Add(c);
                            clusters.Remove(c);
                        }
                    }
                }
                else //b)
                {
                    for (int j = l.Count - 1; j > 0; j--)
                    {
                        clusters[l[0]].Add(clusters[l[j]]);
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

        private int CompareTupleByItem2(Tuple<ZVector3d, double> t1, Tuple<ZVector3d, double> t2)
        {
            return t1.Item2.CompareTo(t2.Item2);
        }

        //begin Prepare3()
        private void Prepare3(List<ZVector3d> zVectors)
        {
            if (zVectors == null || zVectors.Count == 0)
            {
                throw new ArgumentNullException("zVectors");
            }

            this.n = zVectors.Count;//количество zвекторов
            this.x = new List<ZVector3d>();//Список zвекторов
            this.distance = new List<double>();//растояние iго вектора до к ближайшего соседа
            this.px = new List<double>();//плотность для к ближайших соседей
            this.clusters = new List<InitialCluster3d>();
            this.dimension = zVectors[0].List.Count;//количество точек в zвекторе

            x.AddRange(zVectors);
            List<Tuple<ZVector3d, double>> list = new List<Tuple<ZVector3d, double>>();
            for (int i = 0; i < zVectors.Count; i++)
            {
                Tuple<ZVector3d, double> t = new Tuple<ZVector3d, double>(zVectors[i], dk3(zVectors[i]));
                list.Add(t);
            }

            list.Sort(CompareTupleByItem2);
            x.Clear();

            for (int i = 0; i < zVectors.Count; i++)
            {
                x.Add(list[i].Item1);
                distance.Add(list[i].Item2);
                px.Add(p3(i));
            }
        }

        private double dk3(ZVector3d xi)//подсчет растояние zвектора xi со всеми векторами
        {
            int index = x.IndexOf(xi);
            List<double> dictance = new List<double>();
            for (int i = 0; i < n; i++)
            {
                if (i != index)
                {
                    dictance.Add(MathExtended.Distance3(xi.List, x[i].List));
                }
            }
            dictance.Sort();
            return dictance[k];
        }

        private double p3(int i)//Плотность
        {
            return k / (Vk3(i) * n);
        }

        private double Vk3(int i)
        {
            return V3(distance[i], dimension);//dimension == zVectors[0].List3.Count
        }

        private double V3(double R, int dimension)//Объем минимального гипершара
        {
            switch (dimension)
            {
                case 1:
                    return 2 * R;
                case 2:
                    return Math.PI * R * R;
                default:
                    return 2 * Math.PI * R * R * V3(R, dimension - 2) / dimension;
            }
        }
        //end Prepare3()

        private List<double> BuildU3(int i)//(Строим вершину графа) возвращает список растояний i го zвектора с 
        {
            List<double> U = new List<double>();
            double dK = distance[i];
            for (int j = 0; j < i; j++)
            {
                double dij = MathExtended.Distance3(x[i].List, x[j].List);
                double uj = dij <= dK ? dij : 0.0;
                U.Add(uj);
            }
            return U;
        }

        private int w3(ZVector3d zVector)//возвращает номер кластера в котором содержиться zvector, если zvector не содержится ни в одном из кластеров, возврщ -1
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

        private int z3(IEnumerable<int> l)//количество значимых классов
        {
            int z = 0;
            foreach (int i in l)
            {
                if (Check3(clusters[i]))
                {
                    z++;
                }
            }
            return z;
        }
        private bool Check3(InitialCluster3d c)//Проверка на значимость
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

        public bool IsFar(ZVector3d zVector, InitialCluster3d c)
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

    }
}
