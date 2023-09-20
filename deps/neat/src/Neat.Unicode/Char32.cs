using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neat.Unicode
{
  /// <summary>
  /// <see cref="Char32"/> is binary-compatible with <see langword="int"/> and <see langword="uint"/>.
  /// An instance of <see cref="Char32"/> is not necessarily a valid Unicode code point.
  /// Every public method of this type should only dereference <see langword="this"/> once,
  /// a trick used by <see cref="ImmutableArray{T}"/> (though this trick could be coincidence-oriented
  /// programming; see <a href="https://github.com/dotnet/docs/issues/29696">dotnet/docs#29696</a>.)
  /// </summary>
  [DebuggerDisplay("{DebuggerDisplay(),nq}")]
  [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 4)]
  public readonly struct Char32 : IComparable<Char32>, IComparable, IEquatable<Char32>
  {
    [FieldOffset(0)]
    public readonly int Value;

    #region constructors, cast operators

    /// <summary>
    /// Initializes a new instance of <see cref="Char32"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Char32(int value)
    {
      Value = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Char32"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public Char32(uint value)
    {
      Value = (int)value;
    }

    /// <summary>
    /// Converts an <see langword="int"/> to a <see cref="Char32"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator Char32(int value)
    {
      return new Char32(value);
    }

    /// <summary>
    /// Converts a <see langword="uint"/> to a <see cref="Char32"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator Char32(uint value)
    {
      return new Char32((int)value);
    }

    /// <summary>
    /// Converts a <see cref="Char32"/> to an <see langword="int"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator int(Char32 value)
    {
      return value.Value;
    }

    /// <summary>
    /// Converts a <see cref="Char32"/> to a <see langword="uint"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a valid Unicode code point.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator uint(Char32 value)
    {
      return (uint)value.Value;
    }

    #endregion constructors, cast operators

    #region Compare, order operators, IComparable<Char32> members, IComparable members

    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(Char32 x, Char32 y)
    {
      int xx = x.Value, yy = y.Value;
      return xx < yy ? -1 : xx > yy ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(Char32 x, Char32 y)
    {
      return x.Value <= y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(Char32 x, Char32 y)
    {
      return x.Value >= y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(Char32 x, Char32 y)
    {
      return x.Value < y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(Char32 x, Char32 y)
    {
      return x.Value > y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(Char32 other)
    {
      int xx = Value, yy = other.Value;
      return xx < yy ? -1 : xx > yy ? 1 : 0;
    }

    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      if (obj is null)
      {
        return 1;
      }
      if (obj is Char32 other)
      {
        int xx = Value, yy = other.Value;
        return xx < yy ? -1 : xx > yy ? 1 : 0;
      }
      throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.Char32 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<Char32> members, IComparable members

    #region Equals, equality operators, IEquatable<Char32> members, object members

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(Char32 x, Char32 y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(Char32 x, Char32 y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(Char32 x, Char32 y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Char32 other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is Char32 other) && (Value == other.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return Value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly string[] theToStringResults = new string[128]
    {
      "Char32(^@)", "Char32(^A)", "Char32(^B)", "Char32(^C)",
      "Char32(^D)", "Char32(^E)", "Char32(^F)", "Char32(^G)",
      "Char32(^H)", "Char32(^I)", "Char32(^J)", "Char32(^K)",
      "Char32(^L)", "Char32(^M)", "Char32(^N)", "Char32(^O)",
      "Char32(^P)", "Char32(^Q)", "Char32(^R)", "Char32(^S)",
      "Char32(^T)", "Char32(^U)", "Char32(^V)", "Char32(^W)",
      "Char32(^X)", "Char32(^Y)", "Char32(^Z)", "Char32(^[)",
      "Char32(^\\)", "Char32(^])", "Char32(^^)", "Char32(^_)",
      " ", "!", "\"", "#", "$", "%", "&", "'",
      "(", ")", "*", "+", ",", "-", ".", "/",
      "0", "1", "2", "3", "4", "5", "6", "7",
      "8", "9", ":", ";", "<", "=", ">", "?",
      "@", "A", "B", "C", "D", "E", "F", "G",
      "H", "I", "J", "K", "L", "M", "N", "O",
      "P", "Q", "R", "S", "T", "U", "V", "W",
      "X", "Y", "Z", "[", "\\", "]", "^", "_",
      "`", "a", "b", "c", "d", "e", "f", "g",
      "h", "i", "j", "k", "l", "m", "n", "o",
      "p", "q", "r", "s", "t", "u", "v", "w",
      "x", "y", "z", "{", "|", "}", "~", "Char32(^?)"
    };

    [MethodImpl(Helper.OptimizeInline)]
    private static char GetHexit(int below16)
    {
      return (char)((below16 < 10 ? 48 : 55) + below16);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static unsafe string GetString(int value)
    {
      char* char18 = stackalloc char[18];
      if (UtfUnsafe.Char32IsBelow0x80(value))
      {
        return theToStringResults[value];
      }
      if (UtfUnsafe.Char32IsBelow0x10000(value))
      {
        if (UtfUnsafe.Char32IsSurrogate(value))
        {
          goto NotValidCodepoint;
        }
        return new string(UtfUnsafe.Char32To1Char16Unchecked(value), 1);
      }
      if (UtfUnsafe.Char32IsBelow0x110000(value))
      {
        value = UtfUnsafe.Char32To2Char16sUncheckedPrepare(value);
        char18[0] = UtfUnsafe.Char32PreparedTo2Char16sUncheckedHigh(value);
        char18[1] = UtfUnsafe.Char32PreparedTo2Char16sUncheckedLow(value);
        return new string(char18, 0, 2);
      }
    NotValidCodepoint:
      char18[0] = 'C';
      char18[1] = 'h';
      char18[2] = 'a';
      char18[3] = 'r';
      char18[4] = '3';
      char18[5] = '2';
      char18[6] = '(';
      char18[7] = '0';
      char18[8] = 'x';
      char18[17] = ')';
      for (int i = 16; i != 8; --i)
      {
        char18[i] = GetHexit(value & 0xF);
        value >>= 4;
      }
      return new string(char18, 0, 18);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override string ToString()
    {
      return GetString(Value);
    }

    #endregion Equals, equality operators, IEquatable<Char32> members, object members

    #region properties

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a valid Unicode code point.
    /// This means that <see cref="Value"/> is between either <c>0x0</c> and <c>0xD7FF</c> or <c>0xE000</c> and <c>0x10FFFF</c> (all inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsValid
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32IsValid(Value);
      }
    }

    #endregion properties

    #region properties related to Char8

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 1-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between <c>0x0</c> and <c>0x7F</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Is1Char8
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32Is1Char8(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 2-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between <c>0x80</c> and <c>0x7FF</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Is2Char8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32Is2Char8s(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 3-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between either <c>0x800</c> and <c>0xDBFF</c> or <c>0xE000</c> and <c>0xFFFF</c> (all inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Is3Char8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32Is3Char8s(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as a 4-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is between <c>0x10000</c> and <c>0x10FFFF</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Is4Char8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32Is4Char8s(Value);
      }
    }

    /// <summary>
    /// Gets the number of bytes used to encode the <see cref="Char32"/> instance in UTF-8.
    /// Returns <c>-1</c> if the instance is not a valid Unicode code point.
    /// </summary>
    public int LengthInChar8s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32LengthInChar8s(Value);
      }
    }

    #endregion properties related to Char8

    #region properties related to Char16

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a surrogate code point (which is invalid).
    /// This means that <see cref="Value"/> is between <c>0xD800</c> and <c>0xDFFF</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsSurrogate
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32IsSurrogate(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a high (leading) surrogate code point (which is invalid).
    /// This means that <see cref="Value"/> is between <c>0xD800</c> and <c>0xDBFF</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsHighSurrogate
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32IsHighSurrogate(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance is a low (leading) surrogate code point (which is invalid).
    /// This means that <see cref="Value"/> is between <c>0xDC00</c> and <c>0xDFFF</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsLowSurrogate
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32IsLowSurrogate(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as one code unit in UTF-16.
    /// This means that <see cref="Value"/> is between either <c>0x0</c> and <c>0xD7FF</c> or <c>0xE000</c> and <c>0xFFFF</c> (all inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Is1Char16
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32Is1Char16(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char32"/> instance can be encoded as two code units in UTF-16.
    /// This means that <see cref="Value"/> is between <c>0x10000</c> and <c>0x10FFFF</c> (both inclusive).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Is2Char16s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32Is2Char16s(Value);
      }
    }

    /// <summary>
    /// Gets the number of code units used to encode the <see cref="Char32"/> instance in UTF-16.
    /// Returns <c>-1</c> if the instance is not a valid Unicode code point.
    /// </summary>
    public int LengthInChar16s
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char32LengthInChar16s(Value);
      }
    }

    #endregion properties related to Char16

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see cref="Char32"/>.
    /// The comparison is ordinal.
    /// </summary>
    public struct Comparer : IComparer<Char32>, IEqualityComparer2<Char32>
    {
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(Char32 x, Char32 y)
      {
        int xx = x.Value, yy = y.Value;
        return xx < yy ? -1 : xx > yy ? 1 : 0;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(Char32 x, Char32 y)
      {
        return x.Value == y.Value;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(Char32 obj)
      {
        return obj.Value;
      }
    }

    #region debugging

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly string[] theDebuggerDisplayResults = new string[128]
    {
      "0 Char32(^@)", "1 Char32(^A)", "2 Char32(^B)", "3 Char32(^C)",
      "4 Char32(^D)", "5 Char32(^E)", "6 Char32(^F)", "7 Char32(^G)",
      "8 Char32(^H)", "9 Char32(^I)", "10 Char32(^J)", "11 Char32(^K)",
      "12 Char32(^L)", "13 Char32(^M)", "14 Char32(^N)", "15 Char32(^O)",
      "16 Char32(^P)", "17 Char32(^Q)", "18 Char32(^R)", "19 Char32(^S)",
      "20 Char32(^T)", "21 Char32(^U)", "22 Char32(^V)", "23 Char32(^W)",
      "24 Char32(^X)", "25 Char32(^Y)", "26 Char32(^Z)", "27 Char32(^[)",
      "28 Char32(^\\)", "29 Char32(^])", "30 Char32(^^)", "31 Char32(^_)",
      "32 ' '", "33 '!'", "34 '\"'", "35 '#'", "36 '$'", "37 '%'", "38 '&'", "39 '\\''",
      "40 '('", "41 ')'", "42 '*'", "43 '+'", "44 ','", "45 '-'", "46 '.'", "47 '/'",
      "48 '0'", "49 '1'", "50 '2'", "51 '3'", "52 '4'", "53 '5'", "54 '6'", "55 '7'",
      "56 '8'", "57 '9'", "58 ':'", "59 ';'", "60 '<'", "61 '='", "62 '>'", "63 '?'",
      "64 '@'", "65 'A'", "66 'B'", "67 'C'", "68 'D'", "69 'E'", "70 'F'", "71 'G'",
      "72 'H'", "73 'I'", "74 'J'", "75 'K'", "76 'L'", "77 'M'", "78 'N'", "79 'O'",
      "80 'P'", "81 'Q'", "82 'R'", "83 'S'", "84 'T'", "85 'U'", "86 'V'", "87 'W'",
      "88 'X'", "89 'Y'", "90 'Z'", "91 '['", "92 '\\'", "93 ']'", "94 '^'", "95 '_'",
      "96 '`'", "97 'a'", "98 'b'", "99 'c'", "100 'd'", "101 'e'", "102 'f'", "103 'g'",
      "104 'h'", "105 'i'", "106 'j'", "107 'k'", "108 'l'", "109 'm'", "110 'n'", "111 'o'",
      "112 'p'", "113 'q'", "114 'r'", "115 's'", "116 't'", "117 'u'", "118 'v'", "119 'w'",
      "120 'x'", "121 'y'", "122 'z'", "123 '{'", "124 '|'", "125 '}'", "126 '~'", "127 Char32(^?)"
    };

    private string DebuggerDisplay()
    {
      int value = Value;
      if (UtfUnsafe.Char32IsBelow0x80(value))
      {
        return theDebuggerDisplayResults[value];
      }
      string toString = GetString(value);
      if (toString.Length < 6)
      {
        return value.ToString(CultureInfo.InvariantCulture) + " '" + toString + "'";
      }
      if (UtfUnsafe.Char32IsHighSurrogate(value))
      {
        return value.ToString(CultureInfo.InvariantCulture) + " " + toString + " <high surrogate>";
      }
      if (UtfUnsafe.Char32IsLowSurrogate(value))
      {
        return value.ToString(CultureInfo.InvariantCulture) + " " + toString + " <low surrogate>";
      }
      return value.ToString(CultureInfo.InvariantCulture) + " " + toString + " <too large>";
    }

    #endregion debugging
  }
}
