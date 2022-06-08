namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Customizes the BibTeX parser.
  /// This interface should be implemented by <typeparamref name="TOverrides"/>, a value type.
  /// </summary>
  public interface IBib32ParserUnsafeOverrides<TOverrides> where TOverrides : struct, IBib32ParserUnsafeOverrides<TOverrides>
  {
  }
}
