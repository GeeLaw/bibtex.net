using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;

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
    public readonly StringT Name;

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
        .Append(Name.GenericToString())
        .Append(" = ")
      ).ToString();
    }

    /// <param name="name">Must be a valid identifier.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32Field(StringT name, Bib32String value)
    {
      Name = name;
      Value = value;
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
