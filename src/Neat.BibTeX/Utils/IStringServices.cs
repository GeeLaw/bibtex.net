using Neat.BibTeX.Data;
using Neat.Collections;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Facilitates generic implementation of BibTeX parser.
  /// This interface should be implemented by a mutable <see langword="struct"/> and
  /// passed into <see cref="BibCoreParser{TChar, TString, TStringServices}"/>
  /// to obtain a parser that works with the implemented representation of string.
  /// </summary>
  public interface IStringServices<TChar, TString>
  {
    /// <summary>
    /// Converts a <typeparamref name="TChar"/> to an integer.
    /// This method is used during parsing to identify syntactically significant content.
    /// This conversion must coincide with ASCII encoding for printable characters defined by ASCII 1967.
    /// It must map any ASCII-1967-printable character to its ASCII-1967 byte value, and
    /// must not map any non-ASCII-1967-printable character to any printable ASCII-1967 byte value.
    /// </summary>
    int CharToValue(TChar ch);

    /// <summary>
    /// Converts a printable ASCII-1967 byte value to a <typeparamref name="TChar"/>.
    /// This method is used during parsing to normalize the entry types and the field names.
    /// </summary>
    TChar PrintableAsciiValueToChar(int value);

    /// <summary>
    /// Determines whether a <typeparamref name="TChar"/> is considered a whitespace character for parsing purposes.
    /// This method must consider <c>\u0020\f\n\r\t\v</c> as space characters.
    /// </summary>
    bool CharIsSpace(TChar ch);

    /// <summary>
    /// Creates a default <typeparamref name="TString"/>.
    /// This can be <see langword="null"/> or the empty string, depending on the semantics of <typeparamref name="TString"/>.
    /// This method is used when a citation key is absent from a general entry.
    /// </summary>
    void CreateDefaultString(out TString str);

    /// <summary>
    /// Creates the string <c>string</c> (in lower case).
    /// This method is used to retrieve the entry type for <see cref="BibStringEntry{TString}"/>
    /// </summary>
    void CreateStringString(out TString str);

    /// <summary>
    /// Creates the string <c>preamble</c> (in lower case).
    /// This method is used to retrieve the entry type for <see cref="BibStringEntry{TString}"/>
    /// </summary>
    void CreateStringPreamble(out TString str);

    /// <summary>
    /// Creates the string <c>comment</c> (in lower case).
    /// This method is used to retrieve the entry type for <see cref="BibStringEntry{TString}"/>
    /// </summary>
    void CreateStringComment(out TString str);

    /// <summary>
    /// Converts a list of <typeparamref name="TChar"/> (used as a string builder) into a <typeparamref name="TString"/>.
    /// </summary>
    void GetStringFromCharList(ref List2<TChar> sb, out TString str);
  }
}
