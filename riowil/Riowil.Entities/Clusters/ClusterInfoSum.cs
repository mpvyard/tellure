using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riowil.Entities.Clusters
{
	public class ClusterInfoSum
	{
		public int FromPointId { get; set; }
		public int ToPointId { get; set; }
		public int ClusterId { get; set; }
		public double ErrorAdd { get; set; }
		public double ErrorAvoid { get; set; }
		public int Best { get; set; }
		public int Selected { get; set; }
		public int Far { get; set; }
		public int Count { get; set; }

		public ClusterInfoSum() { }
		public ClusterInfoSum(ClusterInfoSum sum)
		{
			FromPointId = sum.FromPointId;
			ToPointId = sum.ToPointId;
			ClusterId = sum.ClusterId;
			ErrorAdd = sum.ErrorAdd;
			ErrorAvoid = sum.ErrorAvoid;
			Best = sum.Best;
			Selected = sum.Selected;
			Far = sum.Far;
			Count = sum.Count;
		}
	
		public ClusterInfoSum(ClusterInfoByPoint info)
		{
			FromPointId = info.PointId;
			ToPointId = info.PointId;
			ClusterId = info.ClusterId;
			if (info.Best)
			{
				ErrorAdd = 0.0;
				ErrorAvoid = info.ErrorDif;
				Best = 1;
			}
			else
			{
				ErrorAdd = info.ErrorDif;
				ErrorAvoid = 0.0;
				Best = 0;
			}
			Selected = info.Selected ? 1 : 0;
			Far = info.Far ? 1 : 0;
			Count = 1;
		}

		public ClusterInfoSum ConvertToSum(ClusterInfoByPoint info)
		{
			return new ClusterInfoSum(info);
		}

		public ClusterInfoSum Add(ClusterInfoSum sum)
		{
			return new ClusterInfoSum
			{
				ClusterId = this.ClusterId,
				Best = Best + sum.Best,
				ErrorAdd = ErrorAdd+sum.ErrorAdd,
				ErrorAvoid = ErrorAvoid+sum.ErrorAvoid,
				Far = Far + sum.Far,
				FromPointId = Math.Min(FromPointId, sum.FromPointId),
				ToPointId = Math.Max(ToPointId, sum.ToPointId),
				Selected = Selected+sum.Selected,
				Count = Count + sum.Count
			};
		}

		public ClusterInfoSum Add(ClusterInfoByPoint info)
		{
			return Add(new ClusterInfoSum(info));
		}
	}
}
