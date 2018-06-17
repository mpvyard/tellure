namespace Tellure.Entities.Clusters
{
	public class ClusterInfoByPoint
	{
		public ClusterInfoByPoint()
		{
			ErrorDif = 0;
			Best = false;
			Selected = false;
			Far = false;
		}
		public int PointId { get; set; }
		public int ClusterId { get; set; }
		public double ErrorDif { get; set; }
		public bool Best { get; set; }
		public bool Selected { get; set; }
		public bool Far { get; set; }
		public int Id { get; set; }
	}
}
