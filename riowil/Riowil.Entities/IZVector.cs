using System;
using System.Collections.Generic;
using System.Text;

namespace Riowil.Entities
{
    public interface IZVector<T>
    {
        List<T> List { get; }
        int Num { get; }
    }
}
