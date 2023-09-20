using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// Provides type-safe access to UTF.
  /// </summary>
  public static class Utf
  {
    public static readonly Char32 ReplacementCharacter32 = new Char32(UtfUnsafe.ReplacementCharacter32);
    public const char ReplacementCharacter16 = UtfUnsafe.ReplacementCharacter16;
    public static readonly Char8 ReplacementCharacter8Lead3 = new Char8(UtfUnsafe.ReplacementCharacter8Lead3);
    public static readonly Char8 ReplacementCharacter8Cont1 = new Char8(UtfUnsafe.ReplacementCharacter8Cont1);
    public static readonly Char8 ReplacementCharacter8Cont2 = new Char8(UtfUnsafe.ReplacementCharacter8Cont2);

    public const int MaximumLength32 = 0x7FEFFFFF;
    public const int MaximumLength16 = 0x7FEFFFFF;
    public const int MaximumLength8 = 0x7FFFFFC7;

    public const string String8WouldBeTooLong = "The string in UTF-8 would be too long.";
    public const string String16WouldBeTooLong = "The string in UTF-16 would be too long.";
    public const string String32WouldBeTooLong = "The string in UTF-32 would be too long.";
    public const string String8IsNotValid = "The string in UTF-8 is not valid.";
    public const string String16IsNotValid = "The string in UTF-16 is not valid.";
    public const string String32IsNotValid = "The string in UTF-32 is not valid.";

    #region decoding or encoding 1 code point

    /// <summary>
    /// Tries to decode a 1-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try1Char8ToChar32(Char8 lead1, out Char32 char32)
    {
      char32 = new Char32(UtfUnsafe.Char8ToChar32Unchecked1(lead1.Value));
      return UtfUnsafe.Char8Leads1(lead1.Value);
    }

    /// <summary>
    /// Tries to decode a 2-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try2Char8sToChar32(Char8 lead2, Char8 cont1, out Char32 char32)
    {
      int value = UtfUnsafe.Char8ToChar32Unchecked2(lead2.Value, cont1.Value);
      char32 = new Char32(value);
      return UtfUnsafe.Char8Leads2(lead2.Value)
        && UtfUnsafe.Char8Continues(cont1.Value)
        && UtfUnsafe.Char32From2Char8sIsValid(value);
    }

    /// <summary>
    /// Tries to decode a 3-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try3Char8sToChar32(Char8 lead3, Char8 cont1, Char8 cont2, out Char32 char32)
    {
      int value = UtfUnsafe.Char8ToChar32Unchecked3(lead3.Value, cont1.Value, cont2.Value);
      char32 = new Char32(value);
      return UtfUnsafe.Char8Leads3(lead3.Value)
        && UtfUnsafe.Char8Continues(cont1.Value)
        && UtfUnsafe.Char8Continues(cont2.Value)
        && UtfUnsafe.Char32From3Char8sIsValid(value);
    }

    /// <summary>
    /// Tries to decode a 4-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try4Char8sToChar32(Char8 lead4, Char8 cont1, Char8 cont2, Char8 cont3, out Char32 char32)
    {
      int value = UtfUnsafe.Char8ToChar32Unchecked4(lead4.Value, cont1.Value, cont2.Value, cont3.Value);
      char32 = new Char32(value);
      return UtfUnsafe.Char8Leads4(lead4.Value)
        && UtfUnsafe.Char8Continues(cont1.Value)
        && UtfUnsafe.Char8Continues(cont2.Value)
        && UtfUnsafe.Char8Continues(cont3.Value)
        && UtfUnsafe.Char32From4Char8sIsValid(value);
    }

    /// <summary>
    /// Tries to decode a 1-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try1Char16ToChar32(char char16, out Char32 char32)
    {
      char32 = new Char32(UtfUnsafe.Char16ToChar32Unchecked1(char16));
      return UtfUnsafe.Char16IsNotSurrogate(char16);
    }

    /// <summary>
    /// Tries to decode a 2-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the decoding was successful,
    /// upon which <paramref name="char32"/> contains the valid decoded Unicode code point.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char32"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool Try2Char16sToChar32(char high, char low, out Char32 char32)
    {
      char32 = new Char32(UtfUnsafe.Char16ToChar32Unchecked2(high, low));
      return UtfUnsafe.Char16IsHighSurrogate(high) && UtfUnsafe.Char16IsLowSurrogate(low);
    }

    /// <summary>
    /// Tries to encode a 1-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead1"/> contains the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="lead1"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To1Char8(Char32 char32, out byte lead1)
    {
      lead1 = UtfUnsafe.Char32To1Char8UncheckedLead1(char32.Value);
      return UtfUnsafe.Char32Is1Char8(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 2-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead2"/>, <paramref name="cont1"/> contain the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="lead2"/>, <paramref name="cont1"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To2Char8s(Char32 char32, out byte lead2, out byte cont1)
    {
      lead2 = UtfUnsafe.Char32To2Char8sUncheckedLead2(char32.Value);
      cont1 = UtfUnsafe.Char32To2Char8sUncheckedCont1(char32.Value);
      return UtfUnsafe.Char32Is2Char8s(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 3-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead3"/>, <paramref name="cont1"/>, <paramref name="cont2"/> contain the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="lead3"/>, <paramref name="cont1"/>, <paramref name="cont2"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To3Char8s(Char32 char32, out byte lead3, out byte cont1, out byte cont2)
    {
      lead3 = UtfUnsafe.Char32To3Char8sUncheckedLead3(char32.Value);
      cont1 = UtfUnsafe.Char32To3Char8sUncheckedCont1(char32.Value);
      cont2 = UtfUnsafe.Char32To3Char8sUncheckedCont2(char32.Value);
      return UtfUnsafe.Char32Is3Char8s(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 4-byte UTF-8 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="lead4"/>, <paramref name="cont1"/>, <paramref name="cont2"/>, <paramref name="cont3"/> contain the valid encoded UTF-8 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="lead4"/>, <paramref name="cont1"/>, <paramref name="cont2"/>, <paramref name="cont3"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To4Char8s(Char32 char32, out byte lead4, out byte cont1, out byte cont2, out byte cont3)
    {
      lead4 = UtfUnsafe.Char32To4Char8sUncheckedLead4(char32.Value);
      cont1 = UtfUnsafe.Char32To4Char8sUncheckedCont1(char32.Value);
      cont2 = UtfUnsafe.Char32To4Char8sUncheckedCont2(char32.Value);
      cont3 = UtfUnsafe.Char32To4Char8sUncheckedCont3(char32.Value);
      return UtfUnsafe.Char32Is4Char8s(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 1-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="char16"/> contains the valid encoded UTF-16 sequence.
    /// Otherwise, there is no guarantee as to what is contained in <paramref name="char16"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To1Char16(Char32 char32, out char char16)
    {
      char16 = UtfUnsafe.Char32To1Char16Unchecked(char32.Value);
      return UtfUnsafe.Char32Is1Char16(char32.Value);
    }

    /// <summary>
    /// Tries to encode a 2-unit UTF-16 sequence.
    /// Returns <see langword="true"/> if and only if the encoding was successful,
    /// upon which <paramref name="high"/>, <paramref name="low"/> contain the valid encoded UTF-16 sequence.
    /// Otherwise, there is no guarantee as to what are contained in <paramref name="high"/>, <paramref name="low"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryChar32To2Char16s(Char32 char32, out char high, out char low)
    {
      int prepared = UtfUnsafe.Char32To2Char16sUncheckedPrepare(char32.Value);
      high = UtfUnsafe.Char32To1Char16Unchecked(prepared);
      low = UtfUnsafe.Char32To1Char16Unchecked(prepared);
      return UtfUnsafe.Char32Is2Char16s(char32.Value);
    }

    #endregion decoding or encoding 1 code point

    [SuppressMessage("Performance", "CA1822", Justification = "Closed delegates are more performant.")]
    private sealed class StringCreateHelper
    {
      [MethodImpl(Helper.JustOptimize)]
      public void FromString8(Span<char> span, Char8[] arg)
      {
        UtfUnsafe.String8ToString16Transform(
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(arg)),
          arg.Length,
          ref MemoryMarshal.GetReference(span),
          span.Length);
      }

      [MethodImpl(Helper.JustOptimize)]
      public void FromString16(Span<char> span, string arg)
      {
        UtfUnsafe.SanitizeChar16s(
          ref MemoryMarshal.GetReference(arg.AsSpan()),
          arg.Length,
          ref MemoryMarshal.GetReference(span),
          span.Length);
      }

      [MethodImpl(Helper.JustOptimize)]
      public void FromString32(Span<char> span, Char32[] arg)
      {
        UtfUnsafe.String32ToString16Transform(
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(arg)),
          arg.Length,
          ref MemoryMarshal.GetReference(span),
          span.Length);
      }
    }

    private static readonly SpanAction<char, Char8[]> theFromString8Action;
    private static readonly SpanAction<char, string> theFromString16Action;
    private static readonly SpanAction<char, Char32[]> theFromString32Action;

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    internal static readonly Char8[] theEmptyChar8s = new Char8[0];

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    internal static readonly Char32[] theEmptyChar32s = new Char32[0];

    #region String8Builder, String32Builder

    [MethodImpl(Helper.OptimizeInline)]
    public static String8 ToString8(this List2<Char8> s8b)
    {
      return new String8(s8b.ToArray());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static String32 ToString32(this List2<Char32> s32b)
    {
      return new String32(s32b.ToArray());
    }

    #endregion String8Builder, String32Builder

    static Utf()
    {
      StringCreateHelper helper = new StringCreateHelper();
      theFromString8Action = helper.FromString8;
      theFromString16Action = helper.FromString16;
      theFromString32Action = helper.FromString32;
    }

    #region conversion from String8

    /// <summary>
    /// Determines whether the specified <see cref="String8"/> is valid UTF-8.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsString8Valid(String8 string8)
    {
      Char8[] data = string8.myData;
      return data is null || UtfUnsafe.FindFirstInvalidChar8(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(data)),
        data.Length) == -1;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if the specified <see cref="String8"/> is not valid UTF-8.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void ThrowIfString8IsNotValid(String8 string8)
    {
      Char8[] data = string8.myData;
      if (data is not null && UtfUnsafe.FindFirstInvalidChar8(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(data)),
        data.Length) != -1)
      {
        throw new ArgumentException(String8IsNotValid, nameof(string8));
      }
    }

    /// <summary>
    /// Determines whether the specified <see cref="String8"/> is valid UTF-8,
    /// and sanitizes it by replacing invalid UTF-8 bytes by the UTF-8 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool CheckAndSanitizeString8(ref String8 string8)
    {
      Char8[] src = string8.myData;
      int src8s;
      long dst8s;
      if (src is null || (dst8s = UtfUnsafe.CountInvalidChar8s(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s = src.Length)) == 0)
      {
        return true;
      }
      dst8s = src8s + dst8s * 2;
      if (dst8s > MaximumLength8)
      {
        throw new ArgumentException(String8WouldBeTooLong, nameof(string8));
      }
      Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
      UtfUnsafe.SanitizeChar8s(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
        (int)dst8s);
      string8 = new String8(dst);
      return false;
    }

    /// <summary>
    /// Sanitizes the specified <see cref="String8"/> by replacing invalid UTF-8 bytes by the UTF-8 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 SanitizeString8(String8 string8)
    {
      Char8[] src = string8.myData;
      int src8s;
      long dst8s;
      if (src is null || (dst8s = UtfUnsafe.CountInvalidChar8s(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s = src.Length)) == 0)
      {
        return new String8(src);
      }
      dst8s = src8s + dst8s * 2;
      if (dst8s > MaximumLength8)
      {
        throw new ArgumentException(String8WouldBeTooLong, nameof(string8));
      }
      Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
      UtfUnsafe.SanitizeChar8s(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
        (int)dst8s);
      return new String8(dst);
    }

    /// <summary>
    /// Tries to convert <see cref="String8"/> to <see langword="string"/>.
    /// If this method returns <see langword="true"/>, the UTF-8 is valid and <paramref name="string16"/> contains its UTF-16 encoding.
    /// Otherwise, the UTF-8 is not valid and <paramref name="string16"/> is <see langword="null"/>.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-16 encoding is also <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryString8ToString16Strict(String8 string8, out string string16)
    {
      string16 = null;
      Char8[] src = string8.myData;
      if (src is null)
      {
        return true;
      }
      int src8s = src.Length;
      if (src8s == 0)
      {
        string16 = "";
        return true;
      }
      int dst16s;
      if (UtfUnsafe.String8ToString16CountStrict(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        out dst16s))
      {
        if (dst16s > MaximumLength16)
        {
          throw new ArgumentException(String16WouldBeTooLong, nameof(string8));
        }
        string16 = string.Create(dst16s, src, theFromString8Action);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts <see cref="String8"/> to <see langword="string"/>.
    /// This method throws <see cref="ArgumentException"/> if the UTF-8 is not valid.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-16 encoding is also <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static string String8ToString16Strict(String8 string8)
    {
      Char8[] src = string8.myData;
      if (src is null)
      {
        return null;
      }
      int src8s = src.Length;
      if (src8s == 0)
      {
        return "";
      }
      int dst16s;
      if (UtfUnsafe.String8ToString16CountStrict(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        out dst16s))
      {
        if (dst16s > MaximumLength16)
        {
          throw new ArgumentException(String16WouldBeTooLong, nameof(string8));
        }
        return string.Create(dst16s, src, theFromString8Action);
      }
      throw new ArgumentException(String8IsNotValid, nameof(string8));
    }

    /// <summary>
    /// Converts <see cref="String8"/> to <see langword="string"/>, replacing invalid UTF-8 bytes by the UTF-16 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-16 encoding is also <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static string String8ToString16Replace(String8 string8)
    {
      Char8[] src = string8.myData;
      if (src is null)
      {
        return null;
      }
      int src8s = src.Length;
      if (src8s == 0)
      {
        return "";
      }
      int dst16s = UtfUnsafe.String8ToString16CountReplace(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s);
      if (dst16s > MaximumLength16)
      {
        throw new ArgumentException(String16WouldBeTooLong, nameof(string8));
      }
      return string.Create(dst16s, src, theFromString8Action);
    }

    /// <summary>
    /// Tries to convert <see cref="String8"/> to <see cref="String32"/>.
    /// If this method returns <see langword="true"/>, the UTF-8 is valid and <paramref name="string32"/> contains its UTF-32 encoding.
    /// Otherwise, the UTF-8 is not valid and <paramref name="string32"/> is the <see langword="null"/> wrapper.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-32 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryString8ToString32Strict(String8 string8, out String32 string32)
    {
      string32 = default(String32);
      Char8[] src = string8.myData;
      if (src is null)
      {
        return true;
      }
      int src8s = src.Length;
      if (src8s == 0)
      {
        string32 = new String32(theEmptyChar32s);
        return true;
      }
      int dst32s;
      if (UtfUnsafe.String8ToString32CountStrict(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        out dst32s))
      {
        if (dst32s > MaximumLength32)
        {
          throw new ArgumentException(String32WouldBeTooLong, nameof(string8));
        }
        Char32[] dst = GC.AllocateUninitializedArray<Char32>(dst32s, false);
        UtfUnsafe.String8ToString32Transform(
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
          src8s,
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
          dst32s);
        string32 = new String32(dst);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts <see cref="String8"/> to <see cref="String32"/>.
    /// This method throws <see cref="ArgumentException"/> if the UTF-8 is not valid.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-32 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 String8ToString32Strict(String8 string8)
    {
      Char8[] src = string8.myData;
      if (src is null)
      {
        return default(String32);
      }
      int src8s = src.Length;
      if (src8s == 0)
      {
        return new String32(theEmptyChar32s);
      }
      int dst32s;
      if (UtfUnsafe.String8ToString32CountStrict(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        out dst32s))
      {
        if (dst32s > MaximumLength32)
        {
          throw new ArgumentException(String32WouldBeTooLong, nameof(string8));
        }
        Char32[] dst = GC.AllocateUninitializedArray<Char32>(dst32s, false);
        UtfUnsafe.String8ToString32Transform(
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
          src8s,
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
          dst32s);
        return new String32(dst);
      }
      throw new ArgumentException(String8IsNotValid, nameof(string8));
    }

    /// <summary>
    /// Converts <see cref="String8"/> to <see cref="String32"/>, replacing invalid UTF-8 bytes by the UTF-32 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-32 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 String8ToString32Replace(String8 string8)
    {
      Char8[] src = string8.myData;
      if (src is null)
      {
        return default(String32);
      }
      int src8s = src.Length;
      if (src8s == 0)
      {
        return new String32(theEmptyChar32s);
      }
      int dst32s = UtfUnsafe.String8ToString32CountReplace(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s);
      if (dst32s > MaximumLength32)
      {
        throw new ArgumentException(String32WouldBeTooLong, nameof(string8));
      }
      Char32[] dst = GC.AllocateUninitializedArray<Char32>(dst32s, false);
      UtfUnsafe.String8ToString32Transform(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(src)),
        src8s,
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
        dst32s);
      return new String32(dst);
    }

    #endregion conversion from String8

    #region conversion from String16

    /// <summary>
    /// Determines whether the specified <see langword="string"/> is valid UTF-16.
    /// The <see langword="null"/> reference is considered valid.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsString16Valid(string string16)
    {
      return string16 is null || UtfUnsafe.FindFirstInvalidChar16(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        string16.Length) == -1;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if the specified <see langword="string"/> is not valid UTF-16.
    /// The <see langword="null"/> reference is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void ThrowIfString16IsNotValid(string string16)
    {
      if (string16 is not null && UtfUnsafe.FindFirstInvalidChar16(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        string16.Length) != -1)
      {
        throw new ArgumentException(String16IsNotValid, nameof(string16));
      }
    }

    /// <summary>
    /// Tries to convert <see langword="string"/> to <see cref="String8"/>.
    /// If this method returns <see langword="true"/>, the UTF-16 is valid and <paramref name="string8"/> contains its UTF-8 encoding.
    /// Otherwise, the UTF-16 is not valid and <paramref name="string8"/> is the <see langword="null"/> wrapper.
    /// The <see langword="null"/> reference is considered valid and its UTF-8 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryString16ToString8Strict(string string16, out String8 string8)
    {
      string8 = default(String8);
      if (string16 is null)
      {
        return true;
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        string8 = new String8(theEmptyChar8s);
        return true;
      }
      long dst8s;
      if (UtfUnsafe.String16ToString8CountStrict(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s,
        out dst8s))
      {
        if (dst8s > MaximumLength8)
        {
          throw new ArgumentException(String8WouldBeTooLong, nameof(string16));
        }
        Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
        UtfUnsafe.String16ToString8Transform(
          ref MemoryMarshal.GetReference(string16.AsSpan()),
          src16s,
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
          (int)dst8s);
        string8 = new String8(dst);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String8"/>.
    /// This method throws <see cref="ArgumentException"/> if the UTF-16 is not valid.
    /// The <see langword="null"/> reference is considered valid and its UTF-8 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 String16ToString8Strict(string string16)
    {
      if (string16 is null)
      {
        return default(String8);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String8(theEmptyChar8s);
      }
      long dst8s;
      if (UtfUnsafe.String16ToString8CountStrict(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s,
        out dst8s))
      {
        if (dst8s > MaximumLength8)
        {
          throw new ArgumentException(String8WouldBeTooLong, nameof(string16));
        }
        Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
        UtfUnsafe.String16ToString8Transform(
          ref MemoryMarshal.GetReference(string16.AsSpan()),
          src16s,
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
          (int)dst8s);
        return new String8(dst);
      }
      throw new ArgumentException(String16IsNotValid, nameof(string16));
    }

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String8"/>, replacing invalid UTF-16 code units by the UTF-8 encoding of the replacement character.
    /// The <see langword="null"/> reference is considered valid and its UTF-8 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 String16ToString8Replace(string string16)
    {
      if (string16 is null)
      {
        return default(String8);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String8(theEmptyChar8s);
      }
      long dst8s = UtfUnsafe.String16ToString8CountReplace(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s);
      if (dst8s > MaximumLength8)
      {
        throw new ArgumentException(String8WouldBeTooLong, nameof(string16));
      }
      Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
      UtfUnsafe.String16ToString8Transform(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s,
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
        (int)dst8s);
      return new String8(dst);
    }

    /// <summary>
    /// Determines whether the specified <see langword="string"/> is valid UTF-16,
    /// and sanitizes it by replacing invalid UTF-16 code units by the UTF-16 encoding of the replacement character.
    /// The <see langword="null"/> reference is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool CheckAndSanitizeString16(ref string string16)
    {
      string src = string16;
      int src16s;
      if (src is null || UtfUnsafe.FindFirstInvalidChar16(
        ref MemoryMarshal.GetReference(src.AsSpan()),
        src16s = src.Length) == -1)
      {
        return true;
      }
      if (src16s > MaximumLength16)
      {
        throw new ArgumentException(String16WouldBeTooLong, nameof(string16));
      }
      string16 = string.Create(src16s, src, theFromString16Action);
      return false;
    }

    /// <summary>
    /// Sanitizes the specified <see langword="string"/> by replacing invalid UTF-16 code units by the UTF-16 encoding of the replacement character.
    /// The <see langword="null"/> reference is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static string SanitizeString16(string string16)
    {
      string src = string16;
      int src16s;
      if (src is null || UtfUnsafe.FindFirstInvalidChar16(
        ref MemoryMarshal.GetReference(src.AsSpan()),
        src16s = src.Length) == -1)
      {
        return src;
      }
      if (src16s > MaximumLength16)
      {
        throw new ArgumentException(String16WouldBeTooLong, nameof(string16));
      }
      return string.Create(src16s, src, theFromString16Action);
    }

    /// <summary>
    /// Tries to convert <see langword="string"/> to <see cref="String32"/>.
    /// If this method returns <see langword="true"/>, the UTF-16 is valid and <paramref name="string32"/> contains its UTF-32 encoding.
    /// Otherwise, the UTF-16 is not valid and <paramref name="string32"/> is the <see langword="null"/> wrapper.
    /// The <see langword="null"/> reference is considered valid and its UTF-32 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryString16ToString32Strict(string string16, out String32 string32)
    {
      string32 = default(String32);
      if (string16 is null)
      {
        return true;
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        string32 = new String32(theEmptyChar32s);
        return true;
      }
      int dst32s;
      if (UtfUnsafe.String16ToString32CountStrict(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s,
        out dst32s))
      {
        if (dst32s > MaximumLength32)
        {
          throw new ArgumentException(String32WouldBeTooLong, nameof(string16));
        }
        Char32[] dst = GC.AllocateUninitializedArray<Char32>(dst32s, false);
        UtfUnsafe.String16ToString32Transform(
          ref MemoryMarshal.GetReference(string16.AsSpan()),
          src16s,
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
          dst32s);
        string32 = new String32(dst);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String32"/>.
    /// This method throws <see cref="ArgumentException"/> if the UTF-16 is not valid.
    /// The <see langword="null"/> reference is considered valid and its UTF-32 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 String16ToString32Strict(string string16)
    {
      if (string16 is null)
      {
        return default(String32);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String32(theEmptyChar32s);
      }
      int dst32s;
      if (UtfUnsafe.String16ToString32CountStrict(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s,
        out dst32s))
      {
        if (dst32s > MaximumLength32)
        {
          throw new ArgumentException(String32WouldBeTooLong, nameof(string16));
        }
        Char32[] dst = GC.AllocateUninitializedArray<Char32>(dst32s, false);
        UtfUnsafe.String16ToString32Transform(
          ref MemoryMarshal.GetReference(string16.AsSpan()),
          src16s,
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
          dst32s);
        return new String32(dst);
      }
      throw new ArgumentException(String16IsNotValid, nameof(string16));
    }

    /// <summary>
    /// Converts <see langword="string"/> to <see cref="String32"/>, replacing invalid UTF-16 code units by the UTF-32 encoding of the replacement character.
    /// The <see langword="null"/> reference is considered valid and its UTF-32 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 String16ToString32Replace(string string16)
    {
      if (string16 is null)
      {
        return default(String32);
      }
      int src16s = string16.Length;
      if (src16s == 0)
      {
        return new String32(theEmptyChar32s);
      }
      int dst32s = UtfUnsafe.String16ToString32CountReplace(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s);
      if (dst32s > MaximumLength32)
      {
        throw new ArgumentException(String32WouldBeTooLong, nameof(string16));
      }
      Char32[] dst = GC.AllocateUninitializedArray<Char32>(dst32s, false);
      UtfUnsafe.String16ToString32Transform(
        ref MemoryMarshal.GetReference(string16.AsSpan()),
        src16s,
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
        dst32s);
      return new String32(dst);
    }

    #endregion conversion from String16

    #region conversion from String32

    /// <summary>
    /// Determines whether the specified <see cref="String32"/> is valid UTF-32.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsString32Valid(String32 string32)
    {
      Char32[] data = string32.myData;
      return data is null || UtfUnsafe.FindFirstInvalidChar32(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(data)),
        data.Length) == -1;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if the specified <see cref="String32"/> is not valid UTF-32.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static void ThrowIfString32IsNotValid(String32 string32)
    {
      Char32[] data = string32.myData;
      if (data is not null && UtfUnsafe.FindFirstInvalidChar32(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(data)),
        data.Length) != -1)
      {
        throw new ArgumentException(String32IsNotValid, nameof(string32));
      }
    }

    /// <summary>
    /// Tries to convert <see cref="String32"/> to <see cref="String8"/>.
    /// If this method returns <see langword="true"/>, the UTF-32 is valid and <paramref name="string8"/> contains its UTF-8 encoding.
    /// Otherwise, the UTF-32 is not valid and <paramref name="string8"/> is the <see langword="null"/> wrapper.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-8 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryString32ToString8Strict(String32 string32, out String8 string8)
    {
      string8 = default(String8);
      Char32[] src = string32.myData;
      if (src is null)
      {
        return true;
      }
      int src32s = src.Length;
      if (src32s == 0)
      {
        string8 = new String8(theEmptyChar8s);
        return true;
      }
      long dst8s;
      if (UtfUnsafe.String32ToString8CountStrict(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        out dst8s))
      {
        if (dst8s > MaximumLength8)
        {
          throw new ArgumentException(String8WouldBeTooLong, nameof(string32));
        }
        Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
        UtfUnsafe.String32ToString8Transform(
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
          src32s,
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
          (int)dst8s);
        string8 = new String8(dst);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts <see cref="String32"/> to <see cref="String8"/>.
    /// This method throws <see cref="ArgumentException"/> if the UTF-32 is not valid.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-8 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 String32ToString8Strict(String32 string32)
    {
      Char32[] src = string32.myData;
      if (src is null)
      {
        return default(String8);
      }
      int src32s = src.Length;
      if (src32s == 0)
      {
        return new String8(theEmptyChar8s);
      }
      long dst8s;
      if (UtfUnsafe.String32ToString8CountStrict(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        out dst8s))
      {
        if (dst8s > MaximumLength8)
        {
          throw new ArgumentException(String8WouldBeTooLong, nameof(string32));
        }
        Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
        UtfUnsafe.String32ToString8Transform(
          ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
          src32s,
          ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
          (int)dst8s);
        return new String8(dst);
      }
      throw new ArgumentException(String32IsNotValid, nameof(string32));
    }

    /// <summary>
    /// Converts <see cref="String32"/> to <see cref="String8"/>, replacing invalid UTF-32 code points by the UTF-8 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-8 encoding is also the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String8 String32ToString8Replace(String32 string32)
    {
      Char32[] src = string32.myData;
      if (src is null)
      {
        return default(String8);
      }
      int src32s = src.Length;
      if (src32s == 0)
      {
        return new String8(theEmptyChar8s);
      }
      long dst8s = UtfUnsafe.String32ToString8CountReplace(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s);
      if (dst8s > MaximumLength8)
      {
        throw new ArgumentException(String8WouldBeTooLong, nameof(string32));
      }
      Char8[] dst = GC.AllocateUninitializedArray<Char8>((int)dst8s, false);
      UtfUnsafe.String32ToString8Transform(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(dst)),
        (int)dst8s);
      return new String8(dst);
    }

    /// <summary>
    /// Tries to convert <see cref="String32"/> to <see langword="string"/>.
    /// If this method returns <see langword="true"/>, the UTF-32 is valid and <paramref name="string16"/> contains its UTF-16 encoding.
    /// Otherwise, the UTF-32 is not valid and <paramref name="string16"/> is <see langword="null"/>.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-16 encoding is also <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool TryString32ToString16Strict(String32 string32, out string string16)
    {
      string16 = null;
      Char32[] src = string32.myData;
      if (src is null)
      {
        return true;
      }
      int src32s = src.Length;
      if (src32s == 0)
      {
        string16 = "";
        return true;
      }
      long dst16s;
      if (UtfUnsafe.String32ToString16CountStrict(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        out dst16s))
      {
        if (dst16s > MaximumLength16)
        {
          throw new ArgumentException(String16WouldBeTooLong, nameof(string32));
        }
        string16 = string.Create((int)dst16s, src, theFromString32Action);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Converts <see cref="String32"/> to <see langword="string"/>.
    /// This method throws <see cref="ArgumentException"/> if the UTF-32 is not valid.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-16 encoding is also <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static string String32ToString16Strict(String32 string32)
    {
      Char32[] src = string32.myData;
      if (src is null)
      {
        return null;
      }
      int src32s = src.Length;
      if (src32s == 0)
      {
        return "";
      }
      long dst16s;
      if (UtfUnsafe.String32ToString16CountStrict(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        out dst16s))
      {
        if (dst16s > MaximumLength16)
        {
          throw new ArgumentException(String16WouldBeTooLong, nameof(string32));
        }
        return string.Create((int)dst16s, src, theFromString32Action);
      }
      throw new ArgumentException(String32IsNotValid, nameof(string32));
    }

    /// <summary>
    /// Converts <see cref="String32"/> to <see langword="string"/>, replacing invalid UTF-32 code points by the UTF-16 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid and its UTF-16 encoding is also <see langword="null"/>.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static string String32ToString16Replace(String32 string32)
    {
      Char32[] src = string32.myData;
      if (src is null)
      {
        return null;
      }
      int src32s = src.Length;
      if (src32s == 0)
      {
        return "";
      }
      long dst16s = UtfUnsafe.String32ToString16CountReplace(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s);
      if (dst16s > MaximumLength16)
      {
        throw new ArgumentException(String16WouldBeTooLong, nameof(string32));
      }
      return string.Create((int)dst16s, src, theFromString32Action);
    }

    /// <summary>
    /// Determines whether the specified <see cref="String32"/> is valid UTF-32,
    /// and sanitizes it by replacing invalid UTF-32 code points by the UTF-32 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static bool CheckAndSanitizeString32(ref String32 string32)
    {
      Char32[] src = string32.myData;
      int src32s;
      if (src is null || UtfUnsafe.FindFirstInvalidChar32(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s = src.Length) == -1)
      {
        return true;
      }
      if (src32s > MaximumLength32)
      {
        throw new ArgumentException(String32WouldBeTooLong, nameof(string32));
      }
      Char32[] dst = GC.AllocateUninitializedArray<Char32>(src32s, false);
      UtfUnsafe.SanitizeChar32s(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
        src32s);
      string32 = new String32(dst);
      return false;
    }

    /// <summary>
    /// Sanitizes the specified <see cref="String32"/> by replacing invalid UTF-32 code points by the UTF-32 encoding of the replacement character.
    /// The <see langword="null"/> wrapper is considered valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public static String32 SanitizeString32(String32 string32)
    {
      Char32[] src = string32.myData;
      int src32s;
      if (src is null || UtfUnsafe.FindFirstInvalidChar32(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s = src.Length) == -1)
      {
        return new String32(src);
      }
      if (src32s > MaximumLength32)
      {
        throw new ArgumentException(String32WouldBeTooLong, nameof(string32));
      }
      Char32[] dst = GC.AllocateUninitializedArray<Char32>(src32s, false);
      UtfUnsafe.SanitizeChar32s(
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(src)),
        src32s,
        ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(dst)),
        src32s);
      return new String32(dst);
    }

    #endregion conversion from String32
  }
}
