namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Provides information about <see cref="Bib32ParserUnsafe{TOverrides}"/>,
  /// <see cref="Bib16ParserUnsafe{TOverrides}"/>, and <see cref="Bib8ParserUnsafe{TOverrides}"/>.
  /// </summary>
  public static class BibParserUnsafe
  {
    public const int SaveEntryTypeGeneral = 0;
    public const int SaveEntryTypeString = 1;
    public const int SaveEntryTypePreamble = 2;
    public const int SaveEntryTypeComment = 3;
  }
}
