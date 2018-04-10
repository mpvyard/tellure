using System;
using System.Collections.Generic;
using System.Text;

namespace Tellure.Lib
{
    class LorenzSystem
    {
        private double Sigma { get; set; }
        private double r { get; set; }
        private double b { get; set; }

        public LorenzSystem(double sigma, double r, double b)
        {
	        this.Sigma = sigma;
	        this.r = r;
	        this.b = b;
        }

        public double X(double x, double y)
        {
	        return Sigma * (y - x);
        }

        public double Y(double x, double y, double z)
        {
	        return x * (r - z) - y;
        }

        public double Z(double x, double y, double z)
        {
	        return x * y - b * z;
        }
	}
}
