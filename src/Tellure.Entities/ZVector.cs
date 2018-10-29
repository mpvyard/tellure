using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Tellure.Entities
{
	public class ZVector
	{
		private readonly List<Vector<float>> list;//sequence has generated from templates 

		public IReadOnlyList<Vector<float>> List
		{
			get { return list; }
		}

		public ZVector(List<Vector<float>> list)
		{
			this.list = list;
		}
    }
}
