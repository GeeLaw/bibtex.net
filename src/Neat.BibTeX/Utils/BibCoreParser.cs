using System;
using System.Runtime.CompilerServices;
using Neat.BibTeX.Data;

namespace Neat.BibTeX.Utils
{
  /// <summary>
  /// Implements the BibTeX parser.
  /// This object can only be used for one pass.
  /// </summary>
  public struct BibCoreParser<TChar, TString, TStringServices>
    where TStringServices : struct, IStringServices<TChar, TString>
  {
    [MethodImpl(Helper.JustOptimize)]
    public void Start(TStringServices services)
    {
      throw new NotImplementedException();
    }

    [MethodImpl(Helper.JustOptimize)]
    public void Eat(TChar ch)
    {
      throw new NotImplementedException();
    }

    [MethodImpl(Helper.JustOptimize)]
    public BibEntry<TString>[] Finish()
    {
      throw new NotImplementedException();
    }
  }
}
