using System.Diagnostics;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>MACRO {name} {"value"}</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32MacroCommand))]
  public sealed class Bst32MacroCommand : Bst32Command
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
    /// The command type of a <c>MACRO</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("MACRO");

    /// <summary>
    /// The name of the macro.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public StringT Name;

    /// <summary>
    /// The value of the macro.
    /// This string should be a valid quote-delimited literal.
    /// </summary>
    public StringT Value;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      return Type.GenericToString() + " {" + Name.GenericToString() + "} {\"" + Value.GenericToString() + "\"}";
    }

    /// <param name="type">Must be <c>MACRO</c> (in any casing).</param>
    /// <param name="name">Must be a valid identifer.</param>
    /// <param name="value">Must be a valid quote-delimited literal.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32MacroCommand(StringT type, StringT name, StringT value)
      : base(type)
    {
      Name = name;
      Value = value;
    }

    #region Bst32Command overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, CommandType)
        && BibBstChars.IsIdentifier(Name)
        && BibBstChars.IsQuoteLiteral(Value);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitMacroCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitMacroCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
