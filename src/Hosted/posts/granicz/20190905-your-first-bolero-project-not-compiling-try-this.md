---
title: "Your first Bolero project not compiling? Try this!"
categories: "bolero,f#,websharper"
abstract: "The Bolero homepage starts off with a video that shows you how to create your first client-server app with three simple commands, ..."
identity: "5792,87267"
---
[Bolero][bolero] is a functional, reactive web library built on top of [Blazor][blazor] for F# developers. It combines Elm's [Model-View-Update architecture][elm-arch] (MVU) via [Elmish][elmish] with powerful features pioneered by [WebSharper][websharper], such as templating and easy remoting, to give developers a sound, effective and highly productive way to structure and implement full-stack .NET web applications that run on WebAssembly.

The Bolero homepage starts off with a video that shows you how to create your first client-server app with three simple commands:

![](https://i.imgur.com/Oi9w7HMh.png)

Assuming you executed these commands in a simple shell (say, `cmd` on Windows), any issue hindering a successful run will show the following unceremonious output:

```
C:\fsbolero>cd MyApp3 && dotnet run -p src/MyApp3.Server
C:\fsbolero\MyApp3\src\MyApp3.Client\Main.fs(233,12): warning FS0044: This construct is deprecated. Use withHotReload instead [C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj]
C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\Blazor.MonoRuntime.targets(633,5): error MSB3073: The command "dotnet "C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\../tools/Microsoft.AspNetCore.Blazor.Build.dll" write-boot-json "obj\Debug\netstandard2.0\MyApp3.Client.dll" --references "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\bootjson-references.txt" --embedded-resources "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\embedded.resources.txt" --linker-enabled --output "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\blazor.boot.json"" exited with code -2147450730. [C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj]

The build failed. Please fix the build errors and run again.
```
When this happens, and even in general, it's a good idea to add the `-v n` suffix to that last command to enable normal build output (or other debug levels as you need.)

For instance, now on an older machine I pulled out from the closet, I get the following more informative output:

```
C:\fsbolero\MyApp3>dotnet run -p src/MyApp3.Server -v n
Build started 9/3/2019 3:04:52 PM.
     1>Project "C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj" on node 1 (Restore target(s)).
     1>Restore:
         Committing restore...
         Committing restore...
         Assets file has not changed. Skipping assets file writing. Path: C:\fsbolero\MyApp3\src\MyApp3.Client\obj\project.assets.json
         Assets file has not changed. Skipping assets file writing. Path: C:\fsbolero\MyApp3\src\MyApp3.Server\obj\project.assets.json
         Restore completed in 46.92 ms for C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj.
         Restore completed in 46.92 ms for C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj.

         NuGet Config files used:
             C:\Users\Adam\AppData\Roaming\NuGet\NuGet.Config
             C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config

         Feeds used:
             https://api.nuget.org/v3/index.json
             https://daily.websharper.com/nuget
             C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\
             https://dotnetfeed.blob.core.windows.net/aspnet-aspnetcore-tooling/index.json
             https://dotnetfeed.blob.core.windows.net/aspnet-aspnetcore/index.json
             https://dotnetfeed.blob.core.windows.net/aspnet-entityframeworkcore/index.json
             https://dotnetfeed.blob.core.windows.net/aspnet-extensions/index.json
             https://dotnetfeed.blob.core.windows.net/dotnet-core/index.json
             https://dotnetfeed.blob.core.windows.net/dotnet-windowsdesktop/index.json
     1>Done Building Project "C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj" (Restore target(s)).
   1:7>Project "C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj" on node 1 (default targets).
     1>_CheckForNETCoreSdkIsPreview:
       C:\Program Files\dotnet\sdk\3.0.100-preview5-011568\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.RuntimeIdentifierInference.targets(157,5): message NETSDK1057: You are using a preview version of .NET Core. See: https://aka.ms/dotnet-core-preview [C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj]
   1:7>Project "C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj" (1:7) is building "C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj" (2:6) on node 1 (default targets).
     2>_HandlePackageFileConflicts:
       C:\Program Files\dotnet\sdk\3.0.100-preview5-011568\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ConflictResolution.targets(39,5): message NETSDK1041: Encountered conflict between 'Reference:C:\Users\Adam\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.Net.Http.dll' and 'Reference:C:\Users\Adam\.nuget\packages\system.net.http\4.3.2\ref\netstandard1.3\System.Net.Http.dll'.  NETSDK1033: Choosing 'Reference:C:\Users\Adam\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.Net.Http.dll' because AssemblyVersion '4.1.2.0' is greater than '4.1.1.1'. [C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj]
       GenerateTargetFrameworkMonikerAttribute:
       Skipping target "GenerateTargetFrameworkMonikerAttribute" because all output files are up-to-date with respect to the input files.
       CoreGenerateAssemblyInfo:
       Skipping target "CoreGenerateAssemblyInfo" because all output files are up-to-date with respect to the input files.
       CoreCompile:
       Skipping target "CoreCompile" because all output files are up-to-date with respect to the input files.
       _CopyFilesMarkedCopyLocal:
         Touching "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\MyApp3.Client.fsproj.CopyComplete".
       _CopyOutOfDateSourceItemsToOutputDirectory:
       Skipping target "_CopyOutOfDateSourceItemsToOutputDirectory" because all output files are up-to-date with respect to the input files.
       CopyFilesToOutputDirectory:
         MyApp3.Client -> C:\fsbolero\MyApp3\src\MyApp3.Client\bin\Debug\netstandard2.0\MyApp3.Client.dll
       _GenerateLinkerDescriptor:
       Skipping target "_GenerateLinkerDescriptor" because all output files are up-to-date with respect to the input files.
       _LinkBlazorApplication:
       Skipping target "_LinkBlazorApplication" because all output files are up-to-date with respect to the input files.
       _GenerateBlazorBootJson:
         dotnet "C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\../tools/Microsoft.AspNetCore.Blazor.Build.dll" write-boot-json "obj\Debug\netstandard2.0\MyApp3.Client.dll" --references "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\bootjson-references.txt" --embedded-resources "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\embedded.resources.txt" --linker-enabled --output "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\blazor.boot.json"
         It was not possible to find any compatible framework version
         The specified framework 'Microsoft.NETCore.App', version '3.0.0-preview7-27912-14' was not found.
           - The following frameworks were found:
               2.0.6 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
               2.0.9 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
               2.1.2 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
               2.1.4 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
               2.1.9 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
               2.2.3 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
               3.0.0-preview5-27626-15 at [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]

         You can resolve the problem by installing the specified framework and/or SDK.

         The .NET Core frameworks can be found at:
           - https://aka.ms/dotnet-download
     2>C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\Blazor.MonoRuntime.targets(633,5): error MSB3073: The command "dotnet "C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\../tools/Microsoft.AspNetCore.Blazor.Build.dll" write-boot-json "obj\Debug\netstandard2.0\MyApp3.Client.dll" --references "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\bootjson-references.txt" --embedded-resources "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\embedded.resources.txt" --linker-enabled --output "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\blazor.boot.json"" exited with code -2147450730. [C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj]
     2>Done Building Project "C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj" (default targets) -- FAILED.
     1>Done Building Project "C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj" (default targets) -- FAILED.

Build FAILED.

       "C:\fsbolero\MyApp3\src\MyApp3.Server\MyApp3.Server.fsproj" (default target) (1:7) ->
       "C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj" (default target) (2:6) ->
       (_GenerateBlazorBootJson target) ->
         C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\Blazor.MonoRuntime.targets(633,5): error MSB3073: The command "dotnet "C:\Users\Adam\.nuget\packages\microsoft.aspnetcore.blazor.build\3.0.0-preview7.19365.7\targets\../tools/Microsoft.AspNetCore.Blazor.Build.dll" write-boot-json "obj\Debug\netstandard2.0\MyApp3.Client.dll" --references "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\bootjson-references.txt" --embedded-resources "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\embedded.resources.txt" --linker-enabled --output "C:\fsbolero\MyApp3\src\MyApp3.Client\obj\Debug\netstandard2.0\blazor\blazor.boot.json"" exited with code -2147450730. [C:\fsbolero\MyApp3\src\MyApp3.Client\MyApp3.Client.fsproj]

    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:02.07

The build failed. Please fix the build errors and run again.
```

### Installing the right .NET Core SDK

Tucked away in the [source repository][bolero-gh], in the main README file, there is a short section that details what you need to compile and run Bolero itself:

> .NET Core SDK 3.0-preview7 or newer. Download it  [here](https://dotnet.microsoft.com/download/dotnet-core/3.0).

Not surprisingly, this same dependency applies to your Bolero applications as well. The normal build output we saw above already listed my installed frameworks, but if I need to, I can also quickly check separately:

```
C:\fsbolero>dotnet --info
.NET Core SDK (reflecting any global.json):
 Version:   3.0.100-preview5-011568
 Commit:    b487ff10aa

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.17134
 OS Platform: Windows
 RID:         win10-x64
 Base Path:   C:\Program Files\dotnet\sdk\3.0.100-preview5-011568\

Host (useful for support):
  Version: 3.0.0-preview5-27626-15
  Commit:  61f30f5a23

.NET Core SDKs installed:
  2.1.104 [C:\Program Files\dotnet\sdk]
  2.1.202 [C:\Program Files\dotnet\sdk]
  2.1.302 [C:\Program Files\dotnet\sdk]
  2.1.402 [C:\Program Files\dotnet\sdk]
  2.1.505 [C:\Program Files\dotnet\sdk]
  2.1.602 [C:\Program Files\dotnet\sdk]
  2.1.700-preview-009597 [C:\Program Files\dotnet\sdk]
  2.1.700-preview-009601 [C:\Program Files\dotnet\sdk]
  2.2.202 [C:\Program Files\dotnet\sdk]
  2.2.300-preview-010046 [C:\Program Files\dotnet\sdk]
  2.2.300-preview-010050 [C:\Program Files\dotnet\sdk]
  3.0.100-preview5-011568 [C:\Program Files\dotnet\sdk]

.NET Core runtimes installed:
  Microsoft.AspNetCore.All 2.1.2 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.All]
  ...
  ...
```

Heading over to the [Microsoft .NET Core download page][dnc-download] and installing `preview7` resolves the issue, and the vanilla Bolero template app now works as expected.

Happy coding!

[blazor]: https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor
[bolero]: https://fsbolero.io
[bolero-gh]: https://github.com/fsbolero/bolero
[dnc-download]: https://dotnet.microsoft.com/download/dotnet-core/3.0
[elm-arch]: https://guide.elm-lang.org/architecture
[elmish]: https://elmish.github.io/elmish
[websharper]: https://websharper.com
