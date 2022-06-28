using Neat.BibTeX.BstModel;

using StringT = Neat.Unicode.String32;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bst32Operator"/>.
  /// </summary>
  public interface IBst32OperatorVisitor
  {
    void VisitInvalid(Bst32Operator invalid);

    void VisitInteger(int integerLiteral);
    void VisitString(StringT stringLiteral);
    void VisitReference(StringT reference);
    void VisitInvocation(StringT invocation);
    void VisitBlock(Bst32Operator[] block);
  }
}
