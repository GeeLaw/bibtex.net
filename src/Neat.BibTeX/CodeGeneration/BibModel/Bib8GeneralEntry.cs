#if BIB_MODEL_CHECKS
using System;
using System.Diagnostics.CodeAnalysis;
#endif
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String8;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a general <c>@type{ key, name1 = {literal} # "literal" # 123 # name, ... }</c> entry.
  /// </summary>
  public sealed class Bib8GeneralEntry : Bib8Entry
  {
    /// <summary>
    /// The entry type of a <c>@comment</c> entry.
    /// </summary>
    public static readonly StringT CommentEntryType = Utf.String16ToString8Strict("comment");

    /// <summary>
    /// The database key of this entry.
    /// This string should be a valid database key and should be compared by <see cref="BibBstComparer"/>.
    /// A valid database key must not contain "," or space characters
    /// (there is no other requirement for validity, and it can be empty).
    /// </summary>
    public StringT Key;

    /// <summary>
    /// The fields of this entry.
    /// </summary>
    public Bib8Field[] Fields;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      /* @type{ key, ... } */
      return string.Format(IsBrace ? "@{0}{{ {1}, ... }}" : "@{0}( {1}, ... )",
        Type.GenericToString(), Key.GenericToString());
    }

    /// <param name="isBrace">Must be <see langword="false"/> if <paramref name="key"/> must use parentheses.</param>
    /// <param name="type">Must be a valid identifier but not any of <c>string</c>, <c>preamble</c>, or <c>comment</c> (in any casing).</param>
    /// <param name="key">Must be a valid database key.</param>
    /// <param name="fields">Must not be <see langword="null"/> (can be empty).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib8GeneralEntry(StringT type, bool isBrace, StringT key, Bib8Field[] fields)
      : base(type, isBrace)
    {
      Key = key;
      Fields = fields;
#if BIB_MODEL_CHECKS
      CtorCheckImpl(null);
    }

    [SuppressMessage("Usage", "CA2208", Justification = "The usage is correct.")]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      if (!IsGeneralEntryType(Type))
      {
        throw new ArgumentException("Bib8GeneralEntry: Type is not a valid general entry type.", name is null ? "type" : name);
      }
      byte type = BibBstChars.GetDatabaseKeyType(Key).Value;
      if (type == BibDatabaseKeyType.MustUseParenthesesValue)
      {
        if (IsBrace)
        {
          throw new ArgumentException("Bib8GeneralEntry: IsBrace is incompatible with Key (must use parentheses).", name is null ? "isBrace" : name);
        }
      }
      else if (type != BibDatabaseKeyType.UseBracesOrParenthesesValue)
      {
        throw new ArgumentException("Bib8GeneralEntry: Key is not a valid database key.", name is null ? "key" : name);
      }
      name = (name is null ? "fields" : name);
      Bib8Field[] fields = Fields;
      if (fields is null)
      {
        throw new ArgumentNullException("Bib8GeneralEntry: Fields is null.", name);
      }
      for (int i = 0; i < fields.Length; ++i)
      {
        fields[i].CtorCheckImpl(name);
      }
#endif
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsGeneralEntryType(StringT type)
    {
      return BibBstChars.IsIdentifier(type)
        && !BibBstComparer.Equals(type, Bib8StringEntry.EntryType)
        && !BibBstComparer.Equals(type, Bib8PreambleEntry.EntryType)
        && !BibBstComparer.Equals(type, CommentEntryType);
    }

    #region Bib8Entry overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      Bib8Field[] fields = Fields;
      if (fields is null || !IsGeneralEntryType(Type))
      {
        return false;
      }
      byte type = BibBstChars.GetDatabaseKeyType(Key).Value;
      if (type == BibDatabaseKeyType.MustUseParenthesesValue)
      {
        if (IsBrace)
        {
          return false;
        }
      }
      else if (type != BibDatabaseKeyType.UseBracesOrParenthesesValue)
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
    public sealed override void AcceptVisitor(IBib8EntryVisitor visitor)
    {
      visitor.VisitGeneralEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib8Entry overrides
  }
}