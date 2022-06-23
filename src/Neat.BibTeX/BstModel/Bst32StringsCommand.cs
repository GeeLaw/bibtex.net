using System;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>STRINGS</c> command.
  /// </summary>
  public sealed class Bst32StringsCommand : Bst32Command
  {
    /// <summary>
    /// The command type of a <c>STRINGS</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("STRINGS");

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override string ToString()
    {
      return Type.ToString();
    }

    /// <param name="type">Must be <c>STRINGS</c> (in any casing).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32StringsCommand(StringT type)
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
      visitor.VisitStringsCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitStringsCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
