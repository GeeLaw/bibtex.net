using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a <c>@preamble { "value" # reference }</c> entry.
  /// </summary>
  public sealed class BibPreambleEntry<TString> : BibEntry<TString>
  {
    /// <summary>
    /// The preamble text.
    /// </summary>
    public readonly BibString<TString> Value;

    /// <summary>
    /// <paramref name="type"/> must be <c>preamble</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public BibPreambleEntry(TString type, bool isBrace, BibString<TString> value)
      : base(type, isBrace)
    {
      Value = value;
    }

    #region BibEntry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBibEntryVisitor<TString> visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void PreventExternalInheritance()
    {
    }

    #endregion BibEntry overrides
  }
}
