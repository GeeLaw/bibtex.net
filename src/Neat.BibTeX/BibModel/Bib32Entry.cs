using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents an entry in <c>.bib</c> files.
  /// An instance could be <see cref="Bib32StringEntry"/>,
  /// <see cref="Bib32PreambleEntry"/>, or
  /// <see cref="Bib32GeneralEntry"/>
  /// (e.g., <c>@type{ ... }</c>).
  /// </summary>
  public abstract class Bib32Entry
  {
    /// <summary>
    /// Indicates the type of this entry.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// The special types are <c>string</c>, <c>preamble</c>, and <c>comment</c>.
    /// However, since comment entries are not modeled, this string should never be <c>comment</c>.
    /// </summary>
    public StringT Type;

    /// <summary>
    /// Indicates whether this entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    public bool IsBrace;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    public abstract override string ToString();

    [MethodImpl(Helper.OptimizeInline)]
    private protected Bib32Entry(StringT type, bool isBrace)
    {
      Type = type;
      IsBrace = isBrace;
    }

    /// <summary>
    /// Determines whether the entry is currently valid.
    /// </summary>
    public abstract bool IsValid();

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
