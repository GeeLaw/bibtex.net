using System.Collections;
using System.Collections.Generic;

namespace Neat.Collections
{
  /// <summary>
  /// Supports one-call enumeration.
  /// Objects implementing this interface should also implement <see cref="IEnumerable2{T, TEnumerator}"/>.
  /// </summary>
  public interface IEnumerable2 : IEnumerable
  {
    /// <summary>
    /// Creates a new enumerator.
    /// Objects implementing this interface should also implement <see cref="IEnumerable2{T, TEnumerator}"/>.
    /// </summary>
    /// <returns>The newly created enumerator.</returns>
    new IEnumerator2 GetEnumerator();
  }

  /// <summary>
  /// Supports generic one-call enumeration.
  /// </summary>
  public interface IEnumerable2<T> : IEnumerable2, IEnumerable<T>
  {
    /// <summary>
    /// Creates a new enumerator.
    /// </summary>
    /// <returns>The newly created enumerator.</returns>
    new IEnumerator2<T> GetEnumerator();
  }

  /// <summary>
  /// Supports generic one-call enumeration with specifically typed enumerator.
  /// </summary>
  public interface IEnumerable2<T, out TEnumerator2> : IEnumerable2<T>
    where TEnumerator2 : IEnumerator2<T>
  {
    /// <summary>
    /// Creates a new specifically typed enumerator.
    /// </summary>
    /// <returns>The newly created specifically typed enumerator.</returns>
    new TEnumerator2 GetEnumerator();
  }
}
