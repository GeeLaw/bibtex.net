#if BIB_CTOR_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a field in a <see cref="Bib32GeneralEntry"/>.
  /// </summary>
  public readonly struct Bib32Field
  {
    /// <summary>
    /// The field name.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public readonly String32 Key;

    /// <summary>
    /// The value of this field.
    /// </summary>
    public readonly Bib32String Value;

    [MethodImpl(Helper.OptimizeInline)]
    public Bib32Field(String32 key, Bib32String value)
    {
      Key = key;
      Value = value;
#if BIB_CTOR_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      if (!BibBstChars.IsIdentifier(Key))
      {
        throw new ArgumentException("Bib32Field: Key is not a valid identifier.", name is null ? "key" : name);
      }
      Value.CtorCheckImpl(name is null ? "value" : name);
#endif
    }
  }
}
