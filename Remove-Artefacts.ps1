[CmdletBinding(SupportsShouldProcess = $True, ConfirmImpact = 'High')]
Param([switch]$Force)

End
{

$local:src = [System.IO.Path]::Combine($PSScriptRoot, 'src')
$local:TestResults = [System.IO.Path]::Combine($src, 'TestResults')
If ((Test-Path -LiteralPath $TestResults) -and
  ($Force -or $PSCmdlet.ShouldProcess($TestResults, 'Remove Directory')))
{
  (Get-Item -LiteralPath $TestResults).Delete($true);
}
Get-ChildItem $src -Include *.DotSettings.user -File -Force -Recurse | ForEach-Object {
  If ($Force -or $PSCmdlet.ShouldProcess($_.FullName, 'Remove File'))
  {
    $_.Delete();
  }
};
Get-ChildItem $src -Include bin, obj, .vs -Directory -Force -Recurse | ForEach-Object {
  If ($Force -or $PSCmdlet.ShouldProcess($_.FullName, 'Remove Directory'))
  {
    $_.Delete($true);
  }
};
Write-Verbose 'Remove-Artefacts.ps1 completed.';

}
