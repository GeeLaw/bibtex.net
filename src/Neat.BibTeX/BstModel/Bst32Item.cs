using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents an item on the BibTeX style evaluation stack.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(Bst32Item))]
  public readonly struct Bst32Item
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        switch (Type.Value)
        {
        default:
          return "(invalid)";
        case BstItemType.EmptyItemValue:
          return "[empty]";
        case BstItemType.IntegerItemValue:
          return IntegerValue.ToString("[integer] 0;[integer] -0", CultureInfo.InvariantCulture);
        case BstItemType.StringItemValue:
          return "[string] " + NameOrValue.GenericToString();
        case BstItemType.ReferenceItemValue:
          return "[ref] " + NameOrValue.GenericToString();
        case BstItemType.MissingFieldItemValue:
          return "[missing] " + NameOrValue.GenericToString();
        case BstItemType.BlockItemValue:
          return "[block] { ... }";
        }
      }
    }

    /// <summary>
    /// Indicates the type of this item.
    /// The <see langword="default"/> instance is <see cref="BstItemType.EmptyItem"/>.
    /// </summary>
    public readonly BstItemType Type;

    /// <summary>
    /// If <see cref="Type"/> is <see cref="BstItemType.IntegerItem"/>,
    /// this field is the value of this integer.
    /// </summary>
    public readonly int IntegerValue;

    /// <summary>
    /// If <see cref="Type"/> is <see cref="BstItemType.StringItem"/>,
    /// this field is the value of this string (should not be <see langword="default"/>).
    /// If <see cref="Type"/> is <see cref="BstItemType.ReferenceItem"/>,
    /// this field is the name of the function (should be a valid identifier).
    /// If <see cref="Type"/> is <see cref="BstItemType.MissingFieldItem"/>,
    /// this field is the name of the missing field (should be a valid identifier).
    /// </summary>
    public readonly StringT NameOrValue;

    /// <summary>
    /// If <see cref="Type"/> is <see cref="BstItemType.BlockItem"/>,
    /// this field is a non-<see langword="null"/> array of the constituents of the block.
    /// </summary>
    public readonly Bst32Operator[] Block;

    /// <summary>
    /// Initializes an integer item.
    /// </summary>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32Item(int integerValue)
    {
      Type = new BstItemType(BstItemType.IntegerItemValue);
      IntegerValue = integerValue;
      NameOrValue = default(StringT);
      Block = null;
    }

    /// <summary>
    /// Initializes a string, reference, or missing field item.
    /// </summary>
    /// <param name="type">Must be <see cref="BstItemType.StringItem"/>, <see cref="BstItemType.ReferenceItem"/>, or <see cref="BstItemType.MissingFieldItem"/>.</param>
    /// <param name="nameOrValue">If <paramref name="type"/> is <see cref="BstItemType.StringItem"/>, this argument must not be <see langword="default"/>.
    /// Otherwise, <paramref name="type"/> is <see cref="BstItemType.ReferenceItem"/> or <see cref="BstItemType.MissingFieldItem"/>, and
    /// this argument must be a valid identifier.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32Item(BstItemType type, StringT nameOrValue)
    {
      Type = type;
      IntegerValue = 0;
      NameOrValue = nameOrValue;
      Block = null;
    }

    /// <summary>
    /// Initializes a block item.
    /// </summary>
    /// <param name="block">Must not be <see langword="null"/> and must contain only valid operators.</param>
    [MethodImpl(Helper.OptimizeInline)]
    public Bst32Item(Bst32Operator[] block)
    {
      Type = new BstItemType(BstItemType.BlockItemValue);
      IntegerValue = 0;
      NameOrValue = default(StringT);
      Block = block;
    }

    /// <summary>
    /// The string representation obtained by this method is informational.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      switch (Type.Value)
      {
      default:
        return "(invalid)";
      case BstItemType.EmptyItemValue:
        return "[empty]";
      case BstItemType.IntegerItemValue:
        return IntegerValue.ToString("[integer] 0;[integer] -0", CultureInfo.InvariantCulture);
      case BstItemType.StringItemValue:
        return "[string] " + NameOrValue.GenericToString();
      case BstItemType.ReferenceItemValue:
        return "[ref] " + NameOrValue.GenericToString();
      case BstItemType.MissingFieldItemValue:
        return "[missing] " + NameOrValue.GenericToString();
      case BstItemType.BlockItemValue:
        Bst32Operator[] block = Block;
        if (block is null || block.Length == 0)
        {
          return "[block] {}";
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("[block]\n  {\n");
        for (int i = 0; i < block.Length; ++i)
        {
          block[i].ToString(sb, 4).Append('\n');
        }
        return sb.Append("  }").ToString();
      }
    }

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBst32ItemVisitor
    {
      switch (Type.Value)
      {
      default:
        visitor.VisitInvalid(this);
        return;
      case BstItemType.EmptyItemValue:
        visitor.VisitEmpty();
        return;
      case BstItemType.IntegerItemValue:
        visitor.VisitInteger(IntegerValue);
        return;
      case BstItemType.StringItemValue:
        visitor.VisitString(NameOrValue);
        return;
      case BstItemType.ReferenceItemValue:
        visitor.VisitReference(NameOrValue);
        return;
      case BstItemType.MissingFieldItemValue:
        visitor.VisitMissingField(NameOrValue);
        return;
      case BstItemType.BlockItemValue:
        visitor.VisitBlock(Block);
        return;
      }
    }

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    [MethodImpl(Helper.JustOptimize)]
    public void AcceptVisitor(IBst32ItemVisitor visitor)
    {
      switch (Type.Value)
      {
      default:
        visitor.VisitInvalid(this);
        return;
      case BstItemType.EmptyItemValue:
        visitor.VisitEmpty();
        return;
      case BstItemType.IntegerItemValue:
        visitor.VisitInteger(IntegerValue);
        return;
      case BstItemType.StringItemValue:
        visitor.VisitString(NameOrValue);
        return;
      case BstItemType.ReferenceItemValue:
        visitor.VisitReference(NameOrValue);
        return;
      case BstItemType.MissingFieldItemValue:
        visitor.VisitMissingField(NameOrValue);
        return;
      case BstItemType.BlockItemValue:
        visitor.VisitBlock(Block);
        return;
      }
    }
  }
}
