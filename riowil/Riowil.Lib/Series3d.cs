using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Riowil.Lib
{
    class Series3d
    {
        private readonly SeriesParams seriesParams;
        private readonly List<Vector3> points3;

        public Series3d(SeriesParams seriesParams, List<Vector3> points3)
        {
            this.seriesParams = seriesParams;
            this.points3 = points3;
        }

        public List<Vector3> GetTeachSeries()
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
    }
}
