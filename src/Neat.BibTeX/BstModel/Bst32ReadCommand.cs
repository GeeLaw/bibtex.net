using System.Diagnostics;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>READ</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32ReadCommand))]
  public sealed class Bst32ReadCommand : Bst32Command
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return ToString();
      }
    }

    /// <summary>
    /// The command type of a <c>READ</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("READ");

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override string ToString()
    {
      return Type.GenericToString();
    }

    /// <param name="type">Must be <c>READ</c> (in any casing).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32ReadCommand(StringT type)
      : base(type)
    {
    }

    #region Bst32Command overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, CommandType);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitReadCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitReadCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
