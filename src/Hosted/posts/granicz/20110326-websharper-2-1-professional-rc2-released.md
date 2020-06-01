---
title: "WebSharper 2.1 Professional RC2 released"
categories: "f#,websharper"
abstract: "We are happy to announce that WebSharper 2.1 Professional RC2 is now available for Visual Studio 2008/2010 developers.  You can find and download the latest version from [...]"
identity: "992,74573"
---
We are happy to announce that WebSharper 2.1 Professional RC2 is now available for Visual Studio 2008/2010 developers. You can find and download the latest version from below:

[Download the latest WebSharper 2.1 RC2 (2.1.60 or higher)](http://www.websharper.com/latest/ws2)

This version contains numerous enhancements and a few bug fixes, keep reading for details. We expect to ship the final WebSharper 2 Professional release shortly. So grab your discounted developer licenses while you can at the [Purchase](http://www.websharper.com/Purchase.aspx) page, available until the final version ships!

### Installation

We advise that you remove all existing installations of WebSharper 2 before installing this release. If you experience any issues during installation you should see a dialog box telling you the location of the installation log file, which in turn contains more information about the source of the problem such as access denied, etc. We kindly ask that you report all such issues on the bug report form on the [Community](http://www.websharper.com/Community.aspx) page.

### Changes from RC1

WebSharper 2 Professional RC2 adds a handful of notable enhancements to the previous releases:

 * **Built-in jQuery 1.5.1 support** - to cater to IE9 and the latest version of jQuery.
 * **Visual Studio templates for WebSharper extensions to JavaScript libraries** - built on the WebSharper Interface Generator (WIG) tool set, these templates enable you to bring third-party JavaScript libraries into the typed discipline of F#, giving you seamless access to virtually unlimited client-side functionality for your WebSharper applications.

<img src="/assets/WIG-VS-Template.png">

 * **Improved logging and error reporting** - both in the compiler and the installer, better error messages should help you pinpoint the root cause of any errors.
 * **Enhanced, more attractive installer** - also compressed to save on download times.

### Change log

A more comprehensive change log is below. You can find out more about each ticket by looking in [WebSharper Bugzilla](https://bugs.intellifactory.com/websharper).

 * Add a VS9 and VS10 template for WIG-based projects (#345)
 * Update bindings to 1.5.1 (#357)
 * Update bindings to 7.0 (#358)
 * Incorrect translation of JQuery.Of(string, JQuery) (#344)
 * Script tag is rendered incorrectly (#348)
 * CharacterCode in KeyPress event is mapped incorrectly (#349)
 * Found conflicts between different versions... (#352)
 * Javascript: Object doesn't support 'get_Body' property or method (#354)
 * JSON error when floats passed as Rpc arguments (#356)
 * MessageBox fails unless run in UserInteractive mode (#359)
 * Provide graceful exception reporting and information collection (#365)
 * CanvasRenderingContext2D.FillText doesn't work (#362)
 * Offline Sitelets and CSS referencing embedded resources (#363)
 * Offline sitelets and clashing resource names (#364)
 * User-defined name is not used when defining assembly name/namespace (#346)
 * The Add-in failed to load (#351)
 * Formatting of floats is not culture invariant. (#73)
 * Title: Visual studio 2008 incompatible? (#74)

Enjoy!
