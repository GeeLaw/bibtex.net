using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Neat.Collections;

namespace Neat.Unicode
{
  /// <summary>
  /// Provides extension methods for <see langword="string"/>.
  /// </summary>
  public static class String16
  {
    /// <summary>
    /// Gets whether the instance is the <see langword="null"/> reference.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsDefault(this string string16)
    {
      return string16 is null;
    }

    /// <summary>
    /// Gets whether the instance is the <see langword="null"/> reference or empty (of <see cref="string.Length"/> zero).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public static bool IsDefaultOrEmpty(this string string16)
    {
      return string16 is null || string16.Length == 0;
    }

    /// <summary>
    /// Equivalent to <see cref="Utf.SanitizeString16(string)"/>.
    /// This method can be called on the <see langword="null"/> reference.
    /// </summary>
    public static string Sanitize(this string string16)
    {
      return Utf.SanitizeString16(string16);
    }

    /// <summary>
    /// Enumerates <see langword="char"/> instances in <see langword="string"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public struct Enumerator : IEnumerator2<char>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private string myData;

      private int myIndex;

#if STRING16_ENUMERATOR_DISPOSE

      private bool myNotDisposed;

      [DoesNotReturn]
      [MethodImpl(Helper.OptimizeNoInline)]
      private static void ThrowDisposed()
      {
        throw new ObjectDisposedException(typeof(Enumerator).FullName, "The enumerator is already disposed. (This check is enabled by STRING16_ENUMERATOR_DISPOSE.)");
      }

#endif

      private string DebuggerDisplay()
      {
        return "Index = " + myIndex.ToString(CultureInfo.InvariantCulture)
#if STRING16_ENUMERATOR_DISPOSE
          + (myNotDisposed ? "" : " <disposed>")
#endif
          ;
      }

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(string data)
      {
        myData = data;
        myIndex = -1;
#if STRING16_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if STRING16_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext(out char item)
      {
#if STRING16_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        string data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
#if STRING16_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = default(char);
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if STRING16_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        string data = myData;
        int index = myIndex;
        myIndex = ++index;
        if (index < data.Length)
        {
          item = data[index];
          return true;
        }
#if STRING16_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = null;
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext()
      {
#if STRING16_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          ThrowDisposed();
        }
#endif
        return ++myIndex < myData.Length;
      }

      public char Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if STRING16_ENUMERATOR_DISPOSE
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
      /// (Although the program will not crash as <see langword="char"/> is a primitive type recoginized by Visual Studio,
      /// this property is still hidden for uniformity with <see cref="String8.Enumerator"/> and <see cref="String32.Enumerator"/>.)
      /// </summary>
      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      object IEnumerator.Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if STRING16_ENUMERATOR_DISPOSE
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
#if STRING16_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    /// <summary>
    /// This method cannot be called if the instance is <see langword="default"/> (the <see langword="null"/> reference).
    /// </summary>
    [SuppressMessage("Style", "IDE0059", Justification = "Avoid discarding with '_'.")]
    [MethodImpl(Helper.OptimizeInline)]
    public static Enumerator GetEnumerator2(this string string16)
    {
      int throwIfNull = string16.Length;
      return new Enumerator(string16);
    }

    /// <summary>
    /// Standard implementation of <see cref="IComparer{T}"/> and <see cref="IEqualityComparer2{T}"/> for <see langword="string"/>.
    /// The comparison is ordinal.
    /// </summary>
    public struct Comparer : IComparer<string>, IEqualityComparer2<string>
    {
      [MethodImpl(Helper.OptimizeInline)]
      public int Compare(string x, string y)
      {
        return string.CompareOrdinal(x, y);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool Equals(string x, string y)
      {
        return string.Equals(x, y);
      }

      [MethodImpl(Helper.OptimizeInline)]
      public int GetHashCode(string obj)
      {
        return obj is null ? 0 : obj.GetHashCode();
      }
    }
  }
}
