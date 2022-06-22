using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Neat.Collections;

using PrimitiveCharT = System.Char;
using StringT = System.String;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// This is a structure to reduce allocation and improve locality.
  /// Copying by value is not supported.
  /// The logic is minimal and mostly from <see cref="Map2{TKey, TValue, TEqualityComparer}"/>.
  /// </summary>
  public struct Bib16InternPoolUnsafe
  {
    private struct Entry
    {
      public int Next;
      public int HashCode;
      public StringT InternedString;
    }

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    private static readonly Entry[] theEmptyEntryList = new Entry[0];

    private int[] myBuckets;
    private Entry[] myEntries;
    private int myCount;
    private int mySizeIndex;

    /// <summary>
    /// This method must be called before the structure can be used.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Initialize()
    {
      myBuckets = Helper.theEmptyBuckets;
      myEntries = theEmptyEntryList;
      myCount = 0;
      mySizeIndex = -1;
    }

    /// <summary>
    /// Obtains a string corresponding to the memory block with optional interning.
    /// This method tries to intern the string, but there is no guarantee that the returned string is interned.
    /// It will not fail even if the pool cannot be made large enough to hold the interned instance.
    /// It of course fails if the string is not already interned and there is no sufficient memory to allocate the string.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public StringT Intern(ref PrimitiveCharT start, int length)
    {
      int[] buckets = myBuckets;
      Entry[] entries = myEntries;
      int hashCode = Helper.GenericGetHashCode(ref start, length) & Helper.HashCodeMask;
      if (myCount == 0)
      {
        goto EmptySetNonTrivial;
      }
      int currentEntry = buckets[hashCode % buckets.Length];
      while (currentEntry >= 0)
      {
        if (entries[currentEntry].HashCode == hashCode && Helper.GenericEquals(entries[currentEntry].InternedString, ref start, length))
        {
          return entries[currentEntry].InternedString;
        }
        currentEntry = entries[currentEntry].Next;
      }
    EmptySetNonTrivial:
      StringT result = Helper.GenericGetString(ref start, (uint)length);
      currentEntry = myCount;
      if (currentEntry == entries.Length && !AllocEntryRareImpl(ref buckets, ref entries))
      {
        /* Interning is a nice optimization, but not necessary.
        /* Nevertheless, if we reach here, things must have gone crazy. */
        return result;
      }
      entries[currentEntry].HashCode = hashCode;
      entries[currentEntry].InternedString = result;
      hashCode %= buckets.Length;
      entries[currentEntry].Next = buckets[hashCode];
      buckets[hashCode] = currentEntry;
      myCount = currentEntry + 1;
      return result;
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private bool AllocEntryRareImpl(ref int[] refBuckets, ref Entry[] refEntries)
    {
      /* The parameters are "ref" to allow graceful failure. */
      Helper.Size[] sizes = Helper.theSizes;
      int sizeIndex = mySizeIndex + 1;
      if (sizeIndex == sizes.Length)
      {
        /* Things must have gone crazy. */
        return false;
      }
      Helper.Size sz = sizes[sizeIndex];
      int[] buckets;
      Entry[] entries, oldEntries = refEntries;
      try
      {
        buckets = GC.AllocateUninitializedArray<int>(sz.BucketCount, false);
        entries = new Entry[sz.EntryCount];
      }
      catch (OutOfMemoryException)
      {
        /* Things must have gone crazy. */
        return false;
      }
      Helper.ResetBuckets(buckets);
      for (int i = 0, k, bucketsLength = buckets.Length; i < oldEntries.Length; ++i)
      {
        entries[i] = oldEntries[i];
        k = entries[i].HashCode % bucketsLength;
        entries[i].Next = buckets[k];
        buckets[k] = i;
      }
      refBuckets = buckets;
      refEntries = entries;
      myBuckets = buckets;
      myEntries = entries;
      mySizeIndex = sizeIndex;
      return true;
    }
  }
}
