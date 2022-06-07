using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents an entry in <c>.bib</c> files.
  /// An instance could be <see cref="Bib32StringEntry"/>,
  /// <see cref="Bib32PreambleEntry"/>, or
  /// <see cref="Bib32GeneralEntry"/>
  /// (e.g., <c>@entry_type{ ... }</c>).
  /// </summary>
  public abstract class Bib32Entry
  {
    /// <summary>
    /// Indicates the type of this entry.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// The special types are <c>string</c>, <c>preamble</c>, and <c>comment</c>.
    /// However, since comment entries are not modeled, this string should never be <c>comment</c>.
    /// </summary>
    public readonly String32 Type;

    /// <summary>
    /// Indicates whether the entry is opened by a brace.
    /// If this field is <see langword="false"/>, then the entry is opened by a parenthesis.
    /// </summary>
    public readonly bool IsBrace;

    [MethodImpl(Helper.OptimizeInline)]
    private protected Bib32Entry(String32 type, bool isBrace)
    {
      Type = type;
      IsBrace = isBrace;
    }

#if BIB_DATA_CHECKS

    /// <summary>
    /// Checks the validity of this instance.
    /// </summary>
    internal abstract void CtorCheckImpl(string name);

#endif

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    public abstract void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib32EntryVisitor;

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    public abstract void AcceptVisitor(IBib32EntryVisitor visitor);

    /// <summary>
    /// This method ensures only this assembly can create non-abstract derived classes of this class.
    /// </summary>
    private protected abstract void InternalInheritance();
  }
}
