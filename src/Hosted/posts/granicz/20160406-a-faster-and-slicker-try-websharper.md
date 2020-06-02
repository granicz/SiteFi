---
title: "A faster and slicker Try WebSharper"
categories: "trywebsharper,f#,websharper"
abstract: "A while ago we rolled out a new UI for Try WebSharper, essentially changing it into a snappy single-page application (SPA). Among others, you can now switch between trying out various snippets and making your own without any noticable delay, no more annoying page refreshes. [more..]"
identity: "4764,81441"
---
A while ago we rolled out a new UI for [Try WebSharper](http://try.websharper.com), essentially changing it into a snappy single-page application (SPA). Among others, you can now switch between trying out various snippets and making your own without any noticable delay, no more annoying page refreshes.

[![Try WebSharper](http://i.imgur.com/zZcMIen.png)](http://i.imgur.com/60FnbVW.png)

### How to get started

You can check out (= run as is, or edit and re-compile) a whole host of snippets by going to home page and picking one at random, or you can search for any particular one that you like by using the filters on the right. Here, under Libraries you can tick any one of the extensions to search by, or even find snippets by authors.

### Logging in

You don't need to sign up or log in to run existing snippets or play with your own, or even to share one, but you will not be able to edit an anonymous snippet, nor will you be able to see all your snippets as a collection that you can manage.

So once you are ready to create snippets for others, you should register and sign in. You can do so from the top menubar:

![Logging in](http://i.imgur.com/lI20Bisl.png)

### Creating snippets

To create a new snippet, hover over the big orange (+) icon in the bottom right. Clicking it will create a brand new snippet with some default code. Alternatively, you can use the GitHub icon to create a snippet from a Gist.

![New snippet](http://i.imgur.com/hGSW6rOt.png)

### Forking snippets and versioning

You can fork a snippet when you want to edit and make a copy of someone else's. This menu option becomes Update when you are editing your own snippet, use that to save a new copy of it.

![Forking snippets](http://i.imgur.com/J2U2Rgel.png)

The left and right buttons allow you to flip back and forth in the history of changes to a snippet.

### Saving and updating snippets

Once you are ready to save a snippet, you can name it, give it a short description and an optional update note (on subsequent updates to tell your users what changed, for instance).

This is also where you can make your snippet public:

[![Saving snippets](http://i.imgur.com/AECYnIal.png)](http://i.imgur.com/AECYnIa.png)

If you want to show a thumbnail for your snippet, you need to first save it and update it again with the Change snapshot option:

[![Saving snippets with a thumbnail](http://i.imgur.com/JqfJJTJl.png)](http://i.imgur.com/JqfJJTJ.png)

Here, you can zoom and pan around to get the right parts into the white box, which will be your new thumbnail. You can also set a timer for when the snapshot should be taken (for instance, 1000 ms after the page load, in the example above), and this comes handy when you have a snippet whose output evolves over time.

### Managing your snippets

You can get a list of your own snippets by navigating to My Snippets from either the left charms menu (the hamburger icon) or via your avatar on the top-right.

[![Charm menu](http://i.imgur.com/aJ9TSA5l.png)](http://i.imgur.com/aJ9TSA5.png)

On My Snippets, you can do all sorts of things to your snippets by clicking the three dots on the top-right corner of each snippet thumbnail.

[![My Snippets](http://i.imgur.com/uOgIzedl.png)](http://i.imgur.com/uOgIzed.png)

For instance, you can Tweet it, get a URL to its embedded and direct versions, make it public or private (ON means public), or delete it if you need to.

![Operations on a snippet](http://i.imgur.com/jdsZqtNm.png)

### Have fun using it!

Try WebSharper makes it easy to create and share WebSharper snippets with others. You can use any one of the available WebSharper extensions in your snippets, including those that are premium otherwise, making Try WebSharper an excellent testbed if you are considering [subscribing to premium WebSharper tools](http://websharper.com/subscriptions), which include a variety of client-side data access, visualization and charting libraries, and advanced libraries of UI controls.

I find it a great tool to demo WebSharper at conferences and user groups, and get great feedback about it. One question I regularly get is whether it's possible to create snippets that use RPC functions, essentially, having a server-side as well.  The short answer is: not YET.  But... keep checking back here for some exciting news about this in the coming weeks.

Happy coding!
