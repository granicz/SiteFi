---
title: "WebSharper 2.5.93 - improved numeric performance"
categories: "f#,websharper"
abstract: "We are happy to announce the availability of the most recent WebSharper update, version 2.5.93, released on Feb 4, 2014. This release brings a new and improved optimizer for the Core language that is used as an intermediate form during compilation to JavaScript. The optimizer is able to eliminate more JavaScript expressions, generates more readable code, and improves JavaScript performance of numeric code (such as code using for loops heavily) by 30-50%."
identity: "3710,77023"
---
We are happy to announce the availability of the most recent WebSharper update, version 2.5.93, released on Feb 4, 2014. This release brings a new and improved optimizer for the Core language that is used as an intermediate form during compilation to JavaScript. The optimizer is able to eliminate more JavaScript expressions, generates more readable code, and improves JavaScript performance of numeric code (such as code using for loops heavily) by 30-50%.
The release also fixes a number of bugs related to code generation and extension templates.

Bugs fixed in this release: [209](http://bitbucket.org/IntelliFactory/websharper/issue/209), [210](http://bitbucket.org/IntelliFactory/websharper/issue/210), [212](http://bitbucket.org/IntelliFactory/websharper/issue/212), [213](http://bitbucket.org/IntelliFactory/websharper/issue/213)

Happy coding!
