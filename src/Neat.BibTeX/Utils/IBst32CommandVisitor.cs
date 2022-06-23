using Neat.BibTeX.BstModel;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the visitor pattern for <see cref="Bst32Command"/>.
  /// </summary>
  public interface IBst32CommandVisitor
  {
    void VisitEntryCommand(Bst32EntryCommand cmd);
    void VisitIntegersCommand(Bst32IntegersCommand cmd);
    void VisitStringsCommand(Bst32StringsCommand cmd);
    void VisitMacroCommand(Bst32MacroCommand cmd);
    void VisitFunctionCommand(Bst32FunctionCommand cmd);
    void VisitReadCommand(Bst32ReadCommand cmd);
    void VisitExecuteCommand(Bst32ExecuteCommand cmd);
    void VisitIterateCommand(Bst32IterateCommand cmd);
    void VisitSortCommand(Bst32SortCommand cmd);
    void VisitReverseCommand(Bst32ReverseCommand cmd);
  }
}
