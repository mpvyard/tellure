using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Riowil.Entities
{
    public class ZVector3d : IZVector<Vector3>
    {
        private readonly int num;
        private readonly int[] pattern;//template 
        private readonly List<Vector3> list;//sequence has generated from templates 

        public List<Vector3> List
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

        public ZVector3d(List<Vector3> list, int[] pattern, int num = -1)
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
}
