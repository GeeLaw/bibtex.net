using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Unicode;

namespace Neat.BibTeX.Utils
{
  public static class BibBstChars
  {
    public const int Invalid = 127;

    /* BibTeX parser */
    public const int At = '@';
    public const int LeftBrace = '{';
    public const int RightBrace = '}';
    public const int LeftParenthesis = '(';
    public const int RightParenthesis = ')';
    public const int Assignment = '=';
    public const int DoubleQuote = '"';
    public const int Concatenation = '#';

    /* BST special character */
    public const int Escape = '\\';
    /* add.period$ */
    public const int Period = '.';
    public const int QuestionMark = '?';
    public const int ExclamationMark = '!';
    /* format.name$ */
    public const int Hyphen = '-';
    public const int Tie = '~';
    /* change.case$ */
    public const int Colon = ':';

    #region Char32

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(String32 str)
    {
      return IsIdentifierImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(Char32 ch)
    {
      return IsIdentifierImpl(ch.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsAlpha(Char32 ch)
    {
      return IsAlphaImpl(ch.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsNumeric(Char32 ch)
    {
      return IsNumericImpl(ch.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsSpace(Char32 ch)
    {
      return IsSpaceImpl(ch.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsValidAndBraceBalanced(String32 str)
    {
      return IsValidAndBraceBalancedImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    #endregion Char32

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 128)]
    private readonly struct IsIdentifierCharacterData
    {
      [SuppressMessage("Style", "IDE0060", Justification = "Avoid parameterless constructor.")]
      internal IsIdentifierCharacterData(int unused)
      {
      }

      public readonly bool Item0 = false;
      public readonly bool Item1 = false;
      public readonly bool Item2 = false;
      public readonly bool Item3 = false;
      public readonly bool Item4 = false;
      public readonly bool Item5 = false;
      public readonly bool Item6 = false;
      public readonly bool Item7 = false;
      public readonly bool Item8 = false;
      public readonly bool Item9 = false;
      public readonly bool Item10 = false;
      public readonly bool Item11 = false;
      public readonly bool Item12 = false;
      public readonly bool Item13 = false;
      public readonly bool Item14 = false;
      public readonly bool Item15 = false;
      public readonly bool Item16 = false;
      public readonly bool Item17 = false;
      public readonly bool Item18 = false;
      public readonly bool Item19 = false;
      public readonly bool Item20 = false;
      public readonly bool Item21 = false;
      public readonly bool Item22 = false;
      public readonly bool Item23 = false;
      public readonly bool Item24 = false;
      public readonly bool Item25 = false;
      public readonly bool Item26 = false;
      public readonly bool Item27 = false;
      public readonly bool Item28 = false;
      public readonly bool Item29 = false;
      public readonly bool Item30 = false;
      public readonly bool Item31 = false;
      public readonly bool Item32 = false;
      public readonly bool Item33 = true;
      public readonly bool Item34 = false;
      public readonly bool Item35 = false;
      public readonly bool Item36 = true;
      public readonly bool Item37 = false;
      public readonly bool Item38 = true;
      public readonly bool Item39 = false;
      public readonly bool Item40 = false;
      public readonly bool Item41 = false;
      public readonly bool Item42 = true;
      public readonly bool Item43 = true;
      public readonly bool Item44 = false;
      public readonly bool Item45 = true;
      public readonly bool Item46 = true;
      public readonly bool Item47 = true;
      public readonly bool Item48 = true;
      public readonly bool Item49 = true;
      public readonly bool Item50 = true;
      public readonly bool Item51 = true;
      public readonly bool Item52 = true;
      public readonly bool Item53 = true;
      public readonly bool Item54 = true;
      public readonly bool Item55 = true;
      public readonly bool Item56 = true;
      public readonly bool Item57 = true;
      public readonly bool Item58 = true;
      public readonly bool Item59 = true;
      public readonly bool Item60 = true;
      public readonly bool Item61 = false;
      public readonly bool Item62 = true;
      public readonly bool Item63 = true;
      /* @ is a valid identifier character, which sounds terrible to me. */
      public readonly bool Item64 = true;
      public readonly bool Item65 = true;
      public readonly bool Item66 = true;
      public readonly bool Item67 = true;
      public readonly bool Item68 = true;
      public readonly bool Item69 = true;
      public readonly bool Item70 = true;
      public readonly bool Item71 = true;
      public readonly bool Item72 = true;
      public readonly bool Item73 = true;
      public readonly bool Item74 = true;
      public readonly bool Item75 = true;
      public readonly bool Item76 = true;
      public readonly bool Item77 = true;
      public readonly bool Item78 = true;
      public readonly bool Item79 = true;
      public readonly bool Item80 = true;
      public readonly bool Item81 = true;
      public readonly bool Item82 = true;
      public readonly bool Item83 = true;
      public readonly bool Item84 = true;
      public readonly bool Item85 = true;
      public readonly bool Item86 = true;
      public readonly bool Item87 = true;
      public readonly bool Item88 = true;
      public readonly bool Item89 = true;
      public readonly bool Item90 = true;
      public readonly bool Item91 = true;
      public readonly bool Item92 = true;
      public readonly bool Item93 = true;
      public readonly bool Item94 = true;
      public readonly bool Item95 = true;
      public readonly bool Item96 = true;
      public readonly bool Item97 = true;
      public readonly bool Item98 = true;
      public readonly bool Item99 = true;
      public readonly bool Item100 = true;
      public readonly bool Item101 = true;
      public readonly bool Item102 = true;
      public readonly bool Item103 = true;
      public readonly bool Item104 = true;
      public readonly bool Item105 = true;
      public readonly bool Item106 = true;
      public readonly bool Item107 = true;
      public readonly bool Item108 = true;
      public readonly bool Item109 = true;
      public readonly bool Item110 = true;
      public readonly bool Item111 = true;
      public readonly bool Item112 = true;
      public readonly bool Item113 = true;
      public readonly bool Item114 = true;
      public readonly bool Item115 = true;
      public readonly bool Item116 = true;
      public readonly bool Item117 = true;
      public readonly bool Item118 = true;
      public readonly bool Item119 = true;
      public readonly bool Item120 = true;
      public readonly bool Item121 = true;
      public readonly bool Item122 = true;
      public readonly bool Item123 = false;
      public readonly bool Item124 = true;
      public readonly bool Item125 = false;
      public readonly bool Item126 = true;
      /* This is different from the original BibTeX implementation,
      /* which did set DEL to be a valid identifier character.
      /* However, DEL has always been set to be an invalid input character,
      /* so this actually does not matter. */
      public readonly bool Item127 = false;
    }

    private static IsIdentifierCharacterData theIsIdentifierCharacter = new IsIdentifierCharacterData(0);

    [MethodImpl(Helper.JustOptimize)]
    internal static bool IsIdentifierImpl(Char32[] data)
    {
      if (data is null || data.Length == 0 || IsNumericImpl(data[0].Value))
      {
        return false;
      }
      for (int i = 0; i < data.Length; ++i)
      {
        if (!IsIdentifierImpl(data[i].Value))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool IsIdentifierImpl(int value)
    {
      /* Aside from 0-9, A-Z, a-z, these characters are valid identifier characters: !$&*+-./:;<>?@[\]^_`|~ */
      return (uint)value < 128u
        && Unsafe.Add(ref Unsafe.As<IsIdentifierCharacterData, bool>(ref theIsIdentifierCharacter), value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool IsAlphaImpl(int value)
    {
      return (uint)(value - 65) <= 25u || (uint)(value - 97) <= 25u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool IsNumericImpl(int value)
    {
      return (uint)(value - 48) <= 9u;
    }

    [MethodImpl(Helper.OptimizeInline)]
    internal static bool IsSpaceImpl(int value)
    {
      /* space, \t, \n, \v, \f, \r */
      return value == 32 || (uint)(value - 9) <= 4u;
    }

    [MethodImpl(Helper.JustOptimize)]
    internal static bool IsValidAndBraceBalancedImpl(Char32[] data)
    {
      if (data is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < data.Length; ++i)
      {
        value = data[i].Value;
        if (value == Invalid)
        {
          return false;
        }
        else if (value == LeftBrace)
        {
          ++depth;
        }
        else if (value == RightBrace)
        {
          if (depth-- == 0)
          {
            return false;
          }
        }
      }
      return depth == 0;
    }
  }
}
