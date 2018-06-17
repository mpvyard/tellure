namespace Tellure.Generator
{
    public struct LorenzSystem
    {
        private float Sigma { get; set; }
        private float R { get; set; }
        private float B { get; set; }

        public LorenzSystem(float sigma, float r, float b)
        {
	        Sigma = sigma;
	        R = r;
	        B = b;
        }

        public float X(float x, float y)
        {
	        return Sigma * (y - x);
        }

        public float Y(float x, float y, float z)
        {
	        return x * (R - z) - y;
        }

        public float Z(float x, float y, float z)
        {
	        return x * y - B * z;
        }
	}
}
