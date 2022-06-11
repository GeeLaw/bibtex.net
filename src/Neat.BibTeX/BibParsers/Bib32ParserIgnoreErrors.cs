using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.BibTeX.BibModel;
using Neat.BibTeX.Utils;
using Neat.Collections;
using Neat.Unicode;

namespace Neat.BibTeX.BibParsers
{
  public sealed class Bib32ParserIgnoreErrors
  {
    private struct Overrides : IBib32ParserUnsafeOverrides<Overrides>
    {
      public String32 EntryType;
      public int EntryTypeCode;
      public List2<Bib32Entry> Entries;
      public String32 StringNameOrFieldName;
      public List2<Bib32StringComponent> StringComponents;
      public String32 DatabaseKey;
      public List2<Bib32Field> Fields;
      public Dictionary<String32, String32> InternedStrings;

      [MethodImpl(Helper.JustOptimize)]
      public void Initialize()
      {
        Entries = new List2<Bib32Entry>();
        StringComponents = new List2<Bib32StringComponent>();
        Fields = new List2<Bib32Field>();
        InternedStrings = new Dictionary<String32, String32>();
      }

      [MethodImpl(Helper.JustOptimize)]
      public void Cleanup()
      {
        EntryType = default(String32);
        StringNameOrFieldName = default(String32);
        StringComponents.Clear();
        DatabaseKey = default(String32);
        Fields.Clear();
      }

      [MethodImpl(Helper.JustOptimize)]
      private String32 GetInternedString(ref int start, uint length)
      {
        String32 str = GetString(ref start, length);
        String32 interned;
        if (!InternedStrings.TryGetValue(str, out interned))
        {
          InternedStrings.Add(str, str);
          interned = str;
        }
        return interned;
      }

      [MethodImpl(Helper.JustOptimize)]
      private String32 GetString(ref int start, uint length)
      {
        if (length == 0)
        {
          return String32.Empty;
        }
        if (length < uint.MaxValue / 4u)
        {
          Char32[] data = new Char32[length];
          Unsafe.CopyBlockUnaligned(
            ref Unsafe.As<Char32, byte>(ref MemoryMarshal.GetArrayDataReference(data)),
            ref Unsafe.As<int, byte>(ref start),
            length * 4u
          );
          return Unsafe.As<Char32[], String32>(ref data);
        }
        /* This must be some stress test or insane! */
        return GetStringRare(ref Unsafe.As<int, byte>(ref start), length);
      }

      [MethodImpl(Helper.JustOptimize)]
      private String32 GetStringRare(ref byte start, uint length)
      {
        Char32[] data = GC.AllocateUninitializedArray<Char32>((int)length, false);
        ref byte data0 = ref Unsafe.As<Char32, byte>(ref MemoryMarshal.GetArrayDataReference(data));
        uint block = length & ~3u;
        Unsafe.CopyBlockUnaligned(ref data0, ref start, block);
        Unsafe.CopyBlockUnaligned(
          ref Unsafe.Add(ref data0, block),
          ref Unsafe.Add(ref start, block),
          block
        );
        Unsafe.CopyBlockUnaligned(
          ref Unsafe.Add(ref Unsafe.Add(ref data0, block), block),
          ref Unsafe.Add(ref Unsafe.Add(ref start, block), block),
          block
        );
        Unsafe.CopyBlockUnaligned(
          ref Unsafe.Add(ref Unsafe.Add(ref Unsafe.Add(ref data0, block), block), block),
          ref Unsafe.Add(ref Unsafe.Add(ref Unsafe.Add(ref start, block), block), block),
          length + 3u * (length & 3u)
        );
        return Unsafe.As<Char32[], String32>(ref data);
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
      public int SaveEntryType(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        String32 entryType = GetInternedString(ref start, (uint)length);
        EntryType = entryType;
        if (BibBstComparer.Equals(entryType, Bib32StringEntry.EntryType))
        {
          EntryTypeCode = 1;
          return 1;
        }
        if (BibBstComparer.Equals(entryType, Bib32PreambleEntry.EntryType))
        {
          EntryTypeCode = 2;
          StringComponents.Clear();
          return 2;
        }
        if (BibBstComparer.Equals(entryType, Bib32GeneralEntry.CommentEntryType))
        {
          EntryTypeCode = 3;
          return 3;
        }
        EntryTypeCode = 0;
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
      public void SaveStringName(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        StringNameOrFieldName = GetInternedString(ref start, (uint)length);
        StringComponents.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveNameComponent(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.NameValue),
          GetInternedString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveQuoteLiteralComponent(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.QuoteLiteralValue),
          GetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveNumericLiteralComponent(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.NumericLiteralValue),
          GetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveBraceLiteralComponent(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        StringComponents.Add(new Bib32StringComponent(
          new BibStringComponentType(BibStringComponentType.BraceLiteralValue),
          GetString(ref start, (uint)length)));
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveDatabaseKey(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
      {
        DatabaseKey = GetString(ref start, (uint)length);
        Fields.Clear();
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void SaveFieldName(ref Bib32ParserUnsafe<Overrides> that, ref int start, int length)
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

      [MethodImpl(Helper.OptimizeInline)]
      public void EntryExpectingType(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringEntryExpectingOpen(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringEntryExpectingName(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringEntryExpectingAssignment(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringEntryGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringEntryExpectingClose(ref Bib32ParserUnsafe<Overrides> that)
      {
        SaveStringEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void PreambleEntryExpectingOpen(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void PreambleEntryGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void PreambleEntryExpectingClose(ref Bib32ParserUnsafe<Overrides> that)
      {
        SavePreambleEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryExpectingOpen(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryExpectingDatabaseKey(ref Bib32ParserUnsafe<Overrides> that)
      {
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryExpectingFirstCommaOrClose(ref Bib32ParserUnsafe<Overrides> that)
      {
        SaveGeneralEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryExpectingFieldNameOrClose(ref Bib32ParserUnsafe<Overrides> that)
      {
        SaveGeneralEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryExpectingAssignment(ref Bib32ParserUnsafe<Overrides> that)
      {
        SaveGeneralEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
        SaveGeneralEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void GeneralEntryExpectingCommaOrClose(ref Bib32ParserUnsafe<Overrides> that)
      {
        SaveField(ref that);
        SaveGeneralEntry(ref that);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringExpectingComponent(ref Bib32ParserUnsafe<Overrides> that)
      {
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringQuoteLiteralGotNegativeBraceDepth(ref Bib32ParserUnsafe<Overrides> that)
      {
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringQuoteLiteralGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      public void StringBraceLiteralGotEndOfInput(ref Bib32ParserUnsafe<Overrides> that)
      {
        if (EntryTypeCode == 0)
        {
          SaveGeneralEntry(ref that);
        }
      }

      #endregion exceptions
    }

    private Bib32ParserUnsafe<Overrides> myParser;

    public readonly List2<Bib32Entry> Entries;

    [MethodImpl(Helper.JustOptimize)]
    public Bib32ParserIgnoreErrors()
    {
      myParser.Overrides.Initialize();
      Entries = myParser.Overrides.Entries;
    }

    /// <summary>
    /// Parses the content of a <c>.bib</c> file.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void Parse(String32 bib)
    {
      if (bib.IsDefault)
      {
        return;
      }
      try
      {
        myParser.Parse(
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(Unsafe.As<String32, Char32[]>(ref bib))),
          bib.Length);
      }
      finally
      {
        myParser.Overrides.Cleanup();
      }
    }
  }
}
