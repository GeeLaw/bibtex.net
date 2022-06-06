using Neat.BibTeX.Data;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="BibStringComponent{TString}"/>.
  /// </summary>
  public interface IBibStringComponentVisitor<TString>
  {
    void VisitReference(TString key);
    void VisitValue(TString value);
  }
}
