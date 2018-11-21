using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TSProcessor.CLI.IO
{
    public class FileReaderVector4 : IFileReader<List<Vector4>>
    {
        public List<Vector4> Read(string fileName)
        {
            List<Vector4> series;
            using (var stream = new StreamReader(fileName))
            {
                /*
                 * this option is added to save backward compatibility with models formated
                 * like [1, 2, 3, ... ]
                 */
                try
                {
                    var arrSeries = ServiceStack.Text.JsonSerializer.DeserializeFromReader<List<float>>(stream);
                    series = arrSeries.Select(x => new Vector4(float.NaN) { X = x }).ToList();
                }
                catch (FormatException)
                {
                    var arrSeries = ServiceStack.Text.JsonSerializer.DeserializeFromReader<List<float[]>>(stream);
                    series = arrSeries.Select(x => ConvertToVector4(x)).ToList();
                }
            }

            return series;
        }

        private static Vector4 ConvertToVector4(float[] arr)
        {
            if (arr.Length > 4)
            {
                throw new InvalidOperationException("Use FileReadedVector intead of FileReaderVector4, it seams that your vector is bigger that 4 dimensions");
            }
            var vec = new Vector4(float.NaN);
            vec.X = arr[0];
            switch (arr.Length)
            {
                case 2:
                    vec.Y = arr[1];
                    break;
                case 3:
                    vec.Y = arr[1];
                    vec.Z = arr[2];
                    break;
                case 4:
                    vec.Y = arr[1];
                    vec.Z = arr[2];
                    vec.W = arr[3];
                    break;
                default:
                    break;
            }

            return vec;
        }
    }


}
