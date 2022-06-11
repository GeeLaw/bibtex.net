using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements BibTeX parser.
  /// This parser can be used to parse multiple files.
  /// </summary>
  [SuppressMessage("Performance", "CA1822", Justification = "Methods are marked semantically.")]
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
    /// This property is meaningful only in exception methods.
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
    /// A line is ended by <c>\r\n</c> or <c>\r</c> or <c>\n</c>.
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
    /// This property is meaningful only in exception methods.
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

    private int myBraceDepth;

    /// <summary>
    /// Gets the outstanding left braces of the delimited literal being parsed
    /// (not counting the opening delimiter if it is <c>{</c>).
    /// This property is meaningful only in certain exception methods.
    /// </summary>
    public int BraceDepth
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myBraceDepth;
      }
    }

    #endregion fields and properties

    /// <summary>
    /// Parses a BibTeX file.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void Parse(ref int data0, int count)
    {
#if BIB_PARSER_CHECKS
      if (Unsafe.IsNullRef(ref data0))
      {
        throw new ArgumentNullException(nameof(data0));
      }
      if (count < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(count));
      }
#endif
      myCount = count;
      myLine = 1;
      myLastLineEndsAfter = -1;
      for (int eaten = EatJunk(ref data0, 0, count); eaten != count; EatJunk(ref data0, 0, count))
      {
#if BIB_PARSER_CHECKS
        /* The data were rudely modified (either by Overrides or from another thread) during parsing. */
        if (Unsafe.Add(ref data0, eaten) != BibBstChars.At)
        {
          throw new InvalidOperationException("An '@' disappeared.");
        }
#endif
        /* Skip '@', optional space, and expect an identifier (entry type). */
        eaten = EatSpace(ref data0, eaten + 1, count);
        int entryTypeLength = EatIdentifier(ref Unsafe.Add(ref data0, eaten), count - eaten);
        if (entryTypeLength == 0)
        {
          myEaten = eaten;
          Overrides.EntryExpectingType(ref this);
          continue;
        }
        int entryType = Overrides.SaveEntryType(ref this, ref Unsafe.Add(ref data0, eaten), entryTypeLength);
        /* Skip the entry type and continue parsing according to the entry type. */
        eaten += entryTypeLength;
        /* In case the switch statement is not compiled into a jump table,
        /* we make the assumption that the order of cases are preserved.
        /* The numbering is determined by the frequency of entry types
        /* so that more frequented types are tested first. */
        switch (entryType)
        {
        case 0:
          eaten = EatGeneralEntry(ref data0, eaten, count);
          continue;
        case 1:
          eaten = EatStringEntry(ref data0, eaten, count);
          continue;
        case 2:
          eaten = EatPreambleEntry(ref data0, eaten, count);
          continue;
        default:
#if BIB_PARSER_CHECKS
          throw new InvalidOperationException("SaveEntryType return value is out of range.");
        case 3:
#endif
          /* The original implementation of BibTeX does not even look at delimiters for @comment. */
          continue;
        }
      }
    }

    /// <summary>
    /// Skips until the current character is <c>@</c>.
    /// </summary>
    private int EatJunk(ref int data0, int eaten, int count)
    {
      bool wasCR = false;
      for (int value; eaten != count; ++eaten)
      {
        value = Unsafe.Add(ref data0, eaten);
        if (wasCR && value == BibBstChars.LF)
        {
          wasCR = false;
          myLastLineEndsAfter = eaten;
          continue;
        }
        if ((wasCR = (value == BibBstChars.CR)) || value == BibBstChars.LF)
        {
          ++myLine;
          myLastLineEndsAfter = eaten;
          continue;
        }
        if (value == BibBstChars.At)
        {
          break;
        }
      }
      return eaten;
    }

    /// <summary>
    /// Skips the space characters.
    /// </summary>
    private int EatSpace(ref int data0, int eaten, int count)
    {
      bool wasCR = false;
      for (int value; eaten != count; ++eaten)
      {
        value = Unsafe.Add(ref data0, eaten);
        if (wasCR && value == BibBstChars.LF)
        {
          wasCR = false;
          myLastLineEndsAfter = eaten;
          continue;
        }
        if ((wasCR = (value == BibBstChars.CR)) || value == BibBstChars.LF)
        {
          ++myLine;
          myLastLineEndsAfter = eaten;
          continue;
        }
        if (!BibBstChars.IsSpaceImpl(value))
        {
          break;
        }
      }
      return eaten;
    }

    /// <summary>
    /// Reads an identifier starting at <paramref name="data0"/>.
    /// </summary>
    private int EatIdentifier(ref int data0, int count)
    {
      int eaten = 0;
      for (int value; eaten != count; ++eaten)
      {
        value = Unsafe.Add(ref data0, eaten);
        if (!BibBstChars.IsIdentifierImpl(value) || (eaten == 0 && BibBstChars.IsNumericImpl(value)))
        {
          break;
        }
      }
      return eaten;
    }

    /// <summary>
    /// Eats a string entry.
    /// </summary>
    private int EatStringEntry(ref int data0, int eaten, int count)
    {
      int value;
      bool isBrace;
      /* Skip optional space, and expect '{' or '('. */
      eaten = EatSpace(ref data0, eaten, count);
      if (eaten == count
        || (!(isBrace = ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace))
          && value != BibBstChars.LeftParenthesis))
      {
        myEaten = eaten;
        Overrides.StringEntryExpectingOpen(ref this);
        return eaten;
      }
      myEntryIsBrace = isBrace;
      /* Skip '{' or '(', optional space, and expect an identifier (string name). */
      eaten = EatSpace(ref data0, eaten + 1, count);
      throw new NotImplementedException();
    }

    /// <summary>
    /// Eats a preamble entry.
    /// </summary>
    private int EatPreambleEntry(ref int data0, int eaten, int count)
    {
      int value;
      bool isBrace;
      /* Skip optional space, and expect '{' or '('. */
      eaten = EatSpace(ref data0, eaten, count);
      if (eaten == count
        || (!(isBrace = ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace))
          && value != BibBstChars.LeftParenthesis))
      {
        myEaten = eaten;
        Overrides.PreambleEntryExpectingOpen(ref this);
        return eaten;
      }
      myEntryIsBrace = isBrace;
      /* Skip '{' or '(', optional space, and expect a series of concatenated strings (preamble text). */
      eaten = EatSpace(ref data0, eaten + 1, count);
      throw new NotImplementedException();
    }

    /// <summary>
    /// Eats a general entry.
    /// </summary>
    private int EatGeneralEntry(ref int data0, int eaten, int count)
    {
      int value;
      bool isBrace;
      /* Skip optional space, and expect '{' or '('. */
      eaten = EatSpace(ref data0, eaten, count);
      if (eaten == count
        || (!(isBrace = ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace))
          && value != BibBstChars.LeftParenthesis))
      {
        myEaten = eaten;
        Overrides.GeneralEntryExpectingOpen(ref this);
        return eaten;
      }
      myEntryIsBrace = isBrace;
      /* Skip '{' or '(', optional space, and parse a database key. */
      eaten = EatSpace(ref data0, eaten + 1, count);
      throw new NotImplementedException();
    }
  }
}
