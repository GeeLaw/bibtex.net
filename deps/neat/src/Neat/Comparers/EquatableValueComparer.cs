using System;
using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct EquatableValueComparer<T> : IEqualityComparer2<T>, IEqualityComparer2<T?>
    where T : struct, IEquatable<T>
  {
    public static EquatableValueComparer<T> Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(EquatableValueComparer<T>); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(T x, T y)
    {
      return x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(T obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(T? x, T? y)
    {
      return x.HasValue
        ? y.HasValue && x.GetValueOrDefault().Equals(y.GetValueOrDefault())
        : !y.HasValue;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(T? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }
  }
}
