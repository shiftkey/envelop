param([Parameter(Mandatory=$true)][string]$Version)

New-Item ..\artifacts -type directory -Force

..\.nuget\NuGet.exe pack ..\Cocoon\Cocoon.csproj -Prop Configuration=Release -Output ..\artifacts -Version $Version

..\.nuget\NuGet.exe pack ..\Cocoon.Data\Cocoon.Data.csproj -Prop Configuration=Release -Output ..\artifacts -Version $Version

..\.nuget\NuGet.exe pack ..\Cocoon.MEF\Cocoon.MEF.csproj -Prop Configuration=Release -Output ..\artifacts -Version $Version

..\.nuget\NuGet.exe pack ..\Cocoon.Autofac\Cocoon.Autofac.csproj -Prop Configuration=Release -Output ..\artifacts -Version $Version