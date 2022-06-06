using System;
using System.Runtime.CompilerServices;

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
  }
}
