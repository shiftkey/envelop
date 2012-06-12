function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value
    Split-Path $Invocation.MyCommand.Path
}

$ScriptDir = (Get-ScriptDirectory)
$BaseDir = Join-Path $ScriptDir ".."
$pfx = "$BaseDir\Cocoon.Tests\Cocoon.Tests_TemporaryKey.pfx"
$pfx
Certutil -importpfx "$pfx"