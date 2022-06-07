#if BIB_DATA_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a <c>@string{ key = "literal" # {literal} # 123 # reference }</c> entry.
  /// </summary>
  public sealed class Bib32StringEntry : Bib32Entry
  {
    public static readonly String32 EntryType = Utf.String16ToString32Strict("string");

    /// <summary>
    /// The key of this referenceable string.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public readonly String32 Key;

    /// <summary>
    /// The value of this referenceable string.
    /// </summary>
    public readonly Bib32String Value;

    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      /* @string{ key = value } */
      return Value.ToString(new StringBuilder()
        .Append('@')
        .Append(Type.ToString())
        .Append("{ ")
        .Append(Key.ToString())
        .Append(" = ")
      ).Append(" }").ToString();
    }

    /// <summary>
    /// <paramref name="type"/> must be <c>string</c> (in any casing).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32StringEntry(String32 type, String32 key, Bib32String value)
      : base(type)
    {
      Key = key;
      Value = value;
#if BIB_DATA_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal sealed override void CtorCheckImpl(string name)
    {
      if (!BibBstComparer.Equals(Type, EntryType))
      {
        throw new ArgumentException("Bib32StringEntry: Type is not \"string\".", name is null ? "type" : name);
      }
      if (!BibBstChars.IsIdentifier(Key))
      {
        throw new ArgumentException("Bib32StringEntry: Key is not a valid identifier.", name is null ? "key" : name);
      }
      Value.CtorCheckImpl(name is null ? "value" : name);
#endif
    }

    #region Bib32Entry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisiString32Entry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBib32EntryVisitor visitor)
    {
      visitor.VisiString32Entry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib32Entry overrides
  }
}
