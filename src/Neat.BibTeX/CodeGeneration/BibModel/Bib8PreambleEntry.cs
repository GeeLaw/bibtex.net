#if BIB_MODEL_CHECKS
using System;
#endif
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String8;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a <c>@preamble{ "literal" # {literal} # 123 # name }</c> entry.
  /// </summary>
  public sealed class Bib8PreambleEntry : Bib8Entry
  {
    /// <summary>
    /// The entry type of a <c>@preamble</c> entry.
    /// </summary>
    public static readonly StringT EntryType = Utf.String16ToString8Strict("preamble");

    /// <summary>
    /// The preamble text.
    /// </summary>
    public Bib8String Text;

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
    public Bib8PreambleEntry(StringT type, bool isBrace, Bib8String text)
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
        throw new ArgumentException("Bib8PreambleEntry: Type is not \"preamble\".", name is null ? "type" : name);
      }
      Text.CtorCheckImpl(name is null ? "text" : name);
#endif
    }

    #region Bib8Entry overrides

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
    public sealed override void AcceptVisitor(IBib8EntryVisitor visitor)
    {
      visitor.VisitPreambleEntry(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bib8Entry overrides
  }
}
