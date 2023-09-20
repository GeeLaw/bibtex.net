using System;
using System.Collections;
using System.Collections.Generic;

namespace Neat.Collections
{
  /// <summary>
  /// Supports one-call enumeration.
  /// </summary>
  public interface IEnumerator2 : IEnumerator, IDisposable
  {
    /// <summary>
    /// Equivalent to <see cref="IEnumerator.MoveNext"/> and
    /// assigning <see cref="IEnumerator.Current"/> to <paramref name="item"/>
    /// if <see cref="IEnumerator.MoveNext"/> returned <see langword="true"/>.
    /// If a call to this method returns <see langword="false"/>, the enumerator is considered disposed, and
    /// further calls to any method except <see cref="IDisposable.Dispose"/> are undefined.
    /// </summary>
    /// <param name="item">The next item if there is another item.</param>
    /// <returns><see langword="true"/> if there is another item.</returns>
    bool MoveNext(out object item);
  }

  /// <summary>
  /// Supports generic one-call enumeration.
  /// </summary>
  public interface IEnumerator2<T> : IEnumerator2, IEnumerator<T>
  {
    /// <summary>
    /// Equivalent to <see cref="IEnumerator.MoveNext"/> and
    /// assigning <see cref="IEnumerator{T}.Current"/> to <paramref name="item"/>
    /// if <see cref="IEnumerator.MoveNext"/> returned <see langword="true"/>.
    /// If a call to this method returns <see langword="false"/>, the enumerator is considered disposed, and
    /// further calls to any method except <see cref="IDisposable.Dispose"/> are undefined.
    /// </summary>
    /// <param name="item">The next item if there is another item.</param>
    /// <returns><see langword="true"/> if there is another item.</returns>
    bool MoveNext(out T item);
  }
}
