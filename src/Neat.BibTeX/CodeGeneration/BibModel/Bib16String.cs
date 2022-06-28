using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a string that is a concatenation of referenced strings and literals (e.g., <c>"literal" # {literal} # 123 # name</c>).
  /// This type avoids allocation for the most common case of single-component strings.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bib16String))]
  public readonly struct Bib16String
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
    /// If <see cref="Components"/> is <see langword="null"/>, then this field is the only component.
    /// Otherwise, this field is <see langword="default"/> and not meaningful.
    /// </summary>
    public readonly Bib16StringComponent OnlyComponent;

    /// <summary>
    /// If this field is <see langword="null"/>, then this string consists of one component, <see cref="OnlyComponent"/>.
    /// Otherwise, this field contains all the components, and <see cref="OnlyComponent"/> is <see langword="default"/>.
    /// This array cannot be non-<see langword="null"/> but have less than 2 items.
    /// </summary>
    public readonly Bib16StringComponent[] Components;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      Bib16StringComponent[] components = Components;
      if (components is null)
      {
        return OnlyComponent.ToString();
      }
      /* component1 # component2 # ... */
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < components.Length; ++i)
      {
        components[i].ToString(i == 0 ? sb : sb.Append(" # "));
      }
      return sb.ToString();
    }

    [MethodImpl(Helper.JustOptimize)]
    internal StringBuilder ToString(StringBuilder sb)
    {
      Bib16StringComponent[] components = Components;
      if (components is null)
      {
        return OnlyComponent.ToString(sb);
      }
      for (int i = 0; i < components.Length; ++i)
      {
        components[i].ToString(i == 0 ? sb : sb.Append(" # "));
      }
      return sb;
    }

    /// <summary>
    /// Initializes a single-component string.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib16String(Bib16StringComponent onlyComponent)
    {
      OnlyComponent = onlyComponent;
      Components = null;
    }

    /// <summary>
    /// Initializes a multi-component string (at least 2 components).
    /// </summary>
    /// <param name="components">Must not be <see langword="null"/> or contain less than 2 items.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib16String(Bib16StringComponent[] components)
    {
      OnlyComponent = default(Bib16StringComponent);
      Components = components;
    }

    /// <summary>
    /// Determines whether the string is currently valid.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public bool IsValid()
    {
      Bib16StringComponent[] components = Components;
      if (components is null)
      {
        return OnlyComponent.IsValid();
      }
      else if (components.Length < 2)
      {
        return false;
      }
      else
      {
        for (int i = 0; i < components.Length; ++i)
        {
          if (!components[i].IsValid())
          {
            return false;
          }
        }
        return true;
      }
    }

    /// <summary>
    /// Dispatches the correct method on each component for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBib16StringComponentVisitor
    {
      Bib16StringComponent[] components = Components;
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
    public void AcceptVisitor(IBib16StringComponentVisitor visitor)
    {
      Bib16StringComponent[] components = Components;
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
