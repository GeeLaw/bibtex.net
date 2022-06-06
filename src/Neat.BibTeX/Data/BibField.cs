using System.Runtime.CompilerServices;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a field in a <see cref="BibGeneralEntry{TString}"/>.
  /// </summary>
  public readonly struct BibField<TString>
  {
    /// <summary>
    /// The field name in lower case.
    /// </summary>
    public readonly TString Key;

    /// <summary>
    /// The value of this field.
    /// </summary>
    public readonly BibString<TString> Value;

    [MethodImpl(Helper.OptimizeInline)]
    public BibField(TString key, BibString<TString> value)
    {
      Key = key;
      Value = value;
    }
  }
}
