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
  /// Represents a general <c>@type{ key, name1 = {literal} # "literal" # 123 # name, ... }</c> entry.
  /// </summary>
  public sealed class Bib32GeneralEntry : Bib32Entry
  {
    /// <summary>
    /// The entry type of a <c>@comment</c> entry.
    /// </summary>
    public static readonly String32 CommentEntryType = Utf.String16ToString32Strict("comment");

    /// <summary>
    /// The database key of this entry.
    /// This string should be a valid database key and should be compared by <see cref="BibBstComparer"/>.
    /// A valid database key must not contain "," or space characters
    /// (there is no other requirement for validity, and it can be empty).
    /// </summary>
    public String32 Key;

    /// <summary>
    /// The fields of this entry.
    /// </summary>
    public Bib32Field[] Fields;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      /* @type{ key, ... } or @type( key, ... ) */
      return string.Format(BibBstChars.MustUseParentheses(Key) ? "@{0}{{ {1}, ... }}" : "@{0}( {1}, ... )",
        Type.ToString(), Key.ToString());
    }

    /// <param name="type">Must be a valid identifier but not any of <c>string</c>, <c>preamble</c>, or <c>comment</c> (in any casing).</param>
    /// <param name="key">Must be a valid database key.</param>
    /// <param name="fields">Must not be <see langword="null"/> (can be empty).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32GeneralEntry(String32 type, String32 key, Bib32Field[] fields)
      : base(type)
    {
      Key = key;
      Fields = fields;
#if BIB_DATA_CHECKS
      CtorCheckImpl(null);
    }

    [SuppressMessage("Usage", "CA2208", Justification = "The usage is correct.")]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      if (!IsValidGeneralEntryType(Type))
      {
        throw new ArgumentException("Bib32GeneralEntry: Type is not a valid general entry type.", name is null ? "type" : name);
      }
      if (!BibBstChars.IsDatabaseKey(Key))
      {
        throw new ArgumentException("Bib32GeneralString: Key is not a valid database key.", name is null ? "key" : name);
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

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      Bib32Field[] fields = Fields;
      if (fields is null || !IsValidGeneralEntryType(Type) || !BibBstChars.IsDatabaseKey(Key))
      {
        return false;
      }
      for (int i = 0; i < fields.Length; ++i)
      {
        if (!fields[i].IsValid())
        {
          return false;
        }
      }
      return true;
    }

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
