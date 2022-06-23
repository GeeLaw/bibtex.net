using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;
using Utf = Neat.Unicode.Utf;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents an <c>ENTRY { ... } { ... } { ... }</c> command.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32EntryCommand))]
  public sealed class Bst32EntryCommand : Bst32Command
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return Type.GenericToString() + " { ... } { ... } { ... }";
      }
    }

    /// <summary>
    /// The command type of a <c>ENTRY</c> command.
    /// </summary>
    public static readonly StringT CommandType = Utf.String16ToString32Strict("ENTRY");

    /// <summary>
    /// The names of fields that are defined.
    /// This array should not be <see langword="null"/>, contain only valid identifiers, and
    /// there should be no duplicates among <see cref="Fields"/>, <see cref="Integers"/>, and <see cref="Strings"/>.
    /// </summary>
    public StringT[] Fields;

    /// <summary>
    /// The names of integer variables defined for each entry.
    /// This array should not be <see langword="null"/>, contain only valid identifiers, and
    /// there should be no duplicates among <see cref="Fields"/>, <see cref="Integers"/>, and <see cref="Strings"/>.
    /// </summary>
    public StringT[] Integers;

    /// <summary>
    /// The names of string variables defined for each entry.
    /// This array should not be <see langword="null"/>, contain only valid identifiers, and
    /// there should be no duplicates among <see cref="Fields"/>, <see cref="Integers"/>, and <see cref="Strings"/>.
    /// </summary>
    public StringT[] Strings;

    [MethodImpl(Helper.JustOptimize)]
    public sealed override string ToString()
    {
      return ToStringHelper(
        ToStringHelper(
          ToStringHelper(
            new StringBuilder().Append(Type.GenericToString()).Append('\n'),
          2, Fields),
        2, Integers),
      2, Strings).ToString();
    }

    /// <param name="type">Must be <c>ENTRY</c> (in any casing).</param>
    /// <param name="fields">Must not be <see langword="null"/>, must contain only valid identifiers, and there must not be duplicates among <paramref name="fields"/>, <paramref name="integers"/>, and <paramref name="strings"/>.</param>
    /// <param name="integers">Must not be <see langword="null"/>, must contain only valid identifiers, and there must not be duplicates among <paramref name="fields"/>, <paramref name="integers"/>, and <paramref name="strings"/>.</param>
    /// <param name="strings">Must not be <see langword="null"/>, must contain only valid identifiers, and there must not be duplicates among <paramref name="fields"/>, <paramref name="integers"/>, and <paramref name="strings"/>.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32EntryCommand(StringT type, StringT[] fields, StringT[] integers, StringT[] strings)
      : base(type)
    {
      Fields = fields;
      Integers = integers;
      Strings = strings;
    }

    #region Bst32Command overrides

    [MethodImpl(Helper.JustOptimize)]
    public sealed override bool IsValid()
    {
      return BibBstComparer.Equals(Type, CommandType)
        && IsNotNullAndContainsOnlyIdentifiersAndContainsNoDuplicates(Fields)
        && IsNotNullAndContainsOnlyIdentifiersAndContainsNoDuplicates(Integers)
        && IsNotNullAndContainsOnlyIdentifiersAndContainsNoDuplicates(Strings)
        && AreNotNullAndContainNoDuplicates(Fields, Integers)
        && AreNotNullAndContainNoDuplicates(Integers, Strings)
        && AreNotNullAndContainNoDuplicates(Strings, Fields);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor<TVisitor>(ref TVisitor visitor)
    {
      visitor.VisitEntryCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    public sealed override void AcceptVisitor(IBst32CommandVisitor visitor)
    {
      visitor.VisitEntryCommand(this);
    }

    [MethodImpl(Helper.OptimizeInline)]
    private protected sealed override void InternalInheritance()
    {
    }

    #endregion Bst32Command overrides
  }
}
