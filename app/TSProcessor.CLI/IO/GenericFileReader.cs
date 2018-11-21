using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TSProcessor.CLI.IO
{
    public sealed class GenericFileReader<T> : IFileReader<T>
    {
        public T Read(string fileName)
        {
            T data;
            using (var stream = new StreamReader(fileName))
            {
                data = ServiceStack.Text.JsonSerializer.DeserializeFromReader<T>(stream);
            }

            return data;
        }
    }
}
