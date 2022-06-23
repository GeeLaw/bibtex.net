using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>STRINGS { ... }</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32StringsCommand))]
  public sealed class Bst32StringsCommand : Bst32Command
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return Type.GenericToString() + " { ... }";
      }
    }

    /// <summary>
    /// The command type of a <c>STRINGS</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("STRINGS");

    /// <summary>
    /// The names of string variables.
    /// This array should not be <see langword="null"/>, should contain only valid identifiers, and should not contain duplicates.
    /// </summary>
    public StringT[] VariableNames;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      return ToStringHelper(new StringBuilder().Append(Type.GenericToString()).Append('\n'), 2, VariableNames).ToString();
    }

    /// <param name="type">Must be <c>STRINGS</c> (in any casing).</param>
    /// <param name="variableNames">Must not be <see langword="null"/>, must contain only valid identifiers, and must not contain duplicates.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32StringsCommand(StringT type, StringT[] variableNames)
      : base(type)
    {
      VariableNames = variableNames;
    }

    #region Bst32Command overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, CommandType)
        && IsNotNullAndContainsOnlyIdentifiersAndContainsNoDuplicates(VariableNames);
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
