using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct ObjectComparer : IEqualityComparer2<object>
  {
    public static ObjectComparer Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(ObjectComparer); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public new bool Equals(object x, object y)
    {
      return object.Equals(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(object obj)
    {
      return obj is null ? 0 : obj.GetHashCode();
    }
  }
}
