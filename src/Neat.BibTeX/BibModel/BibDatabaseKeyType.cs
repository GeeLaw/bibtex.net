using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neat.BibTeX.BibModel
{
  /// <summary>
  /// Represents the type of a database key.
  /// This type should be treated like an <see langword="enum"/> with the underlying type being <see langword="byte"/>.
  /// </summary>
  [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1)]
  public readonly struct BibDatabaseKeyType : IEquatable<BibDatabaseKeyType>
  {
    public const byte InvalidValue = 0;
    public const byte UseBracesOrParenthesesValue = 1;
    public const byte MustUseParenthesesValue = 2;

    /// <summary>
    /// Indicates that the database key is not valid.
    /// </summary>
    public static readonly BibDatabaseKeyType Invalid = new BibDatabaseKeyType(InvalidValue);

    /// <summary>
    /// Indicates that the database key is valid and its delimiters can be either <c>{}</c> or <c>()</c>.
    /// </summary>
    public static readonly BibDatabaseKeyType UseBracesOrParentheses = new BibDatabaseKeyType(UseBracesOrParenthesesValue);

    /// <summary>
    /// Indicates that the database key is valid and its delimiters must be <c>()</c>.
    /// </summary>
    public static readonly BibDatabaseKeyType MustUseParentheses = new BibDatabaseKeyType(MustUseParenthesesValue);

    [FieldOffset(0)]
    public readonly byte Value;

    [MethodImpl(Helper.OptimizeInline)]
    public BibDatabaseKeyType(byte value)
    {
      Value = value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator byte(BibDatabaseKeyType type)
    {
      return type.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static explicit operator BibDatabaseKeyType(byte value)
    {
      return new BibDatabaseKeyType(value);
    }

    public bool IsDefined
    {
      [MethodImpl(Helper.OptimizeInline)]
      get
      {
        return Value < 3u;
      }
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator ==(BibDatabaseKeyType x, BibDatabaseKeyType y)
    {
      return x.Value == y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool operator !=(BibDatabaseKeyType x, BibDatabaseKeyType y)
    {
      return x.Value != y.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public bool Equals(BibDatabaseKeyType other)
    {
      return Value == other.Value;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public override bool Equals(object obj)
    {
      return (obj is BibDatabaseKeyType other) && Value == other.Value;
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
      case UseBracesOrParenthesesValue:
        return nameof(UseBracesOrParentheses);
      case MustUseParenthesesValue:
        return nameof(MustUseParentheses);
      default:
        return value.ToString(CultureInfo.InvariantCulture);
      }
    }

    [MethodImpl(Helper.JustOptimize)]
    public static BibDatabaseKeyType Parse(string str)
    {
      return new BibDatabaseKeyType(
        str == nameof(Invalid)
        ? InvalidValue
        : str == nameof(UseBracesOrParentheses)
        ? UseBracesOrParenthesesValue
        : str == nameof(MustUseParentheses)
        ? MustUseParenthesesValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static BibDatabaseKeyType Parse(string str, bool ignoreCase)
    {
      return ignoreCase ? ParseIgnoreCaseImpl(str) : Parse(str);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static BibDatabaseKeyType ParseIgnoreCaseImpl(string str)
    {
      return new BibDatabaseKeyType(
        string.Equals(str, nameof(Invalid), StringComparison.OrdinalIgnoreCase)
        ? InvalidValue
        : string.Equals(str, nameof(UseBracesOrParentheses), StringComparison.OrdinalIgnoreCase)
        ? UseBracesOrParenthesesValue
        : string.Equals(str, nameof(MustUseParentheses), StringComparison.OrdinalIgnoreCase)
        ? MustUseParenthesesValue
        : byte.Parse(str, CultureInfo.InvariantCulture)
      );
    }

    [MethodImpl(Helper.JustOptimize)]
    public static bool TryParse(string str, out BibDatabaseKeyType type)
    {
      if (str == nameof(Invalid))
      {
        type = new BibDatabaseKeyType(InvalidValue);
        goto ReturnTrue;
      }
      if (str == nameof(UseBracesOrParentheses))
      {
        type = new BibDatabaseKeyType(UseBracesOrParenthesesValue);
        goto ReturnTrue;
      }
      if (str == nameof(MustUseParentheses))
      {
        type = new BibDatabaseKeyType(MustUseParenthesesValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BibDatabaseKeyType, byte>(ref type));
    ReturnTrue:
      return true;
    }

    [MethodImpl(Helper.OptimizeInline)]
    public static bool TryParse(string str, bool ignoreCase, out BibDatabaseKeyType type)
    {
      return ignoreCase ? TryParseIgnoreCaseImpl(str, out type) : TryParse(str, out type);
    }

    [MethodImpl(Helper.JustOptimize)]
    private static bool TryParseIgnoreCaseImpl(string str, out BibDatabaseKeyType type)
    {
      if (string.Equals(str, nameof(Invalid), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibDatabaseKeyType(InvalidValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(UseBracesOrParentheses), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibDatabaseKeyType(UseBracesOrParenthesesValue);
        goto ReturnTrue;
      }
      if (string.Equals(str, nameof(MustUseParentheses), StringComparison.OrdinalIgnoreCase))
      {
        type = new BibDatabaseKeyType(MustUseParenthesesValue);
        goto ReturnTrue;
      }
      Unsafe.SkipInit(out type);
      return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out Unsafe.As<BibDatabaseKeyType, byte>(ref type));
    ReturnTrue:
      return true;
    }
  }
}
