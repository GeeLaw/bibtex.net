using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a command in <c>.bst</c> files.
  /// </summary>
  public abstract class Bst32Command
  {
    /// <summary>
    /// Indicates the type of this entry.
    /// This string should be compared by <see cref="BibBstComparer"/>.
    /// It can only be one of the following:
    /// <c>ENTRY</c>, <c>INTEGERS</c>, <c>STRINGS</c>, <c>MACRO</c>, <c>FUNCTION</c>,
    /// <c>READ</c>, <c>EXECUTE</c>, <c>ITERATE</c>, <c>SORT</c>, <c>REVERSE</c>.
    /// </summary>
    public readonly StringT Type;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    public abstract override string ToString();

    [MethodImpl(Helper.OptimizeInline)]
    private protected Bst32Command(StringT type)
    {
      Type = type;
    }

    /// <summary>
    /// Determines whether the command is currently valid.
    /// </summary>
    public abstract bool IsValid();

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    public abstract void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBst32CommandVisitor;

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    public abstract void AcceptVisitor(IBst32CommandVisitor visitor);

    /// <summary>
    /// This method ensures only this assembly can create non-abstract derived classes of this class.
    /// </summary>
    private protected abstract void InternalInheritance();
  }
}
