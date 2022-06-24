using System.Diagnostics;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents an <c>EXECUTE {skip$}</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32ExecuteCommand))]
  public sealed class Bst32ExecuteCommand : Bst32Command
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
    /// The command type of a <c>EXECUTE</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("EXECUTE");

    /// <summary>
    /// The name of the function to be executed.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public StringT Name;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      return Type.GenericToString() + " {" + Name.GenericToString() + "}";
    }

    /// <param name="type">Must be <c>EXECUTE</c> (in any casing).</param>
    /// <param name="name">Must be a valid identifer.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32ExecuteCommand(StringT type, StringT name)
      : base(type)
    {
      Name = name;
    }

    #region Bst32Command overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, CommandType)
        && BibBstChars.IsIdentifier(Name);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitExecuteCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitExecuteCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
