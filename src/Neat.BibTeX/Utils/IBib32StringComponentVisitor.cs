using Neat.BibTeX.Data;
using Neat.Unicode;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib32StringComponent"/>.
  /// </summary>
  public interface IBib32StringComponentVisitor
  {
    void VisitReference(String32 key);
    void VisitValue(String32 value);
  }
}
