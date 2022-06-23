using System.Runtime.CompilerServices;
using System.Text;
using Neat.BibTeX.Utils;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.BstModel
{
  /// <summary>
  /// Represents a command in <c>.bst</c> files.
  /// </summary>
  public abstract class Bst32Command
  {
    [MethodImpl(Helper.JustOptimize)]
    internal static StringBuilder ToStringHelper(StringBuilder sb, int indentation, StringT[] content)
    {
      if (content is null || content.Length == 0)
      {
        return sb.Append(' ', indentation).Append("{}");
      }
      if (content.Length == 1)
      {
        return sb.Append(' ', indentation).Append("{ ").Append(content[0].GenericToString()).Append(" }");
      }
      sb.Append(' ', indentation).Append("{\n");
      indentation += 2;
      for (int i = 0; i < content.Length; ++i)
      {
        sb.Append(' ', indentation).Append(content[i].GenericToString()).Append('\n');
      }
      return sb.Append(' ', indentation - 2).Append('}');
    }

    /// <summary>
    /// Indicates the type of this entry.
    /// This string should be compared by <see cref="BibBstComparer"/>.
    /// It can only be one of the following:
    /// <c>ENTRY</c>, <c>INTEGERS</c>, <c>STRINGS</c>, <c>MACRO</c>, <c>FUNCTION</c>,
    /// <c>READ</c>, <c>EXECUTE</c>, <c>ITERATE</c>, <c>SORT</c>, <c>REVERSE</c>.
    /// </summary>
    public readonly StringT Type;

    /// <summary>
    /// The string representation obtained by this method is informational and not necessarily valid BibTeX.
    /// </summary>
    public abstract override string ToString();

    [MethodImpl(Helper.OptimizeInline)]
    private protected Bst32Command(StringT type)
    {
      Type = type;
    }

    [MethodImpl(Helper.JustOptimize)]
    internal static bool IsNotNullAndContainsOnlyIdentifiersAndContainsNoDuplicates(StringT[] array)
    {
      if (array is null)
      {
        return false;
      }
      for (int i = 0; i < array.Length; ++i)
      {
        StringT arrayi = array[i];
        if (!BibBstChars.IsIdentifier(arrayi))
        {
          return false;
        }
        for (int j = i + 1; j < array.Length; ++j)
        {
          if (BibBstComparer.Equals(arrayi, array[j]))
          {
            return false;
          }
        }
      }
      return true;
    }

    [MethodImpl(Helper.JustOptimize)]
    internal static bool AreNotNullAndContainNoDuplicates(StringT[] array1, StringT[] array2)
    {
      if (array1 is null || array2 is null)
      {
        return false;
      }
      for (int i = 0; i < array1.Length; ++i)
      {
        for (int j = 0; j < array2.Length; ++j)
        {
          if (BibBstComparer.Equals(array1[i], array2[j]))
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Determines whether the command is currently valid.
    /// </summary>
    public abstract bool IsValid();

    /// <summary>
    /// Dispatches the correct method for the value-type visitor.
    /// </summary>
    public abstract void AcceptVisitor<TVisitor>(ref TVisitor visitor) where TVisitor : struct, IBst32CommandVisitor;

    /// <summary>
    /// Dispatches the correct method for the reference-type visitor.
    /// </summary>
    public abstract void AcceptVisitor(IBst32CommandVisitor visitor);

    /// <summary>
    /// This method ensures only this assembly can create non-abstract derived classes of this class.
    /// </summary>
    private protected abstract void InternalInheritance();
  }
}
