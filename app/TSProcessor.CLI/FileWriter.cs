using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TSProcessor.CLI
{
    class FileWriter
    {
        public void Write<T>(T model, string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentOutOfRangeException(nameof(file));
            }

            WriteToJson(model, file);
        }

        private void WriteToJson<T>(T model, string file)
        {
            using (var writer = new StreamWriter(file))
            {
                ServiceStack.Text.JsonSerializer.SerializeToWriter(model, writer);
            }
        }

        private void WriteToCsv<T>(T model, string file)
        {
            using (var writer = new StreamWriter(file))
            {
                ServiceStack.Text.CsvSerializer.SerializeToWriter(model, writer);
            }
        }
    }
}
