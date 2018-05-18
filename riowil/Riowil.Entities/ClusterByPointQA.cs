namespace Riowil.Entities
{
	public class ClusterByPointQA
	{
		public int ClusterId { get; set; }
		public int PointNum { get; set; }

		public double Error { get; set; }
		public double Distance { get; set; }
		public int Group { get; set; }
	}
}