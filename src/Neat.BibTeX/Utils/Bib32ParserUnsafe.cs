using System;
using System.Runtime.CompilerServices;

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
