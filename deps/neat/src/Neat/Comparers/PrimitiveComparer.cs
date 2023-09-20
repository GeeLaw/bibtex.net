using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neat.Comparers
{
  public struct PrimitiveComparer
    : IEqualityComparer2<bool>, IComparer<bool>,
      IEqualityComparer2<byte>, IComparer<byte>,
      IEqualityComparer2<sbyte>, IComparer<sbyte>,
      IEqualityComparer2<char>, IComparer<char>,
      IEqualityComparer2<Rune>, IComparer<Rune>,
      IEqualityComparer2<decimal>, IComparer<decimal>,
      IEqualityComparer2<double>, IComparer<double>,
      IEqualityComparer2<float>, IComparer<float>,
      IEqualityComparer2<Half>, IComparer<Half>,
      IEqualityComparer2<int>, IComparer<int>,
      IEqualityComparer2<uint>, IComparer<uint>,
      IEqualityComparer2<nint>, IComparer<nint>,
      IEqualityComparer2<nuint>, IComparer<nuint>,
      IEqualityComparer2<long>, IComparer<long>,
      IEqualityComparer2<ulong>, IComparer<ulong>,
      IEqualityComparer2<short>, IComparer<short>,
      IEqualityComparer2<ushort>, IComparer<ushort>,
      IEqualityComparer2<string>, IComparer<string>,
      IEqualityComparer2<bool?>, IComparer<bool?>,
      IEqualityComparer2<byte?>, IComparer<byte?>,
      IEqualityComparer2<sbyte?>, IComparer<sbyte?>,
      IEqualityComparer2<char?>, IComparer<char?>,
      IEqualityComparer2<Rune?>, IComparer<Rune?>,
      IEqualityComparer2<decimal?>, IComparer<decimal?>,
      IEqualityComparer2<double?>, IComparer<double?>,
      IEqualityComparer2<float?>, IComparer<float?>,
      IEqualityComparer2<Half?>, IComparer<Half?>,
      IEqualityComparer2<int?>, IComparer<int?>,
      IEqualityComparer2<uint?>, IComparer<uint?>,
      IEqualityComparer2<nint?>, IComparer<nint?>,
      IEqualityComparer2<nuint?>, IComparer<nuint?>,
      IEqualityComparer2<long?>, IComparer<long?>,
      IEqualityComparer2<ulong?>, IComparer<ulong?>,
      IEqualityComparer2<short?>, IComparer<short?>,
      IEqualityComparer2<ushort?>, IComparer<ushort?>
  {
    public static PrimitiveComparer Instance
    {
      [MethodImpl(Helper.OptimizeInline)]
      get { return default(PrimitiveComparer); }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(bool x, bool y)
    {
      return x ^ y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(bool obj)
    {
      return obj ? -1 : 1;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(bool x, bool y)
    {
      return x ? y ? 0 : 1 : y ? -1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(byte x, byte y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(byte obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(byte x, byte y)
    {
      return (int)x - (int)y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(sbyte x, sbyte y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(sbyte obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(sbyte x, sbyte y)
    {
      return (int)x - (int)y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(char x, char y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(char obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-65536 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(char x, char y)
    {
      return (int)x - (int)y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Rune x, Rune y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(Rune obj)
    {
      return obj.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(Rune x, Rune y)
    {
      return Compare(x.Value, y.Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(decimal x, decimal y)
    {
      return decimal.Equals(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(decimal obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(decimal x, decimal y)
    {
      return decimal.Compare(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(double x, double y)
    {
      /* This handles NaN correctly. */
      return x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(double obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(double x, double y)
    {
      /* This handles NaN correctly. */
      return x.CompareTo(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(float x, float y)
    {
      /* This handles NaN correctly. */
      return x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(float obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(float x, float y)
    {
      /* This handles NaN correctly. */
      return x.CompareTo(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Half x, Half y)
    {
      /* This handles NaN correctly. */
      return x.Equals(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(Half obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(Half x, Half y)
    {
      /* This handles NaN correctly. */
      return x.CompareTo(y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(int x, int y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(int obj)
    {
      return obj;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(int x, int y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(uint x, uint y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(uint obj)
    {
      return (int)obj;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(uint x, uint y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(nint x, nint y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(nint obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(nint x, nint y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(nuint x, nuint y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(nuint obj)
    {
      return obj.GetHashCode();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(nuint x, nuint y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(long x, long y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(long obj)
    {
      return obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(long x, long y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(ulong x, ulong y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(ulong obj)
    {
      return obj.GetHashCode();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(ulong x, ulong y)
    {
      return x < y ? -1 : x > y ? 1 : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(short x, short y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(short obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-65536 subtraction.")]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(short x, short y)
    {
      return (int)x - (int)y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(ushort x, ushort y)
    {
      return x == y;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(ushort obj)
    {
      return obj;
    }

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-65536 subtraction.")]
    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(ushort x, ushort y)
    {
      return (int)x - (int)y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(string x, string y)
    {
      return x == y;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(string obj)
    {
      return obj is null ? 0 : obj.GetHashCode();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(string x, string y)
    {
      return string.CompareOrdinal(x, y);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(bool? x, bool? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() ^ y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(bool? obj)
    {
      return obj.HasValue
        ? obj.GetValueOrDefault()
          ? -1
          : 1
        : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(bool? x, bool? y)
    {
      return (x.HasValue ? x.GetValueOrDefault() ? 2 : 1 : 0)
        - (y.HasValue ? y.GetValueOrDefault() ? 2 : 1 : 0);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(byte? x, byte? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(byte? obj)
    {
      return obj.GetValueOrDefault();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(byte? x, byte? y)
    {
      return (x.HasValue ? x.GetValueOrDefault() : -131072)
        - (y.HasValue ? y.GetValueOrDefault() : -131072);
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(sbyte? x, sbyte? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(sbyte? obj)
    {
      return obj.GetValueOrDefault();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(sbyte? x, sbyte? y)
    {
      return (x.HasValue ? x.GetValueOrDefault() : -131072)
        - (y.HasValue ? y.GetValueOrDefault() : -131072);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(char? x, char? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(char? obj)
    {
      return obj.GetValueOrDefault();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(char? x, char? y)
    {
      return (x.HasValue ? x.GetValueOrDefault() : -131072)
        - (y.HasValue ? y.GetValueOrDefault() : -131072);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Rune? x, Rune? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault().Value == y.GetValueOrDefault().Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(Rune? obj)
    {
      return obj.GetValueOrDefault().Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(Rune? x, Rune? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault().Value, y.GetValueOrDefault().Value)
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(decimal? x, decimal? y)
    {
      return x.HasValue
        ? y.HasValue && decimal.Equals(x.GetValueOrDefault(), y.GetValueOrDefault())
        : !y.HasValue;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(decimal? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(decimal? x, decimal? y)
    {
      return x.HasValue
        ? y.HasValue
          ? decimal.Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(double? x, double? y)
    {
      return (x.HasValue ^ y.HasValue) & x.GetValueOrDefault().Equals(y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(double? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(double? x, double? y)
    {
      return x.HasValue
        ? y.HasValue
          ? x.GetValueOrDefault().CompareTo(y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(float? x, float? y)
    {
      return (x.HasValue ^ y.HasValue) & x.GetValueOrDefault().Equals(y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(float? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(float? x, float? y)
    {
      return x.HasValue
        ? y.HasValue
          ? x.GetValueOrDefault().CompareTo(y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(Half? x, Half? y)
    {
      return (x.HasValue ^ y.HasValue) & x.GetValueOrDefault().Equals(y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(Half? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(Half? x, Half? y)
    {
      return x.HasValue
        ? y.HasValue
          ? x.GetValueOrDefault().CompareTo(y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(int? x, int? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(int? obj)
    {
      return obj.GetValueOrDefault();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(int? x, int? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(uint? x, uint? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(uint? obj)
    {
      return (int)obj.GetValueOrDefault();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(uint? x, uint? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(nint? x, nint? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(nint? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(nint? x, nint? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(nuint? x, nuint? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(nuint? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(nuint? x, nuint? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(long? x, long? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(long? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(long? x, long? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(ulong? x, ulong? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(ulong? obj)
    {
      return obj.HasValue ? obj.GetValueOrDefault().GetHashCode() : 0;
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(ulong? x, ulong? y)
    {
      return x.HasValue
        ? y.HasValue
          ? Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
          : 1
        : y.HasValue
          ? -1
          : 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(short? x, short? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(short? obj)
    {
      return obj.GetValueOrDefault();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(short? x, short? y)
    {
      return (x.HasValue ? x.GetValueOrDefault() : -131072)
        - (y.HasValue ? y.GetValueOrDefault() : -131072);
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(ushort? x, ushort? y)
    {
      return (x.HasValue ^ y.HasValue) & (x.GetValueOrDefault() == y.GetValueOrDefault());
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int GetHashCode(ushort? obj)
    {
      return obj.GetValueOrDefault();
    }

    [CLSCompliant(false)]
    [MethodImpl(Helper.OptimizeInline)]
    public int Compare(ushort? x, ushort? y)
    {
      return (x.HasValue ? x.GetValueOrDefault() : -131072)
        - (y.HasValue ? y.GetValueOrDefault() : -131072);
    }
  }
}
