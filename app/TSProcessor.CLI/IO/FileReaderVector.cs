using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TSProcessor.CLI.IO
{
    public class FileReaderVector : IFileReader<List<Vector<float>>>
    {
        public List<Vector<float>> Read(string fileName)
        {
            List<Vector<float>> series;
            using (var stream = new StreamReader(fileName))
            {
                var arrSeries = ServiceStack.Text.JsonSerializer.DeserializeFromReader<List<float[]>>(stream);
                series = arrSeries.Select(x => new Vector<float>(x)).ToList();
            }

            return series;
        }
    }
}
