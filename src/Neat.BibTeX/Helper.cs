using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Unicode;

[assembly: CLSCompliant(true)]

namespace Neat.BibTeX
{
  internal static class Helper
  {
    internal const MethodImplOptions JustInline = MethodImplOptions.AggressiveInlining;
    internal const MethodImplOptions NoInline = MethodImplOptions.NoInlining;
    internal const MethodImplOptions JustOptimize = MethodImplOptions.AggressiveOptimization;
    internal const MethodImplOptions OptimizeInline = MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining;
    internal const MethodImplOptions OptimizeNoInline = MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining;

    internal static readonly int FnvOffsetBasis;
    internal const int FnvPrime = 16777619;

    static Helper()
    {
      FnvOffsetBasis = Environment.TickCount + "Neat.BibTeX".GetHashCode();
    }

    #region String32: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString

    [MethodImpl(OptimizeInline)]
    internal static string GenericToString(this String32 that)
    {
      return that.ToString();
    }

    [MethodImpl(OptimizeInline)]
    internal static bool GenericIsDefault(this String32 that)
    {
      return that.IsDefault;
    }

    [MethodImpl(OptimizeInline)]
    internal static ref int GenericGetData0(this String32 that)
    {
      return ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(Unsafe.As<String32, Char32[]>(ref that)));
    }

    [MethodImpl(JustOptimize)]
    internal static String32 GenericGetString(ref int start, uint length)
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
      return GenericGetStringRareImpl(ref Unsafe.As<int, byte>(ref start), length);
    }

    [MethodImpl(JustOptimize)]
    private static String32 GenericGetStringRareImpl(ref byte start, uint length)
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

    #endregion String32: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString

    #region String16: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString

    [MethodImpl(OptimizeInline)]
    internal static string GenericToString(this string that)
    {
      /* Allow invocation on null. */
      return that;
    }

    [MethodImpl(OptimizeInline)]
    internal static bool GenericIsDefault(this string that)
    {
      return that is null;
    }

    [MethodImpl(OptimizeInline)]
    internal static ref char GenericGetData0(this string that)
    {
      return ref MemoryMarshal.GetReference(that.AsSpan());
    }

    [MethodImpl(OptimizeInline)]
    internal static string GenericGetString(ref char start, uint length)
    {
      return length == 0 ? "" : new string(MemoryMarshal.CreateReadOnlySpan(ref start, (int)length));
    }

    #endregion String16: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString

    #region String8: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString

    [MethodImpl(OptimizeInline)]
    internal static string GenericToString(this String8 that)
    {
      return that.ToString();
    }

    [MethodImpl(OptimizeInline)]
    internal static bool GenericIsDefault(this String8 that)
    {
      return that.IsDefault;
    }

    [MethodImpl(OptimizeInline)]
    internal static ref byte GenericGetData0(this String8 that)
    {
      return ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(Unsafe.As<String8, Char8[]>(ref that)));
    }

    [MethodImpl(JustOptimize)]
    internal static String8 GenericGetString(ref byte start, uint length)
    {
      if (length == 0)
      {
        return String8.Empty;
      }
      Char8[] data = new Char8[length];
      Unsafe.CopyBlockUnaligned(
        ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(data)),
        ref start,
        length
      );
      return Unsafe.As<Char8[], String8>(ref data);
    }

    #endregion String8: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString
  }
}
