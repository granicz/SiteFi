---
title: "WebSharper forum enhancements"
categories: "website,fpish,websharper"
abstract: "In recent weeks we had been busy with a series of enhancements to the WebSharper website and some underlying FPish services. [more...]"
identity: "4272,79115"
---

In recent weeks we had been busy with a series of enhancements to the WebSharper website and some underlying [FPish](http://fpish.net) services.

These include, among others:

 1. **Editing and marking-as-answer for forum entries**. We took this opportunity to catch up on markdown support and use that as the main input format instead of the custom FPish markup (which has been obsoleted since then), and pushed that change through displaying on FPish properly as well. Editing directly on the WebSharper website makes it more convenient to respond to forum entries or tidying up your past comments if necessary.

 ![Editing and marking-as-answer on forum entries](http://i.imgur.com/JsydLas.png)

 These features are enabled for your own content only. We at times edit/enhance forum topics and their comments by hand, as well. We ask that you submit content 1) without typos, 2) format your code blocks appropriately (setting the language on blocks and quoting identifiers inlined in paragraphs), and 3) use proper markdown for bullet or numbered lists and other elements. If unsure, consult the markdown reference by clicking the Help button in the editor.

 2. **Feeding Github tickets into the forums**. This makes it easier to track tickets and shows what's happening in the [main repository](http://github.com/IntelliFactory/websharper).  The underlying machinery feeds GitHub updates into FPish topics (hidden from the other content), allowing us to do interesting analytics on this growing content in FPish. You can't comment on these forum entries directly, so just follow the GitHub icon to the original ticket.
 
 ![GitHub issues are now fed into the WebSharper forums](http://i.imgur.com/ktPvACc.png)
 
 We are planning to integrate GitHub tickets from extension repositories as well in the near future, starting with [UI.Next](https://github.com/IntelliFactory/websharper.ui.next) and the other open source extensions.

 3. **Fixing authentication with FPish**, so it doesn't expire after a minute. You may have noticed an annoying flickering of the Login menu and the posting controls earlier when commenting or adding new forum topics on the WebSharper website. This turned out was due to a bug in how FPish handled Oauth sessions and is now fixed. To circumvent the issue earlier, we had to code the interaction controls with a timer to renew authentication tokens with FPish every minute, causing a short visual flickering. This is now fixed.
 
 ![](http://i.imgur.com/1lZk3xC.png)  ![](http://i.imgur.com/A27mX70.png)
 
More work is underway. In particular, we are streamlining how commenting works and doing a facelift to the forums in the next round of releases. This is in preparation of rolling out a support surface for WebSharper support customers where they can file tickets and track progress of various items more easily, etc.  This has traditionally lived on FPish, but is now being reimplemented and moved to the WebSharper website to provide a more consistent and streamlined experience.

Overall, we hope you will find the new [forums](http://websharper.com/questions) better, and will take the time to use it to find answers to your WebSharper and related questions. Happy coding!
