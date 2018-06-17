using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Tellure.Entities
{
	public class ZVector : IZVector<float>
	{
		private readonly int num;
		private readonly int[] pattern;//template 
		private readonly List<float> list;//sequence has generated from templates 

		public IReadOnlyList<float> List
		{
			get { return list; }
		}

		public int[] Pattern
		{
			get { return pattern; }
		}

		public int Num
		{
			get { return num; }
		}

		public ZVector(List<float> list, int[] pattern, int num = -1)
		{
			this.list = list;
			this.pattern = pattern;
			this.num = num;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(num.ToString());
			sb.Append(ZVectorFormat.NumSeparator);

			IEnumerable<string> valuesStr = list.Select(x => x.ToString(ZVectorFormat.ValueFormat));
			sb.Append(string.Join(ZVectorFormat.ValueSeparator.ToString(), valuesStr));

			return sb.ToString();
		}
    }

	internal static class ZVectorFormat
	{
		public const char NumSeparator = '_';
		public const string ValueFormat = "0.#############;-0.#############;0";
		public const char ValueSeparator = '#';
	}
}
