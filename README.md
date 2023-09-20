# BibTeX.NET

**This project is undergoing a complete rewrite. This is the old version, now considered obsolete although (hopefully) still functioning. It is provided for convenience. Though legal, I ask you to refrain from relicensing the work to the world or publishing it elsewhere, so that it does not create confusion with my future version of BibTeX.NET.**

BibTeX.NET is a .NET library for working with `.bib` files.

## Quirks of BibTeX 0.99d and Compatibility

Most of the time, BibTeX.NET tries to replicate the quirks of BibTeX 0.99d, but there is no such guarantee. The peculiar incompatibility should not matter for a user who does not try to use extremely ill-written BibTeX files. BibTeX.NET behavior is created by a mixture of running BibTeX 0.99d, reading [Tame the BeaST](deps/tamethebeast), consulting the source code of [BibTeX 0.99d](deps/bibtex), and personal preferences. See [quirks.bib.md](quirks.bib.md) for details.
