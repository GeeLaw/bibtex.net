using System.Runtime.CompilerServices;

namespace Neat.Comparers
{
  public struct ReferentialComparer : IEqualityComparer2<object>
  {
    public static ReferentialComparer Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(ReferentialComparer); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public new bool Equals(object x, object y)
    {
      return ReferenceEquals(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(object obj)
    {
      /* This method handles the null reference. */
      return RuntimeHelpers.GetHashCode(obj);
    }
  }
}
