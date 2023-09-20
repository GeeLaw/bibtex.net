using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if LIST2_SYNCROOT
using System.Threading;
#endif

namespace Neat.Collections
{
  /// <summary>
  /// A dynamically growing array.
  /// All static members of this class are thread-safe.
  /// No instance member of this class is thread-safe unless explicitly stated otherwise.
  /// Any member that could mutate the list will invalidate all existing enumeration operations, even if no actual change is made
  /// (e.g., setting the capacity to its current value will invalidate all existing enumeration operations).
  /// </summary>
  [DebuggerTypeProxy(typeof(List2<>.DebuggerView))]
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public sealed class List2<T> : IEnumerable2<T, List2<T>.Enumerator>, IReadOnlyList<T>, IList<T>, IList
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return "Count = " + myCount.ToString(CultureInfo.InvariantCulture)
#if LIST2_ENUMERATION_VERSION
          + ", Version = " + myVersion.ToString(CultureInfo.InvariantCulture)
#endif
          ;
      }
    }

    private sealed class DebuggerView
    {
      private readonly List2<T> myTarget;

      public DebuggerView(List2<T> target)
      {
        myTarget = target;
      }

      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public T[] Items
      {
        get
        {
          return myTarget.ToArray();
        }
      }
    }

    private T[] myData;
    private int myCount;

#if LIST2_SYNCROOT
    private object mySyncRoot;
#endif

#if LIST2_ENUMERATION_VERSION
    private uint myVersion;
#endif

    [SuppressMessage("Performance", "CA1825", Justification = "Avoid excessive generic instantiations.")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly T[] theEmptyArray = new T[0];

    /// <summary>
    /// The minimum non-zero capacity favored by <see cref="List2{T}"/>.
    /// This value is <see cref="List2.StartingCapacity"/>.
    /// </summary>
    public const int StartingCapacity = List2.StartingCapacity;

    /// <summary>
    /// The maximum capacity of a list of <typeparamref name="T"/>.
    /// This value is <see cref="List2.MaximumCapacityOneByte"/>
    /// for <see langword="bool"/>, <see langword="byte"/>, <see langword="sbyte"/>, and other one-byte structures.
    /// For all other types, this value is <see cref="List2.MaximumCapacityOther"/>.
    /// </summary>
    public static readonly int MaximumCapacity;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly List2.IndexOf<T, T> theFirstOfGeneric;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly List2.IndexOf<T, object> theFirstOfObject;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private static readonly List2.IndexOf<T, T> theLastOfGeneric;

    static List2()
    {
      MaximumCapacity = (!RuntimeHelpers.IsReferenceOrContainsReferences<T>() && Unsafe.SizeOf<T>() == 1
        ? List2.MaximumCapacityOneByte
        : List2.MaximumCapacityOther);
      List2.GetIndexOfDelegates(out theFirstOfGeneric, out theFirstOfObject, out theLastOfGeneric);
    }

    #region public constructors

    /// <summary>
    /// Initializes a new, empty list.
    /// </summary>
    public List2()
    {
      myData = theEmptyArray;
      myCount = 0;
#if LIST2_SYNCROOT
      mySyncRoot = null;
#endif
#if LIST2_ENUMERATION_VERSION
      myVersion = 0;
#endif
    }

    /// <summary>
    /// Initializes a new, empty list whose <see cref="Capacity"/> is exactly <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">This value must be non-negative and not exceed <see cref="MaximumCapacity"/>.</param>
    public List2(int capacity)
    {
      if ((uint)capacity > (uint)MaximumCapacity)
      {
        List2.ThrowCapacity();
      }
      myData = (capacity == 0 ? theEmptyArray : new T[capacity]);
      myCount = 0;
#if LIST2_SYNCROOT
      mySyncRoot = null;
#endif
#if LIST2_ENUMERATION_VERSION
      myVersion = 0;
#endif
    }

    #endregion public constructors

    /// <summary>
    /// This constructor does not validate its arguments.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    private List2(T[] data, int count)
    {
      myData = data;
      myCount = count;
#if LIST2_SYNCROOT
      mySyncRoot = null;
#endif
#if LIST2_ENUMERATION_VERSION
      myVersion = 0;
#endif
    }

    #region GetPinnableReference, AsSpanBeforeNextMutation

    /// <summary>
    /// Gets a read-only reference to the first item of this list.
    /// The result of this method is invalidated as soon as any enumeration-invalidating member of <see cref="List2{T}"/> is invoked.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(Helper.OptimizeInline)]
    public ref readonly T GetPinnableReference()
    {
      return ref MemoryMarshal.GetArrayDataReference(myData);
    }

    /// <summary>
    /// The result of this method is invalidated as soon as any enumeration-invalidating member of <see cref="List2{T}"/> is invoked.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlySpan<T> AsSpanBeforeNextMutation()
    {
      return new ReadOnlySpan<T>(myData, 0, myCount);
    }

    /// <summary>
    /// The result of this method is invalidated as soon as any enumeration-invalidating member of <see cref="List2{T}"/> is invoked.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public ReadOnlySpan<T> AsSpanBeforeNextMutation(int start, int length)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      return new ReadOnlySpan<T>(data, start, length);
    }

    #endregion GetPinnableReference, AsSpanBeforeNextMutation

    #region Count, IReadOnlyCollection<T>.Count, ICollection<T>.Count, ICollection.Count

    /// <summary>
    /// Gets the number of items in this list.
    /// </summary>
    public int Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int IReadOnlyCollection<T>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int ICollection<T>.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int ICollection.Count
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myCount;
      }
    }

    #endregion Count, IReadOnlyCollection<T>.Count, ICollection<T>.Count, ICollection.Count

    #region this[int index], IReadOnlyList<T>.this[int index], IList<T>.this[int index], IList.this[int index]

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    public T this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
#if LIST2_ENUMERATION_VERSION
        ++myVersion;
#endif
        myData[(uint)index < (uint)myCount ? index : -1] = value;
      }
    }

    T IReadOnlyList<T>.this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
    }

    T IList<T>.this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
#if LIST2_ENUMERATION_VERSION
        ++myVersion;
#endif
        myData[(uint)index < (uint)myCount ? index : -1] = value;
      }
    }

    object IList.this[int index]
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData[(uint)index < (uint)myCount ? index : -1];
      }
      [MethodImpl(Helper.OptimizeInline)]
      set
      {
#if LIST2_ENUMERATION_VERSION
        ++myVersion;
#endif
        myData[(uint)index < (uint)myCount ? index : -1] = (T)value;
      }
    }

    #endregion this[int index], IReadOnlyList<T>.this[int index], IList<T>.this[int index], IList.this[int index]

    #region Reverse

    /// <summary>
    /// Reverses the list.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Reverse()
    {
      Array.Reverse(myData, 0, myCount);
    }

    /// <summary>
    /// Reverses the specified range of the list.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void Reverse(int start, int length)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      Array.Reverse(data, start, length);
    }

    #endregion Reverse

    /// <summary>
    /// This method does not validate its arguments.
    /// </summary>
    /// <param name="least">This number should be positive and not exceed <see cref="MaximumCapacity"/>.</param>
    /// <param name="suggested">This number should be greater than or equal to <paramref name="least"/> and not exceed <see cref="MaximumCapacity"/>.</param>
    [MethodImpl(Helper.JustOptimize)]
    private static T[] AllocImpl(int least, int suggested)
    {
    Retry:
      try
      {
        return new T[suggested];
      }
      catch (OutOfMemoryException)
      {
        if (suggested != least)
        {
          suggested = least + (suggested - least) / 2;
          goto Retry;
        }
        throw;
      }
    }

    #region Capacity, SetCapacity, EnsureCapacity, TrimExcess

    /// <summary>
    /// Gets the capacity of the list.
    /// </summary>
    public int Capacity
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return myData.Length;
      }
    }

    /// <summary>
    /// Sets the capacity of the list.
    /// </summary>
    /// <param name="capacity">The new capacity.
    /// This value must be at least <see cref="Count"/> and not exceed <see cref="MaximumCapacity"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="capacity"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public void SetCapacity(int capacity)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (capacity < count || capacity > MaximumCapacity)
      {
        List2.ThrowCapacity();
      }
      if (capacity == 0)
      {
        myData = theEmptyArray;
        return;
      }
      if (capacity != data.Length)
      {
        T[] newData = new T[capacity];
        Array.ConstrainedCopy(data, 0, newData, 0, count);
        /* No more exception is possible beyond this point. */
        myData = newData;
      }
    }

    /// <summary>
    /// Ensures the capacity is at least the specified value and amortizes the cost of growth.
    /// </summary>
    /// <param name="capacity">The minimum value of the new capacity.
    /// This value can be less than <see cref="Count"/> (including being negative),
    /// but must not exceed <see cref="MaximumCapacity"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="capacity"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public void EnsureCapacity(int capacity)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (capacity > MaximumCapacity)
      {
        List2.ThrowCapacity();
      }
      if (capacity > data.Length)
      {
        int suggested = (count > MaximumCapacity / 2
          ? MaximumCapacity
          : count <= StartingCapacity / 2
          ? StartingCapacity
          : count * 2);
        suggested = (suggested < capacity ? capacity : suggested);
        T[] newData = AllocImpl(capacity, suggested);
        Array.ConstrainedCopy(data, 0, newData, 0, count);
        /* No more exception is possible beyond this point. */
        myData = newData;
      }
    }

    /// <summary>
    /// Sets the capacity to <see cref="Count"/> opportunistically.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void TrimExcess()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      /* The non-inlining tries to protect us from read introduction.
      /* See https://github.com/dotnet/docs/issues/29696 */
      TrimExcessImpl(this, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static void TrimExcessImpl(List2<T> that, T[] data, int count)
    {
      if (count == 0)
      {
        that.myData = theEmptyArray;
        return;
      }
      if (count < (int)(data.Length * 0.9))
      {
        T[] newData;
        try
        {
          /* We are going to overwrite every element before the newly allocated array
          /* becomes available to the user, so it is fine to allocate it uninitialized. */
          newData = GC.AllocateUninitializedArray<T>(count, false);
        }
        catch (OutOfMemoryException)
        {
          /* There is no guaratee that the list will shrink
          /* and a no-op is acceptable, although arguably
          /* more out-of-memory issues are arriving soon. */
          return;
        }
        Array.ConstrainedCopy(data, 0, newData, 0, count);
        /* No more exception is possible beyond this point. */
        that.myData = newData;
      }
    }

    #endregion Capacity, SetCapacity, EnsureCapacity, TrimExcess

    #region CopyTo, ICollection<T>.CopyTo, ICollection.CopyTo

    /// <summary>
    /// Except for allocation efficiency,
    /// the effect is equivalent to calling <see cref="Array.ConstrainedCopy(Array, int, Array, int, int)"/>
    /// (see that method for possible exceptions) with the following arguments:
    /// <see cref="ToArray"/>, <c>0</c>, <paramref name="array"/>, <paramref name="arrayIndex"/>, <see cref="Count"/>.
    /// This means <paramref name="array"/> cannot be a covariant reference.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.ConstrainedCopy(myData, 0, array, arrayIndex, myCount);
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      Array.ConstrainedCopy(myData, 0, array, arrayIndex, myCount);
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection.CopyTo(Array array, int index)
    {
      Array.ConstrainedCopy(myData, 0, array, index, myCount);
    }

    #endregion CopyTo, ICollection<T>.CopyTo, ICollection.CopyTo

    #region ToArray, GetRange

    /// <summary>
    /// Gets an array of length <see cref="Count"/> containing the items in this list.
    /// This array is guaranteed to be newly allocated if <see cref="Count"/> is positive.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public T[] ToArray()
    {
      return ToArrayImpl(myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static T[] ToArrayImpl(T[] data, int count)
    {
      if (count == 0)
      {
        return theEmptyArray;
      }
      /* We will copy to every item in this array before it is made available to the user,
      /* therefore, it is fine to allocate it uninitialized. */
      T[] results = GC.AllocateUninitializedArray<T>(count, false);
      Array.ConstrainedCopy(data, 0, results, 0, count);
      return results;
    }

    /// <summary>
    /// Gets an array of length <see cref="Count"/> containing the items in the specified range of this list.
    /// This array is guaranteed to be newly allocated if <paramref name="length"/> is positive.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public T[] ToArray(int start, int length)
    {
      /* Why do we put myData as the first argument but myCount the last?
      /* To reduce the need to shuffle arguments in case ToArray itself is not inlined. */
      return ToArrayImpl(myData, start, length, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static T[] ToArrayImpl(T[] data, int start, int length, int count)
    {
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      if (length == 0)
      {
        return theEmptyArray;
      }
      /* We will copy to every item in this array before it is made available to the user,
      /* therefore, it is fine to allocate it uninitialized. */
      T[] results = GC.AllocateUninitializedArray<T>(length, false);
      Array.ConstrainedCopy(data, start, results, 0, length);
      return results;
    }

    /// <summary>
    /// Creates a newly allocated list containing the items in the specified range of this list.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public List2<T> GetRange(int start, int length)
    {
      return GetRangeImpl(myData, start, length, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static List2<T> GetRangeImpl(T[] data, int start, int length, int count)
    {
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      if (length == 0)
      {
        return new List2<T>(theEmptyArray, 0);
      }
      /* We will copy to every item in this array before it is made available to the user,
      /* therefore, it is fine to allocate it uninitialized. */
      T[] results = GC.AllocateUninitializedArray<T>(length, false);
      Array.ConstrainedCopy(data, start, results, 0, length);
      return new List2<T>(results, length);
    }

    #endregion ToArray, GetRange

    #region Clear, ICollection<T>.Clear, IList.Clear

    /// <summary>
    /// Clears the list.
    /// This method does not perform any allocation.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void Clear()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(myData, 0, myCount);
      }
      myCount = 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<T>.Clear()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(myData, 0, myCount);
      }
      myCount = 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList.Clear()
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(myData, 0, myCount);
      }
      myCount = 0;
    }

    #endregion Clear, ICollection<T>.Clear, IList.Clear

    [MethodImpl(Helper.OptimizeNoInline)]
    private void AddRareImpl(T item)
    {
      T[] data = myData;
      int count = myCount;
      int least = count + 1;
      if (least <= data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      if (least > MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, count);
      newData[count] = item;
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private int IListAddRareImpl(T item)
    {
      T[] data = myData;
      int count = myCount;
      int least = count + 1;
      if (least <= data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = least;
        return count;
      }
      if (least > MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, count);
      newData[count] = item;
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
      return count;
    }

    #region Add, ICollection<T>.Add, IList.Add

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void Add(T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
      }
      else
      {
        AddRareImpl(item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void ICollection<T>.Add(T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
      }
      else
      {
        AddRareImpl(item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IList.Add(object value)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T item = (T)value;
      T[] data = myData;
      int count = myCount;
      if (count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
        return count;
      }
      else
      {
        return IListAddRareImpl(item);
      }
    }

    #endregion Add, ICollection<T>.Add, IList.Add

    /// <summary>
    /// This method validates its arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private void AddRangeImpl(T[] source, int start, int length, int sourceCount)
    {
      if ((uint)start > (uint)sourceCount)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(sourceCount - start))
      {
        List2.ThrowLength();
      }
      if (length == 0)
      {
        return;
      }
      T[] data = myData;
      int count = myCount;
      int least = count + length;
      /* This comparison must be unsigned because "least = count + length" might have overflown. */
      if ((uint)least > (uint)MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      if (least <= data.Length)
      {
        Array.ConstrainedCopy(source, start, data, count, length);
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      suggested = (suggested < least ? least : suggested);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, count);
      Array.ConstrainedCopy(source, start, newData, count, length);
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
    }

    #region AddRange

    /// <summary>
    /// Adds an array of items to the end of this list.
    /// </summary>
    /// <exception cref="NullReferenceException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(T[] array)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      int count = array.Length;
      AddRangeImpl(array, 0, count, count);
    }

    /// <summary>
    /// Adds the items in the specified range of the array to the end of this list.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed the length of <paramref name="array"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the length of <paramref name="array"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(T[] array, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      AddRangeImpl(array, start, length, array.Length);
    }

    /// <summary>
    /// Adds a list of items to the end of this list.
    /// </summary>
    /// <param name="list">This argument can be the list being appended.</param>
    /// <exception cref="NullReferenceException">If <paramref name="list"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(List2<T> list)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = list.myData;
      int count = list.myCount;
      AddRangeImpl(data, 0, count, count);
    }

    /// <summary>
    /// Adds the items in the specified range of the a list to the end of this list.
    /// </summary>
    /// <param name="list">This argument can be the list being appended.</param>
    /// <param name="start">This value must be non-negative and not exceed the <see cref="Count"/> of <paramref name="list"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the <see cref="Count"/> of <paramref name="list"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="list"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void AddRange(List2<T> list, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      AddRangeImpl(list.myData, start, length, list.myCount);
    }

    #endregion AddRange

    [MethodImpl(Helper.OptimizeNoInline)]
    private void InsertRareImpl(int index, T item)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)index > (uint)count)
      {
        List2.ThrowIndex();
      }
      int least = count + 1;
      if (least <= data.Length)
      {
        Array.ConstrainedCopy(data, index, data, index + 1, count - index);
        data[index] = item;
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      if (least > MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      int suggested = (count > MaximumCapacity / 2
        ? MaximumCapacity
        : count <= StartingCapacity / 2
        ? StartingCapacity
        : count * 2);
      T[] newData = AllocImpl(least, suggested);
      Array.ConstrainedCopy(data, 0, newData, 0, index);
      newData[index] = item;
      Array.ConstrainedCopy(data, index, newData, index + 1, count - index);
      /* No more exception is possible beyond this point. */
      myData = newData;
      myCount = least;
    }

    #region Insert, IList<T>.Insert, IList.Insert

    /// <summary>
    /// Inserts the specified item at the specified index.
    /// </summary>
    /// <param name="index">The new index of the inserted item.
    /// This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void Insert(int index, T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index == count && count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
      }
      else
      {
        InsertRareImpl(index, item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList<T>.Insert(int index, T item)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index == count && count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
      }
      else
      {
        InsertRareImpl(index, item);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList.Insert(int index, object value)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T item = (T)value;
      T[] data = myData;
      int count = myCount;
      if (index == count && count < data.Length)
      {
        data[count] = item;
        /* No more exception is possible beyond this point. */
        myCount = count + 1;
      }
      else
      {
        InsertRareImpl(index, item);
      }
    }

    #endregion Insert, IList<T>.Insert, IList.Insert

    /// <summary>
    /// This method validates its arguments.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    private void InsertRangeImpl(int index, T[] source, int start, int length, int sourceCount)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)index > (uint)count)
      {
        List2.ThrowIndex();
      }
      if ((uint)start > (uint)sourceCount)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(sourceCount - start))
      {
        List2.ThrowLength();
      }
      if (length == 0)
      {
        return;
      }
      int least = count + length;
      /* This comparison must be unsigned because "least = count + length" might have overflown. */
      if ((uint)least > (uint)MaximumCapacity)
      {
        List2.ThrowTooMany();
      }
      /* Case 1: Reallocate. It does matter if "source" is "data". */
      if (least > data.Length)
      {
        int suggested = (count > MaximumCapacity / 2
          ? MaximumCapacity
          : count <= StartingCapacity / 2
          ? StartingCapacity
          : count * 2);
        suggested = (suggested < least ? least : suggested);
        T[] newData = AllocImpl(least, suggested);
        Array.ConstrainedCopy(data, 0, newData, 0, index);
        Array.ConstrainedCopy(source, start, newData, index, length);
        Array.ConstrainedCopy(data, index, newData, index + length, count - index);
        /* No more exception is possible beyond this point. */
        myData = newData;
        myCount = least;
        return;
      }
      /* Case 2: "source" is not "data". */
      if (!ReferenceEquals(source, data))
      {
        Array.ConstrainedCopy(data, index, data, index + length, count - index);
        Array.ConstrainedCopy(source, start, data, index, length);
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      /* Below, 1234/pq/PQ/x represent other/source/inserted/uninitialized items.
      /* Case 3: 12pq34   => 12PQpq34    or    12pq34 => 12pqPQ34    (identical effect).
      /* Handle: 12pq34xx => 12pqPQ34
      /*           ^^^^          ^^^^ */
      if (index == start || index == start + length)
      {
        Array.ConstrainedCopy(data, start, data, start + length, count - start);
        /* No more exception is possible beyond this point. */
        myCount = least;
        return;
      }
      /* Case   4: 12pq34   => 1PQ2pq34
      /*   Handle: 12pq34xx => 12p2pq34    then    12p2pq34 => 1pq2pq34
      /*            ^^^^^         ^^^^^                ^^       ^^
      /* Case 5.1: 12pq34   => 12pq3PQ4
      /*   Handle: 12pq34xx => 12pq34x4    then    12pq34x4 => 12pq3PQ4
      /*                ^             ^              ^^             ^^
      /* Case 5.2: 12pq34   => 12pPQq34
      /*   Handle: 12pq34xx => 12pq3q34    then    12pq3q34 => 12pPQq34
      /*              ^^^           ^^^              ^^           ^^
      /* The first step of Case 4/5.1/5.2 is the same.
      /* The second step of Case 5.1/5.2 is the same. */
      Array.ConstrainedCopy(data, index, data, index + length, count - index);
      Array.ConstrainedCopy(data,
        index < start
        ? start + length /* Case 4 --- pq sits at the moved location */
        : start /* Case 5.1 and Case 5.2 --- pq sits at the original location */,
        data, index, length);
      /* No more exception is possible beyond this point. */
      myCount = least;
    }

    #region InsertRange

    /// <summary>
    /// Inserts an array at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, T[] source)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      int length = source.Length;
      InsertRangeImpl(index, source, 0, length, length);
    }

    /// <summary>
    /// Inserts the specified range of an array at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <param name="start">This value must be non-negative and not exceed the length of <paramref name="source"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the length of <paramref name="source"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/>, <paramref name="start"/>, or <paramref name="length"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, T[] source, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      InsertRangeImpl(index, source, start, length, source.Length);
    }

    /// <summary>
    /// Inserts a list at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <param name="source">It is allowed to insert a list into itself.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, List2<T> source)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = source.myData;
      int count = source.myCount;
      InsertRangeImpl(index, data, 0, count, count);
    }

    /// <summary>
    /// Inserts the specified range of a list at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and not exceed <see cref="Count"/>.
    /// This is the new index of the first newly inserted item.</param>
    /// <param name="source">It is allowed to insert a list into itself.</param>
    /// <param name="start">This value must be non-negative and not exceed the length of <paramref name="source"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed the length of <paramref name="source"/> minus <paramref name="start"/>.</param>
    /// <exception cref="NullReferenceException">If <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/>, <paramref name="start"/>, or <paramref name="length"/> is out of range.</exception>
    /// <exception cref="InvalidOperationException">If the number of items will be greater than <see cref="MaximumCapacity"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void InsertRange(int index, List2<T> source, int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      InsertRangeImpl(index, source.myData, start, length, source.myCount);
    }

    #endregion InsertRange

    [MethodImpl(Helper.OptimizeNoInline)]
    private void RemoveAtRareImpl(int index)
    {
      T[] data = myData;
      int count = myCount;
      if ((uint)index >= (uint)count)
      {
        List2.ThrowIndex();
      }
      Array.ConstrainedCopy(data, index + 1, data, index, --count - index);
      data[count] = default(T);
      /* No more exception is possible beyond this point. */
      myCount = count;
    }

    #region RemoveAt, RemoveRange, IList<T>.RemoveAt, IList.RemoveAt

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">This value must be non-negative and less than <see cref="Count"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public void RemoveAt(int index)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index >= 0 && index == count - 1)
      {
        myData[index] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = index;
      }
      else
      {
        RemoveAtRareImpl(index);
      }
    }

    /// <summary>
    /// Removes the items in the specified range.
    /// </summary>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.JustOptimize)]
    public void RemoveRange(int start, int length)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if (length == 0)
      {
        return;
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      count -= length;
      Array.ConstrainedCopy(data, start + length, data, start, count - start);
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(data, count, length);
      }
      /* No more exception is possible beyond this point. */
      myCount = count;
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList<T>.RemoveAt(int index)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index >= 0 && index == count - 1)
      {
        myData[index] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = index;
      }
      else
      {
        RemoveAtRareImpl(index);
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    void IList.RemoveAt(int index)
    {
#if LIST2_ENUMERATION_VERSION
      ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      if (index >= 0 && index == count - 1)
      {
        myData[index] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = index;
      }
      else
      {
        RemoveAtRareImpl(index);
      }
    }

    #endregion RemoveAt, RemoveRange, IList<T>.RemoveAt, IList.RemoveAt

    #region FirstIndexOf, LastIndexOf, IList<T>.IndexOf, IList.IndexOf

    /// <summary>
    /// Finds the index of the item with the smallest index that is equal to <paramref name="item"/>.
    /// The equality comparison method must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public int FirstIndexOf(T item)
    {
      /* "the...Of..." delegate calls user-written "Equals",
      /* which could do terrible things including mutating the list.
      /* We would like to guard against such rudeness. */
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theFirstOfGeneric(myData, myCount, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index;
    }

    /// <summary>
    /// Finds the index of the item with the largest index that is equal to <paramref name="item"/>.
    /// The equality comparison method must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public int LastIndexOf(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theLastOfGeneric(myData, myCount, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index;
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IList<T>.IndexOf(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theFirstOfGeneric(myData, myCount, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index;
    }

    [MethodImpl(Helper.OptimizeInline)]
    int IList.IndexOf(object value)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theFirstOfObject(myData, myCount, value);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index;
    }

    #endregion FirstIndexOf, LastIndexOf, IList<T>.IndexOf, IList.IndexOf

    #region Contains, ICollection<T>.Contains, IList.Contains

    /// <summary>
    /// Determines whether there is an item in the list that is equal to <paramref name="item"/>.
    /// The equality comparison method must not mutate the list.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public bool Contains(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theFirstOfGeneric(myData, myCount, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    bool ICollection<T>.Contains(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theFirstOfGeneric(myData, myCount, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index >= 0;
    }

    [MethodImpl(Helper.OptimizeInline)]
    bool IList.Contains(object value)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = myVersion;
#endif
      int index = theFirstOfObject(myData, myCount, value);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return index >= 0;
    }

    #endregion Contains, ICollection<T>.Contains, IList.Contains

    #region RemoveFirst, RemoveLast, RemoveAll, ICollection<T>.Remove, IList.Remove

    /// <summary>
    /// Removes the item with the smallest index that is equal to <paramref name="item"/>.
    /// The equality comparison method must not mutate the list.
    /// This method returns the old index of the item that was removed.
    /// It returns <c>-1</c> if no item was removed.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public int RemoveFirst(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      int index = theFirstOfGeneric(data, count, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      if (index >= 0)
      {
        Array.ConstrainedCopy(data, index + 1, data, index, --count - index);
        data[count] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = count;
      }
      return index;
    }

    /// <summary>
    /// Removes the item with the largest index that is equal to <paramref name="item"/>.
    /// The equality comparison method must not mutate the list.
    /// This method returns the old index of the item that was removed.
    /// It returns <c>-1</c> if no item was removed.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public int RemoveLast(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      int index = theLastOfGeneric(data, count, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      if (index >= 0)
      {
        Array.ConstrainedCopy(data, index + 1, data, index, --count - index);
        data[count] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = count;
      }
      return index;
    }

    [MethodImpl(Helper.JustOptimize)]
    bool ICollection<T>.Remove(T item)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      int index = theFirstOfGeneric(data, count, item);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      if (index >= 0)
      {
        Array.ConstrainedCopy(data, index + 1, data, index, --count - index);
        data[count] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = count;
        return true;
      }
      return false;
    }

    [MethodImpl(Helper.JustOptimize)]
    void IList.Remove(object value)
    {
#if LIST2_ENUMERATION_VERSION
      uint version = ++myVersion;
#endif
      T[] data = myData;
      int count = myCount;
      int index = theFirstOfObject(data, count, value);
#if LIST2_ENUMERATION_VERSION
      if (version != myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      if (index >= 0)
      {
        Array.ConstrainedCopy(data, index + 1, data, index, --count - index);
        data[count] = default(T);
        /* No more exception is possible beyond this point. */
        myCount = count;
      }
    }

    #endregion RemoveFirst, RemoveLast, RemoveAll, ICollection<T>.Remove, IList.Remove

    #region IPredicate, ThereExists, ForAll, CountSuchThat

    /// <summary>
    /// This interface should be implemented by <see langword="struct"/>s to maximize performance.
    /// </summary>
    public interface IPredicate
    {
      bool Invoke(List2<T> list, int index, T item);
    }

    /// <summary>
    /// Determines whether there exists an item in the list satisfying <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public bool ThereExists<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      return ThereExistsImpl(this, predicate, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static bool ThereExistsImpl<TPredicate>(List2<T> that, TPredicate predicate, T[] data, int count) where TPredicate : IPredicate
    {
      /* A rude predicate.Invoke could mutate the list. */
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      /* It is important to adjust "count", because it could come from an instance corrupted by race conditions.
      /* We want to skip array bounds check while keeping memory safety as far as CLR is concerned. */
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      for (int index = 0; index != count; ++index)
      {
        if (predicate.Invoke(that, index, Unsafe.Add(ref data0, index)))
        {
#if LIST2_ENUMERATION_VERSION
          if (version != that.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return true;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return false;
    }

    /// <summary>
    /// Determines whether all the items in the list satisfy <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public bool ForAll<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      return ForAllImpl(this, predicate, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static bool ForAllImpl<TPredicate>(List2<T> that, TPredicate predicate, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      for (int index = 0; index != count; ++index)
      {
        if (!predicate.Invoke(that, index, Unsafe.Add(ref data0, index)))
        {
#if LIST2_ENUMERATION_VERSION
          if (version != that.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return false;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return true;
    }

    /// <summary>
    /// Gets the number of items in the list that satisfy <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int CountSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      return CountSuchThatImpl(this, predicate, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int CountSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      int countSuchThat = 0;
      for (int index = 0; index != count; ++index)
      {
        if (predicate.Invoke(that, index, Unsafe.Add(ref data0, index)))
        {
          ++countSuchThat;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return countSuchThat;
    }

    #endregion IPredicate, ThereExists, ForAll, CountSuchThat

    #region FirstSuchThat, LastSuchThat

    /// <summary>
    /// Finds the index of the item with the smallest index that satisfies <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int FirstSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      return FirstSuchThatImpl(this, predicate, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int FirstSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      for (int index = 0; index != count; ++index)
      {
        if (predicate.Invoke(that, index, Unsafe.Add(ref data0, index)))
        {
#if LIST2_ENUMERATION_VERSION
          if (version != that.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return index;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return -1;
    }

    /// <summary>
    /// Finds the index of the item with the smallest index greater than or equal to <paramref name="afterInclusive"/> that satisfies <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <param name="afterInclusive">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="afterInclusive"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int FirstSuchThat<TPredicate>(TPredicate predicate, int afterInclusive) where TPredicate : IPredicate
    {
      return FirstSuchThatImpl(this, predicate, afterInclusive, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int FirstSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, int afterInclusive, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      if ((uint)afterInclusive > (uint)count)
      {
        List2.ThrowAfterInclusive();
      }
      for (; afterInclusive != count; ++afterInclusive)
      {
        if (predicate.Invoke(that, afterInclusive, Unsafe.Add(ref data0, afterInclusive)))
        {
#if LIST2_ENUMERATION_VERSION
          if (version != that.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return afterInclusive;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return -1;
    }

    /// <summary>
    /// Finds the index of the item with the smallest index within the specified range that satisfies <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <param name="afterInclusive">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="afterInclusive"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="afterInclusive"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int FirstSuchThat<TPredicate>(TPredicate predicate, int afterInclusive, int length) where TPredicate : IPredicate
    {
      return FirstSuchThatImpl(this, predicate, afterInclusive, length, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int FirstSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, int afterInclusive, int length, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      if ((uint)afterInclusive > (uint)count)
      {
        List2.ThrowAfterInclusive();
      }
      if ((uint)length > (uint)(count - afterInclusive))
      {
        List2.ThrowLength();
      }
      for (count = afterInclusive + length; afterInclusive != count; ++afterInclusive)
      {
        if (predicate.Invoke(that, afterInclusive, Unsafe.Add(ref data0, afterInclusive)))
        {
#if LIST2_ENUMERATION_VERSION
          if (version != that.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return afterInclusive;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return -1;
    }

    /// <summary>
    /// Finds the index of the item with the largest index that satisfies <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int LastSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      return LastSuchThatImpl(this, predicate, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int LastSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      while (count-- != 0)
      {
        if (predicate.Invoke(that, count, Unsafe.Add(ref data0, count)))
        {
          break;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return count;
    }

    /// <summary>
    /// Finds the index of the item with the largest index less than <paramref name="beforeExclusive"/> that satisfies <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <param name="beforeExclusive">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="beforeExclusive"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int LastSuchThat<TPredicate>(TPredicate predicate, int beforeExclusive) where TPredicate : IPredicate
    {
      return LastSuchThatImpl(this, predicate, beforeExclusive, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int LastSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, int beforeExclusive, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      if ((uint)beforeExclusive > (uint)count)
      {
        List2.ThrowBeforeExclusive();
      }
      while (beforeExclusive-- != 0)
      {
        if (predicate.Invoke(that, beforeExclusive, Unsafe.Add(ref data0, beforeExclusive)))
        {
          break;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return beforeExclusive;
    }

    /// <summary>
    /// Finds the index of the item with the largest index within the specified range that satisfies <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// This method returns <c>-1</c> if no item is found.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <param name="beforeExclusive">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <paramref name="beforeExclusive"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="beforeExclusive"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int LastSuchThat<TPredicate>(TPredicate predicate, int beforeExclusive, int length) where TPredicate : IPredicate
    {
      return LastSuchThatImpl(this, predicate, beforeExclusive, length, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int LastSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, int beforeExclusive, int length,
      T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      if ((uint)beforeExclusive > (uint)count)
      {
        List2.ThrowBeforeExclusive();
      }
      if ((uint)length > (uint)beforeExclusive)
      {
        List2.ThrowLength();
      }
      for (count = beforeExclusive - length; beforeExclusive-- != count;)
      {
        if (predicate.Invoke(that, beforeExclusive, Unsafe.Add(ref data0, beforeExclusive)))
        {
          /* Note that the not-found termination does not have "beforeExclusive == -1". */
#if LIST2_ENUMERATION_VERSION
          if (version != that.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return beforeExclusive;
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      return -1;
    }

    #endregion FirstSuchThat, LastSuchThat

    #region RemoveAllSuchThat

    /// <summary>
    /// Removes all the items in the list that satisfy <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// On each invocation, the predicate should not try reading the list for items with indices smaller than the <c>index</c> passed into it,
    /// because that portion is being modified thus unstable.
    /// This method returns the number of items that were removed.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int RemoveAllSuchThat<TPredicate>(TPredicate predicate) where TPredicate : IPredicate
    {
      return RemoveAllSuchThatImpl(this, predicate, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int RemoveAllSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = ++that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      /* Step 1: Find the first item to be removed. */
      int i = 0;
      while (i != count)
      {
        if (predicate.Invoke(that, i, Unsafe.Add(ref data0, i)))
        {
          break;
        }
        ++i;
      }
      /* Step 2: Move items to remain to the new location.
      /*         j = index to put the next item to remain */
      int j = i;
      while (i++ != count)
      {
        if (!predicate.Invoke(that, i, Unsafe.Add(ref data0, i)))
        {
          Unsafe.Add(ref data0, j++) = Unsafe.Add(ref data0, i);
        }
      }
      /* If bad things happen, throw the exception without touching the internal state of the list any more. */
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      /* Now,     j = new count of list
      /*      count = number of removed items */
      count -= j;
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(data, j, count);
      }
      /* No more exception is possible beyond this point. */
      that.myCount = j;
      return count;
    }

    /// <summary>
    /// Removes all the items in the specified range that satisfy <paramref name="predicate"/>.
    /// Calling <see cref="IPredicate.Invoke(List2{T}, int, T)"/> on <paramref name="predicate"/> must not mutate the list.
    /// On each invocation, the predicate should not try reading the list for items with indices
    /// between <paramref name="start"/> (inclusive, cannot read) and the <c>index</c> passed into it (exclusive, can read),
    /// because that portion is being modified thus unstable.
    /// This method returns the number of items that were removed.
    /// </summary>
    /// <param name="predicate">This argument must not be <see langword="null"/>.</param>
    /// <param name="start">This value must be non-negative and not exceed <see cref="Count"/>.</param>
    /// <param name="length">This value must be non-negative and not exceed <see cref="Count"/> minus <paramref name="start"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="start"/> or <paramref name="length"/> is out of range.</exception>
    [MethodImpl(Helper.OptimizeInline)]
    public int RemoveAllSuchThat<TPredicate>(TPredicate predicate, int start, int length) where TPredicate : IPredicate
    {
      return RemoveAllSuchThatImpl(this, predicate, start, length, myData, myCount);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    private static int RemoveAllSuchThatImpl<TPredicate>(List2<T> that, TPredicate predicate, int start, int length, T[] data, int count) where TPredicate : IPredicate
    {
#if LIST2_ENUMERATION_VERSION
      uint version = ++that.myVersion;
#endif
      count = ((uint)count < (uint)data.Length ? count : data.Length);
      ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
      if (predicate is null)
      {
        List2.ThrowPredicate();
      }
      if ((uint)start > (uint)count)
      {
        List2.ThrowStart();
      }
      if ((uint)length > (uint)(count - start))
      {
        List2.ThrowLength();
      }
      int end = start + length;
      while (start != end)
      {
        if (predicate.Invoke(that, start, Unsafe.Add(ref data0, start)))
        {
          break;
        }
        ++start;
      }
      int j = start;
      while (start++ != end)
      {
        if (!predicate.Invoke(that, start, Unsafe.Add(ref data0, start)))
        {
          Unsafe.Add(ref data0, j++) = Unsafe.Add(ref data0, start);
        }
      }
#if LIST2_ENUMERATION_VERSION
      if (version != that.myVersion)
      {
        List2.ThrowVersion();
      }
#endif
      /* Old list is  [0, oldStart) + [oldStart,          end) + [end, count).
      /* Current  is  [0, oldStart) + [oldStart, j) + [j, end) + [end, count).
      /*                                              ^^^^^^^^ should be removed */
      Array.ConstrainedCopy(data, end, data, j, count - end);
      /* Now, end = number of removed items */
      end -= j;
      /* Now,   j = new count of list */
      j = count - end;
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
      {
        Array.Clear(data, j, end);
      }
      /* No more exception is possible beyond this point. */
      that.myCount = j;
      return end;
    }

    #endregion RemoveAllSuchThat

    #region IList.IsFixedSize, ICollection<T>.IsReadOnly, IList.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool IList.IsFixedSize
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool IList.IsReadOnly
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
    bool ICollection.IsSynchronized
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return false;
      }
    }

    /// <summary>
    /// This member is thread-safe.
    /// </summary>
#if LIST2_SYNCROOT
    object ICollection.SyncRoot
    {
      [MethodImpl(Helper.JustOptimize)]
      get
      {
        if (mySyncRoot is null)
        {
          Interlocked.CompareExchange(ref mySyncRoot, new object(), null);
        }
        return mySyncRoot;
      }
    }
#else
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot
    {
      [MethodImpl(Helper.OptimizeNoInline)]
      get
      {
        throw new NotSupportedException("SyncRoot is not supported (the support can be enabled by LIST2_SYNCROOT).");
      }
    }
#endif

    #endregion IList.IsFixedSize, ICollection<T>.IsReadOnly, IList.IsReadOnly, ICollection.IsSynchronized, ICollection.SyncRoot

    /// <summary>
    /// Enumerates items in <see cref="List2{T}"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Enumerator : IEnumerator2<T>
    {
      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private T[] myData;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private int myCount;

      private int myIndex;

#if LIST2_ENUMERATION_VERSION

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private List2<T> myTarget;

      [SuppressMessage("Style", "IDE0044", Justification = "https://codeblog.jonskeet.uk/2014/07/16/micro-optimization-the-surprising-inefficiency-of-readonly-fields/")]
      private uint myVersion;

#endif

#if LIST2_ENUMERATOR_DISPOSE
      private bool myNotDisposed;
#endif

      [DebuggerBrowsable(DebuggerBrowsableState.Never)]
      private string DebuggerDisplay
      {
        get
        {
          return "Count = " + myCount.ToString(CultureInfo.InvariantCulture)
            + ", Index = " + myIndex.ToString(CultureInfo.InvariantCulture)
#if LIST2_ENUMERATION_VERSION
            + ", Version = " + myVersion.ToString(CultureInfo.InvariantCulture)
#endif
#if LIST2_ENUMERATOR_DISPOSE
            + (myNotDisposed ? "" : " <disposed>")
#endif
            ;
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      internal Enumerator(List2<T> target)
      {
        myData = target.myData;
        myCount = target.myCount;
        myIndex = -1;
#if LIST2_ENUMERATION_VERSION
        myTarget = target;
        myVersion = target.myVersion;
#endif
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = true;
#endif
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IEnumerator.Reset()
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        myIndex = -1;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext(out T item)
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        T[] data = myData;
        int count = myCount;
        int index = myIndex;
        myIndex = ++index;
        if (index < count)
        {
          item = data[index];
          return true;
        }
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = default(T);
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      bool IEnumerator2.MoveNext(out object item)
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        T[] data = myData;
        int count = myCount;
        int index = myIndex;
        myIndex = ++index;
        if (index < count)
        {
          item = data[index];
          return true;
        }
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
        item = null;
        return false;
      }

      [MethodImpl(Helper.OptimizeInline)]
      public bool MoveNext()
      {
#if LIST2_ENUMERATOR_DISPOSE
        if (!myNotDisposed)
        {
          List2.ThrowDisposed();
        }
#endif
#if LIST2_ENUMERATION_VERSION
        if (myVersion != myTarget.myVersion)
        {
          List2.ThrowVersion();
        }
#endif
        return ++myIndex < myCount;
      }

      public T Current
      {
        [MethodImpl(Helper.OptimizeInline)]
        get
        {
#if LIST2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            List2.ThrowDisposed();
          }
#endif
#if LIST2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            List2.ThrowVersion();
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
#if LIST2_ENUMERATOR_DISPOSE
          if (!myNotDisposed)
          {
            List2.ThrowDisposed();
          }
#endif
#if LIST2_ENUMERATION_VERSION
          if (myVersion != myTarget.myVersion)
          {
            List2.ThrowVersion();
          }
#endif
          return myData[myIndex];
        }
      }

      [MethodImpl(Helper.OptimizeInline)]
      void IDisposable.Dispose()
      {
#if LIST2_ENUMERATOR_DISPOSE
        myNotDisposed = false;
#endif
      }
    }

    #region GetEnumerator

    [MethodImpl(Helper.OptimizeInline)]
    public Enumerator GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    Enumerator IEnumerable2<T, Enumerator>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2<T> IEnumerable2<T>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator2 IEnumerable2.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new Enumerator(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new Enumerator(this);
    }

    #endregion GetEnumerator
  }

  /// <summary>
  /// Provides information about <see cref="List2{T}"/>.
  /// </summary>
  public static class List2
  {
#if LIST2_ENUMERATION_VERSION
    public static readonly bool LoadedWithEnumerationVersion = true;
    public const bool CompiledWithEnumerationVersion = true;
#else
    public static readonly bool LoadedWithEnumerationVersion = false;
    public const bool CompiledWithEnumerationVersion = false;
#endif

#if LIST2_ENUMERATOR_DISPOSE
    public static readonly bool LoadedWithEnumeratorDispose = true;
    public const bool CompiledWithEnumeratorDispose = true;
#else
    public static readonly bool LoadedWithEnumeratorDispose = false;
    public const bool CompiledWithEnumeratorDispose = false;
#endif

#if LIST2_SYNCROOT
    public static readonly bool LoadedWithSyncRoot = true;
    public const bool CompiledWithSyncRoot = true;
#else
    public static readonly bool LoadedWithSyncRoot = false;
    public const bool CompiledWithSyncRoot = false;
#endif

    public const int StartingCapacity = 8;
    public const int MaximumCapacityOneByte = 0x7FFFFFC7;
    public const int MaximumCapacityOther = 0x7FEFFFFF;

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowStart()
    {
      throw new ArgumentOutOfRangeException("start");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowLength()
    {
      throw new ArgumentOutOfRangeException("length");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowCapacity()
    {
      throw new ArgumentOutOfRangeException("capacity");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowTooMany()
    {
      throw new InvalidOperationException("There will be more than MaximumCapacity number of items in the list.");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowIndex()
    {
      throw new ArgumentOutOfRangeException("index");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowPredicate()
    {
      throw new ArgumentNullException("predicate");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowAfterInclusive()
    {
      throw new ArgumentOutOfRangeException("afterInclusive");
    }

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowBeforeExclusive()
    {
      throw new ArgumentOutOfRangeException("beforeExclusive");
    }

#if LIST2_ENUMERATION_VERSION

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowVersion()
    {
      throw new InvalidOperationException("The list is modified during enumeration. (This check is enabled by LIST2_ENUMERATION_VERSION.)");
    }

#endif

#if LIST2_ENUMERATOR_DISPOSE

    [DoesNotReturn]
    [MethodImpl(Helper.OptimizeNoInline)]
    internal static void ThrowDisposed()
    {
      throw new ObjectDisposedException(typeof(List2<>.Enumerator).FullName,
        "The enumerator is already disposed. (This check is enabled by LIST2_ENUMERATOR_DISPOSE.)");
    }

#endif

    #region IndexOf

    internal delegate int IndexOf<in T, in U>(T[] data, int count, U arg);

    private static readonly IndexOfHelper theIndexOfHelper;
    private static readonly IndexOf<object, object> theFirstOfObject;
    private static readonly IndexOf<object, object> theLastOfObject;
    private static readonly MethodInfo theFirstOfEquatableValueGeneric;
    private static readonly MethodInfo theFirstOfEquatableValueObject;
    private static readonly MethodInfo theLastOfEquatableValueGeneric;
    private static readonly MethodInfo theFirstOfNullableEquatableValueGeneric;
    private static readonly MethodInfo theFirstOfNullableEquatableValueObject;
    private static readonly MethodInfo theLastOfNullableEquatableValueGeneric;
    private static readonly MethodInfo theFirstOfValueGeneric;
    private static readonly MethodInfo theFirstOfValueObject;
    private static readonly MethodInfo theLastOfValueGeneric;
    private static readonly MethodInfo theFirstOfNullableValueGeneric;
    private static readonly MethodInfo theFirstOfNullableValueObject;
    private static readonly MethodInfo theLastOfNullableValueGeneric;
    private static readonly MethodInfo theFirstOfEnum8Generic;
    private static readonly MethodInfo theFirstOfEnum8Object;
    private static readonly MethodInfo theLastOfEnum8Generic;
    private static readonly MethodInfo theFirstOfEnum16Generic;
    private static readonly MethodInfo theFirstOfEnum16Object;
    private static readonly MethodInfo theLastOfEnum16Generic;
    private static readonly MethodInfo theFirstOfEnum32Generic;
    private static readonly MethodInfo theFirstOfEnum32Object;
    private static readonly MethodInfo theLastOfEnum32Generic;
    private static readonly MethodInfo theFirstOfEnum64Generic;
    private static readonly MethodInfo theFirstOfEnum64Object;
    private static readonly MethodInfo theLastOfEnum64Generic;
    private static readonly MethodInfo theFirstOfEnumNativeGeneric;
    private static readonly MethodInfo theFirstOfEnumNativeObject;
    private static readonly MethodInfo theLastOfEnumNativeGeneric;
    private static readonly MethodInfo theFirstOfNullableEnum8Generic;
    private static readonly MethodInfo theFirstOfNullableEnum8Object;
    private static readonly MethodInfo theLastOfNullableEnum8Generic;
    private static readonly MethodInfo theFirstOfNullableEnum16Generic;
    private static readonly MethodInfo theFirstOfNullableEnum16Object;
    private static readonly MethodInfo theLastOfNullableEnum16Generic;
    private static readonly MethodInfo theFirstOfNullableEnum32Generic;
    private static readonly MethodInfo theFirstOfNullableEnum32Object;
    private static readonly MethodInfo theLastOfNullableEnum32Generic;
    private static readonly MethodInfo theFirstOfNullableEnum64Generic;
    private static readonly MethodInfo theFirstOfNullableEnum64Object;
    private static readonly MethodInfo theLastOfNullableEnum64Generic;
    private static readonly MethodInfo theFirstOfNullableEnumNativeGeneric;
    private static readonly MethodInfo theFirstOfNullableEnumNativeObject;
    private static readonly MethodInfo theLastOfNullableEnumNativeGeneric;

    [MethodImpl(Helper.JustOptimize)]
    internal static void GetIndexOfDelegates<T>(out IndexOf<T, T> firstOfGeneric, out IndexOf<T, object> firstOfObject, out IndexOf<T, T> lastOfGeneric)
    {
      Type t = typeof(T);
      Type[] ts;
      if (t.IsValueType)
      {
        IndexOfHelper helper = theIndexOfHelper;
        if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Nullable<>))
        {
          /* T is non-nullable value type */
          ts = new Type[1] { t };
          if (t.IsAssignableTo(typeof(IEquatable<T>)))
          {
            firstOfGeneric = theFirstOfEquatableValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
            firstOfObject = theFirstOfEquatableValueObject.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
            lastOfGeneric = theLastOfEquatableValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
            return;
          }
          if (t.IsEnum)
          {
            Type u = t.GetEnumUnderlyingType();
            if (u == typeof(byte) || u == typeof(sbyte))
            {
              firstOfGeneric = theFirstOfEnum8Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfEnum8Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfEnum8Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(short) || u == typeof(ushort) || u == typeof(char))
            {
              firstOfGeneric = theFirstOfEnum16Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfEnum16Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfEnum16Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(int) || u == typeof(uint))
            {
              firstOfGeneric = theFirstOfEnum32Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfEnum32Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfEnum32Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(long) || u == typeof(ulong))
            {
              firstOfGeneric = theFirstOfEnum64Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfEnum64Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfEnum64Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(IntPtr) || u == typeof(UIntPtr))
            {
              firstOfGeneric = theFirstOfEnumNativeGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfEnumNativeObject.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfEnumNativeGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            /* C# (ECMA-334, 5th Edition, December 2017) Section 9.3.10 only allows
            /*     byte, sbyte, short, ushort, int, uint, long, ulong
            /* as the underlying type of an enumeration type.
            /* However, CLI (ECMA-335, 5th Edition, December 2010) Partition II Section 14.3 also allows
            /*     bool, char, nint, nuint.
            /* I have no idea how to deal with "bool", given the iffy issues with "bool" whose underlying bit pattern
            /* is neither 0x00 nor 0x01, so I left it out and let CLR handle it (potentially with boxing).
            /* Fall through to general value type. */
          }
          firstOfGeneric = theFirstOfValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
          firstOfObject = theFirstOfValueObject.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
          lastOfGeneric = theLastOfValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
          return;
        }
        else
        {
          /* T is Nullable<t> */
          ts = t.GetGenericArguments();
          t = ts[0];
          if (t.IsAssignableTo(typeof(IEquatable<>).MakeGenericType(ts)))
          {
            firstOfGeneric = theFirstOfNullableEquatableValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
            firstOfObject = theFirstOfNullableEquatableValueObject.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
            lastOfGeneric = theLastOfNullableEquatableValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
            return;
          }
          if (t.IsEnum)
          {
            Type u = t.GetEnumUnderlyingType();
            if (u == typeof(byte) || u == typeof(sbyte))
            {
              firstOfGeneric = theFirstOfNullableEnum8Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfNullableEnum8Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfNullableEnum8Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(short) || u == typeof(ushort) || u == typeof(char))
            {
              firstOfGeneric = theFirstOfNullableEnum16Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfNullableEnum16Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfNullableEnum16Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(int) || u == typeof(uint))
            {
              firstOfGeneric = theFirstOfNullableEnum32Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfNullableEnum32Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfNullableEnum32Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(long) || u == typeof(ulong))
            {
              firstOfGeneric = theFirstOfNullableEnum64Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfNullableEnum64Object.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfNullableEnum64Generic.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
            if (u == typeof(IntPtr) || u == typeof(UIntPtr))
            {
              firstOfGeneric = theFirstOfNullableEnumNativeGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              firstOfObject = theFirstOfNullableEnumNativeObject.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
              lastOfGeneric = theLastOfNullableEnumNativeGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
              return;
            }
          }
          firstOfGeneric = theFirstOfNullableValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
          firstOfObject = theFirstOfNullableValueObject.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, object>>(helper);
          lastOfGeneric = theLastOfNullableValueGeneric.MakeGenericMethod(ts).CreateDelegate<IndexOf<T, T>>(helper);
          return;
        }
      }
      else
      {
        /* T is reference type */
        firstOfGeneric = (IndexOf<T, T>)(object)theFirstOfObject;
        firstOfObject = (IndexOf<T, object>)(object)theFirstOfObject;
        lastOfGeneric = (IndexOf<T, T>)(object)theLastOfObject;
        return;
      }
    }

    static List2()
    {
      IndexOfHelper helper = new IndexOfHelper();
      theIndexOfHelper = helper;
      theFirstOfObject = helper.FirstOfObject;
      theLastOfObject = helper.LastOfObject;
      Type t = typeof(IndexOfHelper);
      theFirstOfEquatableValueGeneric = t.GetMethod(nameof(IndexOfHelper.FirstOfEquatableValueGeneric));
      theFirstOfEquatableValueObject = t.GetMethod(nameof(IndexOfHelper.FirstOfEquatableValueObject));
      theLastOfEquatableValueGeneric = t.GetMethod(nameof(IndexOfHelper.LastOfEquatableValueGeneric));
      theFirstOfNullableEquatableValueGeneric = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEquatableValueGeneric));
      theFirstOfNullableEquatableValueObject = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEquatableValueObject));
      theLastOfNullableEquatableValueGeneric = t.GetMethod(nameof(IndexOfHelper.LastOfNullableEquatableValueGeneric));
      theFirstOfValueGeneric = t.GetMethod(nameof(IndexOfHelper.FirstOfValueGeneric));
      theFirstOfValueObject = t.GetMethod(nameof(IndexOfHelper.FirstOfValueObject));
      theLastOfValueGeneric = t.GetMethod(nameof(IndexOfHelper.LastOfValueGeneric));
      theFirstOfNullableValueGeneric = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableValueGeneric));
      theFirstOfNullableValueObject = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableValueObject));
      theLastOfNullableValueGeneric = t.GetMethod(nameof(IndexOfHelper.LastOfNullableValueGeneric));
      theFirstOfEnum8Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum8Generic));
      theFirstOfEnum8Object = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum8Object));
      theLastOfEnum8Generic = t.GetMethod(nameof(IndexOfHelper.LastOfEnum8Generic));
      theFirstOfEnum16Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum16Generic));
      theFirstOfEnum16Object = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum16Object));
      theLastOfEnum16Generic = t.GetMethod(nameof(IndexOfHelper.LastOfEnum16Generic));
      theFirstOfEnum32Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum32Generic));
      theFirstOfEnum32Object = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum32Object));
      theLastOfEnum32Generic = t.GetMethod(nameof(IndexOfHelper.LastOfEnum32Generic));
      theFirstOfEnum64Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum64Generic));
      theFirstOfEnum64Object = t.GetMethod(nameof(IndexOfHelper.FirstOfEnum64Object));
      theLastOfEnum64Generic = t.GetMethod(nameof(IndexOfHelper.LastOfEnum64Generic));
      theFirstOfEnumNativeGeneric = t.GetMethod(nameof(IndexOfHelper.FirstOfEnumNativeGeneric));
      theFirstOfEnumNativeObject = t.GetMethod(nameof(IndexOfHelper.FirstOfEnumNativeObject));
      theLastOfEnumNativeGeneric = t.GetMethod(nameof(IndexOfHelper.LastOfEnumNativeGeneric));
      theFirstOfNullableEnum8Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum8Generic));
      theFirstOfNullableEnum8Object = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum8Object));
      theLastOfNullableEnum8Generic = t.GetMethod(nameof(IndexOfHelper.LastOfNullableEnum8Generic));
      theFirstOfNullableEnum16Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum16Generic));
      theFirstOfNullableEnum16Object = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum16Object));
      theLastOfNullableEnum16Generic = t.GetMethod(nameof(IndexOfHelper.LastOfNullableEnum16Generic));
      theFirstOfNullableEnum32Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum32Generic));
      theFirstOfNullableEnum32Object = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum32Object));
      theLastOfNullableEnum32Generic = t.GetMethod(nameof(IndexOfHelper.LastOfNullableEnum32Generic));
      theFirstOfNullableEnum64Generic = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum64Generic));
      theFirstOfNullableEnum64Object = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnum64Object));
      theLastOfNullableEnum64Generic = t.GetMethod(nameof(IndexOfHelper.LastOfNullableEnum64Generic));
      theFirstOfNullableEnumNativeGeneric = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnumNativeGeneric));
      theFirstOfNullableEnumNativeObject = t.GetMethod(nameof(IndexOfHelper.FirstOfNullableEnumNativeObject));
      theLastOfNullableEnumNativeGeneric = t.GetMethod(nameof(IndexOfHelper.LastOfNullableEnumNativeGeneric));
    }

    [SuppressMessage("Performance", "CA1822", Justification = "Closed delegates are more performant.")]
    private sealed class IndexOfHelper
    {
      #region object (all reference types)

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfObject(object[] data, int count, object item)
      {
        /* We do not try IEquatable<T> for T : class because of the following rationales:
        /* (1) Interface method dispatch is slower than class virtual method dispatch.
        /*     (But I am not sure whether
        /*         https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/botr/virtual-stub-dispatch.md
        /*     makes interface method dispatch on par with class virtual method dispatch
        /*     for monomorphic sites.)
        /* (2) We cannot take advantage of generic delegate contravariance to reduce allocation of delegates.
        /* Although this method is only called through a delegate, we still require it not be inlined
        /* in case the JIT compiler becomes clever enough to inline delegates in the future. */
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref object data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is null)
        {
          for (int i = 0; i != count; ++i)
          {
            if (Unsafe.Add(ref data0, i) is null)
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            /* We should not call data[i].Equals(item) for the following reasons:
            /* (1) data[i] could be null but item is not.
            /* (2) Monomorphic site could have better performance. */
            if (item.Equals(Unsafe.Add(ref data0, i)))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfObject(object[] data, int count, object item)
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref object data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is null)
        {
          while (count-- != 0)
          {
            if (Unsafe.Add(ref data0, count) is null)
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (item.Equals(Unsafe.Add(ref data0, count)))
            {
              break;
            }
          }
        }
        return count;
      }

      #endregion object (all reference types)

      #region struct T : IEquatable<T>

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEquatableValueGeneric<T>(T[] data, int count, T item) where T : struct, IEquatable<T>
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
        for (int i = 0; i != count; ++i)
        {
          /* We should not call data[i].Equals(item) for the following reasons:
          /* (1) A terrible implementation of "bool IEquatable<T>.Equals(T other)" could mutate "this".
          /* (2) We must avoid mutating the list.
          /* (3) "item" is a copy local to this function so it is safe to mutate. */
          if (item.Equals(Unsafe.Add(ref data0, i)))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEquatableValueObject<T>(T[] data, int count, object value) where T : struct, IEquatable<T>
      {
        if (value is T item)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
          for (int i = 0; i != count; ++i)
          {
            if (item.Equals(Unsafe.Add(ref data0, i)))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfEquatableValueGeneric<T>(T[] data, int count, T item) where T : struct, IEquatable<T>
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
        while (count-- != 0)
        {
          if (item.Equals(Unsafe.Add(ref data0, count)))
          {
            break;
          }
        }
        return count;
      }

      #endregion struct T : IEquatable<T>

      #region Nullable<T> where T : struct, IEquatable<T>

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEquatableValueGeneric<T>(T?[] data, int count, T? item) where T : struct, IEquatable<T>
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          for (int i = 0; i != count; ++i)
          {
            if (Unsafe.Add(ref data0, i).HasValue && itemValue.Equals(Unsafe.Add(ref data0, i).GetValueOrDefault()))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEquatableValueObject<T>(T?[] data, int count, object value) where T : struct, IEquatable<T>
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (value is T itemValue)
        {
          for (int i = 0; i != count; ++i)
          {
            if (Unsafe.Add(ref data0, i).HasValue && itemValue.Equals(Unsafe.Add(ref data0, i).GetValueOrDefault()))
            {
              return i;
            }
          }
        }
        else if (value is null)
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        /* This also includes the case when "value" is neither T nor null. */
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableEquatableValueGeneric<T>(T?[] data, int count, T? item) where T : struct, IEquatable<T>
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          while (count-- != 0)
          {
            if (Unsafe.Add(ref data0, count).HasValue && itemValue.Equals(Unsafe.Add(ref data0, count).GetValueOrDefault()))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      #endregion Nullable<T> where T : struct, IEquatable<T>

      #region struct T not implementing IEquatable<T>

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfValueGeneric<T>(T[] data, int count, T item) where T : struct
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
        /* There are two cases:
        /* (1) The most derrived override of "bool object.Equals(object obj)" is on "T".
        /* (2) It is on "ValueType" (not "Enum").
        /* Calling "a.Equals(b)" will...
        /* (1) box "b";
        /* (2) box both "a" and "b".
        /* We always call "CopyOf(data[i]).Equals(value)", where "value" is a cached boxed "item".
        /* In the two cases, ...
        /* (1) there is only 1 boxing;
        /* (2) there are (N+1) boxings.
        /* In the second case, (N+1) boxings is already optimal.
        /* The copying of data[i] is to avoid mutating the list inside the call to "Equals". */
        ValueType value = item;
        for (int i = 0; i != count; ++i)
        {
          T datai = Unsafe.Add(ref data0, i);
          /* If T overrides Equals, this constrained call JIT-compiles to direct instance call.
          /* Otherwise, it JIT-compiles to boxing and virtual call. */
          if (datai.Equals(value))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfValueObject<T>(T[] data, int count, object value) where T : struct
      {
        if (value is T)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
          /* There is no point in casting "value" to "ValueType". */
          for (int i = 0; i != count; ++i)
          {
            T datai = Unsafe.Add(ref data0, i);
            if (datai.Equals(value))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfValueGeneric<T>(T[] data, int count, T item) where T : struct
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T data0 = ref MemoryMarshal.GetArrayDataReference(data);
        ValueType value = item;
        while (count-- != 0)
        {
          T datai = Unsafe.Add(ref data0, count);
          if (datai.Equals(value))
          {
            break;
          }
        }
        return count;
      }

      #endregion struct T not implementing IEquatable<T>

      #region Nullable<T> where T : struct does not implement IEquatable<T>

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableValueGeneric<T>(T?[] data, int count, T? item) where T : struct
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          ValueType value = item;
          for (int i = 0; i != count; ++i)
          {
            /* GetValueOrDefault() will make a copy for us. */
            if (Unsafe.Add(ref data0, i).HasValue && Unsafe.Add(ref data0, i).GetValueOrDefault().Equals(value))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableValueObject<T>(T?[] data, int count, object value) where T : struct
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (value is T)
        {
          for (int i = 0; i != count; ++i)
          {
            if (Unsafe.Add(ref data0, i).HasValue && Unsafe.Add(ref data0, i).GetValueOrDefault().Equals(value))
            {
              return i;
            }
          }
        }
        else if (value is null)
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableValueGeneric<T>(T?[] data, int count, T? item) where T : struct
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          ValueType value = item;
          while (count-- != 0)
          {
            if (Unsafe.Add(ref data0, count).HasValue && Unsafe.Add(ref data0, count).GetValueOrDefault().Equals(value))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      #endregion Nullable<T> where T : struct does not implement IEquatable<T>

      #region enum T : sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum8Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref byte data0 = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(data));
        byte itemNumericValue = Unsafe.As<T, byte>(ref item);
        for (int i = 0; i != count; ++i)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, i))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum8Object<T>(T[] data, int count, object item) where T : struct, Enum
      {
        if (item is T itemValue)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref byte data0 = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(data));
          byte itemNumericValue = Unsafe.As<T, byte>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            if (itemNumericValue == Unsafe.Add(ref data0, i))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfEnum8Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref byte data0 = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(data));
        byte itemNumericValue = Unsafe.As<T, byte>(ref item);
        while (count-- != 0)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, count))
          {
            break;
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum16Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref ushort data0 = ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetArrayDataReference(data));
        ushort itemNumericValue = Unsafe.As<T, ushort>(ref item);
        for (int i = 0; i != count; ++i)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, i))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum16Object<T>(T[] data, int count, object item) where T : struct, Enum
      {
        if (item is T itemValue)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref ushort data0 = ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetArrayDataReference(data));
          ushort itemNumericValue = Unsafe.As<T, ushort>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            if (itemNumericValue == Unsafe.Add(ref data0, i))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfEnum16Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref ushort data0 = ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetArrayDataReference(data));
        ushort itemNumericValue = Unsafe.As<T, ushort>(ref item);
        while (count-- != 0)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, count))
          {
            break;
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum32Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref uint data0 = ref Unsafe.As<T, uint>(ref MemoryMarshal.GetArrayDataReference(data));
        uint itemNumericValue = Unsafe.As<T, uint>(ref item);
        for (int i = 0; i != count; ++i)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, i))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum32Object<T>(T[] data, int count, object item) where T : struct, Enum
      {
        if (item is T itemValue)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref uint data0 = ref Unsafe.As<T, uint>(ref MemoryMarshal.GetArrayDataReference(data));
          uint itemNumericValue = Unsafe.As<T, uint>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            if (itemNumericValue == Unsafe.Add(ref data0, i))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfEnum32Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref uint data0 = ref Unsafe.As<T, uint>(ref MemoryMarshal.GetArrayDataReference(data));
        uint itemNumericValue = Unsafe.As<T, uint>(ref item);
        while (count-- != 0)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, count))
          {
            break;
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum64Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref ulong data0 = ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetArrayDataReference(data));
        ulong itemNumericValue = Unsafe.As<T, ulong>(ref item);
        for (int i = 0; i != count; ++i)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, i))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnum64Object<T>(T[] data, int count, object item) where T : struct, Enum
      {
        if (item is T itemValue)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref ulong data0 = ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetArrayDataReference(data));
          ulong itemNumericValue = Unsafe.As<T, ulong>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            if (itemNumericValue == Unsafe.Add(ref data0, i))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfEnum64Generic<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref ulong data0 = ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetArrayDataReference(data));
        ulong itemNumericValue = Unsafe.As<T, ulong>(ref item);
        while (count-- != 0)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, count))
          {
            break;
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnumNativeGeneric<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref nuint data0 = ref Unsafe.As<T, nuint>(ref MemoryMarshal.GetArrayDataReference(data));
        nuint itemNumericValue = Unsafe.As<T, nuint>(ref item);
        for (int i = 0; i != count; ++i)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, i))
          {
            return i;
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfEnumNativeObject<T>(T[] data, int count, object item) where T : struct, Enum
      {
        if (item is T itemValue)
        {
          count = ((uint)count < (uint)data.Length ? count : data.Length);
          ref nuint data0 = ref Unsafe.As<T, nuint>(ref MemoryMarshal.GetArrayDataReference(data));
          nuint itemNumericValue = Unsafe.As<T, nuint>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            if (itemNumericValue == Unsafe.Add(ref data0, i))
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfEnumNativeGeneric<T>(T[] data, int count, T item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref nuint data0 = ref Unsafe.As<T, nuint>(ref MemoryMarshal.GetArrayDataReference(data));
        nuint itemNumericValue = Unsafe.As<T, nuint>(ref item);
        while (count-- != 0)
        {
          if (itemNumericValue == Unsafe.Add(ref data0, count))
          {
            break;
          }
        }
        return count;
      }

      #endregion enum T : sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint

      #region Nullable<T> where enum T : sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum8Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          byte itemNumericValue = Unsafe.As<T, byte>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, byte>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum8Object<T>(T?[] data, int count, object item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is T itemValue)
        {
          byte itemNumericValue = Unsafe.As<T, byte>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, byte>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableEnum8Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          byte itemNumericValue = Unsafe.As<T, byte>(ref itemValue);
          while (count-- != 0)
          {
            ref T? datai = ref Unsafe.Add(ref data0, count);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, byte>(ref dataiValue))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum16Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          ushort itemNumericValue = Unsafe.As<T, ushort>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, ushort>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum16Object<T>(T?[] data, int count, object item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is T itemValue)
        {
          ushort itemNumericValue = Unsafe.As<T, ushort>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, ushort>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableEnum16Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          ushort itemNumericValue = Unsafe.As<T, ushort>(ref itemValue);
          while (count-- != 0)
          {
            ref T? datai = ref Unsafe.Add(ref data0, count);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, ushort>(ref dataiValue))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum32Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          uint itemNumericValue = Unsafe.As<T, uint>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, uint>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum32Object<T>(T?[] data, int count, object item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is T itemValue)
        {
          uint itemNumericValue = Unsafe.As<T, uint>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, uint>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableEnum32Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          uint itemNumericValue = Unsafe.As<T, uint>(ref itemValue);
          while (count-- != 0)
          {
            ref T? datai = ref Unsafe.Add(ref data0, count);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, uint>(ref dataiValue))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum64Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          ulong itemNumericValue = Unsafe.As<T, ulong>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, ulong>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnum64Object<T>(T?[] data, int count, object item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is T itemValue)
        {
          ulong itemNumericValue = Unsafe.As<T, ulong>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, ulong>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableEnum64Generic<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          ulong itemNumericValue = Unsafe.As<T, ulong>(ref itemValue);
          while (count-- != 0)
          {
            ref T? datai = ref Unsafe.Add(ref data0, count);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, ulong>(ref dataiValue))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnumNativeGeneric<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          nuint itemNumericValue = Unsafe.As<T, nuint>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, nuint>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int FirstOfNullableEnumNativeObject<T>(T?[] data, int count, object item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item is T itemValue)
        {
          nuint itemNumericValue = Unsafe.As<T, nuint>(ref itemValue);
          for (int i = 0; i != count; ++i)
          {
            ref T? datai = ref Unsafe.Add(ref data0, i);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, nuint>(ref dataiValue))
            {
              return i;
            }
          }
        }
        else
        {
          for (int i = 0; i != count; ++i)
          {
            if (!Unsafe.Add(ref data0, i).HasValue)
            {
              return i;
            }
          }
        }
        return -1;
      }

      [MethodImpl(Helper.OptimizeNoInline)]
      public int LastOfNullableEnumNativeGeneric<T>(T?[] data, int count, T? item) where T : struct, Enum
      {
        count = ((uint)count < (uint)data.Length ? count : data.Length);
        ref T? data0 = ref MemoryMarshal.GetArrayDataReference(data);
        if (item.HasValue)
        {
          T itemValue = item.GetValueOrDefault();
          nuint itemNumericValue = Unsafe.As<T, nuint>(ref itemValue);
          while (count-- != 0)
          {
            ref T? datai = ref Unsafe.Add(ref data0, count);
            T dataiValue = datai.GetValueOrDefault();
            if (datai.HasValue && itemNumericValue == Unsafe.As<T, nuint>(ref dataiValue))
            {
              break;
            }
          }
        }
        else
        {
          while (count-- != 0)
          {
            if (!Unsafe.Add(ref data0, count).HasValue)
            {
              break;
            }
          }
        }
        return count;
      }

      #endregion Nullable<T> where enum T : sbyte, byte, short, ushort, int, uint, long, ulong, nint, nuint
    }

    #endregion IndexOf
  }
}
