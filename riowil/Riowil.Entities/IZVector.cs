using System;
using System.Collections.Generic;
using System.Text;

namespace Riowil.Entities
{
    public interface IZVector<T>
    {
        IReadOnlyList<T> List { get; }
        int Num { get; }
    }
}
