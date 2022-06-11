using System;
using System.Runtime.CompilerServices;

namespace Neat.BibTeX
{
  public sealed class BibParserException : ApplicationException
  {
    [MethodImpl(Helper.OptimizeInline)]
    public BibParserException(string message) : base(message)
    {
    }
  }
}
