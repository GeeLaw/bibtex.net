using Neat.BibTeX.BibModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib8Entry"/>.
  /// </summary>
  public interface IBib8EntryVisitor
  {
    void VisitStringEntry(Bib8StringEntry entry);
    void VisitPreambleEntry(Bib8PreambleEntry entry);
    void VisitGeneralEntry(Bib8GeneralEntry entry);
  }
}
