---
title: "Try Websharper: version info about extensions and some embedding improvements"
categories: "trywebsharper,f#,websharper"
abstract: "A new dialog has been added showing a version number for the site and all the version info about the extensions for better transparency. A new feature has also been introduced, which lets the user decide which page they want to show by default when embedding a snippet."
identity: "4559,80399"
---
Try WebSharper has been updated with some minor improvements regarding version info and embedding snippets.

1. There is now an *About* button which shows a dialog with all the relevant version info of the extensions and a version number for Try WebSharper itself. We are going to do our best to keep all the extensions up-to-date.

		[![](http://i.imgur.com/Wof3ArOl.png)](http://i.imgur.com/Wof3ArO.png)
        
2. It is now possible to point at a given tab of an embedded snippet instead of showing the *Result* one by default. You can do it by specifying the tab in the URL by a hash:
		[![](http://i.imgur.com/YpOuTSml.png)](http://i.imgur.com/YpOuTSm.png)
        
  * `#f-sharp`: F# editor
  * `#html`: HTML editor
  * `#result`: Result tab
    
Happy coding!    
