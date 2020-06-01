---
title: "CloudSharper Local alpha 0.9.8"
categories: "cloudsharper,websharper"
abstract: "We are happy to announce CloudSharper Local alpha 0.9.8, encapsulating the latest enhancements to the F# compiler service and delivering several bug fixes to CloudSharper's msbuild support."
identity: "3764,77088"
---
We are happy to announce a new round of CloudSharper Local alpha releases, encapsulating the latest enhancements to the F# compiler service and delivering several bug fixes to CloudSharper's msbuild support.

Subsequent releases will focus on establishing Mono compatibility, project templates that support F# 3.1 out of the box (at the moment, you need F# 3.0 installed on your system to compile CloudSharper project templates), support for remote CloudSharper servers, and synchronizing local workspace files into cloud storage.  These stream of releases will eventually lead to the official 1.0 release in Q2.

The list of bugs fixed in this release:

 * [#286](https://bitbucket.org/IntelliFactory/cloudsharper/issue/286) Right click + Build uses IF.Build
 * [#289](https://bitbucket.org/IntelliFactory/cloudsharper/issue/289) Moving a source file to a folder and back causes it not to type check
 * [#295](https://bitbucket.org/IntelliFactory/cloudsharper/issue/295) Deleting and re-creating a standalone F# file crashes the completion service
 * [#291](https://bitbucket.org/IntelliFactory/cloudsharper/issue/291) Clicking on any errors in the Notifications doesn't switch over to Messages
 * [#290](https://bitbucket.org/IntelliFactory/cloudsharper/issue/290) Duplicate errors on moving files in the solution tree

Happy coding!
