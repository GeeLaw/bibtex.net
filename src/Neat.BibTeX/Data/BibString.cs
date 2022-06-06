using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a string that is a concatenation of referenced strings and literal string values.
  /// This type avoids allocation for the most common case of single-component strings.
  /// </summary>
  public readonly struct BibString<TString>
  {
    /// <summary>
    /// If <see cref="Components"/> is <see langword="null"/>, then this field is the only component.
    /// Otherwise, this field is <see langword="default"/> and not meaningful.
    /// </summary>
    public readonly BibStringComponent<TString> OnlyComponent;

    /// <summary>
    /// If this field is <see langword="null"/>, then this string consists of one component, <see cref="OnlyComponent"/>.
    /// Otherwise, this field contains all the components, and <see cref="OnlyComponent"/> is <see langword="default"/>.
    /// </summary>
    public readonly BibStringComponent<TString>[] Components;

    /// <summary>
    /// Initializes a single-component string.
    /// This constructor does not validate its argument.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public BibString(BibStringComponent<TString> onlyComponent)
    {
      OnlyComponent = onlyComponent;
      Components = null;
    }

    /// <summary>
    /// Initializes a multi-component string (at least one component).
    /// This constructor does not validate its argument.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public BibString(BibStringComponent<TString>[] components)
    {
      OnlyComponent = default(BibStringComponent<TString>);
      Components = components;
    }

    /// <summary>
    /// Dispatches the correct method on each component for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBibStringComponentVisitor<TString>
    {
      BibStringComponent<TString>[] components = Components;
      if (components is null)
      {
        OnlyComponent.AcceptVisitor(ref visitor);
      }
      else
      {
        for (int i = 0; i < components.Length; ++i)
        {
          components[i].AcceptVisitor(ref visitor);
        }
      }
    }

    /// <summary>
    /// Dispatches the correct method on each component for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor(IBibStringComponentVisitor<TString> visitor)
    {
      BibStringComponent<TString>[] components = Components;
      if (components is null)
      {
        OnlyComponent.AcceptVisitor(visitor);
      }
      else
      {
        for (int i = 0; i < components.Length; ++i)
        {
          components[i].AcceptVisitor(visitor);
        }
      }
    }
  }
}
