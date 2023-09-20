using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

namespace Neat.Collections
{
  internal static class Helper
  {
    internal const MethodImplOptions JustInline = MethodImplOptions.AggressiveInlining;
    internal const MethodImplOptions NoInline = MethodImplOptions.NoInlining;
    internal const MethodImplOptions JustOptimize = MethodImplOptions.AggressiveOptimization;
    internal const MethodImplOptions OptimizeInline = MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining;
    internal const MethodImplOptions OptimizeNoInline = MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining;
  }
}
