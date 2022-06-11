#if BIB_MODEL_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = System.String;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a <c>@preamble{ "literal" # {literal} # 123 # name }</c> entry.
  /// </summary>
  public sealed class Bib16PreambleEntry : Bib16Entry
  {
    /// <summary>
    /// The entry type of a <c>@preamble</c> entry.
    /// </summary>
    public const StringT EntryType = "preamble";

    /// <summary>
    /// The preamble text.
    /// </summary>
    public Bib16String Text;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      bool isBrace = IsBrace;
      /* @preamble{ text } */
      return Text.ToString(new StringBuilder()
        .Append('@')
        .Append(Type.GenericToString())
        .Append(isBrace ? "{ " : "( ")
      ).Append(isBrace ? " }" : " )").ToString();
    }

    /// <param name="type">Must be <c>preamble</c> (in any casing).</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bib16PreambleEntry(StringT type, bool isBrace, Bib16String text)
      : base(type, isBrace)
    {
      Text = text;
#if BIB_MODEL_CHECKS
      CtorCheckImpl(null);
    }

    [MethodImpl(Helper.OptimizeNoInline)]
    internal void CtorCheckImpl(string name)
    {
      if (!BibBstComparer.Equals(Type, EntryType))
      {
        throw new ArgumentException("Bib16PreambleEntry: Type is not \"preamble\".", name is null ? "type" : name);
      }
      Text.CtorCheckImpl(name is null ? "text" : name);
#endif
    }

    #region Bib16Entry overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, EntryType)
        && Text.IsValid();
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBib16EntryVisitor visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib16Entry overrides
  }
}
