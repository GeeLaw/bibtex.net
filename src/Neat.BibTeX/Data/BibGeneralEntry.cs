using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a general <c>@entrytype { ... }</c> entry.
  /// </summary>
  public sealed class BibGeneralEntry<TString> : BibEntry<TString>
  {
    /// <summary>
    /// The citation key of this entry.
    /// </summary>
    public readonly TString Key;

    /// <summary>
    /// The fields of this entry.
    /// </summary>
    public readonly BibField<TString>[] Fields;

    /// <summary>
    /// <paramref name="type"/> must not be any of <c>string</c>, <c>preamble</c>, or <c>comment</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public BibGeneralEntry(TString type, bool isBrace, TString key, BibField<TString>[] fields)
      : base(type, isBrace)
    {
      Key = key;
      Fields = fields;
    }

    #region BibEntry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitGeneralEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBibEntryVisitor<TString> visitor)
    {
      visitor.VisitGeneralEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void PreventExternalInheritance()
    {
    }

    #endregion BibEntry overrides
  }
}
