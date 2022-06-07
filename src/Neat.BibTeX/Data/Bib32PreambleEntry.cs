#if BIB_DATA_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using Neat.BibTeX.Utils;
using Neat.Unicode;

namespace Neat.BibTeX.Data
{
  /// <summary>
  /// Represents a <c>@preamble{ "literal" # {literal} # 123 # reference }</c> entry.
  /// </summary>
  public sealed class Bib32PreambleEntry : Bib32Entry
  {
    public static readonly String32 EntryType = Utf.String16ToString32Strict("preamble");

    /// <summary>
    /// The preamble text.
    /// This string should be valid and brace-balanced.
    /// </summary>
    public readonly Bib32String Value;

    /// <summary>
    /// <paramref name="type"/> must be <c>preamble</c> (in any casing).
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib32PreambleEntry(String32 type, bool isBrace, Bib32String value)
      : base(type, isBrace)
    {
      Value = value;
#if BIB_DATA_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal sealed override void CtorCheckImpl(string name)
    {
      if (!BibBstComparer.Equals(Type, EntryType))
      {
        throw new ArgumentException("Bib32PreambleEntry: Type is not \"preamble\".", name is null ? "type" : name);
      }
      Value.CtorCheckImpl(name is null ? "value" : name);
#endif
    }

    #region Bib32Entry overrides

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBib32EntryVisitor visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib32Entry overrides
  }
}
