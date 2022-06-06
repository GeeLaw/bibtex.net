using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a string component, which is either a reference to
  /// a macro defined in <c>.bst</c> files (<c>MACRO { ... } { ... }</c>) or
  /// a string defined by <see cref="BibStringEntry{TString}"/> (<c>@string { ... }</c>),
  /// or a literal string value.
  /// </summary>
  public readonly struct BibStringComponent<TString>
  {
    /// <summary>
    /// Indicates whether this component is a literal string value.
    /// </summary>
    public readonly bool IsValue;

    /// <summary>
    /// The key to the referenced string (if this component is a reference) or
    /// the literal string value (if this component is a literal string value).
    /// </summary>
    public readonly TString KeyOrValue;

    [MethodImpl(Helper.OptimizeInline)]
    public BibStringComponent(bool isValue, TString keyOrValue)
    {
      IsValue = isValue;
      KeyOrValue = keyOrValue;
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBibStringComponentVisitor<TString>
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
    public void AcceptVisitor(IBibStringComponentVisitor<TString> visitor)
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
