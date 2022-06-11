namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Customizes BibTeX parsing behaviors of <see cref="Bib32ParserUnsafe{TOverrides}"/>.
  /// This interface should be implemented by <typeparamref name="TOverrides"/>, a value type.
  /// After calling any exception method, the parser will reset to look for the next <c>@</c>
  /// (no saving method will be called for the current field or entry, even if its parsing is in progress;
  /// the parser could choose to save the partially parsed field or entry.)
  /// </summary>
  public interface IBib32ParserUnsafeOverrides<TOverrides> where TOverrides : struct, IBib32ParserUnsafeOverrides<TOverrides>
  {
    #region normal processing

    /// <summary>
    /// Indicates that the parser has seen <c>@type</c>.
    /// The range passed into this method is precisely the range of <c>type</c>.
    /// </summary>
    /// <returns><c>0</c>, <c>1</c>, <c>2</c>, <c>3</c>
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
    /// Indicates that the parser has seen <c>name</c> (e.g., in <c>{literal} # "literal" # 123 # name</c>) without error.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveNameComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>"literal"</c> without error.
    /// The range passed into this method is precisely the range of <c>literal</c> (without <c>""</c>).
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveQuoteLiteralComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>123</c> (e.g., in <c>{literal} # "literal" # 123</c>) without error.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveNumericLiteralComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

    /// <summary>
    /// Indicates that the parser has seen <c>{literal}</c> without error.
    /// The range passed into this method is precisely the range of <c>literal</c> (without <c>{}</c>).
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void SaveBraceLiteralComponent(ref Bib32ParserUnsafe<TOverrides> that, ref int start, int length);

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

    #region exceptions

    /// <summary>
    /// The parser has read <c>@</c>, but
    /// the end of input is reached, or
    /// the next non-space character is numeric or not a valid identifier character.
    /// </summary>
    void EntryExpectingType(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@string</c>, but
    /// the end of input is reached, or
    /// the next non-space character is neither <c>{</c> nor <c>(</c>.
    /// </summary>
    void StringEntryExpectingOpen(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@string{</c>, but
    /// the end of input is reached, or
    /// the next non-space character is numeric or not a valid identifier character.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the string entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void StringEntryExpectingName(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@string{ name</c>, but
    /// the end of input is reached, or
    /// the next non-space character is not <c>=</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the string entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void StringEntryExpectingAssignment(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@string{ name = {literal} # "literal" # 123 # anothername</c>, but
    /// the end of input is reached.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the string entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX does not save the current entry.
    /// </summary>
    void StringEntryGotEndOfInput(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@string{ name = {literal} # "literal" # 123 # anothername</c>, but
    /// the next non-space character exists and is not the closing delimiter.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the string entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry.
    /// </summary>
    void StringEntryExpectingClose(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@preamble</c>, but
    /// the end of input is reached, or
    /// the next non-space character is neither <c>{</c> nor <c>(</c>.
    /// </summary>
    void PreambleEntryExpectingOpen(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@preamble{ {literal} # "literal" # 123 # name</c>, but
    /// the end of input is reached.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the preamble entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX does not save the current entry.
    /// </summary>
    void PreambleEntryGotEndOfInput(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@preamble{ {literal} # "literal" # 123 # name</c>, but
    /// the next non-space character exists and is not the closing delimiter.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the preamble entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry.
    /// </summary>
    void PreambleEntryExpectingClose(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type</c>, but
    /// the end of input is reached, or
    /// the next non-space character is neither <c>{</c> nor <c>(</c>.
    /// </summary>
    void GeneralEntryExpectingOpen(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type{</c>, but
    /// the end of input is reached.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// </summary>
    void GeneralEntryExpectingDatabaseKey(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type{ key</c>, but
    /// the end of input is reached, or
    /// the next non-space character is neither <c>,</c> nor the closing delimiter.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry.
    /// </summary>
    void GeneralEntryExpectingFirstCommaOrClose(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type{ key,</c> or <c>@type{ key, name1 = {literal} # "literal" # 123 # name,</c>, but
    /// the end of input is reached, or
    /// the next non-space character is numeric or not a valid identifier character.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry with all the previously parsed fields.
    /// </summary>
    void GeneralEntryExpectingFieldNameOrClose(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type{ key, name1</c>, but
    /// the end of input is reached, or
    /// the next non-space character is not <c>=</c>.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry with all the previously parsed fields (excluding the current field that is incomplete).
    /// </summary>
    void GeneralEntryFieldExpectingAssignment(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type{ key, name1 = {literal} # "literal" # 123 # name</c>, but
    /// the end of input is reached.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry with all the previously parsed fields (excluding the current field).
    /// </summary>
    void GeneralEntryFieldGotEndOfInput(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser has read <c>@type{ key, name1 = {literal} # "literal" # 123 # name</c>, but
    /// the next non-space character exists and is neither <c>,</c> nor the closing delimiter.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the general entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX saves the current entry with all the previously parsed fields plus the current field.
    /// </summary>
    void GeneralEntryFieldExpectingCommaOrClose(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser is about to read the next component in a series of concatenated components, potentially due to having read <c>#</c>, but
    /// the end of input is reached, or
    /// the next non-space character is neither <c>{</c>, nor <c>"</c>, nor a valid identifier character.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX will not save the series of concatenated components, i.e.:
    /// if the current entry is a string entry or a preamble entry, the string entry is discarded;
    /// if the current entry is a general entry, the current field is discarded, but the current entry is saved with all the previously parsed fields.
    /// </summary>
    void StringExpectingComponent(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser is reading a quote-delimited literal, but
    /// the next character is <c>}</c> outisde braces.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>.
    /// The original implementation of BibTeX will not save the series of concatenated components, i.e.:
    /// if the current entry is a string entry or a preamble entry, the string entry is discarded;
    /// if the current entry is a general entry, the current field is discarded, but the current entry is saved with all the previously parsed fields.
    /// </summary>
    void StringQuoteLiteralGotNegativeBraceDepth(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser is reading a quote-delimited literal, but
    /// the end of input is reached.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>,
    /// and <see cref="Bib32ParserUnsafe{TOverrides}.BraceDepth"/> indicates the number of outstanding left braces.
    /// The original implementation of BibTeX will not save the series of concatenated components, i.e.:
    /// if the current entry is a string entry or a preamble entry, the string entry is discarded;
    /// if the current entry is a general entry, the current field is discarded, but the current entry is saved with all the previously parsed fields.
    /// </summary>
    void StringQuoteLiteralGotEndOfInput(ref Bib32ParserUnsafe<TOverrides> that);

    /// <summary>
    /// The parser is reading a brace-delimited literal, but
    /// the end of input is reached.
    /// When this method is called, <see cref="Bib32ParserUnsafe{TOverrides}.EntryIsBrace"/> indicates whether the entry uses <c>{}</c> as its delimiters, with the alternate option being <c>()</c>,
    /// and <see cref="Bib32ParserUnsafe{TOverrides}.BraceDepth"/> indicates the number of outstanding left braces (not counting the opening delimiter).
    /// The original implementation of BibTeX will not save the series of concatenated components, i.e.:
    /// if the current entry is a string entry or a preamble entry, the string entry is discarded;
    /// if the current entry is a general entry, the current field is discarded, but the current entry is saved with all the previously parsed fields.
    /// </summary>
    void StringBraceLiteralGotEndOfInput(ref Bib32ParserUnsafe<TOverrides> that);

    #endregion exceptions
  }
}
