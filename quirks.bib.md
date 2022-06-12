# Quirks of BibTeX 0.99d in `.bib` Parsing

This document describes the behavior of [BibTeX 0.99d](deps/bibtex) under certain edge cases.

## Line-based processing

The original implementation of BibTeX processes database files line by line, as shown in [`bibtex.web` L1242 `input_ln`](deps/bibtex/src/bibtex.web#L1242). BibTeX.NET does not use line-based processing.

## Processing multiple files

The original implementation of BibTeX can process multiple files. Parser state (except for scanned entries) is reset after each file is read, so `@misc{ key,` and `title = "Hello"}` in two files do not create an entry whose `title` field is `Hello`.

## What characters can be processed?

[`bibtex.web` L865](deps/bibtex/src/bibtex.web#L865) initializes `lex_class`. It deems `U+007F` and anything below `U+0020` invalid, except `U+0009` (horizontal tab), which is considered a space character. Interestingly, the code never explicitly compares `lex_class[...]` to `illegal`, which means they are accepted by BibTeX. Indeed, the following file in UTF-8

```bib
@misc{ U+001B, title = "Hello" }
```

makes BibTeX create an entry with key `U+001B`, where `U+001B` represents the Unicode character, not the 6-character sequence itself.

For high-performance processing and coding simplicity, BibTeX.NET allows all possible numeric values when processing data. However, **only characters defined in ASCII 1967** are syntactically significant for BibTeX.NET, so it is safe to cut UTF-8, UTF-16, and UTF-32 data at those boundaries. This should be taken into consideration for security reviews.

## What are space characters?

[`bibtex.web` L868](deps/bibtex/src/bibtex.web#L868) defines `U+0020` (space) and `U+0009` (horizontal tab) as space characters. BibTeX.NET considers precisely `[ \t\n\v\f\r]` as space characters.

## What are valid entry types and field names?

[`bibtex.web` L883](deps/bibtex/src/bibtex.web#L883) defines `[0-9A-Za-z]` and anything from ``!$&*+-./:;<>?@[\]^_`|~`` as valid identifier characters. A valid identifier consists of one or more valid identifier characters and does not being with a numeric character (i.e., `[0-9]`). BibTeX.NET follows this definition and will not allow non-ASCII characters in identifiers.

It should be remarked that `@` is an identifier character, so `@@misc{ key, title = "Hello" }` creates an entry of type `@misc`, and `@ @ { key, @ = 2 }` creates an entry of type `@` with a field of name `@`.

## What are valid database keys?

BibTeX is quite tolerant about permissible characters in database keys (i.e., `key` in `@misc{ key, title = "Hello" }`), which [seems](https://tex.stackexchange.com/q/388500) to cause a [lot](https://tex.stackexchange.com/q/408530) of [headache](https://tex.stackexchange.com/q/581901). Looking at [`bibtex.web` L6156 “Scan the entry’s database key”](deps/bibtex/src/bibtex.web#L6156), it depends on the delimiter of the entry:

- If the entry is brace-delimited (i.e., `@misc{ key, title = "Hello" }`), then the database key cannot contain space characters, the comma `,`, or the right brace `}`.
- If the entry is parenthesis-delimited (i.e., `@misc( key, title = "Hello" )`), then the database key cannot contain space characters or the comma `,`.

In particular, a database key does not have to be an identifier, and can contain unrecognized bytes (e.g., UTF-8 encoding of non-ASCII characters). The edge cases are summarized below:

| Content | Valid? | Key |
| :------ | :----: | :-- |
| `@misc{你}` | ✔ | `你` |
| `@misc{你,}` | ✔ | `你` |
| `@misc{}` | ✔ | empty |
| `@misc{,}` | ✔ | empty |
| `@misc{你` | ❌ | `你` |
| `@misc{你,` | ❌ | `你` |
| `@misc{你 你` | ❌ | `你` |
| `@misc{,` | ❌ | empty |
| `@misc{` | ❌ | no entry |
| `@misc((){}{你(}{)}(),)` | ✔ | `(){}{你(}{)}()` |
| `@misc(,)` | ✔ | empty |
| `@misc({你})` | ❌ | `{你})` |
| `@misc()` | ❌ | `)` |
| `@misc(你,` | ❌ | `你` |
| `@misc(你` | ❌ | `你` |
| `@misc(你 你` | ❌ | `你` |
| `@misc(,` | ❌ | empty |
| `@misc(` | ❌ | no entry |

Note that even if the syntax is invalid, an entry might be created. The pattern is that the entry is created as long as there is non-space character following `{` or `(`.

After `{` or `(`, BibTeX will skip space and lines that solely consists of space characters, so

```bib
@misc{

key

}
```

is valid and creates an entry with key `key`.

## How `@comment` works

In fact, `@comment` does not even look for the opening delimiter `{` or `(`. [Tame the BeaST](deps/tamethebeast/src/ttb_en.sec3.tex#L77) claims that `@comment` is used to “comment a large part of the bibliography easily”, which does not hold. The following BibTeX database file creates article entries with key `a`, `b`, `c`.

```bib
@comment{@misc{a,title=1}}
@comment @misc{b,title=1}
@comment
@misc{c,title=1}
```

However, since `@` is an identifier character, `@comment@misc{d,title=1}` will not create an entry.

Comapre [`bibtex.web` L5491 “Process a `@comment` command”](deps/bibtex/src/bibtex.web#L5491) with the first few lines of [`bibtex.web` L5516 “Process a `@preamble` command”](deps/bibtex/src/bibtex.web#L5516), we see that the parser state is reset immediately after seeing `@comment` followed by non-identifier character.

## Under what conditions are `@string` and `@preamble` commands processed?

The following `@string` commands are not processed:

- `@string`
- `@string{`
- `@string{ name`
- `@string{ name =`
- `@string{ name = 你`
- `@string{ name = "Hello"`
- `@string{ name = "Hello" #`
- `@string{ name = "Hello" # "}"`
- `@string{ name = "Hello" # {}`
- `@string( name = "Hello" # }`

However, the following `@string` commands are processed, with the value of `name` being `Hello`:

- `@string{ name = "Hello",`
- `@string{ name = "Hello" 你`
- `@string{ name = "Hel" # {lo} 你`
- `@string( name = "Hel" # {lo} # "" }`
- `@string( name = "Hello" # {}}`

The pattern is as follows:

- If `=` is not reached, the command is not processed.
- After `=`, the parser tries to read a series of concatenated components (let us call this “parsing a value”).
  - The parser starts by expecting a component.
  - If the parser cannot read a well-formed component (brace-delimited, quote-delimited, or bare numeric literal, or an identifier of the referenced string), then the value fails to parse.
  - Otherwise, the parser sees if the end of input is reached, if so, the value fails to parse.
  - Otherwise, the end of input is not reached, the parser checks if the next character is `#`, if so, it goes back to the state of expecting a component. If not, the value succeeds to parse.
- If the value parsing succeeded, the `@string` command is processed, regardless of whether the next character is the correct closing delimiter `}` or `)`.

Similarly, after the opening delimiter `{` or `(` of `@preamble`, the parser tries to parse a value, and if it succeeds, the command is processed.

I have a hard time appreciating the fact that the value parsing process checks for the end of input and fails fast once it sees the end of input, so in BibTeX.NET, the value parsing functon, `EatString`, does not fail if it sees the end of input after a component.

This does not mean BibTeX.NET will exhibit a different behavior by processing `@string{ name = "Hello"`. The check is done after `EatString` returns.

BibTeX.NET provides sufficient mechanism for identifying the different modes of failure, and the user can customize its behavior by implementing `IBib32ParserUnsafeOverrides`. See

- [`IBib32ParserUnsafeOverrides` interface](src/Neat.BibTeX/Utils/IBib32ParserUnsafeOverrides.cs)
- [`Bib32ParserUnsafe` structure](src/Neat.BibTeX/Utils/Bib32ParserUnsafe.cs)
- [`Bib32ParserCatchErrors.Overrides` structure](src/Neat.BibTeX/BibParsers/Bib32ParserCatchErrors.cs)

## Under what conditions are entries and fields created?

Similar to the case of `@string` and `@preamble`, and combining with the behavior of database keys:

- If the end of input is reached before or immediately after the opening delimiter `{` or `(`, the entry is not created. Otherwise, the entry is always created, even if later there is a syntax error.
- For each field, it is created when the parser succeeds in parsing its name, `=`, and its value. (Remember that value parsing fails if the end of input is reached after the last component.)
- It is not an error to have a trailing `,` before the closing delimiter `}` or `)`.

This means `@misc{ key, title = "Hello", author = {Someone}` will create an entry with key `key` and one field, with name `title` and value `Hello`. Note that `author` is not saved. The standard parser of BibTeX.NET replicates this behavior. Similarly, BibTeX.NET provides mechanisms for customization (see the end of the previous section).
