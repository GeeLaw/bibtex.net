/* @< BIB_PARSER_CATCH_ERRORS */
#define BIB_PARSER_CATCH_ERRORS
/* @> */

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Neat.BibTeX.BibModel;
using Neat.BibTeX.Utils;
using Neat.Collections;

/* @< PrimitiveCharT */
using PrimitiveCharT = System.Int32;
/* @> */
/* @< StringT */
using StringT = Neat.Unicode.String32;
/* @> */

namespace Neat.BibTeX.BibParsers
{
  public sealed class Bib32ParserCatchErrors
  {
    private struct Overrides : IBib32ParserUnsafeOverrides<Overrides>
    {
#if BIB_PARSER_CATCH_ERRORS
      private const MethodImplOptions ForExceptionMethods = Helper.OptimizeNoInline;
#else
      private const MethodImplOptions ForExceptionMethods = Helper.OptimizeInline;
#endif

      public List2<Bib32Entry> Entries;
      private List2<Bib32StringComponent> StringComponents;
      private List2<Bib32Field> Fields;
      private Dictionary<StringT, StringT> InternedStrings;
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
        Entries = new List2<Bib32Entry>();
        StringComponents = new List2<Bib32StringComponent>();
        Fields = new List2<Bib32Field>();
        InternedStrings = new Dictionary<StringT, StringT>();
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
      private StringT GetInternedString(ref PrimitiveCharT start, uint length)
      {
        StringT str = Helper.GenericGetString(ref start, length);
        StringT interned;
        if (!InternedStrings.TryGetValue(str, out interned))
        {
          InternedStrings.Add(str, str);
          interned = str;
        }
        return interned;
      }

      [MethodImpl(Helper.JustOptimize)]
      private Bib32String SaveString()
      {
        Bib32String result;
        List2<Bib32StringComponent> components = StringComponents;
        if (components.Count < 2)
        {
          result = new Bib32String(components[0]);
        }
        else
        {
          result = new Bib32String(components.ToArray());
        }
        return result;
      }

      #region normal processing

      [MethodImpl(Helper.JustOptimize)]
      public int SaveEntryType(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringT entryType = GetInternedString(ref start, (uint)length);
        EntryType = entryType;
        if (BibBstComparer.Equals(entryType, Bib32StringEntry.EntryType))
        {
#if BIB_PARSER_CATCH_ERRORS
#else
          EntryTypeCode = 1;
#endif
          return 1;
        }
        if (BibBstComparer.Equals(entryType, Bib32PreambleEntry.EntryType))
        {
#if BIB_PARSER_CATCH_ERRORS
#else
          EntryTypeCode = 2;
#endif
          StringComponents.Clear();
          return 2;
        }
        if (BibBstComparer.Equals(entryType, Bib32GeneralEntry.CommentEntryType))
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
      public void SaveStringEntry(ref Bib32ParserUnsafe<Overrides> that)
      {
        Entries.Add(new Bib32StringEntry(EntryType, that.EntryIsBrace, StringNameOrFieldName, SaveString()));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SavePreambleEntry(ref Bib32ParserUnsafe<Overrides> that)
      {
        Entries.Add(new Bib32PreambleEntry(EntryType, that.EntryIsBrace, SaveString()));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveGeneralEntry(ref Bib32ParserUnsafe<Overrides> that)
      {
        Entries.Add(new Bib32GeneralEntry(EntryType, that.EntryIsBrace, DatabaseKey, Fields.ToArray()));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveStringName(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringNameOrFieldName = GetInternedString(ref start, (uint)length);
        StringComponents.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveNameComponent(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.NameValue),
          GetInternedString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveQuoteLiteralComponent(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.QuoteLiteralValue),
          Helper.GenericGetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveNumericLiteralComponent(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.NumericLiteralValue),
          Helper.GenericGetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveBraceLiteralComponent(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.BraceLiteralValue),
          Helper.GenericGetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveDatabaseKey(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        DatabaseKey = Helper.GenericGetString(ref start, (uint)length);
        Fields.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveFieldName(ref Bib32ParserUnsafe<Overrides> that, ref PrimitiveCharT start, int length)
      {
        StringNameOrFieldName = GetInternedString(ref start, (uint)length);
        StringComponents.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveField(ref Bib32ParserUnsafe<Overrides> that)
      {
        Fields.Add(new Bib32Field(StringNameOrFieldName, SaveString()));
      }

      #endregion normal processing

      #region exceptions

      [MethodImpl(ForExceptionMethods)]
      public void EntryExpectingType(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting identifier (entry type).",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingOpen(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '{' or '(' after '@string'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingName(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting identifier (string name) after '@string{'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingAssignment(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '=' after '@string{ name'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}' or ')' after '@string{ name = value', got end of input.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringEntryExpectingClose(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}' or ')' after '@string{ name = value'.",
          that.Line, that.Column));
#else
        SaveStringEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void PreambleEntryExpectingOpen(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '{' or '(' after '@preamble'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void PreambleEntryGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}' or ')' after '@preamble{ text', got end of input.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void PreambleEntryExpectingClose(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '}' or ')' after '@preamble{ text'.",
          that.Line, that.Column));
#else
        SavePreambleEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingOpen(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '{' or '(' after '@type'.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingDatabaseKey(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting database key after '@type{', got end of input.",
          that.Line, that.Column));
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingFirstCommaOrClose(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting ',' or '}' or ')' after '@type{ key'.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingFieldNameOrClose(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting identifier (field name) or '}' after '@type{ key,'.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingAssignment(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting '=' after '@type{ key, name'.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting ',' or '}' or ')' after '@type{ key, name = value', got end of input.",
          that.Line, that.Column));
#else
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void GeneralEntryExpectingCommaOrClose(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting ',' or '}' or ')' after '@type{ key, name = value'.",
          that.Line, that.Column));
#else
        SaveField(ref that);
        SaveGeneralEntry(ref that);
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringExpectingComponent(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Expecting brace-delimited, quote-delimited, or pure numeric literal, or identifier (name of referenced string) after '@type{ key, name = '.",
          that.Line, that.Column));
#else
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringQuoteLiteralGotNegativeBraceDepth(ref Bib32ParserUnsafe<Overrides> that)
      {
#if BIB_PARSER_CATCH_ERRORS
        throw new BibParserException(string.Format(CultureInfo.InvariantCulture,
          "Ln {0} Ch {1}: Extra '}' in quote-delimited literal.",
          that.Line, that.Column));
#else
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
#endif
      }

      [MethodImpl(ForExceptionMethods)]
      public void StringQuoteLiteralGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
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
      public void StringBraceLiteralGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
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

    private Bib32ParserUnsafe<Overrides> myParser;

    public readonly List2<Bib32Entry> Entries;

    [MethodImpl(Helper.JustOptimize)]
    public Bib32ParserCatchErrors()
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
