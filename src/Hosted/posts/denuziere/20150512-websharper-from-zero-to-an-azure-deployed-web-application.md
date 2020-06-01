---
title: "WebSharper: From Zero to an Azure-deployed Web Application"
categories: "paket,azure,fsharp,deployment,tutorial,web,websharper"
abstract: "Find out how to easily set up a WebSharper application for deployment to Azure."
identity: "4367,79405"
---
We are often asked what the deployment story is like for WebSharper applications. Our usual reply has long been that WebSharper applications follow standard formats, in particular:

* Client-Server Web Applications are simply ASP.NET applications, and can be deployed using usual methods such as publishing from Visual Studio or using Azure's git integration;
* HTML Applications are composed of static files located in `$(WebSharperHtmlDirectory)` (which is `bin/html` by default) and can be deployed from there using the method of your choice.
* Single-Page Applications are also composed of static files, `index.html` and the `Content` folder, so the same methods apply.

However there can be some caveats, in particular with regards to running the F# compiler and referencing FSharp.Core.dll in build-and-deploy environments.

Fortunately, the recently released NuGet package [FSharp.Compiler.Tools](http://www.nuget.org/packages/FSharp.Compiler.Tools) combined with the excellent package manager [Paket](https://fsprojects.github.io/Paket/) now provide a nice and streamlined development and deployment experience for git-hosted, Azure-deployed applications.

This article presents the build and deployment setup for a reimplementation of the popular game 2048, available [on GitHub](https://github.com/intellifactory/2048). To try it out, simply click the button "Deploy to Azure" and follow the instructions.

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/?repository=https://github.com/intellifactory/2048)

## The project

This particular project was created as a Single-Page Application; this project type was chosen because the application runs on a single page and is only composed of client-side code. The solution, `2048.sln`, contains a single project located at `Game2048/Game2048.fsproj`.

If you want to recreate this setup, you can create a Single-Page Application from Visual Studio or Xamarin Studio / MonoDevelop with the [WebSharper extension installed](http://websharper.com/downloads). This deployment setup will also work if you create a Client-Server Application or an HTML Application instead. For Self-Hosted Client-Server Applications, you will additionally need to set up an HttpPlatformHandler to run the generated executable, similarly to [Scott Hanselman's Suave setup](http://www.hanselman.com/blog/RunningSuaveioAndFWithFAKEInAzureWebAppsWithGitAndTheDeployButton.aspx).

## Paket

For package management, the project uses Paket. It offers many advantages over traditional NuGet, [which you can read about here](http://fsprojects.github.io/Paket/faq.html).

Note that `paket restore` is run in the build script before running MSBuild. Indeed, since we will be importing several `.targets` files that come from packages, the packages *must* be restored before running MSBuild or opening the project in an IDE. So for your first build after cloning the 2048 project, you can either run the full `build.cmd`, or if you only want to restore the packages, you can run:

```
.paket/paket.bootstrapper.exe && .paket/paket.exe restore
```

If you want to reproduce this setup for your own project as created in the previous section, here are the steps:

* Remove the WebSharper NuGet package from the project and delete the file `<your_project_name>/packages.config` if it exists.

* Download `paket.bootstrapper.exe` and `paket.targets` from [here](https://github.com/fsprojects/Paket/releases) into the folder `.paket`.

* To ensure that you build with the right package versions after a git pull, add the following to `<your_project_name>.fsproj`:

    ```xml
    <Import Project="..\.paket\paket.targets" />
    ```

* Run the following commands:

    ```
    # Download paket.exe:
    .paket/bootstrapper.exe
    # Initialize paket.dependencies:
    .paket/paket.exe init
    # Install the WebSharper package into your project:
    .paket/paket.exe add nuget WebSharper project <your_project_name>
    ```
    
* The files `paket.dependencies`, `paket.lock` and `<your_project_name>/paket.references` must be committed.

## The F# Compiler

Since `fsc` is not available on Azure, we retrieve it from NuGet. We reference the package `FSharp.Compiler.Tools` which contains the compiler toolchain. By importing `tools/Microsoft.FSharp.targets` from this package in our project file, we instruct MSBuild to use the F# compiler from the package. This means that even when building locally, `fsc` from the package will be used. This ensures consistency between local and deployment builds.

If you want to apply this change to your own project, here are the steps:

* Install the F# compiler package:

    ```
    .paket/paket.exe add nuget FSharp.Compiler.Tools
    ```
    
* Use it in your project: in `<your_project_name>.fsproj`:
    * Remove any references to `Microsoft.FSharp.targets` and `FSharp.Core.dll`. In a Visual Studio-created project, this means removing this whole block:

    ```xml
    <!-- F# targets -->
    <Choose>
      <When Condition="'$(VisualStudioVersion)' == '11.0'">
        <PropertyGroup Condition="Exists('$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets')">
          <FSharpTargetsPath>$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets</FSharpTargetsPath>
        </PropertyGroup>
      </When>
      <Otherwise>
        <PropertyGroup Condition="Exists('$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets')">
          <FSharpTargetsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets</FSharpTargetsPath>
        </PropertyGroup>
      </Otherwise>
    </Choose>
    <Import Project="$(FSharpTargetsPath)" />
    ```
    
    * and add this line instead:

    ```xml
      <Import Project="..\packages\FSharp.Compiler.Tools\tools\Microsoft.FSharp.targets" />
    ```

You now have a project that should build and run fine locally. Try it out!

![2048 Screenshot](http://i.imgur.com/I8Mmwrm.png)

## Azure Deployment

Now, on to the deployment setup itself. We will be using a custom build script, so we need to tell so in the `.deployment` file:

```
[config]
command = build.cmd
```

The `build.cmd` script itself is in three parts:

1. **Package restore**: we retrieve `paket.exe` if it hasn't already been retrieved, and run it to `restore` packages.

    ```
    if not exist .paket\paket.exe (
      .paket\paket.bootstrapper.exe
    )

    .paket\paket.exe restore
    ```

2. **Build**: Azure conveniently points the environment variable `MSBUILD_PATH` to the path to `MSBuild.exe`; in order to be also able to run this script locally, we check for it and set it to the standard installation location if it doesn't exist. Then, we run it.

    ```
    if "%MSBUILD_PATH%" == "" (
      set MSBUILD_PATH="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
    )

    %MSBUILD_PATH% /p:Configuration=Release
    ```

3. **Deploy**: Deploying the application simply consists in copying the application files to the Azure-provided `DEPLOYMENT_TARGET` folder. [The actual file](https://github.com/intellifactory/2048/blob/master/build.cmd#L46-L62) in the 2048 repository is a bit more complex than necessary for Azure because it is also used on AppVeyor to deploy the application to github-pages. But a simple implementation can just copy all files and subdirectories from the project directory to `DEPLOYMENT_TARGET`:

    ```
    if not "%DEPLOYMENT_TARGET%" == "" (
      xcopy /y /e <your_project_name> "%DEPLOYMENT_TARGET%"
    )
    ```

As a recap, here is the full `build.cmd` with some extra error management:

```
@ECHO OFF
setlocal

echo ====== Restoring packages... ======

if not exist .paket\paket.exe (
  .paket\paket.bootstrapper.exe
)

.paket\paket.exe restore

if not %ERRORLEVEL% == 0 (
  echo ====== Failed to restore packages. ======
  exit 1
)

echo ====== Building... ======

if "%MSBUILD_PATH%" == "" (
  set MSBUILD_PATH="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
)

%MSBUILD_PATH% /p:Configuration=Release

if not %ERRORLEVEL% == 0 (
  echo ====== Build failed. ======
  exit 1
)

if not "%DEPLOYMENT_TARGET%" == "" (
  echo ====== Deploying... ======
  xcopy /y /e <your_project_name> "%DEPLOYMENT_TARGET%"
)

echo ====== Done. ======

```

And there you have it! A WebSharper application easily deployed to Azure with a simple configuration and consistent build setup between local and deployed.

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/?repository=https://github.com/intellifactory/2048)

Note that this particular example is a Single-Page Application, but the same setup can be used for Client-Server Applications and HTML Applications. For the latter, make sure to copy the WebSharperHtmlDirectory (`<your_project_name>/bin/html` by default) in the final step rather than the project folder itself.

Thanks to Steffen Forkmann and Don Syme for their quick response on creating the FSharp.Compiler.Tools NuGet package, and to Scott Hanselman for his [Suave Azure deployment tutorial](http://www.hanselman.com/blog/RunningSuaveioAndFWithFAKEInAzureWebAppsWithGitAndTheDeployButton.aspx) which has been of great help to create this one despite the fairly different final setup.

Happy coding!