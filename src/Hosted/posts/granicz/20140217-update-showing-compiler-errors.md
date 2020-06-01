---
title: "Update: Showing compiler errors"
categories: "cloudsharper,f#"
abstract: "As of 0.9.5, you need to do a build (Ctrl+B) before type checking is initialized properly. Without such explicit build, you may face a whole host of errors while you edit your source files."
identity: "3733,77059"
---
As of 0.9.5, you need to do a build (Ctrl+B) before type checking is initialized properly. Without such explicit build, you may face a whole host of errors while you edit your source files. For instance, here is what you get when you open `Client.fs` in a newly created WebSharper Bundle project:

![CS1](http://i.imgur.com/l6pu1cP.png)

Once you build (Ctrl+B), however, things look the same, and you must close and reopen `Client.fs` to make type checking kick in properly:

![CS2](http://i.imgur.com/4kbhWVW.png)

### Where we are heading

Currently, we are working on making type checking work much more smoothly. This has several parts to it. One key part is enabling `msbuild`-based builds inside CloudSharper, in addition to using [IntelliFactory.Build](http://bitbucket.org/IntelliFactory/build) (the build system currently invoked from the available CloudSharper project templates). This means we can quickly determine the files and references within a project simply by consulting the appropriate `.fsproj` file, and thus we can avoid a relatively slow start-up of a dedicated `fsi.exe` process to interpret the matching IntelliFactory.Build build file (`build.fsx`). Having this information quickly and without an explicit initial build is promising to make all the difference, and should result in a much more ideal coding experience.

There are a few loose ends to tie together for this to work properly, but our initial work today is very promising. By the end of the day we managed to recover file and reference information without requiring an explicit build, and experimented with the template changes required to make the same project build with `msbuild` and IntelliFactory.Build.

All in all, we hope to roll out a much more responsive type checking facility shortly, and will keep you posted here on the progress we make.
