using Neat.BibTeX.BibModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib16StringComponent"/>.
  /// </summary>
  public interface IBib16StringComponentVisitor
  {
    /// <summary>
    /// This method is called if the component is not a literal,
    /// including the case when <see cref="Bib16StringComponent.Type"/> is not defined.
    /// </summary>
    void VisitName(Bib16StringComponent name);

    void VisitQuoteLiteral(Bib16StringComponent quoteLiteral);
    void VisitNumericLiteral(Bib16StringComponent numericLiteral);
    void VisitBraceLiteral(Bib16StringComponent braceLiteral);
  }
}
