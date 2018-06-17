namespace Tellure.Entities
{
	public class Error
	{
		public int PointId { get; set; }
		public int ClusterId { get; set; }
		public double PointError { get; set; }
		public double VectorError { get; set; }
		public int Id { get; set; }
	}
}