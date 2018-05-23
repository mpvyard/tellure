using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Riowil.Entities
{
	public class ZVector
	{
		private readonly int num;
		private readonly int[] pattern;//template 
		private readonly List<double> list;//sequence has generated from templates 

		public List<double> List
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

		public ZVector(List<double> list, int[] pattern, int num = -1)
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

		public static ZVector Parse(string str, int[] pattern)
		{
			string[] numList = str.Split(ZVectorFormat.NumSeparator);

			int num = int.Parse(numList[0]);
			List<double> list = new List<double>();

			string[] listStr = numList[1].Split(ZVectorFormat.ValueSeparator);

			foreach (string itemStr in listStr)
			{
				if (!itemStr.Equals(""))
				{
					list.Add(double.Parse(itemStr));
				}
			}

			return new ZVector(list, pattern, num);

		}
    }

	internal static class ZVectorFormat
	{
		public const char NumSeparator = '_';
		public const string ValueFormat = "0.#############;-0.#############;0";
		public const char ValueSeparator = '#';
	}




}
