using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = System.String;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a string component, which is either a name (e.g., <c>name</c>) in reference to
  /// a macro defined in <c>.bst</c> files (<c>MACRO { ... } { ... }</c>) or
  /// a string defined by <see cref="Bib16StringEntry"/> (<c>@string{ ... }</c>),
  /// or a literal (e.g., <c>{literal}</c>, <c>"literal"</c>, <c>123</c>).
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bib16StringComponent))]
  public readonly struct Bib16StringComponent
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
    /// The type of the literal.
    /// </summary>
    public readonly BibStringComponentType Type;

    /// <summary>
    /// The name of the referenced string (if this component is a name) or the literal (if this component is a literal).
    /// If this is the name, it should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// If this is the quote literal, it should not be <see langword="default"/>, should not contain <c>"</c> outside braces, and should be brace-balanced.
    /// If this is the numeric literal, it should not be <see langword="default"/> or empty and should contain only numeric characters.
    /// If this is the delimited literal, it should not be <see langword="default"/> and should be brace-balanced.
    /// </summary>
    public readonly StringT NameOrLiteral;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      switch (Type.Value)
      {
      default:
      case BibStringComponentType.InvalidValue:
        return "(invalid)";
      case BibStringComponentType.NameValue:
      case BibStringComponentType.NumericLiteralValue:
        return NameOrLiteral.GenericToString();
      case BibStringComponentType.BraceLiteralValue:
        return "{" + NameOrLiteral.GenericToString() + "}";
      case BibStringComponentType.QuoteLiteralValue:
        return "\"" + NameOrLiteral.GenericToString() + "\"";
      }
    }

    [MethodImpl(Helper.JustOptimize)]
    internal StringBuilder ToString(StringBuilder sb)
    {
      switch (Type.Value)
      {
      default:
      case BibStringComponentType.InvalidValue:
        return sb.Append("(invalid)");
      case BibStringComponentType.NameValue:
      case BibStringComponentType.NumericLiteralValue:
        return sb.Append(NameOrLiteral.GenericToString());
      case BibStringComponentType.BraceLiteralValue:
        return sb.Append('{').Append(NameOrLiteral.GenericToString()).Append('}');
      case BibStringComponentType.QuoteLiteralValue:
        return sb.Append('"').Append(NameOrLiteral.GenericToString()).Append('"');
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public Bib16StringComponent(BibStringComponentType type, StringT nameOrLiteral)
    {
      Type = type;
      NameOrLiteral = nameOrLiteral;
    }

    /// <summary>
    /// Determines whether the component is currently valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public bool IsValid()
    {
      switch (Type.Value)
      {
      default:
      case BibStringComponentType.InvalidValue:
        return false;
      case BibStringComponentType.NameValue:
        return BibBstChars.IsIdentifier(NameOrLiteral);
      case BibStringComponentType.QuoteLiteralValue:
        return BibBstChars.IsQuoteLiteral(NameOrLiteral);
      case BibStringComponentType.NumericLiteralValue:
        return BibBstChars.IsNumericLiteral(NameOrLiteral);
      case BibStringComponentType.BraceLiteralValue:
        return BibBstChars.IsBraceBalanced(NameOrLiteral);
      }
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib16StringComponentVisitor
    {
      switch (Type.Value)
      {
      default:
      case BibStringComponentType.InvalidValue:
        visitor.VisitInvalid(this);
        return;
      case BibStringComponentType.NameValue:
        visitor.VisitName(NameOrLiteral);
        return;
      case BibStringComponentType.QuoteLiteralValue:
        visitor.VisitQuoteLiteral(NameOrLiteral);
        return;
      case BibStringComponentType.NumericLiteralValue:
        visitor.VisitNumericLiteral(NameOrLiteral);
        return;
      case BibStringComponentType.BraceLiteralValue:
        visitor.VisitBraceLiteral(NameOrLiteral);
        return;
      }
    }

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor(IBib16StringComponentVisitor visitor)
    {
      switch (Type.Value)
      {
      default:
      case BibStringComponentType.InvalidValue:
        visitor.VisitInvalid(this);
        return;
      case BibStringComponentType.NameValue:
        visitor.VisitName(NameOrLiteral);
        return;
      case BibStringComponentType.QuoteLiteralValue:
        visitor.VisitQuoteLiteral(NameOrLiteral);
        return;
      case BibStringComponentType.NumericLiteralValue:
        visitor.VisitNumericLiteral(NameOrLiteral);
        return;
      case BibStringComponentType.BraceLiteralValue:
        visitor.VisitBraceLiteral(NameOrLiteral);
        return;
      }
    }
  }
}
