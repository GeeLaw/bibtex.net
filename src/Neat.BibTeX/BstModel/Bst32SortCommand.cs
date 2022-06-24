using System.Diagnostics;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>SORT</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32SortCommand))]
  public sealed class Bst32SortCommand : Bst32Command
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
    /// The command type of a <c>SORT</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("SORT");

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override string ToString()
    {
      return Type.GenericToString();
    }

    /// <param name="type">Must be <c>SORT</c> (in any casing).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32SortCommand(StringT type)
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
      visitor.VisitSortCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitSortCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
