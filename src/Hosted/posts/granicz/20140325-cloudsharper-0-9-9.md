---
title: "CloudSharper 0.9.9"
categories: "cloudsharper,f#,websharper"
abstract: "This release continues our efforts to require fewer dependencies on your machine before you can use CloudSharper. In earlier releases, various CloudSharper templates (most notably, the WebSharper Sitelet Website template) required .targets files that were part of Visual Studio. This dependency is now eliminated."
identity: "3767,77089"
---
This release continues our efforts to require fewer dependencies on your machine before you can use CloudSharper. In earlier releases, various CloudSharper templates (most notably, the WebSharper Sitelet Website template) required `.targets` files that were part of Visual Studio. This dependency is now eliminated.

We also made further template changes to require F# 3.1 installed on your machine, as opposed to earlier 3.0 installations, to match the updated code service that parses and type checks according to the F# 3.1 specification.

These bring the requirements for using CloudSharper now in line with what's stated on the Login page.

Other issues addressed in this release are:

 * [#297](https://bitbucket.org/IntelliFactory/cloudsharper/issue/297/embedded-application-template-doesnt) - Embedded application template doesn't compile
 * [#294](https://bitbucket.org/IntelliFactory/cloudsharper/issue/294/show-the-full-path-in-a-tabs-tooltip) - Show the full path in a tab's tooltip
 * [#296](https://bitbucket.org/IntelliFactory/cloudsharper/issue/296/worktree-close-all-folders-by-default-and) - Worktree: close all folders by default and remember which folders are open
 * [#282](https://bitbucket.org/IntelliFactory/cloudsharper/issue/282/doing-a-build-on-an-empty-workspace-should) - Doing a build on an empty workspace should say that there is nothing to build
 * [#298](https://bitbucket.org/IntelliFactory/cloudsharper/issue/298/when-creating-a-project-if-it-has-a-readme) - When creating a project, if it has a README, open it

Next up on our TODO list is enabling remote CloudSharper servers. This is an essential feature to enable installation-less scenarios, such as using CloudSharper as a platform at developer conferences and workshops, and those in academia such as programming in-class sessions and distance/e-learning.

More exciting developments will be announced here as they unfold, until then be sure to follow us on Twitter at [@CloudSharper](https://twitter.com/CloudSharper).

Happy coding!
