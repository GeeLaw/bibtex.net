using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// <see cref="String8"/> is binary-compatbile with a reference to an array of <see cref="Char8"/>.
  /// Represents a string encoded in UTF-8, which is not necessarily valid.
  /// Similar to <see langword="string"/>, the underlying memory of any <see cref="String8"/> must be immutable.
  /// Every public method of this type should only dereference <see langword="this"/> once,
  /// a trick used by <see cref="ImmutableArray{T}"/> (though this trick could be coincidence-oriented
  /// programming; see <a href="https://github.com/dotnet/docs/issues/29696">dotnet/docs#29696</a>.)
  /// </summary>
  [DebuggerDisplay("{ToString(),nq}")]
  [StructLayout(LayoutKind.Explicit)]
  public readonly struct String8
    : IComparable<String8>, IComparable, IEquatable<String8>,
      IEnumerable2<Char8, String8.Enumerator>, IReadOnlyList<Char8>
  {
    [FieldOffset(0)]
    internal readonly Char8[] myData;

    /// <summary>
    /// Initializes a new instance of <see cref="String8"/>.
    /// </summary>
    /// <param name="immutableData">This array must not escape to the user.</param>
    [MethodImpl(Helper.OptimizeInline)]
    internal String8(Char8[] immutableData)
    {
      myData = immutableData;
    }

    /// <summary>
    /// Gets an instance of <see cref="String8"/> of <see cref="Length"/> zero.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static readonly String8 Empty = new String8(Utf.theEmptyChar8s);

    /// <summary>
    /// Gets whether the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public bool IsDefault
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData is null;
      }
    }

    /// <summary>
    /// Gets whether the instance is <see langword="default"/> (the <see langword="null"/> wrapper) or empty (of <see cref="Length"/> zero).
    /// </summary>
    public bool IsDefaultOrEmpty
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        Char8[] data = myData;
        return data is null || data.Length == 0;
      }
    }

    /// <summary>
    /// Gets the number of UTF-8 bytes.
    /// This property cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    public int Length
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    /// <summary>
    /// Gets the UTF-8 byte at the specified index.
    /// This indexer cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Char8 this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[index];
      }
    }

    /// <summary>
    /// Gets the number of UTF-8 bytes.
    /// This property cannot be read if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int IReadOnlyCollection<Char8>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    #region Compare, order operators, IComparable<String8> members, IComparable members

    [SuppressMessage("Style", "IDE0004", Justification = "Avoid mistaking it for mod-256 subtraction.")]
    [MethodImpl(Helper.JustOptimize)]
    private static int CompareImpl(Char8[] x, Char8[] y)
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
      ref byte x0 = ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(x));
      ref byte y0 = ref Unsafe.As<Char8, byte>(ref MemoryMarshal.GetArrayDataReference(y));
      for (int i = 0, length = (xlength < ylength ? xlength : ylength), result; i != length; ++i)
      {
        result = (int)Unsafe.Add(ref x0, i) - (int)Unsafe.Add(ref y0, i);
        if (result != 0)
        {
          return result;
        }
      }
      return xlength - ylength;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static int Compare(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <=(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) <= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >=(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator <(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) < 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator >(String8 x, String8 y)
    {
      return CompareImpl(x.myData, y.myData) > 0;
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public int CompareTo(String8 other)
    {
      return CompareImpl(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    int IComparable.CompareTo(object obj)
    {
      return obj is null
        ? 1
        : obj is String8 other
        ? CompareImpl(myData, other.myData)
        : throw new ArgumentException("The argument '" + nameof(obj) + "' must be Neat.Unicode.String8 or null.", nameof(obj));
    }

    #endregion Compare, order operators, IComparable<String8> members, IComparable members

    #region Equals, equality operators, IEquatable<String8> members, object members

    [MethodImpl(Helper.JustOptimize)]
    private static bool EqualsImpl(Char8[] x, Char8[] y)
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
      ref int x0 = ref Unsafe.As<Char8, int>(ref MemoryMarshal.GetArrayDataReference(x));
      ref int y0 = ref Unsafe.As<Char8, int>(ref MemoryMarshal.GetArrayDataReference(y));
      int mask = (length & 3);
      length >>= 2;
      for (int i = 0; i != length; i++)
      {
        if (Unsafe.Add(ref x0, i) != Unsafe.Add(ref y0, i))
        {
          return false;
        }
      }
      switch (mask)
      {
      case 1:
        mask = Helper.TrailingMask1;
        break;
      case 2:
        mask = Helper.TrailingMask2;
        break;
      case 3:
        mask = Helper.TrailingMask3;
        break;
      default:
        return true;
      }
      /* We take a leap of faith to assume that it is safe to read past the end
      /* on the belief that array data have alignment and packing of 4 bytes.
      /* We do NOT read past the end if the valid data are exactly a multiple of 4 bytes. */
      return ((Unsafe.Add(ref x0, length) ^ Unsafe.Add(ref y0, length)) & mask) == 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool Equals(String8 x, String8 y)
    {
      return EqualsImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(String8 x, String8 y)
    {
      return EqualsImpl(x.myData, y.myData);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(String8 x, String8 y)
    {
      return !EqualsImpl(x.myData, y.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(String8 other)
    {
      return EqualsImpl(myData, other.myData);
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is String8 other) && EqualsImpl(myData, other.myData);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static int GetHashCodeImpl(Char8[] x)
    {
      if (x is null)
      {
        return 0;
      }
      int hash = Helper.FnvOffsetBasis;
      for (int i = 0; i < x.Length; ++i)
      {
        hash = (hash ^ x[i].Value) * Helper.FnvPrime;
      }
      return hash ^ x.Length;
    }

    /// <summary>
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return GetHashCodeImpl(myData);
    }

    /// <summary>
    /// Equivalent to <see cref="Utf.String8ToString16Replace(String8)"/>.
    /// This method can be called even if the instance is <see langword="default"/> (the <see langword="null"/> wrapper),
    /// in which case it returns <see langword="null"/>.
    /// </summary>
    public override string ToString()
    {
      return Utf.String8ToString16Replace(this);
    }

    #endregion Equals, equality operators, IEquatable<String8> members, object members

    /// <summary>
    /// Equivalent to <see cref="Utf.SanitizeString8(String8)"/>.
    /// This method can be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public String8 Sanitize()
    {
      return Utf.SanitizeString8(this);
    }

    #region Create, GetPinnableReference, AsSpan, AsMemory

    /// <summary>
    /// Creates a new <see cref="String8"/> and initializes it with the specified delegate.
    /// Similar to <see cref="string.Create{TState}(int, TState, SpanAction{char, TState})"/>.
    /// </summary>
    [SuppressMessage("Style", "IDE0004", Justification = "Make promotion of int explicit.")]
    [MethodImpl(Helper.JustOptimize)]
    public static String8 Create<TState>(int length, TState state, SpanAction<Char8, TState> action)
    {
      if ((uint)length > (uint)Utf.MaximumLength8)
      {
        throw new ArgumentOutOfRangeException(nameof(length));
      }
      Char8[] data = new Char8[length];
      action(new Span<Char8>(data), state);
      return new String8(data);
    }

    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(Helper.OptimizeInline)]
    public readonly ref Char8 GetPinnableReference()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return ref MemoryMarshal.GetArrayDataReference(data);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlySpan<Char8> AsSpan()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new ReadOnlySpan<Char8>(data);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlySpan<Char8> AsSpan(int start)
    {
      Char8[] data = myData;
      /* See String32.AsSpan(int) on why this throws the desired exception. */
      return new ReadOnlySpan<Char8>(data, start, data.Length - start);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlySpan<Char8> AsSpan(int start, int length)
    {
      Char8[] data = myData;
      /* See String32.AsSpan(int, int) on why this throws the desired exception. */
      int throwIfNull = data.Length;
      return new ReadOnlySpan<Char8>(data, start, length);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlyMemory<Char8> AsMemory()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new ReadOnlyMemory<Char8>(data);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlyMemory<Char8> AsMemory(int start)
    {
      Char8[] data = myData;
      /* See String32.AsSpan(int) on why this throws the desired exception. */
      return new ReadOnlyMemory<Char8>(data, start, data.Length - start);
    }

    /// <summary>
    /// This method cannot be called on the <see langword="null"/> wrapper.
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlyMemory<Char8> AsMemory(int start, int length)
    {
      Char8[] data = myData;
      /* See String32.AsSpan(int, int) on why this throws the desired exception. */
      int throwIfNull = data.Length;
      return new ReadOnlyMemory<Char8>(data, start, length);
    }

    #endregion Create, GetPinnableReference, AsSpan, AsMemory

    /// <summary>
    /// Enumerates <see cref="Char8"/> instances in a <see cref="String8"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public struct Enumerator : IEnumerator2<Char8>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private Char8[] myData;

      private int myIndex;

#if STRING8_ENUMERATOR_DISPOSE

      private bool myNotDisposed;

      [DoesNotReturn]
      [MethodImpl(Helper.OptimizeNoInline)]
      private static void ThrowDisposed()
      {
        throw new ObjectDisposedException(typeof(Enumerator).FullName, "The enumerator is already disposed. (This check is enabled by STRING8_ENUMERATOR_DISPOSE.)");
      }

#endif

      private string DebuggerDisplay()
      {
        return "Index = " + myIndex.ToString(CultureInfo.InvariantCulture)
#if STRING8_ENUMERATOR_DISPOSE
          + (myNotDisposed ? "" : " <disposed>")
#endif
          ;
      }

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(Char8[] data)
      {
        myData = data;
        myIndex = -1;
#if STRING8_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if STRING8_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext(out Char8 item)
      {
#if STRING8_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        Char8[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
#if STRING8_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = default(Char8);
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if STRING8_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        Char8[] data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
#if STRING8_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = null;
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext()
      {
#if STRING8_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        return ++myIndex < myData.Length;
      }

      public Char8 Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if STRING8_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            ThrowDisposed();
          }
#endif
          return myData[myIndex];
        }
      }

      /// <summary>
      /// This property is hidden in the debugging view to work around
      /// <a href="https://developercommunity.visualstudio.com/t/Inspecting-a-property-returning-a-field/10056308">this bug of Visual Studio</a>.
      /// </summary>
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      object IEnumerator.Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if STRING8_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            ThrowDisposed();
          }
#endif
          return myData[myIndex];
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if STRING8_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    #region GetEnumerator

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2<Char8> IEnumerable2<Char8>.GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator<Char8> IEnumerable<Char8>.GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> wrapper).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator IEnumerable.GetEnumerator()
    {
      Char8[] data = myData;
      int throwIfNull = data.Length;
      return new Enumerator(data);
    }

    #endregion GetEnumerator

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see cref="String8"/>.
    /// The comparison is ordinal.
    /// </summary>
    public struct Comparer : IComparer<String8>, IEqualityComparer2<String8>
    {
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(String8 x, String8 y)
      {
        return CompareImpl(x.myData, y.myData);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(String8 x, String8 y)
      {
        return EqualsImpl(x.myData, y.myData);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(String8 obj)
      {
        return GetHashCodeImpl(obj.myData);
      }
    }
  }
}
