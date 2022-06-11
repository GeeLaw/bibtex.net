using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.BibTeX.BibModel;
using Neat.Unicode;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Provides information about characters and strings in BibTeX.
  /// </summary>
  public static class BibBstChars
  {
    public const int CR = '\r';
    public const int LF = '\n';

    /* BibTeX parser */
    public const int At = '@';
    public const int LeftBrace = '{';
    public const int RightBrace = '}';
    public const int LeftParenthesis = '(';
    public const int RightParenthesis = ')';
    public const int Assignment = '=';
    public const int DoubleQuote = '"';
    public const int Concatenation = '#';
    public const int Comma = ',';

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

    #region String32, Char32

    /// <summary>
    /// An identifier is non-<see langword="default"/>, non-empty, cannot start with a numeric character, and
    /// consists of only identifier characters.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(String32 str)
    {
      return IsIdentifierImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    /// <summary>
    /// Consult the original implementation of BibTeX for what constitutes a valid identifier character.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(Char32 ch)
    {
      return IsIdentifierImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the database key is valid and whether the general entry must use parentheses.
    /// A database key must not be <see langword="default"/> or contain <c>,</c> or space characters.
    /// It can be empty.
    /// The general entry must use parenthesis if the database key contains <c>}</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static BibDatabaseKeyType GetDatabaseKeyType(String32 str)
    {
      return GetDatabaseKeyTypeImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    /// <summary>
    /// Determines whether the character is alphabetc (only A-Z and a-z are considered alphabetic).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsAlpha(Char32 ch)
    {
      return IsAlphaImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the character is numeric (only 0-9 are considered numeric).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsNumeric(Char32 ch)
    {
      return IsNumericImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the character is space (only <c>U+0020</c> and <c>\t\n\v\f\r</c> are considered space).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsSpace(Char32 ch)
    {
      return IsSpaceImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the literal can be quote-delimited.
    /// Such a literal must not be <see langword="default"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsQuoteLiteral(String32 str)
    {
      return IsQuoteLiteralImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    /// <summary>
    /// Determines whether the literal is a valid numeric literal.
    /// Such a literal must not be <see langword="default"/> or empty.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsNumericLiteral(String32 str)
    {
      return IsNumericLiteralImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    /// <summary>
    /// Determines whether the literal is brace-balanced.
    /// Such a literal must not be <see langword="default"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsBraceBalanced(String32 str)
    {
      return IsBraceBalancedImpl(Unsafe.As<String32, Char32[]>(ref str));
    }

    #endregion String32, Char32

    #region String16, Char16

    /// <summary>
    /// An identifier is non-<see langword="default"/>, non-empty, cannot start with a numeric character, and
    /// consists of only identifier characters.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool IsIdentifier(string str)
    {
      if (str is null || str.Length == 0 || IsNumericImpl(str[0]))
      {
        return false;
      }
      for (int i = 0, value; i < str.Length; ++i)
      {
        value = str[i];
        if ((uint)value >= 128u
          || !Unsafe.Add(ref Unsafe.As<IsIdentifierCharacterData, bool>(ref theIsIdentifierCharacter), value))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Consult the original implementation of BibTeX for what constitutes a valid identifier character.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(char ch)
    {
      return IsIdentifierImpl(ch);
    }

    /// <summary>
    /// Determines whether the database key is valid and whether the general entry must use parentheses.
    /// A database key must not be <see langword="default"/> or contain <c>,</c> or space characters.
    /// It can be empty.
    /// The general entry must use parenthesis if the database key contains <c>}</c>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static BibDatabaseKeyType GetDatabaseKeyType(string str)
    {
      if (str is null)
      {
        return new BibDatabaseKeyType(BibDatabaseKeyType.InvalidValue);
      }
      bool mustUseParentheses = false;
      for (int i = 0, value; i < str.Length; ++i)
      {
        value = str[i];
        if (value == Comma || IsSpaceImpl(value))
        {
          return new BibDatabaseKeyType(BibDatabaseKeyType.InvalidValue);
        }
        if (value == RightBrace)
        {
          mustUseParentheses = true;
        }
      }
      return new BibDatabaseKeyType(mustUseParentheses
        ? BibDatabaseKeyType.MustUseParenthesesValue
        : BibDatabaseKeyType.UseBracesOrParenthesesValue);
    }

    /// <summary>
    /// Determines whether the character is alphabetc (only A-Z and a-z are considered alphabetic).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsAlpha(char ch)
    {
      return IsAlphaImpl(ch);
    }

    /// <summary>
    /// Determines whether the character is numeric (only 0-9 are considered numeric).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsNumeric(char ch)
    {
      return IsNumericImpl(ch);
    }

    /// <summary>
    /// Determines whether the character is space (only <c>U+0020</c> and <c>\t\n\v\f\r</c> are considered space).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsSpace(char ch)
    {
      return IsSpaceImpl(ch);
    }

    /// <summary>
    /// Determines whether the literal can be quote-delimited.
    /// Such a literal must not be <see langword="default"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool IsQuoteLiteral(string str)
    {
      if (str is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < str.Length; ++i)
      {
        if ((value = str[i]) == LeftBrace)
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
        else if (value == DoubleQuote && depth == 0)
        {
          return false;
        }
      }
      return depth == 0;
    }

    /// <summary>
    /// Determines whether the literal is a valid numeric literal.
    /// Such a literal must not be <see langword="default"/> or empty.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool IsNumericLiteral(string str)
    {
      if (str is null || str.Length == 0)
      {
        return false;
      }
      for (int i = 0; i < str.Length; ++i)
      {
        if (!IsNumericImpl(str[i]))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Determines whether the literal is brace-balanced.
    /// Such a literal must not be <see langword="default"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool IsBraceBalanced(string str)
    {
      if (str is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < str.Length; ++i)
      {
        if ((value = str[i]) == LeftBrace)
        {
          ++depth;
        }
        else if (value == RightBrace && depth-- == 0)
        {
          return false;
        }
      }
      return depth == 0;
    }

    #endregion String16, Char16

    #region String8, Char8

    /// <summary>
    /// An identifier is non-<see langword="default"/>, non-empty, cannot start with a numeric character, and
    /// consists of only identifier characters.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(String8 str)
    {
      return IsIdentifierImpl(Unsafe.As<String8, Char8[]>(ref str));
    }

    /// <summary>
    /// Consult the original implementation of BibTeX for what constitutes a valid identifier character.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsIdentifier(Char8 ch)
    {
      return IsIdentifierImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the database key is valid and whether the general entry must use parentheses.
    /// A database key must not be <see langword="default"/> or contain <c>,</c> or space characters.
    /// It can be empty.
    /// The general entry must use parenthesis if the database key contains <c>}</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static BibDatabaseKeyType GetDatabaseKeyType(String8 str)
    {
      return GetDatabaseKeyTypeImpl(Unsafe.As<String8, Char8[]>(ref str));
    }

    /// <summary>
    /// Determines whether the character is alphabetc (only A-Z and a-z are considered alphabetic).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsAlpha(Char8 ch)
    {
      return IsAlphaImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the character is numeric (only 0-9 are considered numeric).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsNumeric(Char8 ch)
    {
      return IsNumericImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the character is space (only <c>U+0020</c> and <c>\t\n\v\f\r</c> are considered space).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsSpace(Char8 ch)
    {
      return IsSpaceImpl(ch.Value);
    }

    /// <summary>
    /// Determines whether the literal can be quote-delimited.
    /// Such a literal must not be <see langword="default"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsQuoteLiteral(String8 str)
    {
      return IsQuoteLiteralImpl(Unsafe.As<String8, Char8[]>(ref str));
    }

    /// <summary>
    /// Determines whether the literal is a valid numeric literal.
    /// Such a literal must not be <see langword="default"/> or empty.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsNumericLiteral(String8 str)
    {
      return IsNumericLiteralImpl(Unsafe.As<String8, Char8[]>(ref str));
    }

    /// <summary>
    /// Determines whether the literal is brace-balanced.
    /// Such a literal must not be <see langword="default"/>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsBraceBalanced(String8 str)
    {
      return IsBraceBalancedImpl(Unsafe.As<String8, Char8[]>(ref str));
    }

    #endregion String8, Char8

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 128)]
    private readonly struct IsIdentifierCharacterData
    {
      /* It is important that we use explicit (instead of sequential) layout.
      /* Since "bool" is not blittable, sequential layout does not control the managed representation.
      /* We would like to access ItemI by
      /*     Unsafe.Add(ref Unsafe.As<IsIdentifierCharacterData, byte>(ref that), I),
      /* therefore, we must force the correct managed representation. */

      [SuppressMessage("Style", "IDE0060", Justification = "Avoid parameterless constructor.")]
      internal IsIdentifierCharacterData(int unused)
      {
      }

      [FieldOffset(0)]
      public readonly bool Item0 = false;

      [FieldOffset(1)]
      public readonly bool Item1 = false;

      [FieldOffset(2)]
      public readonly bool Item2 = false;

      [FieldOffset(3)]
      public readonly bool Item3 = false;

      [FieldOffset(4)]
      public readonly bool Item4 = false;

      [FieldOffset(5)]
      public readonly bool Item5 = false;

      [FieldOffset(6)]
      public readonly bool Item6 = false;

      [FieldOffset(7)]
      public readonly bool Item7 = false;

      [FieldOffset(8)]
      public readonly bool Item8 = false;

      [FieldOffset(9)]
      public readonly bool Item9 = false;

      [FieldOffset(10)]
      public readonly bool Item10 = false;

      [FieldOffset(11)]
      public readonly bool Item11 = false;

      [FieldOffset(12)]
      public readonly bool Item12 = false;

      [FieldOffset(13)]
      public readonly bool Item13 = false;

      [FieldOffset(14)]
      public readonly bool Item14 = false;

      [FieldOffset(15)]
      public readonly bool Item15 = false;

      [FieldOffset(16)]
      public readonly bool Item16 = false;

      [FieldOffset(17)]
      public readonly bool Item17 = false;

      [FieldOffset(18)]
      public readonly bool Item18 = false;

      [FieldOffset(19)]
      public readonly bool Item19 = false;

      [FieldOffset(20)]
      public readonly bool Item20 = false;

      [FieldOffset(21)]
      public readonly bool Item21 = false;

      [FieldOffset(22)]
      public readonly bool Item22 = false;

      [FieldOffset(23)]
      public readonly bool Item23 = false;

      [FieldOffset(24)]
      public readonly bool Item24 = false;

      [FieldOffset(25)]
      public readonly bool Item25 = false;

      [FieldOffset(26)]
      public readonly bool Item26 = false;

      [FieldOffset(27)]
      public readonly bool Item27 = false;

      [FieldOffset(28)]
      public readonly bool Item28 = false;

      [FieldOffset(29)]
      public readonly bool Item29 = false;

      [FieldOffset(30)]
      public readonly bool Item30 = false;

      [FieldOffset(31)]
      public readonly bool Item31 = false;

      [FieldOffset(32)]
      public readonly bool Item32 = false;

      [FieldOffset(33)]
      public readonly bool Item33 = true;

      [FieldOffset(34)]
      public readonly bool Item34 = false;

      [FieldOffset(35)]
      public readonly bool Item35 = false;

      [FieldOffset(36)]
      public readonly bool Item36 = true;

      [FieldOffset(37)]
      public readonly bool Item37 = false;

      [FieldOffset(38)]
      public readonly bool Item38 = true;

      [FieldOffset(39)]
      public readonly bool Item39 = false;

      [FieldOffset(40)]
      public readonly bool Item40 = false;

      [FieldOffset(41)]
      public readonly bool Item41 = false;

      [FieldOffset(42)]
      public readonly bool Item42 = true;

      [FieldOffset(43)]
      public readonly bool Item43 = true;

      [FieldOffset(44)]
      public readonly bool Item44 = false;

      [FieldOffset(45)]
      public readonly bool Item45 = true;

      [FieldOffset(46)]
      public readonly bool Item46 = true;

      [FieldOffset(47)]
      public readonly bool Item47 = true;

      [FieldOffset(48)]
      public readonly bool Item48 = true;

      [FieldOffset(49)]
      public readonly bool Item49 = true;

      [FieldOffset(50)]
      public readonly bool Item50 = true;

      [FieldOffset(51)]
      public readonly bool Item51 = true;

      [FieldOffset(52)]
      public readonly bool Item52 = true;

      [FieldOffset(53)]
      public readonly bool Item53 = true;

      [FieldOffset(54)]
      public readonly bool Item54 = true;

      [FieldOffset(55)]
      public readonly bool Item55 = true;

      [FieldOffset(56)]
      public readonly bool Item56 = true;

      [FieldOffset(57)]
      public readonly bool Item57 = true;

      [FieldOffset(58)]
      public readonly bool Item58 = true;

      [FieldOffset(59)]
      public readonly bool Item59 = true;

      [FieldOffset(60)]
      public readonly bool Item60 = true;

      [FieldOffset(61)]
      public readonly bool Item61 = false;

      [FieldOffset(62)]
      public readonly bool Item62 = true;

      [FieldOffset(63)]
      public readonly bool Item63 = true;

      [FieldOffset(64)]
      public readonly bool Item64 = true;

      [FieldOffset(65)]
      public readonly bool Item65 = true;

      [FieldOffset(66)]
      public readonly bool Item66 = true;

      [FieldOffset(67)]
      public readonly bool Item67 = true;

      [FieldOffset(68)]
      public readonly bool Item68 = true;

      [FieldOffset(69)]
      public readonly bool Item69 = true;

      [FieldOffset(70)]
      public readonly bool Item70 = true;

      [FieldOffset(71)]
      public readonly bool Item71 = true;

      [FieldOffset(72)]
      public readonly bool Item72 = true;

      [FieldOffset(73)]
      public readonly bool Item73 = true;

      [FieldOffset(74)]
      public readonly bool Item74 = true;

      [FieldOffset(75)]
      public readonly bool Item75 = true;

      [FieldOffset(76)]
      public readonly bool Item76 = true;

      [FieldOffset(77)]
      public readonly bool Item77 = true;

      [FieldOffset(78)]
      public readonly bool Item78 = true;

      [FieldOffset(79)]
      public readonly bool Item79 = true;

      [FieldOffset(80)]
      public readonly bool Item80 = true;

      [FieldOffset(81)]
      public readonly bool Item81 = true;

      [FieldOffset(82)]
      public readonly bool Item82 = true;

      [FieldOffset(83)]
      public readonly bool Item83 = true;

      [FieldOffset(84)]
      public readonly bool Item84 = true;

      [FieldOffset(85)]
      public readonly bool Item85 = true;

      [FieldOffset(86)]
      public readonly bool Item86 = true;

      [FieldOffset(87)]
      public readonly bool Item87 = true;

      [FieldOffset(88)]
      public readonly bool Item88 = true;

      [FieldOffset(89)]
      public readonly bool Item89 = true;

      [FieldOffset(90)]
      public readonly bool Item90 = true;

      [FieldOffset(91)]
      public readonly bool Item91 = true;

      [FieldOffset(92)]
      public readonly bool Item92 = true;

      [FieldOffset(93)]
      public readonly bool Item93 = true;

      [FieldOffset(94)]
      public readonly bool Item94 = true;

      [FieldOffset(95)]
      public readonly bool Item95 = true;

      [FieldOffset(96)]
      public readonly bool Item96 = true;

      [FieldOffset(97)]
      public readonly bool Item97 = true;

      [FieldOffset(98)]
      public readonly bool Item98 = true;

      [FieldOffset(99)]
      public readonly bool Item99 = true;

      [FieldOffset(100)]
      public readonly bool Item100 = true;

      [FieldOffset(101)]
      public readonly bool Item101 = true;

      [FieldOffset(102)]
      public readonly bool Item102 = true;

      [FieldOffset(103)]
      public readonly bool Item103 = true;

      [FieldOffset(104)]
      public readonly bool Item104 = true;

      [FieldOffset(105)]
      public readonly bool Item105 = true;

      [FieldOffset(106)]
      public readonly bool Item106 = true;

      [FieldOffset(107)]
      public readonly bool Item107 = true;

      [FieldOffset(108)]
      public readonly bool Item108 = true;

      [FieldOffset(109)]
      public readonly bool Item109 = true;

      [FieldOffset(110)]
      public readonly bool Item110 = true;

      [FieldOffset(111)]
      public readonly bool Item111 = true;

      [FieldOffset(112)]
      public readonly bool Item112 = true;

      [FieldOffset(113)]
      public readonly bool Item113 = true;

      [FieldOffset(114)]
      public readonly bool Item114 = true;

      [FieldOffset(115)]
      public readonly bool Item115 = true;

      [FieldOffset(116)]
      public readonly bool Item116 = true;

      [FieldOffset(117)]
      public readonly bool Item117 = true;

      [FieldOffset(118)]
      public readonly bool Item118 = true;

      [FieldOffset(119)]
      public readonly bool Item119 = true;

      [FieldOffset(120)]
      public readonly bool Item120 = true;

      [FieldOffset(121)]
      public readonly bool Item121 = true;

      [FieldOffset(122)]
      public readonly bool Item122 = true;

      [FieldOffset(123)]
      public readonly bool Item123 = false;

      [FieldOffset(124)]
      public readonly bool Item124 = true;

      [FieldOffset(125)]
      public readonly bool Item125 = false;

      [FieldOffset(126)]
      public readonly bool Item126 = true;

      [FieldOffset(127)]
      public readonly bool Item127 = true;
    }

    private static IsIdentifierCharacterData theIsIdentifierCharacter = new IsIdentifierCharacterData(0);

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsIdentifierImpl(Char32[] data)
    {
      if (data is null || data.Length == 0 || IsNumericImpl(data[0].Value))
      {
        return false;
      }
      for (int i = 0, value; i < data.Length; ++i)
      {
        value = data[i].Value;
        if ((uint)value >= 128u
          || !Unsafe.Add(ref Unsafe.As<IsIdentifierCharacterData, bool>(ref theIsIdentifierCharacter), value))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsIdentifierImpl(Char8[] data)
    {
      if (data is null || data.Length == 0 || IsNumericImpl(data[0].Value))
      {
        return false;
      }
      for (int i = 0, value; i < data.Length; ++i)
      {
        value = data[i].Value;
        if ((uint)value >= 128u
          || !Unsafe.Add(ref Unsafe.As<IsIdentifierCharacterData, bool>(ref theIsIdentifierCharacter), value))
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

    [MethodImpl(Helper.JustOptimize)]
    private static BibDatabaseKeyType GetDatabaseKeyTypeImpl(Char32[] data)
    {
      if (data is null)
      {
        return new BibDatabaseKeyType(BibDatabaseKeyType.InvalidValue);
      }
      bool mustUseParentheses = false;
      for (int i = 0, value; i < data.Length; ++i)
      {
        value = data[i].Value;
        if (value == Comma || IsSpaceImpl(value))
        {
          return new BibDatabaseKeyType(BibDatabaseKeyType.InvalidValue);
        }
        if (value == RightBrace)
        {
          mustUseParentheses = true;
        }
      }
      return new BibDatabaseKeyType(mustUseParentheses
        ? BibDatabaseKeyType.MustUseParenthesesValue
        : BibDatabaseKeyType.UseBracesOrParenthesesValue);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static BibDatabaseKeyType GetDatabaseKeyTypeImpl(Char8[] data)
    {
      if (data is null)
      {
        return new BibDatabaseKeyType(BibDatabaseKeyType.InvalidValue);
      }
      bool mustUseParentheses = false;
      for (int i = 0, value; i < data.Length; ++i)
      {
        value = data[i].Value;
        if (value == Comma || IsSpaceImpl(value))
        {
          return new BibDatabaseKeyType(BibDatabaseKeyType.InvalidValue);
        }
        if (value == RightBrace)
        {
          mustUseParentheses = true;
        }
      }
      return new BibDatabaseKeyType(mustUseParentheses
        ? BibDatabaseKeyType.MustUseParenthesesValue
        : BibDatabaseKeyType.UseBracesOrParenthesesValue);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private static bool IsAlphaImpl(int value)
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
    private static bool IsQuoteLiteralImpl(Char32[] data)
    {
      if (data is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < data.Length; ++i)
      {
        if ((value = data[i].Value) == LeftBrace)
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
        else if (value == DoubleQuote && depth == 0)
        {
          return false;
        }
      }
      return depth == 0;
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsQuoteLiteralImpl(Char8[] data)
    {
      if (data is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < data.Length; ++i)
      {
        if ((value = data[i].Value) == LeftBrace)
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
        else if (value == DoubleQuote && depth == 0)
        {
          return false;
        }
      }
      return depth == 0;
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsNumericLiteralImpl(Char32[] data)
    {
      if (data is null || data.Length == 0)
      {
        return false;
      }
      for (int i = 0; i < data.Length; ++i)
      {
        if (!IsNumericImpl(data[i].Value))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsNumericLiteralImpl(Char8[] data)
    {
      if (data is null || data.Length == 0)
      {
        return false;
      }
      for (int i = 0; i < data.Length; ++i)
      {
        if (!IsNumericImpl(data[i].Value))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsBraceBalancedImpl(Char32[] data)
    {
      if (data is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < data.Length; ++i)
      {
        if ((value = data[i].Value) == LeftBrace)
        {
          ++depth;
        }
        else if (value == RightBrace && depth-- == 0)
        {
          return false;
        }
      }
      return depth == 0;
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool IsBraceBalancedImpl(Char8[] data)
    {
      if (data is null)
      {
        return false;
      }
      int depth = 0;
      for (int i = 0, value; i < data.Length; ++i)
      {
        if ((value = data[i].Value) == LeftBrace)
        {
          ++depth;
        }
        else if (value == RightBrace && depth-- == 0)
        {
          return false;
        }
      }
      return depth == 0;
    }
  }
}
