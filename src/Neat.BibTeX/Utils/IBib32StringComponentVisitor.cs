using Neat.BibTeX.BibModel;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib32StringComponent"/>.
  /// </summary>
  public interface IBib32StringComponentVisitor
  {
    void VisitInvalid(Bib32StringComponent invalid);

    void VisitName(StringT name);
    void VisitQuoteLiteral(StringT quoteLiteral);
    void VisitNumericLiteral(StringT numericLiteral);
    void VisitBraceLiteral(StringT braceLiteral);
  }
}
