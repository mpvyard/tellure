namespace Domain.Entity
{
	public class PredictionQuality
	{
		public string Name { get; set; }
		public double RMSE { get; set; }
		public double MAE { get; set; }
		public int NonPredictable { get; set; }
	}
}