using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Unicode;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements case-insensitive comparison as far as BibTeX is concerned.
  /// The strings are assumed to be ASCII-printable.
  /// </summary>
  public struct BibBstComparer
    : IEqualityComparer2<String32>, IComparer<String32>
  {
    #region String32

    [SuppressMessage("Style", "IDE0004", Justification = "Make interger promotion explicit.")]
    [MethodImpl(Helper.JustOptimize)]
    private static int CompareImpl(Char32[] x, Char32[] y)
    {
      if (ReferenceEquals(x, y))
      {
        return 0;
      }
      if (x is null)
      {
        return -1;
      }
      if (y is null)
      {
        return 1;
      }
      int xlength = x.Length, ylength = y.Length;
      ref int x0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(x));
      ref int y0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(y));
      for (int i = 0, length = (xlength < ylength ? xlength : ylength), xi, yi, z; i != length; ++i)
      {
        xi = Unsafe.Add(ref x0, i);
        yi = Unsafe.Add(ref y0, i);
        /* It is fine to do a simple subtraction because both strings are assumed to be ASCII-printable. */
        z = ((uint)(xi - 'A') <= (uint)('Z' - 'A') ? xi + ('a' - 'A') : xi) - ((uint)(yi - 'A') <= (uint)('Z' - 'A') ? yi + ('a' - 'A') : yi);
        if (z != 0)
        {
          return z;
        }
      }
      return xlength - ylength;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Make interger promotion explicit.")]
    [MethodImpl(Helper.JustOptimize)]
    private static bool EqualsImpl(Char32[] x, Char32[] y)
    {
      if (ReferenceEquals(x, y))
      {
        return true;
      }
      if (x is null || y is null)
      {
        return false;
      }
      int length = x.Length;
      if (length != y.Length)
      {
        return false;
      }
      ref int x0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(x));
      ref int y0 = ref Unsafe.As<Char32, int>(ref MemoryMarshal.GetArrayDataReference(y));
      for (int i = 0, xi, yi; i != length; i++)
      {
        xi = Unsafe.Add(ref x0, i);
        yi = Unsafe.Add(ref y0, i);
        if (((uint)(xi - 'A') <= (uint)('Z' - 'A') ? xi + ('a' - 'A') : xi) != ((uint)(yi - 'A') <= (uint)('Z' - 'A') ? yi + ('a' - 'A') : yi))
        {
          return false;
        }
      }
      return true;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Make interger promotion explicit.")]
    [MethodImpl(Helper.JustOptimize)]
    private static int GetHashCodeImpl(Char32[] x)
    {
      if (x is null)
      {
        return 0;
      }
      int hash = Helper.FnvOffsetBasis;
      for (int i = 0, value; i < x.Length; ++i)
      {
        value = x[i].Value;
        value = ((uint)(value - 'A') <= (uint)('Z' - 'A') ? value + ('a' - 'A') : value);
        hash = (hash ^ value) * Helper.FnvPrime;
      }
      return hash ^ x.Length;
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IComparer<String32>.Compare(String32 x, String32 y)
    {
      return CompareImpl(Unsafe.As<String32, Char32[]>(ref x), Unsafe.As<String32, Char32[]>(ref y));
    }

    [MethodImpl(Helper.OptimizeInline)]
    bool IEqualityComparer<String32>.Equals(String32 x, String32 y)
    {
      return EqualsImpl(Unsafe.As<String32, Char32[]>(ref x), Unsafe.As<String32, Char32[]>(ref y));
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IEqualityComparer<String32>.GetHashCode(String32 obj)
    {
      return GetHashCodeImpl(Unsafe.As<String32, Char32[]>(ref obj));
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IEqualityComparer2<String32>.GetHashCode(String32 obj)
    {
      return GetHashCodeImpl(Unsafe.As<String32, Char32[]>(ref obj));
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(String32 x, String32 y)
    {
      return CompareImpl(Unsafe.As<String32, Char32[]>(ref x), Unsafe.As<String32, Char32[]>(ref y));
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(String32 x, String32 y)
    {
      return EqualsImpl(Unsafe.As<String32, Char32[]>(ref x), Unsafe.As<String32, Char32[]>(ref y));
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int GetHashCode(String32 obj)
    {
      return GetHashCodeImpl(Unsafe.As<String32, Char32[]>(ref obj));
    }

    #endregion String32
  }
}
