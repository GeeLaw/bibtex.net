using System;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>ENTRY</c> command.
  /// </summary>
  public sealed class Bst32EntryCommand : Bst32Command
  {
    /// <summary>
    /// The command type of a <c>ENTRY</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("ENTRY");

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override string ToString()
    {
      return Type.ToString();
    }

    /// <param name="type">Must be <c>ENTRY</c> (in any casing).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32EntryCommand(StringT type)
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
      visitor.VisitEntryCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitEntryCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
