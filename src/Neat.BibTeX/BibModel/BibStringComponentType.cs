using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents the type of a string component.
  /// This type should be treated like an <see langword="enum"/> with the underlying type being <see langword="byte"/>.
  /// </summary>
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}", Type = nameof(BibStringComponentType))]
  [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1)]
  public readonly struct BibStringComponentType : IEquatable<BibStringComponentType>
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
    public const byte NameValue = 1;
    public const byte QuoteLiteralValue = 2;
    public const byte NumericLiteralValue = 3;
    public const byte BraceLiteralValue = 4;

    /// <summary>
    /// Indicates that the component is not valid.
    /// </summary>
    public static readonly BibStringComponentType Invalid = new BibStringComponentType(InvalidValue);

    /// <summary>
    /// Indicates that the component is the name of a referenced string.
    /// </summary>
    public static readonly BibStringComponentType Name = new BibStringComponentType(NameValue);

    /// <summary>
    /// Indicates that the component is a quote-delimited literal (e.g., <c>"literal"</c>).
    /// </summary>
    public static readonly BibStringComponentType QuoteLiteral = new BibStringComponentType(QuoteLiteralValue);

    /// <summary>
    /// Indicates that the component is a bare numeric literal (e.g., <c>123</c>).
    /// </summary>
    public static readonly BibStringComponentType NumericLiteral = new BibStringComponentType(NumericLiteralValue);

    /// <summary>
    /// Indicates that the component is a brace-delimited literal (e.g., <c>{literal}</c>).
    /// </summary>
    public static readonly BibStringComponentType BraceLiteral = new BibStringComponentType(BraceLiteralValue);

    [FieldOffset(0)]
    public readonly byte Value;

    [MethodImpl(Helper.OptimizeInline)]
    public BibStringComponentType(byte value)
    {
      Value = value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator byte(BibStringComponentType type)
    {
      return type.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator BibStringComponentType(byte value)
    {
      return new BibStringComponentType(value);
    }

    public bool IsDefined
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Value < 5u;
      }
    }

    public bool IsDelimitedLiteral
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        byte value = Value;
        return value == 2u || value == 4u;
      }
    }

    public bool IsNameOrNumericLiteral
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        byte value = Value;
        return value == 1u || value == 3u;
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(BibStringComponentType x, BibStringComponentType y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(BibStringComponentType x, BibStringComponentType y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(BibStringComponentType other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is BibStringComponentType other) && Value == other.Value;
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
      case NameValue:
        return nameof(Name);
      case QuoteLiteralValue:
        return nameof(QuoteLiteral);
      case NumericLiteralValue:
        return nameof(NumericLiteral);
      case BraceLiteralValue:
        return nameof(BraceLiteral);
      default:
        return value.ToString(CultureInfo.InvariantCulture);
      }
    }

    [MethodImpl(Helper.JustOptimize)]
    public static BibStringComponentType Parse(string str)
    {
      return new BibStringComponentType(
        str == nameof(Invalid)
        ? InvalidValue
        : str == nameof(Name)
        ? NameValue
        : str == nameof(QuoteLiteral)
        ? QuoteLiteralValue
        : str == nameof(NumericLiteral)
        ? NumericLiteralValue
        : str == nameof(BraceLiteral)
        ? BraceLiteralValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static BibStringComponentType Parse(string str, bool ignoreCase)
    {
      return ignoreCase ? ParseIgnoreCaseImpl(str) : Parse(str);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static BibStringComponentType ParseIgnoreCaseImpl(string str)
    {
      return new BibStringComponentType(
        string.Equals(str, nameof(Invalid), StringComparison.OrdinalIgnoreCase)
        ? InvalidValue
        : string.Equals(str, nameof(Name), StringComparison.OrdinalIgnoreCase)
        ? NameValue
        : string.Equals(str, nameof(QuoteLiteral), StringComparison.OrdinalIgnoreCase)
        ? QuoteLiteralValue
        : string.Equals(str, nameof(NumericLiteral), StringComparison.OrdinalIgnoreCase)
        ? NumericLiteralValue
        : string.Equals(str, nameof(BraceLiteral), StringComparison.OrdinalIgnoreCase)
        ? BraceLiteralValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.JustOptimize)]
    public static bool TryParse(string str, out BibStringComponentType type)
    {
      if (str == nameof(Invalid))
      {
        type = new BibStringComponentType(InvalidValue);
        goto ReturnTrue;
      }
      if (str == nameof(Name))
      {
        type = new BibStringComponentType(NameValue);
        goto ReturnTrue;
      }
      if (str == nameof(QuoteLiteral))
      {
        type = new BibStringComponentType(QuoteLiteralValue);
        goto ReturnTrue;
      }
      if (str == nameof(NumericLiteral))
      {
        type = new BibStringComponentType(NumericLiteralValue);
        goto ReturnTrue;
      }
      if (str == nameof(BraceLiteral))
      {
        type = new BibStringComponentType(BraceLiteralValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BibStringComponentType, byte>(ref type));
    ReturnTrue:
      return true;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool TryParse(string str, bool ignoreCase, out BibStringComponentType type)
    {
      return ignoreCase ? TryParseIgnoreCaseImpl(str, out type) : TryParse(str, out type);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool TryParseIgnoreCaseImpl(string str, out BibStringComponentType type)
    {
      if (string.Equals(str, nameof(Invalid), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibStringComponentType(InvalidValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(Name), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibStringComponentType(NameValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(QuoteLiteral), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibStringComponentType(QuoteLiteralValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(NumericLiteral), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibStringComponentType(NumericLiteralValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(BraceLiteral), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibStringComponentType(BraceLiteralValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BibStringComponentType, byte>(ref type));
    ReturnTrue:
      return true;
    }
  }
}
