using System;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>FUNCTION</c> command.
  /// </summary>
  public sealed class Bst32FunctionCommand : Bst32Command
  {
    /// <summary>
    /// The command type of a <c>FUNCTION</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("FUNCTION");

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override string ToString()
    {
      return Type.ToString();
    }

    /// <param name="type">Must be <c>FUNCTION</c> (in any casing).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32FunctionCommand(StringT type)
      : base(type)
    {
    }

    #region Bst32Command overrides

    public sealed override bool IsValid()
    {
      throw new NotImplementedException();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitFunctionCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitFunctionCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}