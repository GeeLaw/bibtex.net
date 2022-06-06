using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a <c>@comment { ... }</c> entry.
  /// </summary>
  public sealed class BibCommentEntry<TString> : BibEntry<TString>
  {
    /// <summary>
    /// The verbatim content between <c>{}</c> or <c>()</c>.
    /// </summary>
    public readonly TString Value;

    /// <summary>
    /// <paramref name="type"/> must be <c>comment</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public BibCommentEntry(TString type, bool isBrace, TString value)
      : base(type, isBrace)
    {
      Value = value;
    }

    #region BibEntry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitCommentEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBibEntryVisitor<TString> visitor)
    {
      visitor.VisitCommentEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void PreventExternalInheritance()
    {
    }

    #endregion BibEntry overrides
  }
}
