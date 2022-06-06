using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents an entry in <c>.bib</c> files.
  /// An instance could be <see cref="BibStringEntry{TString}"/>,
  /// <see cref="BibPreambleEntry{TString}"/>,
  /// <see cref="BibCommentEntry{TString}"/>, or
  /// <see cref="BibGeneralEntry{TString}"/>.
  /// </summary>
  public abstract class BibEntry<TString>
  {
    /// <summary>
    /// Indicates the type of this entry.
    /// This string consists of only lower-case <c>a</c> to <c>z</c> characters.
    /// The special types are <c>string</c>, <c>preamble</c>, and <c>comment</c>.
    /// </summary>
    public readonly TString Type;

    /// <summary>
    /// Indicates whether the entry is opened by a brace.
    /// If this field is <see langword="false"/>, then the entry is opened by a parenthesis.
    /// </summary>
    public readonly bool IsBrace;

    [MethodImpl(Helper.OptimizeInline)]
    private protected BibEntry(TString type, bool isBrace)
    {
      Type = type;
      IsBrace = isBrace;
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    public abstract void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBibEntryVisitor<TString>;

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    public abstract void AcceptVisitor(IBibEntryVisitor<TString> visitor);

    /// <summary>
    /// Prevents outsiders from deriving from this class.
    /// </summary>
    private protected abstract void PreventExternalInheritance();
  }
}
