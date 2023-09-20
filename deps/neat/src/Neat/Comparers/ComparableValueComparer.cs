using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct ComparableValueComparer<T> : IComparer<T>, IComparer<T?>
    where T : struct, IComparable<T>
  {
    public static ComparableValueComparer<T> Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(ComparableValueComparer<T>); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(T x, T y)
    {
      return x.CompareTo(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(T? x, T? y)
    {
      return x.HasValue
        ? y.HasValue
          ? x.GetValueOrDefault().CompareTo(y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }
  }
}
