using Neat.BibTeX.Data;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib32Entry"/>.
  /// </summary>
  public interface IBib32EntryVisitor
  {
    void VisitStringEntry(Bib32StringEntry entry);
    void VisitPreambleEntry(Bib32PreambleEntry entry);
    void VisitGeneralEntry(Bib32GeneralEntry entry);
  }
}
