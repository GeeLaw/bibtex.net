#if BIB_DATA_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a string component, which is either a name (e.g., <c>name</c>) in reference to
  /// a macro defined in <c>.bst</c> files (<c>MACRO { ... } { ... }</c>) or
  /// a string defined by <see cref="Bib32StringEntry"/> (<c>@string{ ... }</c>),
  /// or a literal (e.g., <c>{literal}</c>, <c>"literal"</c>, <c>123</c>).
  /// </summary>
  public readonly struct Bib32StringComponent
  {
    /// <summary>
    /// Indicates whether this component is a literal.
    /// </summary>
    public readonly bool IsLiteral;

    /// <summary>
    /// The name of the referenced string (if this component is a name) or the literal (if this component is a literal).
    /// If this is the name, it should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// If this is the literal, it should be valid and brace-balanced.
    /// </summary>
    public readonly String32 NameOrLiteral;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      /* {literal} or name */
      return IsLiteral
        ? "{" + NameOrLiteral.ToString() + "}"
        : NameOrLiteral.ToString();
    }

    [MethodImpl(Helper.JustOptimize)]
    internal StringBuilder ToString(StringBuilder sb)
    {
      return IsLiteral
        ? sb.Append('{').Append(NameOrLiteral.ToString()).Append('}')
        : sb.Append(NameOrLiteral.ToString());
    }

    /// <param name="nameOrLiteral">If <paramref name="isLiteral"/>, this value must be a valid identifier.
    /// Otherwise, this value must be brace-balanced.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32StringComponent(bool isLiteral, String32 nameOrLiteral)
    {
      IsLiteral = isLiteral;
      NameOrLiteral = nameOrLiteral;
#if BIB_DATA_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      name = (name is null ? "nameOrLiteral" : name);
      if (IsLiteral)
      {
        if (!BibBstChars.IsBraceBalanced(NameOrLiteral))
        {
          throw new ArgumentException("Bib32StringComponent: NameOrLiteral (literal) is not brace-balanced.", name);
        }
      }
      else
      {
        if (!BibBstChars.IsIdentifier(NameOrLiteral))
        {
          throw new ArgumentException("Bib32StringComponent: NameOrLiteral (name) is a not valid identifier.", name);
        }
      }
#endif
    }

    /// <summary>
    /// Determines whether the component is currently valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public bool IsValid()
    {
      return IsLiteral
        ? BibBstChars.IsBraceBalanced(NameOrLiteral)
        : BibBstChars.IsIdentifier(NameOrLiteral);
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib32StringComponentVisitor
    {
      if (IsLiteral)
      {
        visitor.VisitLiteral(NameOrLiteral);
      }
      else
      {
        visitor.VisitName(NameOrLiteral);
      }
    }

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void AcceptVisitor(IBib32StringComponentVisitor visitor)
    {
      if (IsLiteral)
      {
        visitor.VisitLiteral(NameOrLiteral);
      }
      else
      {
        visitor.VisitName(NameOrLiteral);
      }
    }
  }
}
