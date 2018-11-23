using System;
using System.IO;

namespace TSProcessor.CLI.IO
{
    sealed class FileWriter
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
    }
}
