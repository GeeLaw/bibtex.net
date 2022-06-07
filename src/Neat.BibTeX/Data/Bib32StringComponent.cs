#if BIB_DATA_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a string component, which is either a reference to
  /// a macro defined in <c>.bst</c> files (<c>MACRO { ... } { ... }</c>) or
  /// a string defined by <see cref="Bib32StringEntry"/> (<c>@string{ ... }</c>),
  /// or a literal string value (e.g., <c>reference</c>, <c>{literal}</c>, <c>"literal"</c>, <c>123</c>).
  /// </summary>
  public readonly struct Bib32StringComponent
  {
    /// <summary>
    /// Indicates whether this component is a literal string value.
    /// </summary>
    public readonly bool IsValue;

    /// <summary>
    /// The key to the referenced string (if this component is a reference) or
    /// the literal string value (if this component is a literal string value).
    /// If this is the key, it should be a valid identifier and should be compared by <see cref="BibBstComparer"/>.
    /// If this is the value, it should be valid and brace-balanced.
    /// </summary>
    public readonly String32 KeyOrValue;

    [MethodImpl(Helper.OptimizeInline)]
    public Bib32StringComponent(bool isValue, String32 keyOrValue)
    {
      IsValue = isValue;
      KeyOrValue = keyOrValue;
#if BIB_DATA_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      name = (name is null ? "keyOrValue" : name);
      if (IsValue)
      {
        if (!BibBstChars.IsValidAndBraceBalanced(KeyOrValue))
        {
          throw new ArgumentException("Bib32StringComponent: KeyOrValue (value) is not both valid and brace-balanced.", name);
        }
      }
      else
      {
        if (!BibBstChars.IsIdentifier(KeyOrValue))
        {
          throw new ArgumentException("Bib32StringComponent:KeyOrValue (key) is a not valid identifier.", name);
        }
      }
#endif
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib32StringComponentVisitor
    {
      if (IsValue)
      {
        visitor.VisitValue(KeyOrValue);
      }
      else
      {
        visitor.VisitReference(KeyOrValue);
      }
    }

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void AcceptVisitor(IBib32StringComponentVisitor visitor)
    {
      if (IsValue)
      {
        visitor.VisitValue(KeyOrValue);
      }
      else
      {
        visitor.VisitReference(KeyOrValue);
      }
    }
  }
}
