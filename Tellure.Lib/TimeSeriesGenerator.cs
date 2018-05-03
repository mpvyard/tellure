using System.Collections.Generic;
using System.Numerics;

namespace Tellure.Lib
{
    public class TimeSeriesGenerator
    {
        private LorenzSystem _system;
        public TimeSeriesGenerator(LorenzSystem system)
        {
            _system = system;
        }

        public TimeSeriesGenerator(float sigma, float r, float b)
            : this(new LorenzSystem(sigma, r, b)) { }

        //TODO: try to use Span<T> for generation
        public List<Vector3> Generate(Vector3 y0, float step, int count)
        {
            Vector3 k1, k2, k3, k4;
            List<Vector3> result = new List<Vector3>
            {
                y0
            };

            for (int i = 0; i < count; i++)
            {
                k1.X = _system.X(result[i].X, result[i].Y) * step;
                k1.Y = _system.Y(result[i].X, result[i].Y, result[i].Z) * step;
                k1.Z = _system.Z(result[i].X, result[i].Y, result[i].Z) * step;
                                           
                k2.X = _system.X(result[i].X + 0.5f * k1.X, result[i].Y + 0.5f * k1.Y) * step;
                k2.Y = _system.Y(result[i].X + 0.5f * k1.X, result[i].Y + 0.5f * k1.Y, result[i].Z + 0.5f * k1.Z) * step;
                k2.Z = _system.Z(result[i].X + 0.5f * k1.X, result[i].Y + 0.5f * k1.Y, result[i].Z + 0.5f * k1.Z) * step;

                k3.X = _system.X(result[i].X + 0.5f * k2.X, result[i].Y + 0.5f * k2.Y) * step;
                k3.Y = _system.Y(result[i].X + 0.5f * k2.X, result[i].Y + 0.5f * k2.Y, result[i].Z + 0.5f * k2.Z) * step;
                k3.Z = _system.Z(result[i].X + 0.5f * k2.X, result[i].Y + 0.5f * k2.Y, result[i].Z + 0.5f * k2.Z) * step;

                k4.X = _system.X(result[i].X + k3.X, result[i].Y + k3.Y) * step;
                k4.Y = _system.Y(result[i].X + k3.X, result[i].Y + k3.Y, result[i].Z + k3.Z) * step;
                k4.Z = _system.Z(result[i].X + k3.X, result[i].Y + k3.Y, result[i].Z + k3.Z) * step;

                result.Add(new Vector3(               
                    result[i].X + (k1.X + 2.0f * k2.X + 2.0f * k3.X + k4.X) / 6.0f,
                    result[i].Y + (k1.Y + 2.0f * k2.Y + 2.0f * k3.Y + k4.Y) / 6.0f,
                    result[i].Z + (k1.Z + 2.0f * k2.Z + 2.0f * k3.Z + k4.Z) / 6.0f
                ));
            }
            return result;
        }
    }
}
