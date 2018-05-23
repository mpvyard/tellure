using System.Collections.Generic;
using System.Numerics;

namespace Riowil.Lib
{
	public class Series
	{
        
		private readonly SeriesParams seriesParams;
		private readonly List<double> points;

		public SeriesParams SeriesParams
		{
			get { return seriesParams; }
		}

		public List<double> Points
		{
			get { return points; }
		}

		public Series(SeriesParams seriesParams, List<double> points)
		{
			this.seriesParams = seriesParams;
			this.points = points;
		}

        public List<double> GetCheckSeries(int extraPoints = 0)
		{
			List<double> series = new List<double>(seriesParams.CountInCheck);
			for (int i = seriesParams.FistInCheck - extraPoints; i < seriesParams.FistInCheck + seriesParams.CountInCheck; i++)
			{
				series.Add(points[i]);
			}
			return series;
		}

		public List<double> GetTestSeries(int extraPoints = 0)
		{
			List<double> series = new List<double>(seriesParams.CountInTest);
			for (int i = seriesParams.FirstInTest - extraPoints; i < seriesParams.FirstInTest + seriesParams.CountInTest; i++)
			{
				series.Add(points[i]);
			}
			return series;
		}

		public List<double> GetTeachSeries()
		{
			List<double> series = new List<double>(seriesParams.CountInTraining);
			int i = seriesParams.FirstInTraining;

			while (series.Count < seriesParams.CountInTraining && i < seriesParams.FirstInTest)
			{
				series.Add(points[i++]);
			}

			i += seriesParams.CountInTest;

			while (series.Count < seriesParams.CountInTraining && i < seriesParams.FistInCheck)
			{
				series.Add(points[i++]);
			}

			i += seriesParams.CountInCheck;

			while (series.Count < seriesParams.CountInTraining)
			{
				series.Add(points[i++]);
			}

			return series;
		}



        //For Vector3
        private readonly List<Vector3> points3;

        public Series(SeriesParams seriesParams, List<Vector3> points3)
        {
            this.seriesParams = seriesParams;
            this.points3 = points3;
        }

        public List<Vector3> GetTeachSeries3()
        {
            List<Vector3> series = new List<Vector3>(seriesParams.CountInTraining);
            int i = seriesParams.FirstInTraining;

            while (series.Count < seriesParams.CountInTraining && i < seriesParams.FirstInTest)
            {
                series.Add(points3[i++]);
            }

            i += seriesParams.CountInTest;

            while (series.Count < seriesParams.CountInTraining && i < seriesParams.FistInCheck)
            {
                series.Add(points3[i++]);
            }

            i += seriesParams.CountInCheck;

            while (series.Count < seriesParams.CountInTraining)
            {
                series.Add(points3[i++]);
            }
            return series;
        }
        //
    }
}
