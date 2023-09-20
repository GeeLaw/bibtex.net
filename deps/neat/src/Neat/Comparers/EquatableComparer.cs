using System;
using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct EquatableComparer<T> : IEqualityComparer2<T>
    where T : IEquatable<T>
  {
    public static EquatableComparer<T> Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(EquatableComparer<T>); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(T x, T y)
    {
      return x is null ? y is null : x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(T obj)
    {
      return obj is null ? 0 : obj.GetHashCode();
    }
  }
}
