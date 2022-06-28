using System.Runtime.CompilerServices;
using Neat.BibTeX.BibModel;
using Neat.BibTeX.BstModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Provides extension methods for <see cref="IBib32EntryVisitor"/>, <see cref="IBib16EntryVisitor"/>, and <see cref="IBib8EntryVisitor"/>.
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

    /// <summary>
    /// Dispatches the correct method on each entry for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor<TVisitor>(this Bib16Entry[] that, ref TVisitor visitor)
      where TVisitor : struct, IBib16EntryVisitor
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
    public static void AcceptVisitor(this Bib16Entry[] that, IBib16EntryVisitor visitor)
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(visitor);
      }
    }

    /// <summary>
    /// Dispatches the correct method on each entry for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor<TVisitor>(this Bib8Entry[] that, ref TVisitor visitor)
      where TVisitor : struct, IBib8EntryVisitor
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
    public static void AcceptVisitor(this Bib8Entry[] that, IBib8EntryVisitor visitor)
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(visitor);
      }
    }

    /// <summary>
    /// Dispatches the correct method on each operator for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor<TVisitor>(this Bst32Operator[] that, ref TVisitor visitor)
      where TVisitor : struct, IBst32OperatorVisitor
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(ref visitor);
      }
    }

    /// <summary>
    /// Dispatches the correct method on each operator for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor(this Bst32Operator[] that, IBst32OperatorVisitor visitor)
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(visitor);
      }
    }
  }
}
