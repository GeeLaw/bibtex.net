#if BIB_DATA_CHECKS
using System;
using System.Diagnostics.CodeAnalysis;
#endif
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a general <c>@entry_type{ key, key1 = {literal} # "literal" # 123 # reference, ... }</c> entry.
  /// </summary>
  public sealed class Bib32GeneralEntry : Bib32Entry
  {
    public static readonly String32 CommentEntryType = Utf.String16ToString32Strict("comment");

    /// <summary>
    /// The citation key of this entry.
    /// This string should be a valid citation key and should be compared by <see cref="BibBstComparer"/>.
    /// A valid citation key must not contain "," or space characters
    /// (there is no other requirement for validity, and it can be empty).
    /// </summary>
    public readonly String32 Key;

    /// <summary>
    /// The fields of this entry.
    /// This array should not be <see langword="null"/>.
    /// </summary>
    public readonly Bib32Field[] Fields;

    /// <summary>
    /// <paramref name="type"/> must not be any of <c>string</c>, <c>preamble</c>, or <c>comment</c> (in any casing).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32GeneralEntry(String32 type, bool isBrace, String32 key, Bib32Field[] fields)
      : base(type, isBrace)
    {
      Key = key;
      Fields = fields;
#if BIB_DATA_CHECKS
      CtorCheckImpl(null);
    }

    [SuppressMessage("Usage", "CA2208", Justification = "The usage is correct.")]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal sealed override void CtorCheckImpl(string name)
    {
      if (!IsValidGeneralEntryType(Type))
      {
        throw new ArgumentException("Bib32GeneralEntry: Type is not a valid general entry type.", name is null ? "type" : name);
      }
      if (!BibBstChars.IsCitationKey(Key))
      {
        throw new ArgumentException("Bib32GeneralString: Key is not a valid citation key.", name is null ? "key" : name);
      }
      name = (name is null ? "fields" : name);
      Bib32Field[] fields = Fields;
      if (fields is null)
      {
        throw new ArgumentNullException("Bib32GeneralEntry: Fields is null.", name);
      }
      for (int i = 0; i < fields.Length; ++i)
      {
        fields[i].CtorCheckImpl(name);
      }
#endif
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsValidGeneralEntryType(String32 type)
    {
      return BibBstChars.IsIdentifier(type)
        && !BibBstComparer.Equals(type, Bib32StringEntry.EntryType)
        && !BibBstComparer.Equals(type, Bib32PreambleEntry.EntryType)
        && !BibBstComparer.Equals(type, CommentEntryType);
    }

    #region Bib32Entry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitGeneralEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBib32EntryVisitor visitor)
    {
      visitor.VisitGeneralEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib32Entry overrides
  }
}
