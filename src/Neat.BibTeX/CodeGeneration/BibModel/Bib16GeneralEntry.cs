using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = System.String;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a general <c>@type{ key, name1 = {literal} # "literal" # 123 # name, ... }</c> entry.
  /// </summary>
  public sealed class Bib16GeneralEntry : Bib16Entry
  {
    /// <summary>
    /// The entry type of a <c>@comment</c> entry.
    /// </summary>
    public const StringT CommentEntryType = "comment";

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
    public Bib16Field[] Fields;

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
    public Bib16GeneralEntry(StringT type, bool isBrace, StringT key, Bib16Field[] fields)
      : base(type, isBrace)
    {
      Key = key;
      Fields = fields;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsGeneralEntryType(StringT type)
    {
      return BibBstChars.IsIdentifier(type)
        && !BibBstComparer.Equals(type, Bib16StringEntry.EntryType)
        && !BibBstComparer.Equals(type, Bib16PreambleEntry.EntryType)
        && !BibBstComparer.Equals(type, CommentEntryType);
    }

    #region Bib16Entry overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      Bib16Field[] fields = Fields;
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
    public sealed override void AcceptVisitor(IBib16EntryVisitor visitor)
    {
      visitor.VisitGeneralEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib16Entry overrides
  }
}
