---
title: "Update: Server exited with code 0"
categories: "cloudsharper,f#"
abstract: ""
identity: "3732,77058"
---
Since the first 0.9.1 release, several users reported that they were unable to start CloudSharper Services properly, with the console showing the following trace:

```
Install ID: [your-GUID-here]
14/02/2014 10:27:29 [Info]
Server started at ws://localhost:8089
Web server started at http://localhost:8090/
Running the CloudSharper service. Press Enter to stop

Server exited with code 0.
```

When trying to work in the CloudSharper IDE, you get this:

![CS](http://i.imgur.com/XvKRv8J.png)

This issue is now filed as [#272](https://bitbucket.org/IntelliFactory/cloudsharper/issue/272/console-doesnt-start-service-properly), and a possible workaround is the following:

 * Locate `CloudSharper.exe` in the CloudSharper installation folder, and run it as 
    ```
    CloudSharper.exe -url http://cloudsharper.com
    ```
 * Keep the host shell open.

Let us know if this does or doesn't resolve the issue that you may be seeing, and also keep us posted if you find any other issue by adding it to the [public issue tracker](https://bitbucket.org/IntelliFactory/cloudsharper/issues?status=new&status=open).

Thanks and Happy coding!
