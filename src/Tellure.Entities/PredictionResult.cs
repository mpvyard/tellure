using System.Collections.Generic;

namespace Tellure.Entities
{
	public class PredictionResult
	{
		public List<double> PredictedPoints { get; set; }
		public List<double> RealPoints { get; set; }
		public List<int> NonPredictablePositions { get; set; } 
	}
}