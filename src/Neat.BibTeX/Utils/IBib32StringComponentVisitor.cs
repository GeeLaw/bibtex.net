using Neat.BibTeX.BibModel;
using Neat.Unicode;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib32StringComponent"/>.
  /// </summary>
  public interface IBib32StringComponentVisitor
  {
    void VisitName(String32 name);
    void VisitLiteral(String32 literal);
  }
}
