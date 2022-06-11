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

    internal static readonly int TrailingMask1;
    internal static readonly int TrailingMask2;
    internal static readonly int TrailingMask3;

    internal static readonly int FnvOffsetBasis;
    internal const int FnvPrime = 16777619;

    static Helper()
    {
      int mask = 0;
      Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref mask), (byte)0xFF);
      TrailingMask1 = mask;
      Unsafe.WriteUnaligned(ref Unsafe.Add(ref Unsafe.As<int, byte>(ref mask), 1), (byte)0xFF);
      TrailingMask2 = mask;
      Unsafe.WriteUnaligned(ref Unsafe.Add(ref Unsafe.As<int, byte>(ref mask), 2), (byte)0xFF);
      TrailingMask3 = mask;
      FnvOffsetBasis = Environment.TickCount + "Neat.BibTeX".GetHashCode();
    }

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
  }
}
