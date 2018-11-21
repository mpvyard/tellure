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
		private readonly List<Vector4> list;//sequence has generated from templates 

		public IReadOnlyList<Vector4> List
		{
			get { return list; }
		}

        public int Num { get; private set; }

        public ZVector(List<Vector4> list, int num = -1)
		{
			this.list = list;
            Num = num;
		}
    }
}
