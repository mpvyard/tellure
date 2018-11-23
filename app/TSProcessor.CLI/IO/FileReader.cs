using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TSProcessor.CLI.IO
{
    sealed class FileReader
    {
        public T Read<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentOutOfRangeException(nameof(fileName));
            }

            T data;
            using (var stream = new StreamReader(fileName))
            {
                data = ServiceStack.Text.JsonSerializer.DeserializeFromReader<T>(stream);
            }

            return data;
        }
    }
}
