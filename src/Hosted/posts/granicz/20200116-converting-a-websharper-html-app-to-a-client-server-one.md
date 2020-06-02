---
title: "Converting a WebSharper HTML app to a client-server one"
categories: "blogging,f#,websharper"
abstract: "In my previous article about \"F# metablogging: introducing BlogEngine for your static markdown-based F# blog\", I briefly outlined a quick strategy to switch a WebSharper HTML project to a hosted, client-server app, and vice versa. In this article, I will walk through some suprises that came along the way and what I did to resolve them."
identity: "5863,87652"
---
In my previous article about [F# metablogging: introducing BlogEngine for your static markdown-based F# blog](//intellifactory.com/user/granicz/20191226-f-metablogging-introducing-blogengine-for-your-static-markdown-based-f-blog), I briefly outlined a quick strategy to switch a WebSharper HTML project to a hosted, client-server app, and vice versa. The steps mentioned there involved:

 1) changing the project type from `"bundle"` to `"site"`, or vice versa, in `wsconfig.json`, and
 
 2) switching between a module-bound sitelet value vs. a type implementing `IWebsite<_>` wrapping the same sitelet, and enumerating the endpoints for static content generation.

Upon doing this myself in [BlogEngine](https://github.com/granicz/BlogEngine), I found that I had to take care of a couple extra steps, and it was not always obvious how to hammer out some of the thorny details that popped up:

 1) In the client-server app, the `.fsproj` file should define itself as an ASP.NET Core web app project - so you need to change the first line in the file to:

	```xml
    <Project Sdk="Microsoft.NET.Sdk.Web">
	```
	Similarly, if you are switching back to a HTML app, change this line to:

	```xml
	<Project Sdk="Microsoft.NET.Sdk">
	```
    
 2) I thought I was "almost there", but after some further experimenting concluded that multiple .NET Core projects just don't like to live in the same folder. This manifested in unusual compiler errors about missing versions of `System.Runtime` and other weirdness, which was likely due to the build failing to work with two or more projects targeting mixed frameworks, and having a single `obj\project.assets.json` file.

 3) So I ended up moving the new web-hosted project file to its own folder, which later became the `Hosted` project. I also moved `Main.fs` and `index.html` from `Website`, so its content simply became:

    ```xml
    <ItemGroup>
      <Compile Include="../hosted/Main.fs" />
      <Content Include="../hosted/index.html" />
      <None Include="extra.files" />
      <None Include="wsconfig.json" />
    </ItemGroup>
    ```

 4) With two projects now needing access to the same static resources (the generated JS code from `Client`, the CSS files, images, etc.), I moved these to `Hosted` as well, and modified `Website`'s build script to copy them to the static output folder (`../../build`) for SSG use. This took me a while to get right, first I tried to make these visible under `Website` in the VS's project view using `<Link>` and friends, so that despite being in another project they could be opened and edited directly. But VS doesn't seem to be able to correctly render a nested directory structure included this way, and instead shows a flat view of the files inside them, littering up the project tree. So I ended up with simple copying instead:

    ```xml
      <ItemGroup>
        <ExtraFiles Include="../hosted/img/**" linkBase="img" />
        <ExtraFiles Include="../hosted/css/**"  linkBase="css" />
        <ExtraFiles Include="../hosted/js/**" linkBase="js" />
        <ExtraFiles Include="../hosted/node_modules/**" linkBase="node_modules" />
        <ExtraFiles Include="../hosted/scss/**" linkBase="scss" />
      </ItemGroup>
      
      <Target Name="CopyFiles" AfterTargets="Build">
        <Copy SourceFiles="@(ExtraFiles)" DestinationFolder="../../build/%(linkBase)/%(RecursiveDir)" />
      </Target>
    ```
    
   In case you are wondering why not put these into `extra.files` and have WebSharper copy them for you, I wondered the same, but it turns out that WebSharper doesn't allow `extra.files` source entries to be named from outside the project root folder. I am still contemplating about filing an issue, and implementing a [WebSharper/core](https://github.com/dotnet-websharper/core) change to get this relaxed. You can read about this and other details on the [WebSharper Gitter channel](https://gitter.im/intellifactory/websharper) and chip in with your opinion.
   
 5) When I ran `Hosted` after making these changes, I got nothing. Then I quickly realized that's because I haven't set up the hosting pipeline yet which that serve me what I wanted. So I added the standard WebSharper `Startup.fs` text to address that, so `Hosted`'s contents ended up with a second F# file:

    ```xml
      <ItemGroup>
        <Content Include="posts/**/*.*" />
        <Content Include="scss/**/*.*" />
        <Content Include="Properties/launchSettings.json" />
        <Compile Include="Main.fs" />
        <Compile Include="Startup.fs" />
        <Content Include="index.html" CopyToPublishDirectory="Always" />
        <None Include="wsconfig.json" />
      </ItemGroup>
    ```
  
6) As a bit of icing on the cake, I also set up different `launchSettings.json` files in both projects, so you can run the "HTML" profile from `Website` to SSG your blog, or the "Hosted" profile from `Hosted` to host your blog and benefit from the extra productivity when making code changes.

At the end, I maximized code sharing, added different build profiles to launch easily in the two blog modes, put everything in `Hosted` and wired `Website` to work from there, set up `Main.fs` to make that happen in both launch scenarios, and added the ASP.NET Core startup preamble in `Startup.fs`, making the whole blogging solution now consist of two F# files, and ~300 LOC.

If you want to follow along extending BlogEngine with further features, such as [adding RSS support](https://github.com/granicz/BlogEngine/issues/2) or [article categories/labels](https://github.com/granicz/BlogEngine/issues/3), don't hesitate to look in the [BlogEngine repo](https://github.com/granicz/BlogEngine) and help me with other features you think are important for your blogging needs.

If you want to look at the changes I made to extend BlogEngine with a hosted project,  you can check the relevant [PR](https://github.com/granicz/BlogEngine/commit/2650b6bb6a635e9c5f0019aa2d794a252707e9a1) that contains the jist of it.

Happy blogging!
