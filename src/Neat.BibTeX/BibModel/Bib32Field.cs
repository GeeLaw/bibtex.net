#if BIB_MODEL_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a field in a <see cref="Bib32GeneralEntry"/> (e.g., <c>name1 = {literal} # "literal" # 123 # name</c>).
  /// </summary>
  public readonly struct Bib32Field
  {
    /// <summary>
    /// The name of this field.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public readonly String32 Name;

    /// <summary>
    /// The value of this field.
    /// </summary>
    public readonly Bib32String Value;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      /* name = value */
      return Value.ToString(new StringBuilder()
        .Append(Name.ToString())
        .Append(" = ")
      ).ToString();
    }

    /// <param name="name">Must be a valid identifier.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32Field(String32 name, Bib32String value)
    {
      Name = name;
      Value = value;
#if BIB_MODEL_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      if (!BibBstChars.IsIdentifier(Name))
      {
        throw new ArgumentException("Bib32Field: Name is not a valid identifier.", name is null ? "name" : name);
      }
      Value.CtorCheckImpl(name is null ? "value" : name);
#endif
    }

    /// <summary>
    /// Determines whether the field is currently valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public bool IsValid()
    {
      return BibBstChars.IsIdentifier(Name)
        && Value.IsValid();
    }
  }
}
