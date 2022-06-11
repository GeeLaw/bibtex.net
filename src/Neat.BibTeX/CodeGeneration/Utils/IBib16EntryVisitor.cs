using Neat.BibTeX.BibModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib16Entry"/>.
  /// </summary>
  public interface IBib16EntryVisitor
  {
    void VisitStringEntry(Bib16StringEntry entry);
    void VisitPreambleEntry(Bib16PreambleEntry entry);
    void VisitGeneralEntry(Bib16GeneralEntry entry);
  }
}
