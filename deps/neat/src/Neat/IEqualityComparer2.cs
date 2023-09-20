using System.Collections.Generic;

namespace Neat
{
  /// <summary>
  /// Defines methods to support the comparison of objects for equality.
  /// This interface requires that <see cref="IEqualityComparer2{T}.GetHashCode(T)"/> compute the hash code of <see langword="null"/>.
  /// </summary>
  public interface IEqualityComparer2<in T> : IEqualityComparer<T>
  {
    /// <summary>
    /// Returns a hash code for the specified object, which could be <see langword="null"/>.
    /// </summary>
    /// <param name="obj">This object is allowed to be <see langword="null"/>.</param>
    new int GetHashCode(T obj);
  }
}
