using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents an operator (function body definition token) in BibTeX style function definition.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32Operator))]
  public readonly struct Bst32Operator
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        switch (Type.Value)
        {
        default:
        case BstOperatorType.InvalidValue:
          return "(invalid)";
        case BstOperatorType.IntegerLiteralValue:
          return IntegerValue.ToString("\\#0;\\#-0", CultureInfo.InvariantCulture);
        case BstOperatorType.StringLiteralValue:
          return "\"" + NameOrLiteral.GenericToString() + "\"";
        case BstOperatorType.ReferenceValue:
          return "'" + NameOrLiteral.GenericToString();
        case BstOperatorType.InvocationValue:
          return NameOrLiteral.GenericToString();
        case BstOperatorType.BlockValue:
          return "{ ... }";
        }
      }
    }

    /// <summary>
    /// Indicates the type of this operator.
    /// </summary>
    public readonly BstOperatorType Type;

    /// <summary>
    /// If <see cref="Type"/> is <see cref="BstOperatorType.IntegerLiteral"/>,
    /// this field is the value of this integer literal.
    /// </summary>
    public readonly int IntegerValue;

    /// <summary>
    /// If <see cref="Type"/> is <see cref="BstOperatorType.StringLiteral"/>,
    /// this field is the value of this string literal (should not contain <c>\r</c>, <c>\n</c>, or <c>"</c>).
    /// If <see cref="Type"/> is <see cref="BstOperatorType.Reference"/>,
    /// this field is the name of the referenced function (should be a valid identifier).
    /// If <see cref="Type"/> is <see cref="BstOperatorType.Invocation"/>,
    /// this field is the name of the function to invoke (should be a valid identifier).
    /// </summary>
    public readonly StringT NameOrLiteral;

    /// <summary>
    /// If <see cref="Type"/> is <see cref="BstOperatorType.Block"/>,
    /// this field is a non-<see langword="null"/> array of the constituents of the block.
    /// </summary>
    public readonly Bst32Operator[] Block;

    /* Technically, it is possible to have overlapping "NameOrLiteral" and "Block",
    /* as both, when unwrapped, are ultimately just one object reference.
    /* However, it ruins type safety if "Bst32Operator" is accessed from multiple threads due to tearing. */

    /// <summary>
    /// Initializes an integer literal operator.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32Operator(int integerValue)
    {
      Type = new BstOperatorType(BstOperatorType.IntegerLiteralValue);
      IntegerValue = integerValue;
      NameOrLiteral = default(StringT);
      Block = null;
    }

    /// <summary>
    /// Initializes a string, reference, or invocation operator.
    /// </summary>
    /// <param name="type">Must be one of <see cref="BstOperatorType.StringLiteral"/>, <see cref="BstOperatorType.Reference"/>, <see cref="BstOperatorType.Invocation"/>.</param>
    /// <param name="nameOrLiteral">If <paramref name="type"/> is <see cref="BstOperatorType.StringLiteral"/>, this argument must not contain <c>\r</c>, <c>\n</c>, or <c>"</c>.
    /// Otherwise, <paramref name="type"/> is <see cref="BstOperatorType.Reference"/> or <see cref="BstOperatorType.Invocation"/>, and
    /// this argument must be a valid identifier.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32Operator(BstOperatorType type, StringT nameOrLiteral)
    {
      Type = type;
      IntegerValue = 0;
      NameOrLiteral = nameOrLiteral;
      Block = null;
    }

    /// <summary>
    /// Initializes a block operator.
    /// </summary>
    /// <param name="block">Must not be <see langword="null"/> and must contain only valid operators.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32Operator(Bst32Operator[] block)
    {
      Type = new BstOperatorType(BstOperatorType.BlockValue);
      IntegerValue = 0;
      NameOrLiteral = default(StringT);
      Block = block;
    }

    /// <summary>
    /// Determines whether the operator is currently valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public bool IsValid()
    {
      switch (Type.Value)
      {
      default:
      case BstOperatorType.InvalidValue:
        return false;
      case BstOperatorType.IntegerLiteralValue:
        return true;
      case BstOperatorType.StringLiteralValue:
        return BibBstChars.IsStringLiteral(NameOrLiteral);
      case BstOperatorType.ReferenceValue:
      case BstOperatorType.InvocationValue:
        return BibBstChars.IsIdentifier(NameOrLiteral);
      case BstOperatorType.BlockValue:
        Bst32Operator[] block = Block;
        for (int i = 0; i < block.Length; ++i)
        {
          if (!block[i].IsValid())
          {
            return false;
          }
        }
        return true;
      }
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBst32OperatorVisitor
    {
      switch (Type.Value)
      {
      default:
      case BstOperatorType.InvalidValue:
        visitor.VisitInvalid(this);
        return;
      case BstOperatorType.IntegerLiteralValue:
        visitor.VisitInteger(IntegerValue);
        return;
      case BstOperatorType.StringLiteralValue:
        visitor.VisitString(NameOrLiteral);
        return;
      case BstOperatorType.ReferenceValue:
        visitor.VisitReference(NameOrLiteral);
        return;
      case BstOperatorType.InvocationValue:
        visitor.VisitInvocation(NameOrLiteral);
        return;
      case BstOperatorType.BlockValue:
        visitor.VisitBlock(Block);
        return;
      }
    }

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor(IBst32OperatorVisitor visitor)
    {
      switch (Type.Value)
      {
      default:
      case BstOperatorType.InvalidValue:
        visitor.VisitInvalid(this);
        return;
      case BstOperatorType.IntegerLiteralValue:
        visitor.VisitInteger(IntegerValue);
        return;
      case BstOperatorType.StringLiteralValue:
        visitor.VisitString(NameOrLiteral);
        return;
      case BstOperatorType.ReferenceValue:
        visitor.VisitReference(NameOrLiteral);
        return;
      case BstOperatorType.InvocationValue:
        visitor.VisitInvocation(NameOrLiteral);
        return;
      case BstOperatorType.BlockValue:
        visitor.VisitBlock(Block);
        return;
      }
    }

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      switch (Type.Value)
      {
      default:
      case BstOperatorType.InvalidValue:
        return "(invalid)";
      case BstOperatorType.IntegerLiteralValue:
        return IntegerValue.ToString("\\#0;\\#-0", CultureInfo.InvariantCulture);
      case BstOperatorType.StringLiteralValue:
        return "\"" + NameOrLiteral.GenericToString() + "\"";
      case BstOperatorType.ReferenceValue:
        return "'" + NameOrLiteral.GenericToString();
      case BstOperatorType.InvocationValue:
        return NameOrLiteral.GenericToString();
      case BstOperatorType.BlockValue:
        return ToStringHelper(new StringBuilder(), 0, Block).ToString();
      }
    }

    [MethodImpl(Helper.JustOptimize)]
    private static StringBuilder ToStringHelper(StringBuilder sb, int indentation, Bst32Operator[] block)
    {
      if (block is null || block.Length == 0)
      {
        return sb.Append(' ', indentation).Append("{}");
      }
      sb.Append(' ', indentation).Append("{\n");
      indentation += 2;
      for (int i = 0; i < block.Length; ++i)
      {
        block[i].ToString(sb, indentation).Append('\n');
      }
      return sb.Append(' ', indentation - 2).Append('}');
    }

    [MethodImpl(Helper.JustOptimize)]
    internal StringBuilder ToString(StringBuilder sb, int indentation)
    {
      switch (Type.Value)
      {
      default:
      case BstOperatorType.InvalidValue:
        return sb.Append("(invalid)");
      case BstOperatorType.IntegerLiteralValue:
        return sb.Append(' ', indentation).Append('#').Append(IntegerValue.ToString(CultureInfo.InvariantCulture));
      case BstOperatorType.StringLiteralValue:
        return sb.Append(' ', indentation).Append('"').Append(NameOrLiteral.GenericToString()).Append('"');
      case BstOperatorType.ReferenceValue:
        return sb.Append(' ', indentation).Append('\'').Append(NameOrLiteral.GenericToString());
      case BstOperatorType.InvocationValue:
        return sb.Append(' ', indentation).Append(NameOrLiteral.GenericToString());
      case BstOperatorType.BlockValue:
        return ToStringHelper(sb, indentation, Block);
      }
    }
  }
}
