using System;
using System.Diagnostics.CodeAnalysis;
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

    #region InternPool (see Neat.Collections.Map2)

    internal const int HashCodeMask = 0x7FFFFFFF;

    public const int MaximumCapacity = 881646013;

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    internal static readonly int[] theEmptyBuckets = new int[0];

    internal readonly struct Size
    {
      public readonly int BucketCount;
      public readonly int EntryCount;

      public Size(int bucketCount, int entryCount)
      {
        BucketCount = bucketCount;
        EntryCount = entryCount;
      }
    }

    internal static readonly Size[] theSizes = new Size[]
    {
      new Size(17, 12),
      new Size(37, 27),
      new Size(79, 59),
      new Size(163, 122),
      new Size(331, 248),
      new Size(673, 504),
      new Size(1361, 1020),
      new Size(2729, 2046),
      new Size(5471, 4103),
      new Size(10949, 8211),
      new Size(21911, 16433),
      new Size(43853, 32889),
      new Size(87719, 65789),
      new Size(175447, 131585),
      new Size(350899, 263174),
      new Size(701819, 526364),
      new Size(1052731, 894821),
      new Size(1579099, 1342234),
      new Size(2368649, 2013351),
      new Size(3552977, 3020030),
      new Size(5329469, 4530048),
      new Size(7994213, 6795081),
      new Size(11991373, 10192667),
      new Size(17987089, 15289025),
      new Size(26980697, 22933592),
      new Size(40471061, 34400401),
      new Size(60706627, 51600632),
      new Size(91059949, 77400956),
      new Size(136590001, 116101500),
      new Size(204885013, 174152261),
      new Size(307327523, 261228394),
      new Size(460991303, 391842607),
      new Size(691487003, 587763952),
      new Size(881646013, 881646013)
    };

    [MethodImpl(OptimizeNoInline)]
    internal static void ResetBuckets(int[] buckets)
    {
      Unsafe.InitBlockUnaligned(
        ref Unsafe.As<int, byte>(ref MemoryMarshal.GetArrayDataReference(buckets)),
        0xFF,
        (uint)buckets.Length * 4u);
    }

    #endregion InternPool (see Neat.Collections.Map2)

    #region String32: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString, GenericEquals, GenericGetHashCode

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

    [MethodImpl(JustOptimize)]
    internal static bool GenericEquals(String32 that, ref int start, int length)
    {
      if (that.Length != length)
      {
        return false;
      }
      ref int data0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(Unsafe.As<String32, Char32[]>(ref that)));
      for (int i = 0; i != length; ++i)
      {
        if (Unsafe.Add(ref data0, i) != Unsafe.Add(ref start, i))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(JustOptimize)]
    internal static int GenericGetHashCode(ref int start, int length)
    {
      int hash = FnvOffsetBasis;
      for (int i = 0, value; i != length; ++i)
      {
        value = Unsafe.Add(ref start, i);
        hash = (hash ^ (value & 0xFF)) * FnvPrime;
        value = (int)((uint)value >> 8);
        hash = (hash ^ (value & 0xFF)) * FnvPrime;
        value = (int)((uint)value >> 8);
        hash = (hash ^ (value & 0xFF)) * FnvPrime;
        value = (int)((uint)value >> 8);
        hash = (hash ^ value) * FnvPrime;
      }
      return hash ^ length;
    }

    #endregion String32: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString, GenericEquals, GenericGetHashCode

    #region String16: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString, GenericEquals, GenericGetHashCode

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

    [MethodImpl(JustOptimize)]
    internal static bool GenericEquals(string that, ref char start, int length)
    {
      if (that.Length != length)
      {
        return false;
      }
      ref char data0 = ref MemoryMarshal.GetReference(that.AsSpan());
      for (int i = 0; i != length; ++i)
      {
        if (Unsafe.Add(ref data0, i) != Unsafe.Add(ref start, i))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(JustOptimize)]
    internal static int GenericGetHashCode(ref char start, int length)
    {
      int hash = FnvOffsetBasis;
      for (int i = 0, value; i != length; ++i)
      {
        value = Unsafe.Add(ref start, i);
        hash = (hash ^ (value & 0xFF)) * FnvPrime;
        value = (int)((uint)value >> 8);
        hash = (hash ^ value) * FnvPrime;
      }
      return hash ^ length;
    }

    #endregion String16: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString, GenericEquals, GenericGetHashCode

    #region String8: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString, GenericEquals, GenericGetHashCode

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

    [MethodImpl(JustOptimize)]
    internal static bool GenericEquals(String8 that, ref byte start, int length)
    {
      if (that.Length != length)
      {
        return false;
      }
      ref byte data0 = ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(Unsafe.As<String8, Char8[]>(ref that)));
      for (int i = 0; i != length; ++i)
      {
        if (Unsafe.Add(ref data0, i) != Unsafe.Add(ref start, i))
        {
          return false;
        }
      }
      return true;
    }

    [MethodImpl(JustOptimize)]
    internal static int GenericGetHashCode(ref byte start, int length)
    {
      int hash = FnvOffsetBasis;
      for (int i = 0; i != length; ++i)
      {
        hash = (hash ^ Unsafe.Add(ref start, i)) * FnvPrime;
      }
      return hash ^ length;
    }

    #endregion String8: GenericToString, GenericIsDefault, GenericGetData0, GenericGetString, GenericEquals, GenericGetHashCode
  }
}
