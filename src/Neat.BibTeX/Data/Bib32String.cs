#if BIB_DATA_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a string that is a concatenation of referenced strings and literal string values (e.g., <c>"literal" # {literal} # 123 # reference</c>).
  /// This type avoids allocation for the most common case of single-component strings.
  /// </summary>
  public readonly struct Bib32String
  {
    /// <summary>
    /// If <see cref="Components"/> is <see langword="null"/>, then this field is the only component.
    /// Otherwise, this field is <see langword="default"/> and not meaningful.
    /// </summary>
    public readonly Bib32StringComponent OnlyComponent;

    /// <summary>
    /// If this field is <see langword="null"/>, then this string consists of one component, <see cref="OnlyComponent"/>.
    /// Otherwise, this field contains all the components, and <see cref="OnlyComponent"/> is <see langword="default"/>.
    /// This array cannot be non-<see langword="null"/> empty.
    /// </summary>
    public readonly Bib32StringComponent[] Components;

    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      Bib32StringComponent[] components = Components;
      if (components is null)
      {
        return OnlyComponent.ToString();
      }
      /* component1 # component2 # ... */
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < components.Length; ++i)
      {
        if (i != 0)
        {
          sb.Append(" # ");
        }
        components[i].ToString(sb);
      }
      return sb.ToString();
    }

    [MethodImpl(Helper.JustOptimize)]
    internal StringBuilder ToString(StringBuilder sb)
    {
      Bib32StringComponent[] components = Components;
      if (components is null)
      {
        return OnlyComponent.ToString(sb);
      }
      for (int i = 0; i < components.Length; ++i)
      {
        if (i != 0)
        {
          sb.Append(" # ");
        }
        components[i].ToString(sb);
      }
      return sb;
    }

    /// <summary>
    /// Initializes a single-component string.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32String(Bib32StringComponent onlyComponent)
    {
      OnlyComponent = onlyComponent;
      Components = null;
#if BIB_DATA_CHECKS
      CtorCheckImpl("onlyComponent");
#endif
    }

    /// <summary>
    /// Initializes a multi-component string (at least one component).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32String(Bib32StringComponent[] components)
    {
      OnlyComponent = default(Bib32StringComponent);
      Components = components;
#if BIB_DATA_CHECKS
      CtorCheckImpl("components");
#endif
    }

#if BIB_DATA_CHECKS

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      Bib32StringComponent[] components = Components;
      if (components is null)
      {
        OnlyComponent.CtorCheckImpl(name is null ? "(unknown)" : name);
      }
      else
      {
        name = (name is null ? "components" : name);
        if (components.Length == 0)
        {
          throw new ArgumentException("Bib32String: Components is empty.", name);
        }
        else
        {
          for (int i = 0; i < components.Length; ++i)
          {
            components[i].CtorCheckImpl(name);
          }
        }
      }
    }

#endif

    /// <summary>
    /// Dispatches the correct method on each component for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib32StringComponentVisitor
    {
      Bib32StringComponent[] components = Components;
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
    public void AcceptVisitor(IBib32StringComponentVisitor visitor)
    {
      Bib32StringComponent[] components = Components;
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
