using Neat.BibTeX.BibModel;

using StringT = Neat.Unicode.String8;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib8StringComponent"/>.
  /// </summary>
  public interface IBib8StringComponentVisitor
  {
    void VisitInvalid(Bib8StringComponent invalid);

    void VisitName(StringT name);
    void VisitQuoteLiteral(StringT quoteLiteral);
    void VisitNumericLiteral(StringT numericLiteral);
    void VisitBraceLiteral(StringT braceLiteral);
  }
}
