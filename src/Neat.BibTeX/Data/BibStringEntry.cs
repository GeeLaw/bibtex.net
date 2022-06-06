using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a <c>@string { key = "value" # reference }</c> entry.
  /// </summary>
  public sealed class BibStringEntry<TString> : BibEntry<TString>
  {
    /// <summary>
    /// The key of this referenceable string.
    /// </summary>
    public readonly TString Key;

    /// <summary>
    /// The value of this referenceable string.
    /// </summary>
    public readonly BibString<TString> Value;

    /// <summary>
    /// <paramref name="type"/> must be <c>string</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public BibStringEntry(TString type, bool isBrace, TString key, BibString<TString> value)
      : base(type, isBrace)
    {
      Key = key;
      Value = value;
    }

    #region BibEntry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitStringEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBibEntryVisitor<TString> visitor)
    {
      visitor.VisitStringEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void PreventExternalInheritance()
    {
    }

    #endregion BibEntry overrides
  }
}
