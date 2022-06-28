using Neat.BibTeX.BibModel;

using StringT = System.String;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib16StringComponent"/>.
  /// </summary>
  public interface IBib16StringComponentVisitor
  {
    void VisitInvalid(Bib16StringComponent invalid);

    void VisitName(StringT name);
    void VisitQuoteLiteral(StringT quoteLiteral);
    void VisitNumericLiteral(StringT numericLiteral);
    void VisitBraceLiteral(StringT braceLiteral);
  }
}
