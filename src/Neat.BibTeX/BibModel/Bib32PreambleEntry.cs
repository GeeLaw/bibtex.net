using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents a <c>@preamble{ "literal" # {literal} # 123 # name }</c> entry.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bib32PreambleEntry))]
  public sealed class Bib32PreambleEntry : Bib32Entry
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
    /// The entry type of a <c>@preamble</c> entry.
    /// </summary>
    public static readonly StringT EntryType = Utf.String16ToString32Strict("preamble");

    /// <summary>
    /// The preamble text.
    /// </summary>
    public Bib32String Text;

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
    public Bib32PreambleEntry(StringT type, bool isBrace, Bib32String text)
      : base(type, isBrace)
    {
      Text = text;
    }

    #region Bib32Entry overrides

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
