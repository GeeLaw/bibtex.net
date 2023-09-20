using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct ComparableComparer<T> : IComparer<T>
    where T : IComparable<T>
  {
    public static ComparableComparer<T> Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(ComparableComparer<T>); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(T x, T y)
    {
      return x is null
        ? y is null
          ? 0
          : -1
        : y is null
          ? 1
          : x.CompareTo(y);
    }
  }
}
