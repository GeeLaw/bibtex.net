#if BIB_MODEL_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a <c>@string{ name = "literal" # {literal} # 123 # anothername }</c> entry.
  /// </summary>
  public sealed class Bib32StringEntry : Bib32Entry
  {
    /// <summary>
    /// The entry type of a <c>@string</c> entry.
    /// </summary>
    public static readonly StringT EntryType = Utf.String16ToString32Strict("string");

    /// <summary>
    /// The name of this referenceable string.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public StringT Name;

    /// <summary>
    /// The value of this referenceable string.
    /// </summary>
    public Bib32String Value;

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
    public Bib32StringEntry(StringT type, bool isBrace, StringT name, Bib32String value)
      : base(type, isBrace)
    {
      Name = name;
      Value = value;
#if BIB_MODEL_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      if (!BibBstComparer.Equals(Type, EntryType))
      {
        throw new ArgumentException("Bib32StringEntry: Type is not \"string\".", name is null ? "type" : name);
      }
      if (!BibBstChars.IsIdentifier(Name))
      {
        throw new ArgumentException("Bib32StringEntry: Name is not a valid identifier.", name is null ? "name" : name);
      }
      Value.CtorCheckImpl(name is null ? "value" : name);
#endif
    }

    #region Bib32Entry overrides

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
    public sealed override void AcceptVisitor(IBib32EntryVisitor visitor)
    {
      visitor.VisitStringEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib32Entry overrides
  }
}
