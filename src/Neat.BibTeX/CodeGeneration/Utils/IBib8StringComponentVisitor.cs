using Neat.BibTeX.BibModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bib8StringComponent"/>.
  /// </summary>
  public interface IBib8StringComponentVisitor
  {
    /// <summary>
    /// This method is called if the component is not a literal,
    /// including the case when <see cref="Bib8StringComponent.Type"/> is not defined.
    /// </summary>
    void VisitName(Bib8StringComponent name);

    void VisitQuoteLiteral(Bib8StringComponent quoteLiteral);
    void VisitNumericLiteral(Bib8StringComponent numericLiteral);
    void VisitBraceLiteral(Bib8StringComponent braceLiteral);
  }
}
