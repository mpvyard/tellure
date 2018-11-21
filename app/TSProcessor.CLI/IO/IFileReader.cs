using System;
using System.Collections.Generic;
using System.Text;

namespace TSProcessor.CLI.IO
{
    public interface IFileReader<T>
    {
        T Read(string fileName);
    }
}
