#if BIB_PARSER_CATCH_ERRORS
using System.Globalization;
#endif
using System.Runtime.CompilerServices;
using Neat.BibTeX.BibModel;
using Neat.BibTeX.Utils;
using Neat.Collections;

using PrimitiveCharT = System.Byte;
using StringT = Neat.Unicode.String8;

namespace Neat.BibTeX.BibParsers
{
  public sealed class Bib8ParserIgnoreErrors
  {
    private struct Overrides : IBib8ParserUnsafeOverrides<Overrides>
    {
#if BIB_PARSER_CATCH_ERRORS
      private const MethodImplOptions ForExceptionMethods = Helper.OptimizeNoInline;
#else
      private const MethodImplOptions ForExceptionMethods = Helper.OptimizeInline;
#endif

      public List2<Bib8Entry> Entries;
      private List2<Bib8StringComponent> StringComponents;
      private List2<Bib8Field> Fields;
      private Bib8InternPoolUnsafe InternedStrings;
      private StringT EntryType;
#if BIB_PARSER_CATCH_ERRORS
#else
      private int EntryTypeCode;
#endif
      private StringT StringNameOrFieldName;
      private StringT DatabaseKey;

      [MethodImpl(Helper.JustOptimize)]
      public void Initialize()
      {
        Entries = new List2<Bib8Entry>();
        StringComponents = new List2<Bib8StringComponent>();
        Fields = new List2<Bib8Field>();
        InternedStrings.Initialize();
      }

      [MethodImpl(Helper.JustOptimize)]
      public void Cleanup()
      {
        StringComponents.Clear();
        Fields.Clear();
        EntryType = default(StringT);
        StringNameOrFieldName = default(StringT);
        DatabaseKey = default(StringT);
      }

      [MethodImpl(Helper.JustOptimize)]
      private Bib8String SaveString()
      {
        Bib8String result;
        List2<Bib8StringComponent> components = StringComponents;
        if (components.Count < 2)
        {
          result = new Bib8String(components[0]);
        }
        else
        {
          result = new Bib8String(components.ToArray());
        }
        return result;
      }

      #region normal processing

      [MethodImpl(Helper.JustOptimize)]
      public int SaveEntryType(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringT entryType = InternedStrings.Intern(ref start, length);
        EntryType = entryType;
        if (BibBstComparer.Equals(entryType, Bib8StringEntry.EntryType))
        {
#if BIB_PARSER_CATCH_ERRORS
#else
          EntryTypeCode = 1;
#endif
          return 1;
        }
        if (BibBstComparer.Equals(entryType, Bib8PreambleEntry.EntryType))
        {
#if BIB_PARSER_CATCH_ERRORS
#else
          EntryTypeCode = 2;
#endif
          StringComponents.Clear();
          return 2;
        }
        if (BibBstComparer.Equals(entryType, Bib8GeneralEntry.CommentEntryType))
        {
#if BIB_PARSER_CATCH_ERRORS
#else
          EntryTypeCode = 3;
#endif
          return 3;
        }
#if BIB_PARSER_CATCH_ERRORS
#else
        EntryTypeCode = 0;
#endif
        return 0;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveStringEntry(ref Bib8ParserUnsafe<Overrides> that)
      {
        Entries.Add(new Bib8StringEntry(EntryType, that.EntryIsBrace, StringNameOrFieldName, SaveString()));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SavePreambleEntry(ref Bib8ParserUnsafe<Overrides> that)
      {
        Entries.Add(new Bib8PreambleEntry(EntryType, that.EntryIsBrace, SaveString()));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveGeneralEntry(ref Bib8ParserUnsafe<Overrides> that)
      {
        Entries.Add(new Bib8GeneralEntry(EntryType, that.EntryIsBrace, DatabaseKey, Fields.ToArray()));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveStringName(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringNameOrFieldName = InternedStrings.Intern(ref start, length);
        StringComponents.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveNameComponent(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib8StringComponent(
          new BibStringComponentType(BibStringComponentType.NameValue),
          InternedStrings.Intern(ref start, length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveQuoteLiteralComponent(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib8StringComponent(
          new BibStringComponentType(BibStringComponentType.QuoteLiteralValue),
          Helper.GenericGetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveNumericLiteralComponent(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib8StringComponent(
          new BibStringComponentType(BibStringComponentType.NumericLiteralValue),
          Helper.GenericGetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveBraceLiteralComponent(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib8StringComponent(
          new BibStringComponentType(BibStringComponentType.BraceLiteralValue),
          Helper.GenericGetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveDatabaseKey(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        DatabaseKey = Helper.GenericGetString(ref start, (uint)length);
        Fields.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveFieldName(ref Bib8ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringNameOrFieldName = InternedStrings.Intern(ref start, length);
        StringComponents.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveField(ref Bib8ParserUnsafe<Overrides> that)
      {
        Fields.Add(new Bib8Field(StringNameOrFieldName, SaveString()));
      }

      #endregion normal processing

      #region exceptions

      [MethodImpl(ForExceptionMethods)]
      public void EntryExpectingType(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting identifier (entry type).",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingOpen(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '{{' or '(' after '@string'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingName(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting identifier (string name) after '@string{{'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingAssignment(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '=' after '@string{{ name'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryGotEndOfInput(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}}' or ')' after '@string{{ name = value', got end of input.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingClose(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}}' or ')' after '@string{{ name = value'.",
          that.Line, that.Column));
#else
        SaveStringEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void PreambleEntryExpectingOpen(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '{{' or '(' after '@preamble'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void PreambleEntryGotEndOfInput(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}}' or ')' after '@preamble{{ text', got end of input.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void PreambleEntryExpectingClose(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}}' or ')' after '@preamble{{ text'.",
          that.Line, that.Column));
#else
        SavePreambleEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingOpen(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '{{' or '(' after '@type'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingDatabaseKey(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting database key after '@type{{', got end of input.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingFirstCommaOrClose(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting ',' or '}}' or ')' after '@type{{ key'.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingFieldNameOrClose(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting identifier (field name) or '}}' after '@type{{ key,'.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingAssignment(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '=' after '@type{{ key, name'.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryGotEndOfInput(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting ',' or '}}' or ')' after '@type{{ key, name = value', got end of input.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingCommaOrClose(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting ',' or '}}' or ')' after '@type{{ key, name = value'.",
          that.Line, that.Column));
#else
        SaveField(ref that);
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringExpectingComponent(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting brace-delimited, quote-delimited, or pure numeric literal, or identifier (name of referenced string) after '@type{{ key, name = '.",
          that.Line, that.Column));
#else
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringQuoteLiteralGotNegativeBraceDepth(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Extra '}}' in quote-delimited literal.",
          that.Line, that.Column));
#else
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringQuoteLiteralGotEndOfInput(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Unclosed quote-delimited literal with {2} outstanding left brace(s).",
          that.Line, that.Column, that.BraceDepth));
#else
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringBraceLiteralGotEndOfInput(ref Bib8ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Unclosed brace-delimited literal with ({2} + 1) outstanding left brace(s).",
          that.Line, that.Column, that.BraceDepth));
#else
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
#endif
      }

      #endregion exceptions
    }

    private Bib8ParserUnsafe<Overrides> myParser;

    public readonly List2<Bib8Entry> Entries;

    [MethodImpl(Helper.JustOptimize)]
    public Bib8ParserIgnoreErrors()
    {
      myParser.Overrides.Initialize();
      Entries = myParser.Overrides.Entries;
    }

    /// <summary>
    /// Parses the content of a <c>.bib</c> file.
    /// </summary>
    /// <exception cref="BibParserException">If the original implementation of BibTeX would complain about the content.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public void Parse(StringT bib)
    {
      if (bib.GenericIsDefault())
      {
        return;
      }
      try
      {
        myParser.Parse(ref bib.GenericGetData0(), bib.Length);
      }
      finally
      {
        myParser.Overrides.Cleanup();
      }
    }
  }
}
