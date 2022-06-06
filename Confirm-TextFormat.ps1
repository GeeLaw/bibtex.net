[CmdletBinding()]
Param()

End
{

$txt = @(
  '.gitattributes',
  '.gitignore',
  '.md',
  '.editorconfig',
  '.sln',
  '.csproj',
  '.ps1',
  '.cs',
  '.tt',
  '.json',
  '.ps1xml'
);
$bin = @('.user');
$dir = @('.git', '.vs', 'bin', 'obj');

$recursiveCheck = {
  If ($_.PSIsContainer)
  {
    If ($_.Name.ToLowerInvariant() -in $dir)
    {
      Write-Verbose "Ignored folder: $($_.FullName)";
      Return;
    }
    Get-ChildItem -LiteralPath $_.FullName -Force |
      ForEach-Object $recursiveCheck;
    Return;
  }
  $ext = $_.Extension.ToLowerInvariant();
  If ($ext -in $txt)
  {
    $content = [System.IO.File]::ReadAllBytes($_.FullName);
    If ($content.Count -ge 3 -and
      $content[0] -eq 0xEF -and
      $content[1] -eq 0xBB -and
      $content[2] -eq 0xBF)
    {
      Write-Warning "   BOM in UTF8: $($_.FullName)";
    }
    ElseIf (0 -in $content)
    {
      Write-Warning "\0 or UTF16/32: $($_.FullName)";
    }
    ElseIf (13 -in $content)
    {
      Write-Warning "  with CR (\r): $($_.FullName)";
    }
  }
  ElseIf ($ext -notin $bin)
  {
    Write-Warning "  Unknown type: $($_.FullName)";
  }
};

Get-ChildItem -LiteralPath $PSScriptRoot -Force |
  ForEach-Object $recursiveCheck;
Write-Verbose 'Confirm-TextFormat.ps1 completed.';

}
