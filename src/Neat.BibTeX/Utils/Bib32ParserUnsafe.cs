using System;
using System.Runtime.CompilerServices;
using Neat.BibTeX.BibModel;
using Neat.Collections;
using Neat.Unicode;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements BibTeX parser.
  /// This parser can be used to parse multiple files.
  /// </summary>
  public struct Bib32ParserUnsafe<TOverrides> where TOverrides : struct, IBib32ParserUnsafeOverrides<TOverrides>
  {
    #region fields and properties

    /// <summary>
    /// Provides direct access to the embedded overrides.
    /// </summary>
    public TOverrides Overrides;

    /// <summary>
    /// The entries.
    /// </summary>
    public readonly List2<Bib32Entry> Entries;

    /// <summary>
    /// The components of the string being parsed.
    /// </summary>
    public readonly List2<Bib32StringComponent> StringComponents;

    /// <summary>
    /// The fields of the general entry being parsed.
    /// </summary>
    public readonly List2<Bib32Field> Fields;

    private int myCount;

    /// <summary>
    /// Gets the total number of characters.
    /// </summary>
    public int Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    private int myEaten;

    /// <summary>
    /// Gets the number of characters that have been processed.
    /// </summary>
    public int Eaten
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myEaten;
      }
    }

    private int myLine;

    /// <summary>
    /// Gets the (1-based) line number at the next character.
    /// </summary>
    public int Line
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myLine;
      }
    }

    private int myLastLineEndsAfter;

    /// <summary>
    /// Gets the (1-based) column number of the space at the next character.
    /// </summary>
    public int Column
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myEaten - myLastLineEndsAfter;
      }
    }

    private String32 myEntryType;

    /// <summary>
    /// Gets the entry type of the entry being parsed.
    /// </summary>
    public String32 EntryType
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myEntryType;
      }
    }

    private bool myEntryIsBrace;

    /// <summary>
    /// Indicates whether the entry being parsed uses <c>{}</c> as its delimiters with the alternate being <c>()</c>.
    /// </summary>
    public bool EntryIsBrace
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myEntryIsBrace;
      }
    }

    private String32 myStringOrFieldName;

    /// <summary>
    /// Gets the name of the string entry or the field being parsed.
    /// </summary>
    public String32 StringOrFieldName
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myStringOrFieldName;
      }
    }

    private String32 myDatabaseKey;

    /// <summary>
    /// Gets the database key of the general entry being parsed.
    /// </summary>
    public String32 DatabaseKey
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myDatabaseKey;
      }
    }

    private bool myLiteralIsBrace;

    /// <summary>
    /// Indicates whether the literal being parsed uses <c>{}</c> as its delimiters with the alternate being <c>""</c>.
    /// (A bare numeric literal cannot be seen by the overrides.)
    /// </summary>
    public bool LiteralIsBrace
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myLiteralIsBrace;
      }
    }

    #endregion fields and properties

    /// <summary>
    /// Parses a BibTeX file.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void Parse(ref int data0, int count)
    {
      throw new NotImplementedException();
    }
  }
}
