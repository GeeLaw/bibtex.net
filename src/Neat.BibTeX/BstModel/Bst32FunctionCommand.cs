using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a <c>FUNCTION {name} { ... }</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32FunctionCommand))]
  public sealed class Bst32FunctionCommand : Bst32Command
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return Type.GenericToString() + " {" + Name.GenericToString() + "} { ... }";
      }
    }

    /// <summary>
    /// The command type of a <c>FUNCTION</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("FUNCTION");

    /// <summary>
    /// The name of the function to be defined.
    /// This string should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// </summary>
    public StringT Name;

    /// <summary>
    /// The body of the function to be defined.
    /// This array should not be <see langword="null"/> and should contain only valid operators.
    /// The definition order should be a topological sorting of the call graph, which prohibits recursive functions.
    /// (However, <see cref="IsValid"/> does not detect recursion.)
    /// </summary>
    public Bst32Operator[] Block;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      Bst32Operator[] block = Block;
      if (block is null || block.Length == 0)
      {
        return Type.GenericToString() + " {" + Name.GenericToString() + "} {}";
      }
      StringBuilder sb = new StringBuilder();
      sb.Append(Type.GenericToString()).Append(" {").Append(Name.GenericToString()).Append("}\n{\n");
      for (int i = 0; i < block.Length; ++i)
      {
        block[i].ToString(sb, 2).Append('\n');
      }
      return sb.Append('}').ToString();
    }

    /// <param name="type">Must be <c>FUNCTION</c> (in any casing).</param>
    /// <param name="name">Must be a valid identifier.</param>
    /// <param name="block">Must not be <see langword="null"/> and must contain only valid operators.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32FunctionCommand(StringT type, StringT name, Bst32Operator[] block)
      : base(type)
    {
      Name = name;
      Block = block;
    }

    #region Bst32Command overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      if (!BibBstComparer.Equals(Type, CommandType)
        || !BibBstChars.IsIdentifier(Name))
      {
        return false;
      }
      Bst32Operator[] block = Block;
      if (block is null)
      {
        return false;
      }
      for (int i = 0; i < block.Length; ++i)
      {
        if (!block[i].IsValid())
        {
          return false;
        }
      }
      return true;
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
