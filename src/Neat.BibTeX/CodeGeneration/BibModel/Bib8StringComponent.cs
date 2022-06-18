using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String8;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a string component, which is either a name (e.g., <c>name</c>) in reference to
  /// a macro defined in <c>.bst</c> files (<c>MACRO { ... } { ... }</c>) or
  /// a string defined by <see cref="Bib8StringEntry"/> (<c>@string{ ... }</c>),
  /// or a literal (e.g., <c>{literal}</c>, <c>"literal"</c>, <c>123</c>).
  /// </summary>
  public readonly struct Bib8StringComponent
  {
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
      byte type = Type.Value;
      return type == BibStringComponentType.BraceLiteralValue
        ? "{" + NameOrLiteral.GenericToString() + "}"
        : type == BibStringComponentType.QuoteLiteralValue
        ? "\"" + NameOrLiteral.GenericToString() + "\""
        : NameOrLiteral.GenericToString();
    }

    [MethodImpl(Helper.JustOptimize)]
    internal StringBuilder ToString(StringBuilder sb)
    {
      byte type = Type.Value;
      return type == BibStringComponentType.BraceLiteralValue
        ? sb.Append('{').Append(NameOrLiteral.GenericToString()).Append('}')
        : type == BibStringComponentType.QuoteLiteralValue
        ? sb.Append('"').Append(NameOrLiteral.GenericToString()).Append('"')
        : sb.Append(NameOrLiteral.GenericToString());
    }

    [MethodImpl(Helper.OptimizeInline)]
    public Bib8StringComponent(BibStringComponentType type, StringT nameOrLiteral)
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
      StringT nameOrLiteral = NameOrLiteral;
      switch (Type.Value)
      {
      case BibStringComponentType.NameValue:
        return BibBstChars.IsIdentifier(nameOrLiteral);
      case BibStringComponentType.QuoteLiteralValue:
        return BibBstChars.IsQuoteLiteral(nameOrLiteral);
      case BibStringComponentType.NumericLiteralValue:
        return BibBstChars.IsNumericLiteral(nameOrLiteral);
      case BibStringComponentType.BraceLiteralValue:
        return BibBstChars.IsBraceBalanced(nameOrLiteral);
      default:
        return false;
      }
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib8StringComponentVisitor
    {
      switch (Type.Value)
      {
      default:
        visitor.VisitName(this);
        break;
      case BibStringComponentType.QuoteLiteralValue:
        visitor.VisitQuoteLiteral(this);
        break;
      case BibStringComponentType.NumericLiteralValue:
        visitor.VisitNumericLiteral(this);
        break;
      case BibStringComponentType.BraceLiteralValue:
        visitor.VisitBraceLiteral(this);
        break;
      }
    }

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor(IBib8StringComponentVisitor visitor)
    {
      switch (Type.Value)
      {
      default:
        visitor.VisitName(this);
        break;
      case BibStringComponentType.QuoteLiteralValue:
        visitor.VisitQuoteLiteral(this);
        break;
      case BibStringComponentType.NumericLiteralValue:
        visitor.VisitNumericLiteral(this);
        break;
      case BibStringComponentType.BraceLiteralValue:
        visitor.VisitBraceLiteral(this);
        break;
      }
    }
  }
}
