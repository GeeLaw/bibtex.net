using System.Diagnostics;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents an <c>ITERATE {skip$}</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32IterateCommand))]
  public sealed class Bst32IterateCommand : Bst32Command
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
    /// The command type of a <c>ITERATE</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("ITERATE");

    /// <summary>
    /// The name of the function to be executed for each entry.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public StringT Name;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      return Type.GenericToString() + " {" + Name.GenericToString() + "}";
    }

    /// <param name="type">Must be <c>ITERATE</c> (in any casing).</param>
    /// <param name="name">Must be a valid identifer.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32IterateCommand(StringT type, StringT name)
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
      visitor.VisitIterateCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitIterateCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
