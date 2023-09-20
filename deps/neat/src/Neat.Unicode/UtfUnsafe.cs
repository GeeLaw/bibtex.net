using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neat.Unicode
{
  /// <summary>
  /// Provides high-performance access to UTF.
  /// </summary>
  public static class UtfUnsafe
  {
    public const int ReplacementCharacter32 = 0xFFFD;
    public const char ReplacementCharacter16 = (char)0xFFFD;
    public const byte ReplacementCharacter8Lead3 = 0xEF;
    public const byte ReplacementCharacter8Cont1 = 0xBF;
    public const byte ReplacementCharacter8Cont2 = 0xBD;

    #region generic property determination for Char8

    /// <summary>
    /// Determines whether the byte is a leading byte of 1-byte UTF-8 sequence,
    /// without considering overlong seuqences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of byte explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char8Leads1(byte value)
    {
      return (uint)value < 0x80u;
    }

    /// <summary>
    /// Determines whether the byte is a continuation of any-length UTF-8 sequence,
    /// without considering overlong seuqences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char8Continues(byte value)
    {
      return (value & 0xC0) == 0x80;
    }

    /// <summary>
    /// Determines whether the byte is a leading byte of 2-byte UTF-8 sequence,
    /// without considering overlong seuqences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char8Leads2(byte value)
    {
      return (value & 0xE0) == 0xC0;
    }

    /// <summary>
    /// Determines whether the byte is a leading byte of 3-byte UTF-8 sequence,
    /// without considering overlong seuqences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char8Leads3(byte value)
    {
      return (value & 0xF0) == 0xE0;
    }

    /// <summary>
    /// Determines whether the byte is a leading byte of 4-byte UTF-8 sequence,
    /// without considering overlong seuqences, surrogate code points, or values above <c>0x10FFFF</c>.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char8Leads4(byte value)
    {
      return (value & 0xF8) == 0xF0;
    }

    #endregion generic property determination for Char8

    #region Char8 to Char32

    /// <summary>
    /// Converts a 1-byte UTF-8 sequence to a UTF-32 code point,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char8ToChar32Unchecked1(byte lead1)
    {
      return lead1;
    }

    /// <summary>
    /// Converts a 2-byte UTF-8 sequence to a UTF-32 code point,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char8ToChar32Unchecked2(byte lead2, byte cont1)
    {
      return ((lead2 & 0x1F) << 6) | (cont1 & 0x3F);
    }

    /// <summary>
    /// Converts a 3-byte UTF-8 sequence to a UTF-32 code point,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char8ToChar32Unchecked3(byte lead3, byte cont1, byte cont2)
    {
      return ((lead3 & 0x0F) << 12) | ((cont1 & 0x3F) << 6) | (cont2 & 0x3F);
    }

    /// <summary>
    /// Converts a 4-byte UTF-8 sequence to a UTF-32 code point,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char8ToChar32Unchecked4(byte lead4, byte cont1, byte cont2, byte cont3)
    {
      return ((lead4 & 0x07) << 18) | ((cont1 & 0x3F) << 12) | ((cont2 & 0x3F) << 6) | (cont3 & 0x3F);
    }

    #endregion Char8 to Char32

    #region generic property determination for Char16

    /// <summary>
    /// Determines whether the UTF-16 code unit is a surrogate code unit.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsSurrogate(char value)
    {
      return (value & 0xFFFFF800) == 0xD800;
    }

    /// <summary>
    /// Determines whether the UTF-16 code unit is not a surrogate code unit.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsNotSurrogate(char value)
    {
      return (value & 0xFFFFF800) != 0xD800;
    }

    /// <summary>
    /// Determines whether the UTF-16 code unit is a high surrogate code unit.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsHighSurrogate(char value)
    {
      return (value & 0xFFFFFC00) == 0xD800;
    }

    /// <summary>
    /// Determines whether the UTF-16 code unit is a low surrogate code unit.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsLowSurrogate(char value)
    {
      return (value & 0xFFFFFC00) == 0xDC00;
    }

    /// <summary>
    /// Determines whether the UTF-16 code unit is a not low surrogate code unit.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsNotLowSurrogate(char value)
    {
      return (value & 0xFFFFFC00) != 0xDC00;
    }

    #endregion generic property determination for Char16

    #region Char16 to Char32

    /// <summary>
    /// Converts a 1-unit UTF-16 sequence to a UTF-32 code point,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char16ToChar32Unchecked1(char value)
    {
      return value;
    }

    /// <summary>
    /// Converts a 2-unit UTF-16 sequence to a UTF-32 code point,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char16ToChar32Unchecked2(char high, char low)
    {
      return (((high & 0x3FF) << 10) | (low & 0x3FF)) + 0x10000;
    }

    #endregion Char16 to Char32

    #region range check for Char32

    /// <summary>
    /// Determines whether the UTF-32 code point is below <c>0x80</c> (exclusive).
    /// Code points in this range are always valid, 1 byte in UTF-8, and 1 code unit in UTF-16.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsBelow0x80(int value)
    {
      return (uint)value < 0x80u;
    }

    /// <summary>
    /// Determines whether the UTF-32 code point is below <c>0x800</c> (exclusive).
    /// Code points in this range are always valid, at most 2 bytes in UTF-8, and 1 code unit in UTF-16.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsBelow0x800(int value)
    {
      return (uint)value < 0x800u;
    }

    /// <summary>
    /// Determines whether the UTF-32 code point is below <c>0x10000</c> (exclusive).
    /// Code points in this range are not always valid (surrogate code points).
    /// Valid code points are at most 3 bytes in UTF-8 and 1 code unit in UTF-16.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsBelow0x10000(int value)
    {
      return (uint)value < 0x10000u;
    }

    /// <summary>
    /// Determines whether the UTF-32 code point is below <c>0x110000</c> (exclusive).
    /// Code points in this range are not always valid (surrogate code points).
    /// Valid code points are at most 4 bytes in UTF-8 and 2 code units in UTF-16.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsBelow0x110000(int value)
    {
      return (uint)value < 0x110000u;
    }

    /// <summary>
    /// Determines whether the UTF-32 code point is above <c>0x10000</c> (inclusive) below <c>0x110000</c> (exclusive).
    /// Code points in this range are always valid, 4 bytes in UTF-8, and 2 code units in UTF-16.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsAbove0x10000AndBelow0x110000(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    #endregion range check for Char32

    #region range check for Char16

    /// <summary>
    /// Determines whether the UTF-16 code unit is below <c>0x80</c> (exclusive).
    /// Code units in this range are always valid, 1 byte in UTF-8, and 1 code unit in UTF-16.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of char explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsBelow0x80(char value)
    {
      return (uint)value < 0x80u;
    }

    /// <summary>
    /// Determines whether the UTF-16 code unit is below <c>0x80</c> (exclusive).
    /// Code units in this range are always valid, at most 2 bytes in UTF-8, and 1 code unit in UTF-16.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of char explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char16IsBelow0x800(char value)
    {
      return (uint)value < 0x800u;
    }

    #endregion range check for Char16

    #region generic property determination for Char32

    /// <summary>
    /// Determines whether a UTF-32 code point is valid.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsValid(int value)
    {
      return (uint)value < 0x110000u && (value & 0xFFFFF800) != 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is not valid.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsNotValid(int value)
    {
      return (uint)value >= 0x110000u || (value & 0xFFFFF800) == 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is a surrogate code point.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsSurrogate(int value)
    {
      return (value & 0xFFFFF800) == 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is not a surrogate code point.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsNotSurrogate(int value)
    {
      return (value & 0xFFFFF800) != 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is not a high surrogate code point.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsHighSurrogate(int value)
    {
      return (value & 0xFFFFFC00) == 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is not a low surrogate code point.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32IsLowSurrogate(int value)
    {
      return (value & 0xFFFFFC00) == 0xDC00;
    }

    #endregion generic property determination for Char32

    #region generic encoding length counting for Char32 in Char8s

    /// <summary>
    /// Determines whether a UTF-32 code point is 1 byte in UTF-8.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32Is1Char8(int value)
    {
      return (uint)value < 0x80u;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is 2 bytes in UTF-8.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32Is2Char8s(int value)
    {
      return (uint)(value - 0x80) < (uint)(0x800 - 0x80);
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is 3 bytes in UTF-8.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32Is3Char8s(int value)
    {
      return (uint)(value - 0x800) < (uint)(0x10000 - 0x800) && (value & 0xFFFFF800) != 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is 4 bytes in UTF-8.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32Is4Char8s(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    /// <summary>
    /// Given a UTF-32 code point, determines the number of bytes in UTF-8.
    /// Returns <c>-1</c> if the code point is not valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int Char32LengthInChar8s(int value)
    {
      if (Char32IsBelow0x80(value))
      {
        return 1;
      }
      if (Char32IsBelow0x800(value))
      {
        return 2;
      }
      if (Char32IsBelow0x10000(value))
      {
        if (Char32IsNotSurrogate(value))
        {
          return 3;
        }
        goto Invalid;
      }
      if (Char32IsBelow0x110000(value))
      {
        return 4;
      }
    Invalid:
      return -1;
    }

    #endregion generic encoding length counting for Char32 in Char8s

    #region generic encoding length counting for Char32 in Char16s

    /// <summary>
    /// Determines whether a UTF-32 code point is 1 code unit in UTF-16.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32Is1Char16(int value)
    {
      return (uint)value < 0x10000u && (value & 0xFFFFF800) != 0xD800;
    }

    /// <summary>
    /// Determines whether a UTF-32 code point is 2 code units in UTF-16.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32Is2Char16s(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    /// <summary>
    /// Given a UTF-32 code point, determines the number of code units in UTF-16.
    /// Returns <c>-1</c> if the code point is not valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int Char32LengthInChar16s(int value)
    {
      if (Char32IsBelow0x10000(value))
      {
        if (Char32IsNotSurrogate(value))
        {
          return 1;
        }
        goto Invalid;
      }
      if (Char32IsBelow0x110000(value))
      {
        return 2;
      }
    Invalid:
      return -1;
    }

    #endregion generic encoding length counting for Char32 in Char16s

    #region Char32 to Char8

    /// <summary>
    /// Given a UTF-32 code point, computes the leading byte in its 1-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To1Char8UncheckedLead1(int value)
    {
      return (byte)value;
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the leading byte in its 2-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To2Char8sUncheckedLead2(int value)
    {
      return (byte)((value >> 6) | 0xC0);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the first continuation byte in its 2-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To2Char8sUncheckedCont1(int value)
    {
      return (byte)((value & 0x3F) | 0x80);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the leading byte in its 3-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To3Char8sUncheckedLead3(int value)
    {
      return (byte)((value >> 12) | 0xE0);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the first continuation byte in its 3-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To3Char8sUncheckedCont1(int value)
    {
      return (byte)(((value >> 6) & 0x3F) | 0x80);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the second continuation byte in its 3-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To3Char8sUncheckedCont2(int value)
    {
      return (byte)((value & 0x3F) | 0x80);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the leading byte in its 4-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To4Char8sUncheckedLead4(int value)
    {
      return (byte)((value >> 18) | 0xF0);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the first continuation byte in its 4-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To4Char8sUncheckedCont1(int value)
    {
      return (byte)(((value >> 12) & 0x3F) | 0x80);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the second continuation byte in its 4-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To4Char8sUncheckedCont2(int value)
    {
      return (byte)(((value >> 6) & 0x3F) | 0x80);
    }

    /// <summary>
    /// Given a UTF-32 code point, computes the third continuation byte in its 4-byte UTF-8 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static byte Char32To4Char8sUncheckedCont3(int value)
    {
      return (byte)((value & 0x3F) | 0x80);
    }

    #endregion Char32 to Char8

    #region Char32 to Char16

    /// <summary>
    /// Given a UTF-32 code point, computes the code unit in its 1-unit UTF-16 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static char Char32To1Char16Unchecked(int value)
    {
      return (char)value;
    }

    /// <summary>
    /// Given a UTF-32 code point, prepares it for computing its 2-unit UTF-16 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static int Char32To2Char16sUncheckedPrepare(int value)
    {
      return value - 0x10000;
    }

    /// <summary>
    /// Given a prepared UTF-32 code point, computes the high surrogate in its 2-unit UTF-16 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static char Char32PreparedTo2Char16sUncheckedHigh(int prepared)
    {
      return (char)((prepared >> 10) | 0xD800);
    }

    /// <summary>
    /// Given a prepared UTF-32 code point, computes the low surrogate in its 2-unit UTF-16 sequence,
    /// without performing any check.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static char Char32PreparedTo2Char16sUncheckedLow(int prepared)
    {
      return (char)((prepared & 0x3FF) | 0xDC00);
    }

    #endregion Char32 to Char16

    #region validity of Char32 from Char8 (overlong, surrogate, above 0x10FFFF)

    /// <summary>
    /// Verifies whether a UTF-32 code point obtained from <see cref="Char8ToChar32Unchecked2(byte, byte)"/> is valid
    /// (forbids overlong sequences, surrogate code points, and values exceeding <c>0x10FFFF</c>).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32From2Char8sIsValid(int value)
    {
      return 0x80u <= (uint)value;
    }

    /// <summary>
    /// Verifies whether a UTF-32 code point obtained from <see cref="Char8ToChar32Unchecked3(byte, byte, byte)"/> is valid
    /// (forbids overlong sequences, surrogate code points, and values exceeding <c>0x10FFFF</c>).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32From3Char8sIsValid(int value)
    {
      return 0x800u <= (uint)value && (value & 0xFFFFF800) != 0xD800;
    }

    /// <summary>
    /// Verifies whether a UTF-32 code point obtained from <see cref="Char8ToChar32Unchecked4(byte, byte, byte, byte)"/> is valid
    /// (forbids overlong sequences, surrogate code points, and values exceeding <c>0x10FFFF</c>).
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static bool Char32From4Char8sIsValid(int value)
    {
      return (uint)(value - 0x10000) < (uint)(0x110000 - 0x10000);
    }

    #endregion validity of Char32 from Char8 (overlong, surrogate, above 0x10FFFF)

    #region String8 to String8

    /// <summary>
    /// Finds the first invalid <see cref="Char8"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int FindFirstInvalidChar8(ref byte src0, int src8s)
    {
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        return i;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see cref="Char8"/> instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int CountInvalidChar8s(ref byte src0, int src8s)
    {
      int invalids = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        ++invalids;
        continue;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see cref="Char8"/> instances by the UTF-8 encoding of the replacement character.
    /// Each invalid instance becomes 3 valid instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void SanitizeChar8s(ref byte src0, int src8s, ref byte dst0, int dst8s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j = 0; i != src8s && k != dst8s; j = ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k++) = lead;
          continue;
        }
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        lead = ReplacementCharacter8Lead3;
        cont1 = ReplacementCharacter8Cont1;
        cont2 = ReplacementCharacter8Cont2;
        goto IsReplacement;
      Valid2:
        i = j;
        if (dst8s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = lead;
        Unsafe.Add(ref dst0, k++) = cont1;
        continue;
      Valid3:
        i = j;
      IsReplacement:
        if ((uint)dst8s <= (uint)(k + 2))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = lead;
        Unsafe.Add(ref dst0, k++) = cont1;
        Unsafe.Add(ref dst0, k++) = cont2;
        continue;
      Valid4:
        i = j;
        if ((uint)dst8s <= (uint)(k + 3))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = lead;
        Unsafe.Add(ref dst0, k++) = cont1;
        Unsafe.Add(ref dst0, k++) = cont2;
        Unsafe.Add(ref dst0, k++) = cont3;
        continue;
      }
      while (k != dst8s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String8 to String8

    #region String16 to String16

    /// <summary>
    /// Finds the first invalid <see langword="char"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int FindFirstInvalidChar16(ref char src0, int src16s)
    {
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 */
            continue;
          }
          /* InvalidDecrease */
          return --i;
        }
        /* Invalid */
        return i;
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see langword="char"/> instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int CountInvalidChar16s(ref char src0, int src16s)
    {
      int invalids = 0;
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 */
            continue;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        ++invalids;
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see langword="char"/> instances by the UTF-16 encoding of the replacement character.
    /// Each invalid instance becomes 1 valid instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void SanitizeChar16s(ref char src0, int src16s, ref char dst0, int dst16s)
    {
      int k = 0;
      char first, low;
      for (int i = 0; i != src16s && k != dst16s; ++i)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Valid1;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(low = Unsafe.Add(ref src0, i)))
          {
            goto Valid2;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        first = ReplacementCharacter16;
      Valid1:
        Unsafe.Add(ref dst0, k++) = first;
        continue;
      Valid2:
        if (dst16s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = first;
        Unsafe.Add(ref dst0, k++) = low;
        continue;
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    #endregion String16 to String16

    #region String32 to String32

    /// <summary>
    /// Finds the first invalid <see cref="Char32"/> instance.
    /// This method returns <c>-1</c> if the stream is valid.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int FindFirstInvalidChar32(ref int src0, int src32s)
    {
      for (int i = 0; i != src32s; ++i)
      {
        if (Char32IsNotValid(Unsafe.Add(ref src0, i)))
        {
          return i;
        }
      }
      return -1;
    }

    /// <summary>
    /// Counts the number of invalid <see cref="Char32"/> instances.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int CountInvalidChar32s(ref int src0, int src32s)
    {
      int invalids = 0;
      for (int i = 0; i != src32s; ++i)
      {
        if (Char32IsNotValid(Unsafe.Add(ref src0, i)))
        {
          ++invalids;
        }
      }
      return invalids;
    }

    /// <summary>
    /// Replaces invalid <see cref="Char32"/> instances by the UTF-32 encoding of the replacement character.
    /// Each invalid instance becomes 1 valid instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void SanitizeChar32s(ref int src0, int src32s, ref int dst0, int dst32s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst32s; ++i, ++k)
      {
        Unsafe.Add(ref dst0, k) = Char32IsValid(value = Unsafe.Add(ref src0, i))
          ? value
          : ReplacementCharacter32;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String32 to String32

    #region String8 to String32

    /// <summary>
    /// Given UTF-8, computes UTF-32 length.
    /// Returns <see langword="true"/> if and only if UTF-8 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char32"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char8"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool String8ToString32CountStrict(ref byte src0, int src8s, out int countIndex)
    {
      int dst32s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst32s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        countIndex = i;
        return false;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      countIndex = dst32s;
      return true;
    }

    /// <summary>
    /// Given UTF-8, computes UTF-32 length, with invalid <see cref="Char8"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int String8ToString32CountReplace(ref byte src0, int src8s)
    {
      int dst32s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst32s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          /* Invalid */
          continue;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          /* Invalid */
          continue;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          /* Invalid */
          continue;
        }
        /* Invalid */
        continue;
      Valid2:
      Valid3:
      Valid4:
        i = j;
      }
      return dst32s;
    }

    /// <summary>
    /// Transforms UTF-8 to UTF-32, with invalid <see cref="Char8"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst32s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void String8ToString32Transform(ref byte src0, int src8s, ref int dst0, int dst32s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j, value; i != src8s && k != dst32s; ++i, ++k)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k) = Char8ToChar32Unchecked1(lead);
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(value = Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(value = Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(value = Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        Unsafe.Add(ref dst0, k) = ReplacementCharacter32;
        continue;
      Valid2:
      Valid3:
      Valid4:
        i = j;
        Unsafe.Add(ref dst0, k) = value;
        continue;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String8 to String32

    #region String16 to String32

    /// <summary>
    /// Given UTF-16, computes UTF-32 length.
    /// Returns <see langword="true"/> if and only if UTF-16 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char32"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see langword="char"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool String16ToString32CountStrict(ref char src0, int src16s, out int countIndex)
    {
      int dst32s = 0;
      char first;
      for (int i = 0; i != src16s; ++i, ++dst32s)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 */
            continue;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        countIndex = i;
        return false;
      }
      countIndex = dst32s;
      return true;
    }

    /// <summary>
    /// Given UTF-16, computes UTF-32 length, with invalid <see langword="char"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int String16ToString32CountReplace(ref char src0, int src16s)
    {
      int dst32s = 0;
      for (int i = 0; i != src16s; ++i, ++dst32s)
      {
        if (Char16IsHighSurrogate(Unsafe.Add(ref src0, i))
          && (++i == src16s || Char16IsNotLowSurrogate(Unsafe.Add(ref src0, i))))
        {
          /* InvalidDecrease */
          --i;
        }
        /* Valid1, Valid2, Invalid (including falling through from InvalidDecrease) */
      }
      return dst32s;
    }

    /// <summary>
    /// Transforms UTF-16 to UTF-32, with invalid <see langword="char"/> instances replaced by the UTF-32 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst32s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void String16ToString32Transform(ref char src0, int src16s, ref int dst0, int dst32s)
    {
      int k = 0;
      char first, low;
      for (int i = 0; i != src16s && k != dst32s; ++i, ++k)
      {
        if (Char16IsNotSurrogate(first = Unsafe.Add(ref src0, i)))
        {
          goto Valid1;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(low = Unsafe.Add(ref src0, i)))
          {
            goto Valid2;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        first = ReplacementCharacter16;
      Valid1:
        Unsafe.Add(ref dst0, k) = Char16ToChar32Unchecked1(first);
        continue;
      Valid2:
        Unsafe.Add(ref dst0, k) = Char16ToChar32Unchecked2(first, low);
        continue;
      }
      while (k != dst32s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String16 to String32

    #region String32 to String8

    /// <summary>
    /// Given UTF-32, computes UTF-8 length.
    /// Returns <see langword="true"/> if and only if UTF-32 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char8"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char32"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool String32ToString8CountStrict(ref int src0, int src32s, out long countIndex)
    {
      int valid2s = 0, valid3s = 0, valid4s = 0;
      for (int i = 0, value; i != src32s; ++i)
      {
        if (Char32IsBelow0x80(value = Unsafe.Add(ref src0, i)))
        {
          continue;
        }
        if (Char32IsBelow0x800(value))
        {
          ++valid2s;
          continue;
        }
        if (Char32IsBelow0x10000(value))
        {
          if (Char32IsNotSurrogate(value))
          {
            ++valid3s;
            continue;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          ++valid4s;
          continue;
        }
      Invalid:
        countIndex = i;
        return false;
      }
      countIndex = src32s + (long)valid2s + valid3s * 2L + valid4s * 3L;
      return true;
    }

    /// <summary>
    /// Given UTF-32, computes UTF-8 length, with invalid <see cref="Char32"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static long String32ToString8CountReplace(ref int src0, int src32s)
    {
      int valid2s = 0, valid3invalids = 0, valid4s = 0;
      for (int i = 0, value; i != src32s; ++i)
      {
        if (Char32IsBelow0x80(value = Unsafe.Add(ref src0, i)))
        {
          continue;
        }
        if (Char32IsBelow0x800(value))
        {
          ++valid2s;
          continue;
        }
        if (Char32IsAbove0x10000AndBelow0x110000(value))
        {
          ++valid4s;
          continue;
        }
        ++valid3invalids;
        continue;
      }
      return src32s + (long)valid2s + valid3invalids * 2L + valid4s * 3L;
    }

    /// <summary>
    /// Transforms UTF-32 to UTF-8, with invalid <see cref="Char32"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst8s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void String32ToString8Transform(ref int src0, int src32s, ref byte dst0, int dst8s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst8s; ++i)
      {
        if (Char32IsBelow0x80(value = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k++) = Char32To1Char8UncheckedLead1(value);
          continue;
        }
        if (Char32IsBelow0x800(value))
        {
          goto Valid2;
        }
        if (Char32IsBelow0x10000(value))
        {
          if (Char32IsNotSurrogate(value))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          goto Valid4;
        }
      Invalid:
        value = ReplacementCharacter32;
        goto Valid3;
      Valid2:
        if (dst8s == k + 1)
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedLead2(value);
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedCont1(value);
        continue;
      Valid3:
        if ((uint)dst8s <= (uint)(k + 2))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedLead3(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont2(value);
        continue;
      Valid4:
        if ((uint)dst8s <= (uint)(k + 3))
        {
          break;
        }
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedLead4(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont2(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont3(value);
        continue;
      }
      while (k != dst8s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String32 to String8

    #region String32 to String16

    /// <summary>
    /// Given UTF-32, computes UTF-16 length.
    /// Returns <see langword="true"/> if and only if UTF-32 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see langword="char"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char32"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool String32ToString16CountStrict(ref int src0, int src32s, out long countIndex)
    {
      int dst16sMoreThanSrc32s = 0;
      for (int i = 0, value; i != src32s; ++i)
      {
        if (Char32IsBelow0x10000(value = Unsafe.Add(ref src0, i)))
        {
          if (Char32IsNotSurrogate(value))
          {
            /* Valid1 */
            continue;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          /* Valid2 */
          ++dst16sMoreThanSrc32s;
          continue;
        }
      Invalid:
        countIndex = i;
        return false;
      }
      countIndex = (long)src32s + dst16sMoreThanSrc32s;
      return true;
    }

    /// <summary>
    /// Given UTF-32, computes UTF-16 length, with invalid <see cref="Char32"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static long String32ToString16CountReplace(ref int src0, int src32s)
    {
      int dst16sMoreThanSrc32s = 0;
      for (int i = 0; i != src32s; ++i)
      {
        if (Char32Is2Char16s(Unsafe.Add(ref src0, i)))
        {
          /* Valid2 */
          ++dst16sMoreThanSrc32s;
        }
        /* Valid1, Invalid */
      }
      return (long)src32s + dst16sMoreThanSrc32s;
    }

    /// <summary>
    /// Transforms UTF-32 to UTF-16, with invalid <see cref="Char32"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst16s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void String32ToString16Transform(ref int src0, int src32s, ref char dst0, int dst16s)
    {
      int k = 0;
      for (int i = 0, value; i != src32s && k != dst16s; ++i)
      {
        if (Char32IsBelow0x10000(value = Unsafe.Add(ref src0, i)))
        {
          if (Char32IsNotSurrogate(value))
          {
            goto Valid1;
          }
          goto Invalid;
        }
        if (Char32IsBelow0x110000(value))
        {
          goto Valid2;
        }
      Invalid:
        value = ReplacementCharacter16;
      Valid1:
        Unsafe.Add(ref dst0, k++) = Char32To1Char16Unchecked(value);
        continue;
      Valid2:
        if (dst16s == k + 1)
        {
          break;
        }
        value = Char32To2Char16sUncheckedPrepare(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedHigh(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedLow(value);
        continue;
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    #endregion String32 to String16

    #region String8 to String16

    /// <summary>
    /// Given UTF-8, computes UTF-16 length.
    /// Returns <see langword="true"/> if and only if UTF-8 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see langword="char"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see cref="Char8"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool String8ToString16CountStrict(ref byte src0, int src8s, out int countIndex)
    {
      int dst16s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst16s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        countIndex = i;
        return false;
      Valid4:
        ++dst16s;
      Valid2:
      Valid3:
        i = j;
        continue;
      }
      countIndex = dst16s;
      return true;
    }

    /// <summary>
    /// Given UTF-8, computes UTF-16 length, with invalid <see cref="Char8"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static int String8ToString16CountReplace(ref byte src0, int src8s)
    {
      int dst16s = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j; i != src8s; ++i, ++dst16s)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        continue;
      Valid4:
        ++dst16s;
      Valid2:
      Valid3:
        i = j;
        continue;
      }
      return dst16s;
    }

    /// <summary>
    /// Transforms UTF-8 to UTF-16, with invalid <see cref="Char8"/> instances replaced by the UTF-16 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst16s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void String8ToString16Transform(ref byte src0, int src8s, ref char dst0, int dst16s)
    {
      int k = 0;
      byte lead, cont1, cont2, cont3;
      for (int i = 0, j, value; i != src8s && k != dst16s; ++i)
      {
        if (Char8Leads1(lead = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 */
          Unsafe.Add(ref dst0, k++) = Char32To1Char16Unchecked(Char8ToChar32Unchecked1(lead));
          continue;
        }
        j = i;
        if (Char8Leads2(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && Char32From2Char8sIsValid(value = Char8ToChar32Unchecked2(lead, cont1)))
          {
            goto Valid2;
          }
          goto Invalid;
        }
        if (Char8Leads3(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && Char32From3Char8sIsValid(value = Char8ToChar32Unchecked3(lead, cont1, cont2)))
          {
            goto Valid3;
          }
          goto Invalid;
        }
        if (Char8Leads4(lead))
        {
          if (++j != src8s && Char8Continues(cont1 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont2 = Unsafe.Add(ref src0, j))
            && ++j != src8s && Char8Continues(cont3 = Unsafe.Add(ref src0, j))
            && Char32From4Char8sIsValid(value = Char8ToChar32Unchecked4(lead, cont1, cont2, cont3)))
          {
            goto Valid4;
          }
          goto Invalid;
        }
      Invalid:
        Unsafe.Add(ref dst0, k++) = ReplacementCharacter16;
        continue;
      Valid2:
      Valid3:
        i = j;
        Unsafe.Add(ref dst0, k++) = Char32To1Char16Unchecked(value);
        continue;
      Valid4:
        if (dst16s == k + 1)
        {
          break;
        }
        i = j;
        value = Char32To2Char16sUncheckedPrepare(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedHigh(value);
        Unsafe.Add(ref dst0, k++) = Char32PreparedTo2Char16sUncheckedLow(value);
        continue;
      }
      while (k != dst16s)
      {
        Unsafe.Add(ref dst0, k++) = (char)0;
      }
    }

    #endregion String8 to String16

    #region String16 to String8

    /// <summary>
    /// Given UTF-16, computes UTF-8 length.
    /// Returns <see langword="true"/> if and only if UTF-16 is valid,
    /// upon which <paramref name="countIndex"/> contains the number of <see cref="Char8"/> instances needed.
    /// Otherwise, <paramref name="countIndex"/> contains the index of the first invalid <see langword="char"/> instance.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool String16ToString8CountStrict(ref char src0, int src16s, out long countIndex)
    {
      int ch1b2s = 0, ch1b3ch2b4s = 0;
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsBelow0x80(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 in UTF-16, Valid1 in UTF-8 */
          continue;
        }
        if (Char16IsBelow0x800(first))
        {
          /* Valid1 in UTF-16, Valid2 in UTF-8 */
          ++ch1b2s;
          continue;
        }
        if (Char16IsNotSurrogate(first))
        {
          /* Valid1 in UTF-16, Valid3 in UTF-8 */
          goto Ch1B3;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(Unsafe.Add(ref src0, i)))
          {
            /* Valid2 in UTF-16, Valid4 in UTF-8 */
            goto Ch2B4;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        countIndex = i;
        return false;
      Ch1B3:
      Ch2B4:
        ++ch1b3ch2b4s;
        continue;
      }
      countIndex = src16s + (long)ch1b2s + ch1b3ch2b4s * 2L;
      return true;
    }

    /// <summary>
    /// Given UTF-16, computes UTF-8 length, with invalid <see langword="char"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static long String16ToString8CountReplace(ref char src0, int src16s)
    {
      int ch1b2s = 0, ch1b3ch2b4invalids = 0;
      char first;
      for (int i = 0; i != src16s; ++i)
      {
        if (Char16IsBelow0x80(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 in UTF-16, Valid1 in UTF-8 */
          continue;
        }
        if (Char16IsBelow0x800(first))
        {
          /* Valid1 in UTF-16, Valid2 in UTF-8 */
          ++ch1b2s;
          continue;
        }
        if (Char16IsHighSurrogate(first)
          && (++i == src16s || Char16IsNotLowSurrogate(Unsafe.Add(ref src0, i))))
        {
          /* InvalidDecrease */
          --i;
        }
        /* Ch1B3, Ch2B4, Invalid (including falling through from InvalidDecrease) */
        ++ch1b3ch2b4invalids;
        continue;
      }
      return src16s + (long)ch1b2s + ch1b3ch2b4invalids * 2L;
    }

    /// <summary>
    /// Transforms UTF-16 to UTF-8, with invalid <see langword="char"/> instances replaced by the UTF-8 encoding of the replacement character.
    /// This method does not validate arguments, and will write exactly <paramref name="dst8s"/> elements beginning <paramref name="dst0"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void String16ToString8Transform(ref char src0, int src16s, ref byte dst0, int dst8s)
    {
      int k = 0;
      char first, low;
      for (int i = 0, value; i != src16s && k != dst8s; ++i)
      {
        if (Char16IsBelow0x80(first = Unsafe.Add(ref src0, i)))
        {
          /* Valid1 in UTF-16, Valid1 in UTF-8 */
          Unsafe.Add(ref dst0, k++) = Char32To1Char8UncheckedLead1(Char16ToChar32Unchecked1(first));
          continue;
        }
        if (Char16IsBelow0x800(first))
        {
          /* Valid1 in UTF-16, Valid2 in UTF-8 */
          goto Ch1B2;
        }
        if (Char16IsNotSurrogate(first))
        {
          /* Valid1 in UTF-16, Valid3 in UTF-8 */
          goto Ch1B3;
        }
        if (Char16IsHighSurrogate(first))
        {
          if (++i != src16s && Char16IsLowSurrogate(low = Unsafe.Add(ref src0, i)))
          {
            /* Valid2 in UTF-16, Valid4 in UTF-8 */
            goto Ch2B4;
          }
          /* InvalidDecrease */
          --i;
        }
        /* Invalid */
        first = ReplacementCharacter16;
        goto Ch1B3;
      Ch1B2:
        if (dst8s == k + 1)
        {
          break;
        }
        value = Char16ToChar32Unchecked1(first);
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedLead2(value);
        Unsafe.Add(ref dst0, k++) = Char32To2Char8sUncheckedCont1(value);
        continue;
      Ch1B3:
        if ((uint)dst8s <= (uint)(k + 2))
        {
          break;
        }
        value = Char16ToChar32Unchecked1(first);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedLead3(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To3Char8sUncheckedCont2(value);
        continue;
      Ch2B4:
        if ((uint)dst8s <= (uint)(k + 3))
        {
          break;
        }
        value = Char16ToChar32Unchecked2(first, low);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedLead4(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont1(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont2(value);
        Unsafe.Add(ref dst0, k++) = Char32To4Char8sUncheckedCont3(value);
        continue;
      }
      while (k != dst8s)
      {
        Unsafe.Add(ref dst0, k++) = 0;
      }
    }

    #endregion String16 to String8
  }
}
