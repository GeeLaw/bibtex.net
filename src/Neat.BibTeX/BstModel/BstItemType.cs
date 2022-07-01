using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents the type of an item on the BibTeX style evaluation stack.
  /// This type should be treated like an <see langword="enum"/> with the underlying type being <see langword="byte"/>.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(BstItemType))]
  [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1)]
  public readonly struct BstItemType : IEquatable<BstItemType>
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return ToString();
      }
    }

    public const byte EmptyItemValue = 0;
    public const byte IntegerItemValue = 1;
    public const byte StringItemValue = 2;
    public const byte ReferenceItemValue = 3;
    public const byte MissingFieldItemValue = 4;
    public const byte BlockItemValue = 5;

    /// <summary>
    /// Indicates that the item is popped from an empty stack.
    /// </summary>
    public static readonly BstItemType EmptyItem = new BstItemType(EmptyItemValue);

    /// <summary>
    /// Indicates that the item is an integer.
    /// </summary>
    public static readonly BstItemType IntegerItem = new BstItemType(IntegerItemValue);

    /// <summary>
    /// Indicates that the item is a string.
    /// </summary>
    public static readonly BstItemType StringItem = new BstItemType(StringItemValue);

    /// <summary>
    /// Indicates that the item is a reference to a function.
    /// </summary>
    public static readonly BstItemType ReferenceItem = new BstItemType(ReferenceItemValue);

    /// <summary>
    /// Indicates that the item is a missing field.
    /// </summary>
    public static readonly BstItemType MissingFieldItem = new BstItemType(MissingFieldItemValue);

    /// <summary>
    /// Indicates that the item is a block of operators.
    /// </summary>
    public static readonly BstItemType BlockItem = new BstItemType(BlockItemValue);

    [FieldOffset(0)]
    public readonly byte Value;

    [MethodImpl(Helper.OptimizeInline)]
    public BstItemType(byte value)
    {
      Value = value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator byte(BstItemType type)
    {
      return type.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator BstItemType(byte value)
    {
      return new BstItemType(value);
    }

    public bool IsDefined
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Value < 6u;
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(BstItemType x, BstItemType y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(BstItemType x, BstItemType y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(BstItemType other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is BstItemType other) && Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override int GetHashCode()
    {
      return Value;
    }

    [MethodImpl(Helper.JustOptimize)]
    public override string ToString()
    {
      byte value = Value;
      switch (value)
      {
      case EmptyItemValue:
        return nameof(EmptyItem);
      case IntegerItemValue:
        return nameof(IntegerItem);
      case StringItemValue:
        return nameof(StringItem);
      case ReferenceItemValue:
        return nameof(ReferenceItem);
      case MissingFieldItemValue:
        return nameof(MissingFieldItem);
      case BlockItemValue:
        return nameof(BlockItem);
      default:
        return value.ToString(CultureInfo.InvariantCulture);
      }
    }

    [MethodImpl(Helper.JustOptimize)]
    public static BstItemType Parse(string str)
    {
      return new BstItemType(
        str == nameof(EmptyItem)
        ? EmptyItemValue
        : str == nameof(IntegerItem)
        ? IntegerItemValue
        : str == nameof(StringItem)
        ? StringItemValue
        : str == nameof(ReferenceItem)
        ? ReferenceItemValue
        : str == nameof(MissingFieldItem)
        ? MissingFieldItemValue
        : str == nameof(BlockItem)
        ? BlockItemValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static BstItemType Parse(string str, bool ignoreCase)
    {
      return ignoreCase ? ParseIgnoreCaseImpl(str) : Parse(str);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static BstItemType ParseIgnoreCaseImpl(string str)
    {
      return new BstItemType(
        string.Equals(str, nameof(EmptyItem), StringComparison.OrdinalIgnoreCase)
        ? EmptyItemValue
        : string.Equals(str, nameof(IntegerItem), StringComparison.OrdinalIgnoreCase)
        ? IntegerItemValue
        : string.Equals(str, nameof(StringItem), StringComparison.OrdinalIgnoreCase)
        ? StringItemValue
        : string.Equals(str, nameof(ReferenceItem), StringComparison.OrdinalIgnoreCase)
        ? ReferenceItemValue
        : string.Equals(str, nameof(MissingFieldItem), StringComparison.OrdinalIgnoreCase)
        ? MissingFieldItemValue
        : string.Equals(str, nameof(BlockItem), StringComparison.OrdinalIgnoreCase)
        ? BlockItemValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.JustOptimize)]
    public static bool TryParse(string str, out BstItemType type)
    {
      if (str == nameof(EmptyItem))
      {
        type = new BstItemType(EmptyItemValue);
        goto ReturnTrue;
      }
      if (str == nameof(IntegerItem))
      {
        type = new BstItemType(IntegerItemValue);
        goto ReturnTrue;
      }
      if (str == nameof(StringItem))
      {
        type = new BstItemType(StringItemValue);
        goto ReturnTrue;
      }
      if (str == nameof(ReferenceItem))
      {
        type = new BstItemType(ReferenceItemValue);
        goto ReturnTrue;
      }
      if (str == nameof(MissingFieldItem))
      {
        type = new BstItemType(MissingFieldItemValue);
        goto ReturnTrue;
      }
      if (str == nameof(BlockItem))
      {
        type = new BstItemType(BlockItemValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BstItemType, byte>(ref type));
    ReturnTrue:
      return true;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool TryParse(string str, bool ignoreCase, out BstItemType type)
    {
      return ignoreCase ? TryParseIgnoreCaseImpl(str, out type) : TryParse(str, out type);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool TryParseIgnoreCaseImpl(string str, out BstItemType type)
    {
      if (string.Equals(str, nameof(EmptyItem), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstItemType(EmptyItemValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(IntegerItem), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstItemType(IntegerItemValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(StringItem), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstItemType(StringItemValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(ReferenceItem), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstItemType(ReferenceItemValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(MissingFieldItem), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstItemType(MissingFieldItemValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(BlockItem), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstItemType(BlockItemValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BstItemType, byte>(ref type));
    ReturnTrue:
      return true;
    }
  }
}
