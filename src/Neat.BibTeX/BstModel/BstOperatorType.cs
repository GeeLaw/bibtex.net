using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents the type of an operator (function body definition token) in BibTeX style function definition.
  /// This type should be treated like an <see langword="enum"/> with the underlying type being <see langword="byte"/>.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(BstOperatorType))]
  [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1)]
  public readonly struct BstOperatorType : IEquatable<BstOperatorType>
  {
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        return ToString();
      }
    }

    public const byte InvalidValue = 0;
    public const byte IntegerLiteralValue = 1;
    public const byte StringLiteralValue = 2;
    public const byte ReferenceValue = 3;
    public const byte InvocationValue = 4;
    public const byte BlockValue = 5;

    /// <summary>
    /// Indicates that the operator type is not valid.
    /// </summary>
    public static readonly BstOperatorType Invalid = new BstOperatorType(InvalidValue);

    /// <summary>
    /// Indicates that the operator is an integer literal (e.g., <c>#123</c>).
    /// </summary>
    public static readonly BstOperatorType IntegerLiteral = new BstOperatorType(IntegerLiteralValue);

    /// <summary>
    /// Indicates that the operator is a string literal (e.g., <c>"{literal]"</c>).
    /// </summary>
    public static readonly BstOperatorType StringLiteral = new BstOperatorType(StringLiteralValue);

    /// <summary>
    /// Indicates that the operator is a reference (a.k.a. quoted function, e.g., <c>'skip$</c>).
    /// </summary>
    public static readonly BstOperatorType Reference = new BstOperatorType(ReferenceValue);

    /// <summary>
    /// Indicates that the operator is an invocation (a.k.a. already-defined function, e.g., <c>skip$</c>).
    /// </summary>
    public static readonly BstOperatorType Invocation = new BstOperatorType(InvocationValue);

    /// <summary>
    /// Indicates that the operator is a block of operators (e.g., <c>{ 1 1 + }</c>).
    /// </summary>
    public static readonly BstOperatorType Block = new BstOperatorType(BlockValue);

    [FieldOffset(0)]
    public readonly byte Value;

    [MethodImpl(Helper.OptimizeInline)]
    public BstOperatorType(byte value)
    {
      Value = value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator byte(BstOperatorType type)
    {
      return type.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator BstOperatorType(byte value)
    {
      return new BstOperatorType(value);
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
    public static bool operator ==(BstOperatorType x, BstOperatorType y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(BstOperatorType x, BstOperatorType y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(BstOperatorType other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is BstOperatorType other) && Value == other.Value;
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
      case InvalidValue:
        return nameof(Invalid);
      case IntegerLiteralValue:
        return nameof(IntegerLiteral);
      case StringLiteralValue:
        return nameof(StringLiteral);
      case ReferenceValue:
        return nameof(Reference);
      case InvocationValue:
        return nameof(Invocation);
      case BlockValue:
        return nameof(Block);
      default:
        return value.ToString(CultureInfo.InvariantCulture);
      }
    }

    [MethodImpl(Helper.JustOptimize)]
    public static BstOperatorType Parse(string str)
    {
      return new BstOperatorType(
        str == nameof(Invalid)
        ? InvalidValue
        : str == nameof(IntegerLiteral)
        ? IntegerLiteralValue
        : str == nameof(StringLiteral)
        ? StringLiteralValue
        : str == nameof(Reference)
        ? ReferenceValue
        : str == nameof(Invocation)
        ? InvocationValue
        : str == nameof(Block)
        ? BlockValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static BstOperatorType Parse(string str, bool ignoreCase)
    {
      return ignoreCase ? ParseIgnoreCaseImpl(str) : Parse(str);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static BstOperatorType ParseIgnoreCaseImpl(string str)
    {
      return new BstOperatorType(
        string.Equals(str, nameof(Invalid), StringComparison.OrdinalIgnoreCase)
        ? InvalidValue
        : string.Equals(str, nameof(IntegerLiteral), StringComparison.OrdinalIgnoreCase)
        ? IntegerLiteralValue
        : string.Equals(str, nameof(StringLiteral), StringComparison.OrdinalIgnoreCase)
        ? StringLiteralValue
        : string.Equals(str, nameof(Reference), StringComparison.OrdinalIgnoreCase)
        ? ReferenceValue
        : string.Equals(str, nameof(Invocation), StringComparison.OrdinalIgnoreCase)
        ? InvocationValue
        : string.Equals(str, nameof(Block), StringComparison.OrdinalIgnoreCase)
        ? BlockValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.JustOptimize)]
    public static bool TryParse(string str, out BstOperatorType type)
    {
      if (str == nameof(Invalid))
      {
        type = new BstOperatorType(InvalidValue);
        goto ReturnTrue;
      }
      if (str == nameof(IntegerLiteral))
      {
        type = new BstOperatorType(IntegerLiteralValue);
        goto ReturnTrue;
      }
      if (str == nameof(StringLiteral))
      {
        type = new BstOperatorType(StringLiteralValue);
        goto ReturnTrue;
      }
      if (str == nameof(Reference))
      {
        type = new BstOperatorType(ReferenceValue);
        goto ReturnTrue;
      }
      if (str == nameof(Invocation))
      {
        type = new BstOperatorType(InvocationValue);
        goto ReturnTrue;
      }
      if (str == nameof(Block))
      {
        type = new BstOperatorType(BlockValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BstOperatorType, byte>(ref type));
    ReturnTrue:
      return true;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool TryParse(string str, bool ignoreCase, out BstOperatorType type)
    {
      return ignoreCase ? TryParseIgnoreCaseImpl(str, out type) : TryParse(str, out type);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool TryParseIgnoreCaseImpl(string str, out BstOperatorType type)
    {
      if (string.Equals(str, nameof(Invalid), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstOperatorType(InvalidValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(IntegerLiteral), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstOperatorType(IntegerLiteralValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(StringLiteral), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstOperatorType(StringLiteralValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(Reference), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstOperatorType(ReferenceValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(Invocation), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstOperatorType(InvocationValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(Block), StringComparison.OrdinalIgnoreCase))
      {
        type = new BstOperatorType(BlockValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BstOperatorType, byte>(ref type));
    ReturnTrue:
      return true;
    }
  }
}
