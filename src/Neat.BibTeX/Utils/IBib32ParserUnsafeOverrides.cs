namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Customizes BibTeX parsing behaviors of <see cref="Bib32ParserUnsafe{TOverrides}"/>.
  /// This interface should be implemented by <typeparamref name="TOverrides"/>, a value type.
  /// </summary>
  public interface IBib32ParserUnsafeOverrides<TOverrides> where TOverrides : struct, IBib32ParserUnsafeOverrides<TOverrides>
  {
    #region normal processing

    /// <summary>
    /// Indicates that the parser has seen <c>@type</c>.
    /// The range passed into this method is precisely the range of <c>type</c>.
    /// </summary>
    /// <returns><c>0</c>, <c>1</c>, <c>2</c>, anything else
    /// for general, string, preamble, comment entry, respectively.</returns>
    int SaveEntryType(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>@string{ name = {literal} # "literal" # 123 # anothername }</c> without error.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the string entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveStringEntry(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// Indicates that the parser has seen <c>@preamble{ {literal} # "literal" # 123 # anothername }</c> without error.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the preamble entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SavePreambleEntry(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// Indicates that the parser has seen <c>@type{ key, name1 = {literal} # "literal" # 123 # anothername, ... }</c> without error.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveGeneralEntry(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// Indicates that the parser has seen <c>@string { name</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the string entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveStringName(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>{literal}</c> or <c>"literal"</c> without error.
    /// The range passed into this method is precisely the range of <c>literal</c> (without the delimiters).
    /// When this method is called,
    /// <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>, and
    /// <see cref="Bib32ParserUnsafe{TOverrides}.LiteralIsBrace"/> indicates whether the literal uses <c>{}</c> as its delimiteres, with the alternate option being <c>""</c>.
    /// </summary>
    void SaveDelimitedLiteralComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>{literal} # "literal" # 123</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveNumericLiteralComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>{literal} # "literal" # 123 # name</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveNameComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>@type { key</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveDatabaseKey(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>@type{ key, name1</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveFieldName(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>name1 = {literal} # "literal" # 123 # anothername</c> plus optional space and has not reached the end of input.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveField(ref Bib32ParserUnsafe<TOverrides> that);

    #endregion normal processing
  }
}
