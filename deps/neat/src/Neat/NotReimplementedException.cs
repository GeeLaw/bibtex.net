using System;

namespace Neat
{
  /// <summary>
  /// Marks an explicit interface member implementation as <see langword="abstract"/>.
  /// </summary>
  public sealed class NotReimplementedException : NotImplementedException
  {
    public NotReimplementedException()
      : base("This interface member should have been reimplemented.")
    {
    }
  }
}
