using System.Runtime.CompilerServices;
using Neat.BibTeX.BibModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Provides extension methods for <see cref="IBib32EntryVisitor"/>.
  /// </summary>
  public static class BibBstVisitorsExtensions
  {
    /// <summary>
    /// Dispatches the correct method on each entry for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor<TVisitor>(this Bib32Entry[] that, ref TVisitor visitor)
      where TVisitor : struct, IBib32EntryVisitor
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(ref visitor);
      }
    }

    /// <summary>
    /// Dispatches the correct method on each entry for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor(this Bib32Entry[] that, IBib32EntryVisitor visitor)
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(visitor);
      }
    }
  }
}
