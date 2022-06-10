using Neat.BibTeX.BibModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib32StringComponent"/>.
  /// </summary>
  public interface IBib32StringComponentVisitor
  {
    /// <summary>
    /// This method is called if the component is not a literal,
    /// including the case when <see cref="Bib32StringComponent.Type"/> is not defined.
    /// </summary>
    void VisitName(Bib32StringComponent name);

    void VisitQuoteLiteral(Bib32StringComponent quoteLiteral);
    void VisitNumericLiteral(Bib32StringComponent numericLiteral);
    void VisitBraceLiteral(Bib32StringComponent braceLiteral);
  }
}
