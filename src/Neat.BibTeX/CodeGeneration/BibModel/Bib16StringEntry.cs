using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = System.String;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a <c>@string{ name = "literal" # {literal} # 123 # anothername }</c> entry.
  /// </summary>
  public sealed class Bib16StringEntry : Bib16Entry
  {
    /// <summary>
    /// The entry type of a <c>@string</c> entry.
    /// </summary>
    public const StringT EntryType = "string";

    /// <summary>
    /// The name of this referenceable string.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public StringT Name;

    /// <summary>
    /// The value of this referenceable string.
    /// </summary>
    public Bib16String Value;

    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      bool isBrace = IsBrace;
      /* @string{ name = value } */
      return Value.ToString(new StringBuilder()
        .Append('@')
        .Append(Type.GenericToString())
        .Append(isBrace ? "{ " : "( ")
        .Append(Name.GenericToString())
        .Append(" = ")
      ).Append(isBrace ? " }" : " )").ToString();
    }

    /// <param name="type">Must be <c>string</c> (in any casing).</param>
    /// <param name="name">Must be a valid identifier.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib16StringEntry(StringT type, bool isBrace, StringT name, Bib16String value)
      : base(type, isBrace)
    {
      Name = name;
      Value = value;
    }

    #region Bib16Entry overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, EntryType)
        && BibBstChars.IsIdentifier(Name)
        && Value.IsValid();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitStringEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBib16EntryVisitor visitor)
    {
      visitor.VisitStringEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib16Entry overrides
  }
}
