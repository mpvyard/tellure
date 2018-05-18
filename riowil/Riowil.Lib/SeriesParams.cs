using System.ComponentModel;
using Riowil.Entities;

namespace Riowil.Lib
{
	public class SeriesParams
	{
		public SeriesParams() { }

		public SeriesParams(SeriesParams seriesParams)
		{
			this.Type = seriesParams.Type;
			this.FirstInTest = seriesParams.FirstInTest;
			this.FirstInTraining = seriesParams.FirstInTraining;
			this.FistInCheck = seriesParams.FistInCheck;
			this.CountInCheck = seriesParams.CountInCheck;
			this.CountInTraining = seriesParams.CountInTraining;
			this.CountInTest = seriesParams.CountInTest;
			this.Category = seriesParams.Category;
		}

		private int count = 0;

		public SeriesType Type { get; set; }

		public int Count
		{
			get
			{
				return count == 0 ? CountInTest + CountInTraining + CountInCheck : count;
			}
			set
			{
				count = value;
			}
		}

		public int FirstInTest { get; set; }
		public int CountInTest { get; set; }

		public int FirstInTraining { get; set; }
		public int CountInTraining { get; set; }

		public int FistInCheck { get; set; }
		public int CountInCheck { get; set; }

		[DefaultValue("")]
		public string Category { get; set; }

		[DefaultValue("")]
		public string Name { get; set; }

		public bool IsPerturbed { get; set; }
		public double Eps { get; set; }
	}
}