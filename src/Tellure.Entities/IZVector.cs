using System;
using System.Collections.Generic;
using System.Text;

namespace Tellure.Entities
{
    public interface IZVector<T>
    {
        IReadOnlyList<T> List { get; }
        int Num { get; }
    }
}
