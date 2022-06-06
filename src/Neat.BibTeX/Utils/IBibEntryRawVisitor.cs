using System.Runtime.CompilerServices;
using Neat.BibTeX.Data;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="BibEntry{TString}"/>.
  /// </summary>
  public interface IBibEntryVisitor<TString>
  {
    void VisitStringEntry(BibStringEntry<TString> entry);
    void VisitPreambleEntry(BibPreambleEntry<TString> entry);
    void VisitCommentEntry(BibCommentEntry<TString> entry);
    void VisitGeneralEntry(BibGeneralEntry<TString> entry);
  }

  /// <summary>
  /// Provides extension methods for <see cref="IBibEntryVisitor{TString}"/>.
  /// </summary>
  public static class IBibEntryVisitor
  {
    /// <summary>
    /// Dispatches the correc method on each entry for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor<TString, TVisitor>(this BibEntry<TString>[] that, ref TVisitor visitor)
      where TVisitor : struct, IBibEntryVisitor<TString>
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(ref visitor);
      }
    }

    /// <summary>
    /// Dispatches the correc method on each entry for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void AcceptVisitor<TString>(this BibEntry<TString>[] that, IBibEntryVisitor<TString> visitor)
    {
      for (int i = 0; i < that.Length; ++i)
      {
        that[i].AcceptVisitor(visitor);
      }
    }
  }
}
