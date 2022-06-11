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
    /// Gets the index of the first character in the current line.
    /// If this property is equal to <see cref="Eaten"/>, then <see cref="Column"/> is <c>1</c>.
    /// This property is meaningful only in exception methods.
    /// </summary>
    public int LineStartsOn
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myLastLineEndsAfter + 1;
      }
    }

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
      for (int eaten = EatJunk(ref data0, 0, count); eaten != count; eaten = EatJunk(ref data0, eaten, count))
      {
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
    [MethodImpl(Helper.JustOptimize)]
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
    [MethodImpl(Helper.JustOptimize)]
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
    [MethodImpl(Helper.JustOptimize)]
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
    /// Eats a string entry (<paramref name="data0"/> is immediately after <c>@string</c>).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
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
        goto ErrorOpen;
      }
      myEntryIsBrace = isBrace;
      /* Skip '{' or '(', optional space, and expect an identifier (string name). */
      eaten = EatSpace(ref data0, eaten + 1, count);
      int nameLength = EatIdentifier(ref Unsafe.Add(ref data0, eaten), count - eaten);
      if (nameLength == 0)
      {
        goto ErrorName;
      }
      Overrides.SaveStringName(ref this, ref Unsafe.Add(ref data0, eaten), nameLength);
      /* Skip the string name, optional space, and expect '='. */
      eaten = EatSpace(ref data0, eaten + nameLength, count);
      if (eaten == count || Unsafe.Add(ref data0, eaten) != BibBstChars.Assignment)
      {
        goto ErrorAssignment;
      }
      /* Skip '=', optional space, and expect a series of concatenated components (string value). */
      eaten = EatSpace(ref data0, eaten + 1, count);
      if (EatString(ref data0, ref eaten, count))
      {
        return eaten;
      }
      /* Expect '}' or ')'. */
      if (eaten == count)
      {
        goto ErrorEndOfInput;
      }
      if (Unsafe.Add(ref data0, eaten) != (isBrace ? BibBstChars.RightBrace : BibBstChars.RightParenthesis))
      {
        goto ErrorClose;
      }
      Overrides.SaveStringEntry(ref this);
      /* Skip '}' or ')'. */
      return eaten + 1;
    ErrorOpen:
      myEaten = eaten;
      Overrides.StringEntryExpectingOpen(ref this);
      return eaten;
    ErrorName:
      myEaten = eaten;
      Overrides.StringEntryExpectingName(ref this);
      return eaten;
    ErrorAssignment:
      myEaten = eaten;
      Overrides.StringEntryExpectingAssignment(ref this);
      return eaten;
    ErrorEndOfInput:
      myEaten = eaten;
      Overrides.StringEntryGotEndOfInput(ref this);
      return eaten;
    ErrorClose:
      myEaten = eaten;
      Overrides.StringEntryExpectingClose(ref this);
      return eaten;
    }

    /// <summary>
    /// Eats a preamble entry (<paramref name="data0"/> is immediately after <c>@type</c>).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
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
        goto ErrorOpen;
      }
      myEntryIsBrace = isBrace;
      /* Skip '{' or '(', optional space, and expect a series of concatenated components (preamble text). */
      eaten = EatSpace(ref data0, eaten + 1, count);
      if (EatString(ref data0, ref eaten, count))
      {
        return eaten;
      }
      /* Expect '}' or ')'. */
      if (eaten == count)
      {
        goto ErrorEndOfInput;
      }
      if (Unsafe.Add(ref data0, eaten) != (isBrace ? BibBstChars.RightBrace : BibBstChars.RightParenthesis))
      {
        goto ErrorClose;
      }
      Overrides.SavePreambleEntry(ref this);
      /* Skip '}' or ')'. */
      return eaten + 1;
    ErrorOpen:
      myEaten = eaten;
      Overrides.PreambleEntryExpectingOpen(ref this);
      return eaten;
    ErrorEndOfInput:
      myEaten = eaten;
      Overrides.PreambleEntryGotEndOfInput(ref this);
      return eaten;
    ErrorClose:
      myEaten = eaten;
      Overrides.PreambleEntryExpectingClose(ref this);
      return eaten;
    }

    /// <summary>
    /// Eats a general entry.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private int EatGeneralEntry(ref int data0, int eaten, int count)
    {
      int valueOrLength;
      bool isBrace;
      /* Skip optional space, and expect '{' or '('. */
      eaten = EatSpace(ref data0, eaten, count);
      if (eaten == count
        || (!(isBrace = ((valueOrLength = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace))
          && valueOrLength != BibBstChars.LeftParenthesis))
      {
        goto ErrorOpen;
      }
      myEntryIsBrace = isBrace;
      /* Skip '{' or '(', optional space, and expect a database key. */
      eaten = EatSpace(ref data0, eaten + 1, count);
      if (eaten == count)
      {
        goto ErrorDatabaseKey;
      }
      valueOrLength = (isBrace
        ? EatBraceDatabaseKey(ref Unsafe.Add(ref data0, eaten), count - eaten)
        : EatParenthesisDatabaseKey(ref Unsafe.Add(ref data0, eaten), count - eaten));
      Overrides.SaveDatabaseKey(ref this, ref Unsafe.Add(ref data0, eaten), valueOrLength);
      /* Skip the database key, optional space, and expect ',' or '}' or ')'. */
      eaten = EatSpace(ref data0, eaten + valueOrLength, count);
      if (eaten == count)
      {
        goto ErrorFirstCommaOrClose;
      }
      int entryClose = (isBrace ? BibBstChars.RightBrace : BibBstChars.RightParenthesis);
      if ((valueOrLength = Unsafe.Add(ref data0, eaten)) == BibBstChars.Comma)
      {
        ++eaten;
        goto ExpectSpaceThenFieldNameOrClose;
      }
      if (valueOrLength == entryClose)
      {
        goto Save;
      }
      goto ErrorFirstCommaOrClose;
    ExpectSpaceThenFieldNameOrClose:
      /* Skip optional space and expect '}' or ')' or an identifier (field name). */
      eaten = EatSpace(ref data0, eaten, count);
      if (Unsafe.Add(ref data0, eaten) == entryClose)
      {
        goto Save;
      }
      valueOrLength = EatIdentifier(ref Unsafe.Add(ref data0, eaten), count - eaten);
      if (valueOrLength == 0)
      {
        goto ErrorFieldNameOrClose;
      }
      Overrides.SaveFieldName(ref this, ref Unsafe.Add(ref data0, eaten), valueOrLength);
      /* Skip the field name, optional space, and expect '='. */
      eaten = EatSpace(ref data0, eaten + valueOrLength, count);
      if (Unsafe.Add(ref data0, eaten) != BibBstChars.Assignment)
      {
        goto ErrorAssignment;
      }
      /* Skip '=', optional space, and expect a series of concatenated components (field value). */
      eaten = EatSpace(ref data0, eaten + 1, count);
      if (EatString(ref data0, ref eaten, count))
      {
        return eaten;
      }
      if (eaten == count)
      {
        goto ErrorEndOfInput;
      }
      if ((valueOrLength = Unsafe.Add(ref data0, eaten)) == BibBstChars.Comma)
      {
        Overrides.SaveField(ref this);
        ++eaten;
        goto ExpectSpaceThenFieldNameOrClose;
      }
      if (valueOrLength != entryClose)
      {
        goto ErrorCommaOrClose;
      }
      Overrides.SaveField(ref this);
      /* Fall through to save. */
    Save:
      Overrides.SaveGeneralEntry(ref this);
      /* Skip '}' or ')'. */
      return eaten + 1;
    ErrorOpen:
      myEaten = eaten;
      Overrides.GeneralEntryExpectingOpen(ref this);
      return eaten;
    ErrorDatabaseKey:
      myEaten = eaten;
      Overrides.GeneralEntryExpectingDatabaseKey(ref this);
      return eaten;
    ErrorFirstCommaOrClose:
      myEaten = eaten;
      Overrides.GeneralEntryExpectingFirstCommaOrClose(ref this);
      return eaten;
    ErrorFieldNameOrClose:
      myEaten = eaten;
      Overrides.GeneralEntryExpectingFieldNameOrClose(ref this);
      return eaten;
    ErrorAssignment:
      myEaten = eaten;
      Overrides.GeneralEntryExpectingAssignment(ref this);
      return eaten;
    ErrorEndOfInput:
      myEaten = eaten;
      Overrides.GeneralEntryGotEndOfInput(ref this);
      return eaten;
    ErrorCommaOrClose:
      myEaten = eaten;
      Overrides.GeneralEntryExpectingCommaOrClose(ref this);
      return eaten;
    }

    /// <summary>
    /// Eats a series of concatenated components starting at <c>data[oldEaten]</c>.
    /// Upon returning from this method (this does not apply if an exception is thrown), <c>data[newEaten]</c> is a non-space character.
    /// This method returns <see langword="true"/> if an exception method was called.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private bool EatString(ref int data0, ref int oldNewEaten, int count)
    {
      int eaten = oldNewEaten, valueOrNewEatenOrLength;
    ExpectComponent:
      if (eaten == count)
      {
        goto ErrorComponent;
      }
      if ((valueOrNewEatenOrLength = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace)
      {
        /* Skip '{'. */
        valueOrNewEatenOrLength = ++eaten;
        if (EatBraceLiteral(ref data0, ref valueOrNewEatenOrLength, count))
        {
          goto ErrorHandled;
        }
        Overrides.SaveBraceLiteralComponent(ref this, ref Unsafe.Add(ref data0, eaten), valueOrNewEatenOrLength - eaten);
        /* Skip the brace-delimited literal and '}'. */
        eaten = valueOrNewEatenOrLength + 1;
        /* Fall through to skip optional space and check for '#'. */
      }
      else if (valueOrNewEatenOrLength == BibBstChars.DoubleQuote)
      {
        /* Skip '"'. */
        valueOrNewEatenOrLength = ++eaten;
        if (EatQuoteLiteral(ref data0, ref valueOrNewEatenOrLength, count))
        {
          goto ErrorHandled;
        }
        Overrides.SaveQuoteLiteralComponent(ref this, ref Unsafe.Add(ref data0, eaten), valueOrNewEatenOrLength - eaten);
        /* Skip the quote-delimited literal and '"'. */
        eaten = valueOrNewEatenOrLength + 1;
        /* Fall through to skip optional space and check for '#'. */
      }
      else if (BibBstChars.IsNumericImpl(valueOrNewEatenOrLength))
      {
        valueOrNewEatenOrLength = EatNumericLiteral(ref Unsafe.Add(ref data0, eaten), count - eaten);
        Overrides.SaveNumericLiteralComponent(ref this, ref Unsafe.Add(ref data0, eaten), valueOrNewEatenOrLength);
        /* Skip the numeric literal. */
        eaten += valueOrNewEatenOrLength;
        /* Fall through to skip optional space and check for '#'. */
      }
      else
      {
        /* Expect an identifier (name of the referenced string). */
        valueOrNewEatenOrLength = EatIdentifier(ref Unsafe.Add(ref data0, eaten), count - eaten);
        if (valueOrNewEatenOrLength == 0)
        {
          goto ErrorComponent;
        }
        Overrides.SaveNameComponent(ref this, ref Unsafe.Add(ref data0, eaten), valueOrNewEatenOrLength);
        /* Skip the name of the referenced string. */
        eaten += valueOrNewEatenOrLength;
        /* Fall through to skip optional space and check for '#'. */
      }
      /* Skip optional space and check if the next character is '#'. */
      eaten = EatSpace(ref data0, eaten, count);
      if (eaten != count && Unsafe.Add(ref data0, eaten) == BibBstChars.Concatenation)
      {
        /* Skip '#', optional space, and expect a component. */
        eaten = EatSpace(ref data0, eaten + 1, count);
        goto ExpectComponent;
      }
      /* No more components. */
      oldNewEaten = eaten;
      return false;
    ErrorComponent:
      myEaten = eaten;
      Overrides.StringExpectingComponent(ref this);
      oldNewEaten = eaten;
      return true;
    ErrorHandled:
      /* We jump here from an error handled by EatBraceLiteral or EatQuoteLiteral. */
      oldNewEaten = valueOrNewEatenOrLength;
      return true;
    }

    /// <summary>
    /// Eats a brace-delimited literal (<c>data[oldEaten]</c> is right after the opening delimiter <c>{</c>).
    /// Upon returning from this method (this does not apply if an exception is thrown), <c>data[newEaten]</c> is right at the closing delimiter <c>}</c>.
    /// This method returns <see langword="true"/> if an exception method was called.
    /// This method does not call <see cref="IBib32ParserUnsafeOverrides{TOverrides}.SaveBraceLiteralComponent(ref Bib32ParserUnsafe{TOverrides}, ref int, int)"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private bool EatBraceLiteral(ref int data0, ref int oldNewEaten, int count)
    {
      int eaten = oldNewEaten, depth = 0;
      for (int value; eaten != count; ++eaten)
      {
        if ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace)
        {
          ++depth;
          continue;
        }
        if (value == BibBstChars.RightBrace)
        {
          if (depth-- != 0)
          {
            continue;
          }
          oldNewEaten = eaten;
          return false;
        }
      }
      /* The end of input is reached but the literal is not closed. */
      myEaten = eaten;
      myBraceDepth = depth;
      Overrides.StringBraceLiteralGotEndOfInput(ref this);
      oldNewEaten = eaten;
      return true;
    }

    /// <summary>
    /// Eats a quote-delimited literal (<c>data[oldEaten]</c> is right after the opening delimiter <c>"</c>).
    /// Upon returning from this method (this does not apply if an exception is thrown), <c>data[newEaten]</c> is right at the closing delimiter <c>"</c>.
    /// This method returns <see langword="true"/> if an exception method was called.
    /// This method does not call <see cref="IBib32ParserUnsafeOverrides{TOverrides}.SaveQuoteLiteralComponent(ref Bib32ParserUnsafe{TOverrides}, ref int, int)"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private bool EatQuoteLiteral(ref int data0, ref int oldNewEaten, int count)
    {
      int eaten = oldNewEaten, depth = 0;
      for (int value; eaten != count; ++eaten)
      {
        if ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.LeftBrace)
        {
          ++depth;
          continue;
        }
        if (value == BibBstChars.RightBrace)
        {
          if (depth-- != 0)
          {
            continue;
          }
          myEaten = eaten;
          Overrides.StringQuoteLiteralGotNegativeBraceDepth(ref this);
          oldNewEaten = eaten;
          return true;
        }
        if (depth == 0 && value == BibBstChars.DoubleQuote)
        {
          oldNewEaten = eaten;
          return false;
        }
      }
      /* The end of input is reached but the literal is not closed. */
      myEaten = eaten;
      myBraceDepth = depth;
      Overrides.StringQuoteLiteralGotEndOfInput(ref this);
      oldNewEaten = eaten;
      return true;
    }

    /// <summary>
    /// Eats a numeric literal starting at <paramref name="data0"/>.
    /// Note that the character at <paramref name="data0"/> is not read (again), and
    /// <paramref name="count"/> is assumed to be positive without checking.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private int EatNumericLiteral(ref int data0, int count)
    {
      int eaten = 1;
      while (eaten != count && BibBstChars.IsNumericImpl(Unsafe.Add(ref data0, eaten)))
      {
        ++eaten;
      }
      return eaten;
    }

    /// <summary>
    /// Reads a database key (of a brace-delimited entry) starting at <paramref name="data0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private int EatBraceDatabaseKey(ref int data0, int count)
    {
      int eaten, value;
      for (eaten = 0; eaten != count; ++eaten)
      {
        if ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.Comma
          || value == BibBstChars.RightBrace
          || BibBstChars.IsSpaceImpl(value))
        {
          break;
        }
      }
      return eaten;
    }

    /// <summary>
    /// Reads a database key (of a parenthesis-delimited entry) starting at <paramref name="data0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private int EatParenthesisDatabaseKey(ref int data0, int count)
    {
      int eaten, value;
      for (eaten = 0; eaten != count; ++eaten)
      {
        if ((value = Unsafe.Add(ref data0, eaten)) == BibBstChars.Comma
          || BibBstChars.IsSpaceImpl(value))
        {
          break;
        }
      }
      return eaten;
    }
  }
}
