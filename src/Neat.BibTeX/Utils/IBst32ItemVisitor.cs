using Neat.BibTeX.BstModel;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bst32Item"/>.
  /// </summary>
  public interface IBst32ItemVisitor
  {
    void VisitInvalid(Bst32Item invalid);

    void VisitEmpty();
    void VisitInteger(int integerItem);
    void VisitString(StringT stringItem);
    void VisitReference(StringT reference);
    void VisitMissingField(StringT missingField);
    void VisitBlock(Bst32Operator[] block);
  }
}
