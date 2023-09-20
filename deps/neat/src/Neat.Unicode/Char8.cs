using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neat.Unicode
{
  /// <summary>
  /// <see cref="Char8"/> is binary-compatible with <see langword="byte"/> and <see langword="sbyte"/>.
  /// An instance of <see cref="Char8"/> is not necessarily a possible byte in a valid UTF-8 sequence.
  /// Every public method of this type should only dereference <see langword="this"/> once,
  /// a trick used by <see cref="ImmutableArray{T}"/> (though this trick could be coincidence-oriented
  /// programming; see <a href="https://github.com/dotnet/docs/issues/29696">dotnet/docs#29696</a>.)
  /// </summary>
  [DebuggerDisplay("{DebuggerDisplay(),nq}")]
  [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1)]
  public readonly struct Char8 : IComparable<Char8>, IComparable, IEquatable<Char8>
  {
    [FieldOffset(0)]
    public readonly byte Value;

    #region constructors, cast operators

    /// <summary>
    /// Initializes a new instance of <see cref="Char8"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Char8(byte value)
    {
      Value = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Char8"/>.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public Char8(sbyte value)
    {
      Value = (byte)value;
    }

    /// <summary>
    /// Converts a <see langword="byte"/> to a <see cref="Char8"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator Char8(byte value)
    {
      return new Char8(value);
    }

    /// <summary>
    /// Converts a <see langword="sbyte"/> to a <see cref="Char8"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator Char8(sbyte value)
    {
      return new Char8((byte)value);
    }

    /// <summary>
    /// Converts a <see cref="Char8"/> to a <see langword="byte"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator byte(Char8 value)
    {
      return value.Value;
    }

    /// <summary>
    /// Converts a <see cref="Char8"/> to a <see langword="sbyte"/>.
    /// This conversion always succeeds.
    /// </summary>
    /// <param name="value">The value does not have to be a possible byte in a valid UTF-8 sequence.</param>
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator sbyte(Char8 value)
    {
      return (sbyte)value.Value;
    }

    #endregion constructors, cast operators

    #region Compare, order operators, IComparable<Char8> members, IComparable members

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(Char8 x, Char8 y)
    {
      return (int)x.Value - (int)y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(Char8 x, Char8 y)
    {
      return x.Value <= y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(Char8 x, Char8 y)
    {
      return x.Value >= y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(Char8 x, Char8 y)
    {
      return x.Value < y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(Char8 x, Char8 y)
    {
      return x.Value > y.Value;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(Char8 other)
    {
      return (int)Value - (int)other.Value;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      return obj is null
        ? 1
        : obj is Char8 other
        ? (int)Value - (int)other.Value
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.Char8 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<Char8> members, IComparable members

    #region Equals, equality operators, IEquatable<Char8> members, object members

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(Char8 x, Char8 y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(Char8 x, Char8 y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(Char8 x, Char8 y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Char8 other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is Char8 other) && (Value == other.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return Value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly string[] theToStringResults = new string[256]
    {
      "Char8(^@)", "Char8(^A)", "Char8(^B)", "Char8(^C)",
      "Char8(^D)", "Char8(^E)", "Char8(^F)", "Char8(^G)",
      "Char8(^H)", "Char8(^I)", "Char8(^J)", "Char8(^K)",
      "Char8(^L)", "Char8(^M)", "Char8(^N)", "Char8(^O)",
      "Char8(^P)", "Char8(^Q)", "Char8(^R)", "Char8(^S)",
      "Char8(^T)", "Char8(^U)", "Char8(^V)", "Char8(^W)",
      "Char8(^X)", "Char8(^Y)", "Char8(^Z)", "Char8(^[)",
      "Char8(^\\)", "Char8(^])", "Char8(^^)", "Char8(^_)",
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
      "x", "y", "z", "{", "|", "}", "~", "Char8(^?)",
      "Char8(0x80)", "Char8(0x81)", "Char8(0x82)", "Char8(0x83)",
      "Char8(0x84)", "Char8(0x85)", "Char8(0x86)", "Char8(0x87)",
      "Char8(0x88)", "Char8(0x89)", "Char8(0x8A)", "Char8(0x8B)",
      "Char8(0x8C)", "Char8(0x8D)", "Char8(0x8E)", "Char8(0x8F)",
      "Char8(0x90)", "Char8(0x91)", "Char8(0x92)", "Char8(0x93)",
      "Char8(0x94)", "Char8(0x95)", "Char8(0x96)", "Char8(0x97)",
      "Char8(0x98)", "Char8(0x99)", "Char8(0x9A)", "Char8(0x9B)",
      "Char8(0x9C)", "Char8(0x9D)", "Char8(0x9E)", "Char8(0x9F)",
      "Char8(0xA0)", "Char8(0xA1)", "Char8(0xA2)", "Char8(0xA3)",
      "Char8(0xA4)", "Char8(0xA5)", "Char8(0xA6)", "Char8(0xA7)",
      "Char8(0xA8)", "Char8(0xA9)", "Char8(0xAA)", "Char8(0xAB)",
      "Char8(0xAC)", "Char8(0xAD)", "Char8(0xAE)", "Char8(0xAF)",
      "Char8(0xB0)", "Char8(0xB1)", "Char8(0xB2)", "Char8(0xB3)",
      "Char8(0xB4)", "Char8(0xB5)", "Char8(0xB6)", "Char8(0xB7)",
      "Char8(0xB8)", "Char8(0xB9)", "Char8(0xBA)", "Char8(0xBB)",
      "Char8(0xBC)", "Char8(0xBD)", "Char8(0xBE)", "Char8(0xBF)",
      "Char8(0xC0)", "Char8(0xC1)", "Char8(0xC2)", "Char8(0xC3)",
      "Char8(0xC4)", "Char8(0xC5)", "Char8(0xC6)", "Char8(0xC7)",
      "Char8(0xC8)", "Char8(0xC9)", "Char8(0xCA)", "Char8(0xCB)",
      "Char8(0xCC)", "Char8(0xCD)", "Char8(0xCE)", "Char8(0xCF)",
      "Char8(0xD0)", "Char8(0xD1)", "Char8(0xD2)", "Char8(0xD3)",
      "Char8(0xD4)", "Char8(0xD5)", "Char8(0xD6)", "Char8(0xD7)",
      "Char8(0xD8)", "Char8(0xD9)", "Char8(0xDA)", "Char8(0xDB)",
      "Char8(0xDC)", "Char8(0xDD)", "Char8(0xDE)", "Char8(0xDF)",
      "Char8(0xE0)", "Char8(0xE1)", "Char8(0xE2)", "Char8(0xE3)",
      "Char8(0xE4)", "Char8(0xE5)", "Char8(0xE6)", "Char8(0xE7)",
      "Char8(0xE8)", "Char8(0xE9)", "Char8(0xEA)", "Char8(0xEB)",
      "Char8(0xEC)", "Char8(0xED)", "Char8(0xEE)", "Char8(0xEF)",
      "Char8(0xF0)", "Char8(0xF1)", "Char8(0xF2)", "Char8(0xF3)",
      "Char8(0xF4)", "Char8(0xF5)", "Char8(0xF6)", "Char8(0xF7)",
      "Char8(0xF8)", "Char8(0xF9)", "Char8(0xFA)", "Char8(0xFB)",
      "Char8(0xFC)", "Char8(0xFD)", "Char8(0xFE)", "Char8(0xFF)"
    };

    [MethodImpl(Helper.OptimizeInline)]
    public override string ToString()
    {
      return theToStringResults[Value];
    }

    #endregion Equals, equality operators, IEquatable<Char8> members, object members

    #region properties

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid one-byte sequence in UTF-8.
    /// This means that <see cref="Value"/> is <c>0xxxxxxx</c>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Leads1
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads1(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid continuation byte in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>10xxxxxx</c>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Continues
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Continues(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid leading byte for a 2-byte sequence in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>110xxxxx</c>.
    /// (In particular, this property is <see langword="true"/> even if <see cref="Value"/> is <c>11000000</c> or <c>11000001</c>,
    /// which cannot appear in any valid UTF-8 sequence as overlong sequences are invalid.)
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Leads2
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads2(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid leading byte for a 3-byte sequence in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>1110xxxx</c>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Leads3
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads3(Value);
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if and only if the <see cref="Char8"/> instance is a valid leading byte for a 4-byte sequence in UTF-8,
    /// without considering overlong sequences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// This means that <see cref="Value"/> is <c>11110xxx</c>.
    /// (In particular, this property is <see langword="true"/> even if <see cref="Value"/> is <c>11110111</c>,
    /// which cannot appear in any valid UTF-8 sequence.)
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Leads4
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return UtfUnsafe.Char8Leads4(Value);
      }
    }

    #endregion properties

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see cref="Char8"/>.
    /// The comparison is ordinal.
    /// </summary>
    public struct Comparer : IComparer<Char8>, IEqualityComparer2<Char8>
    {
      [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(Char8 x, Char8 y)
      {
        return (int)x.Value - (int)y.Value;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(Char8 x, Char8 y)
      {
        return x.Value == y.Value;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(Char8 obj)
      {
        return obj.Value;
      }
    }

    #region debugging

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly string[] theDebuggerDisplayResults = new string[256]
    {
      "0 Char8(^@) <L1>", "1 Char8(^A) <L1>", "2 Char8(^B) <L1>", "3 Char8(^C) <L1>",
      "4 Char8(^D) <L1>", "5 Char8(^E) <L1>", "6 Char8(^F) <L1>", "7 Char8(^G) <L1>",
      "8 Char8(^H) <L1>", "9 Char8(^I) <L1>", "10 Char8(^J) <L1>", "11 Char8(^K) <L1>",
      "12 Char8(^L) <L1>", "13 Char8(^M) <L1>", "14 Char8(^N) <L1>", "15 Char8(^O) <L1>",
      "16 Char8(^P) <L1>", "17 Char8(^Q) <L1>", "18 Char8(^R) <L1>", "19 Char8(^S) <L1>",
      "20 Char8(^T) <L1>", "21 Char8(^U) <L1>", "22 Char8(^V) <L1>", "23 Char8(^W) <L1>",
      "24 Char8(^X) <L1>", "25 Char8(^Y) <L1>", "26 Char8(^Z) <L1>", "27 Char8(^[) <L1>",
      "28 Char8(^\\) <L1>", "29 Char8(^]) <L1>", "30 Char8(^^) <L1>", "31 Char8(^_) <L1>",
      "32 ' ' <L1>", "33 '!' <L1>", "34 '\"' <L1>", "35 '#' <L1>", "36 '$' <L1>", "37 '%' <L1>", "38 '&' <L1>", "39 '\\'' <L1>",
      "40 '(' <L1>", "41 ')' <L1>", "42 '*' <L1>", "43 '+' <L1>", "44 ',' <L1>", "45 '-' <L1>", "46 '.' <L1>", "47 '/' <L1>",
      "48 '0' <L1>", "49 '1' <L1>", "50 '2' <L1>", "51 '3' <L1>", "52 '4' <L1>", "53 '5' <L1>", "54 '6' <L1>", "55 '7' <L1>",
      "56 '8' <L1>", "57 '9' <L1>", "58 ':' <L1>", "59 ';' <L1>", "60 '<' <L1>", "61 '=' <L1>", "62 '>' <L1>", "63 '?' <L1>",
      "64 '@' <L1>", "65 'A' <L1>", "66 'B' <L1>", "67 'C' <L1>", "68 'D' <L1>", "69 'E' <L1>", "70 'F' <L1>", "71 'G' <L1>",
      "72 'H' <L1>", "73 'I' <L1>", "74 'J' <L1>", "75 'K' <L1>", "76 'L' <L1>", "77 'M' <L1>", "78 'N' <L1>", "79 'O' <L1>",
      "80 'P' <L1>", "81 'Q' <L1>", "82 'R' <L1>", "83 'S' <L1>", "84 'T' <L1>", "85 'U' <L1>", "86 'V' <L1>", "87 'W' <L1>",
      "88 'X' <L1>", "89 'Y' <L1>", "90 'Z' <L1>", "91 '[' <L1>", "92 '\\' <L1>", "93 ']' <L1>", "94 '^' <L1>", "95 '_' <L1>",
      "96 '`' <L1>", "97 'a' <L1>", "98 'b' <L1>", "99 'c' <L1>", "100 'd' <L1>", "101 'e' <L1>", "102 'f' <L1>", "103 'g' <L1>",
      "104 'h' <L1>", "105 'i' <L1>", "106 'j' <L1>", "107 'k' <L1>", "108 'l' <L1>", "109 'm' <L1>", "110 'n' <L1>", "111 'o' <L1>",
      "112 'p' <L1>", "113 'q' <L1>", "114 'r' <L1>", "115 's' <L1>", "116 't' <L1>", "117 'u' <L1>", "118 'v' <L1>", "119 'w' <L1>",
      "120 'x' <L1>", "121 'y' <L1>", "122 'z' <L1>", "123 '{' <L1>", "124 '|' <L1>", "125 '}' <L1>", "126 '~' <L1>", "127 Char8(^?) <L1>",
      "128 Char8(0x80) <C>", "129 Char8(0x81) <C>", "130 Char8(0x82) <C>", "131 Char8(0x83) <C>",
      "132 Char8(0x84) <C>", "133 Char8(0x85) <C>", "134 Char8(0x86) <C>", "135 Char8(0x87) <C>",
      "136 Char8(0x88) <C>", "137 Char8(0x89) <C>", "138 Char8(0x8A) <C>", "139 Char8(0x8B) <C>",
      "140 Char8(0x8C) <C>", "141 Char8(0x8D) <C>", "142 Char8(0x8E) <C>", "143 Char8(0x8F) <C>",
      "144 Char8(0x90) <C>", "145 Char8(0x91) <C>", "146 Char8(0x92) <C>", "147 Char8(0x93) <C>",
      "148 Char8(0x94) <C>", "149 Char8(0x95) <C>", "150 Char8(0x96) <C>", "151 Char8(0x97) <C>",
      "152 Char8(0x98) <C>", "153 Char8(0x99) <C>", "154 Char8(0x9A) <C>", "155 Char8(0x9B) <C>",
      "156 Char8(0x9C) <C>", "157 Char8(0x9D) <C>", "158 Char8(0x9E) <C>", "159 Char8(0x9F) <C>",
      "160 Char8(0xA0) <C>", "161 Char8(0xA1) <C>", "162 Char8(0xA2) <C>", "163 Char8(0xA3) <C>",
      "164 Char8(0xA4) <C>", "165 Char8(0xA5) <C>", "166 Char8(0xA6) <C>", "167 Char8(0xA7) <C>",
      "168 Char8(0xA8) <C>", "169 Char8(0xA9) <C>", "170 Char8(0xAA) <C>", "171 Char8(0xAB) <C>",
      "172 Char8(0xAC) <C>", "173 Char8(0xAD) <C>", "174 Char8(0xAE) <C>", "175 Char8(0xAF) <C>",
      "176 Char8(0xB0) <C>", "177 Char8(0xB1) <C>", "178 Char8(0xB2) <C>", "179 Char8(0xB3) <C>",
      "180 Char8(0xB4) <C>", "181 Char8(0xB5) <C>", "182 Char8(0xB6) <C>", "183 Char8(0xB7) <C>",
      "184 Char8(0xB8) <C>", "185 Char8(0xB9) <C>", "186 Char8(0xBA) <C>", "187 Char8(0xBB) <C>",
      "188 Char8(0xBC) <C>", "189 Char8(0xBD) <C>", "190 Char8(0xBE) <C>", "191 Char8(0xBF) <C>",
      "192 Char8(0xC0) <L2 overlong>", "193 Char8(0xC1) <L2 overlong>", "194 Char8(0xC2) <L2>", "195 Char8(0xC3) <L2>",
      "196 Char8(0xC4) <L2>", "197 Char8(0xC5) <L2>", "198 Char8(0xC6) <L2>", "199 Char8(0xC7) <L2>",
      "200 Char8(0xC8) <L2>", "201 Char8(0xC9) <L2>", "202 Char8(0xCA) <L2>", "203 Char8(0xCB) <L2>",
      "204 Char8(0xCC) <L2>", "205 Char8(0xCD) <L2>", "206 Char8(0xCE) <L2>", "207 Char8(0xCF) <L2>",
      "208 Char8(0xD0) <L2>", "209 Char8(0xD1) <L2>", "210 Char8(0xD2) <L2>", "211 Char8(0xD3) <L2>",
      "212 Char8(0xD4) <L2>", "213 Char8(0xD5) <L2>", "214 Char8(0xD6) <L2>", "215 Char8(0xD7) <L2>",
      "216 Char8(0xD8) <L2>", "217 Char8(0xD9) <L2>", "218 Char8(0xDA) <L2>", "219 Char8(0xDB) <L2>",
      "220 Char8(0xDC) <L2>", "221 Char8(0xDD) <L2>", "222 Char8(0xDE) <L2>", "223 Char8(0xDF) <L2>",
      "224 Char8(0xE0) <L3>", "225 Char8(0xE1) <L3>", "226 Char8(0xE2) <L3>", "227 Char8(0xE3) <L3>",
      "228 Char8(0xE4) <L3>", "229 Char8(0xE5) <L3>", "230 Char8(0xE6) <L3>", "231 Char8(0xE7) <L3>",
      "232 Char8(0xE8) <L3>", "233 Char8(0xE9) <L3>", "234 Char8(0xEA) <L3>", "235 Char8(0xEB) <L3>",
      "236 Char8(0xEC) <L3>", "237 Char8(0xED) <L3>", "238 Char8(0xEE) <L3>", "239 Char8(0xEF) <L3>",
      "240 Char8(0xF0) <L4>", "241 Char8(0xF1) <L4>", "242 Char8(0xF2) <L4>", "243 Char8(0xF3) <L4>",
      "244 Char8(0xF4) <L4>", "245 Char8(0xF5) <L4 too large>", "246 Char8(0xF6) <L4 too large>", "247 Char8(0xF7) <L4 too large>",
      "248 Char8(0xF8) <L5 too large>", "249 Char8(0xF9) <L5 too large>", "250 Char8(0xFA) <L5 too large>", "251 Char8(0xFB) <L5 too large>",
      "252 Char8(0xFC) <L6 too large>", "253 Char8(0xFD) <L6 too large>", "254 Char8(0xFE) <invalid>", "255 Char8(0xFF) <invalid>"
    };

    private string DebuggerDisplay()
    {
      return theDebuggerDisplayResults[Value];
    }

    #endregion debugging
  }
}
