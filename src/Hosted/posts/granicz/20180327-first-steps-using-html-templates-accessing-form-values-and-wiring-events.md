---
title: "First steps: Using HTML templates, accessing form values, and wiring events"
categories: "templating,ui,f#,websharper"
abstract: "Congratulations on taking the first step to learn WebSharper! We have carefully put together this hands-on tutorial with the aim to help you get started with WebSharper and on your way to learn functional, reactive web development, putting you on a fast track to unleash the real web developer in you. The skills you pick up with WebSharper will make you a better web developer, and the concepts you learn will remain valid and useful with other functional, reactive web frameworks and libraries as well."
identity: "5516,84858"
---
Congratulations on taking the first step to learn WebSharper! We have carefully put together this hands-on tutorial with the aim to help you get started with WebSharper and on your way to learn functional, reactive web development, putting you on a fast track to unleash the real web developer in you. The skills you pick up with WebSharper will make you a better web developer, and the concepts you learn will remain valid and useful with other functional, reactive web frameworks and libraries as well. You can find links to further material below and the sources of this tutorial in the bottom of this page, and you can [see the resulting app in action live on Try WebSharper](http://try.websharper.com/snippet/adam.granicz/0000Jn).

### What you will learn and where you can find out more

1. **Using HTML templates** (these work on the client and server alike) instead of inline HTML combinators. *Further reading*: the "HTML Templates" section of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page of the main documentation.
2. **Reading and writing the values of input controls** (text boxes, text areas, checkboxes, etc.) in your HTML page through your template's data model. *Further reading*: the "Accessing the template's model" subsection of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page of the main documentation.
3. **Wiring events** such as button clicks. *Further reading*: the bottom of the "Holes" subsection of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page of the main documentation.
4. **Using reactive variables** - and reflecting their state to the UI. In this tutorial you will see how to apply CSS classes dependent on reactive variables and how to use the **V notation**. *Further reading*: the "Reactive layer" and "The V shorthand" sections of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page in the main documentation. We will cover **two-way binding more complex data models** in another tutorial.

### Our application: Login page for an SPA

In this tutorial, you will learn how to work with external HTML files (aka. templates) and how to implement your application logic into them. For this, you will take a [login page](https://dansup.github.io/bulma-templates/templates/login.html) from [dansup](https://github.com/dansup)'s wonderful free [Bulma template collection](https://dansup.github.io/bulma-templates/). Your app will look pretty much the same and also implement basic form validation:

![](https://i.imgur.com/E8Uv7oNl.png)

> The main takeaway of this tutorial is that you should use HTML templates as much as possible instead of inlining HTML code into your application logic. While it's certainly easy to construct HTML in C# or F# (by using the HTML combinators defined in `WebSharper.UI.Html`), it's our recommendation that you avoid it as much as you can for **better logic vs. presentation separation**.

### Prerequisites

To get the most out of this tutorial, make sure you have installed:

 * .NET Core 2.0+ and ASP.NET Core
 * the [latest WebSharper templates](http://websharper.com/downloads)
 * Visual Studio Code with [Ionide](http://ionide.io/) and/or Visual Studio 2017
 
### 1. Create your first SPA with WebSharper

Grab a command prompt, `cd` into the folder you want to use for your new project, and type:
```text
dotnet new websharper-spa -lang f# -n MyProject
```
This will create a new WebSharper SPA project for you. You will use F# for this tutorial, but you can also choose to create a C# project (just leave off the `-lang f#` part) and adapt the sources we discuss here. Go ahead and open this project with your favorite editor.

### 2. Project structure

![](https://i.imgur.com/nhnYqYBm.png)

* `wwwroot/index.html` - Your main SPA - this is the file you open to run your app
* `Client.fs` - The logic for your SPA - this is where your F# code will be
* `MyProject.fsproj` - The .NET Core project file for your SPA
* `Program.fs` / `Startup.fs` - The minimal boilerplate to run/host your app with the default ASP.NET Core web server
* `wsconfig.json` - Your WebSharper configuration file - normally, it's all set up for you

### 3. Bring your HTML

The SPA project you just created consists of a sample template that you can simply replace with the new markup from the login template. To see what's going on underneath, follow these steps:

1. Replace `wwwroot/index.html` with the source of the [login page](https://dansup.github.io/bulma-templates/templates/login.html)
2. Download `login.css` from the login template as `wwwroot/css/login.css`, and update the reference to it in `wwwroot/index.html` accordingly:

    ```html
      ..
      <link rel="stylesheet" type="text/css" href="css/login.css">
    </head>
    ```
3. Feel free to remove the reference to `bulma.js` - it is not strictly needed.
4. Re-add the following block to the bottom of the `<head>` section:
    ```html
    <style>
      [ws-template], [ws-children-template] { display: none; }
     .hidden { display: none; }
   </style>

   <script type="text/javascript" src="Content/MyProject.head.js"></script>
    ```
    This will give you a `.hidden` CSS class to hide things (always comes handy), and also make sure that **any dependencies are correctly brought into the page** by WebSharper when needed.

5. Re-add the following block to the bottom of the `<body>` section:
    ```html
    <script type="text/javascript" src="Content/MyProject.min.js"></script>
    ```
    This will **load the JavaScript code WebSharper generates** for your SPA.

6. Feel free to update the `<title>...</title>` with the title you prefer to give to your app.

7. Change the links below the login form as you see fit (we won't deal with those in this tutorial.)

### 4. Main task: implementing login with validation

Now that you have the skeleton of your login page, you can quickly wire in the necessary logic. First, you need to be able to access the email and password values the user types in. The WebSharper way of doing that is going into the template and marking the input controls that supply values to the F# layer with a `ws-var` attribute. You also want to implement some basic validation and use Bulma's `is-danger` class to visually indicate when an input control is not giving what you are expecting. So add in `ws-attr` attributes as well, and change the relevant lines to:

```html
...
    <input ws-var="Email" ws-attr="AttrEmail" class="input is-large" type="email" placeholder="Your Email" autofocus="">
...
    <input ws-var="Password" ws-attr="AttrPassword" class="input is-large" type="password" placeholder="Your Password">
...
    <input ws-var="RememberMe" type="checkbox">
...
```

Also, add a `ws-onclick` attribute to the Login button so you can wire a click event handler to it:

```html
...
   <button ws-onclick="Login" class="button is-block is-info is-large is-fullwidth">Login</button>
...
```
Now, you are ready to write your F# logic and switch over to `Client.fs`. If you didn't have to worry about validation, things would be super simple, but in this case you want the full enchilada, so you will also use a couple reactive variables as a mini data model (`passwordValid` and `emailValid`) to tell whether the email and password fields are valid.

![](https://i.imgur.com/KblwDbd.png)

At this point, you can see that the WebSharper UI templating type provider conveniently feeds back the reactive variables and attributes you defined in your master template, and you can simply set these up as follows:

1. Show a visual validation error for the Email input box if `emailValid` is false by applying Bulma's `is-danger` class:
    ```fsharp
    MySPA()
        .AttrEmail(Attr.ClassPred "is-danger" (not emailValid.V))
    ```
	> Here, `emailValid.V` is a shorthand WebSharper uses (it's a type-directed macro) that enables to treat a Var or a View as the underlying value in reactive scenarios/functions (such as `Attr.ClassPred`.) 

2. Similary, show a validation error for the password field is `passwordValid` is false:
    ```fsharp
        .AttrPassword(Attr.ClassPred "is-danger" (not passwordValid.V))
    ```

3. Now, handle the Login button click - this is what decides what constitutes a valid input (no empty fields) and simply putting up a popup alert if login is successful (this is where you would do a server call to authenticate your users and to log them in by creating a user session):
    ```fsharp
        .Login(fun e ->
            passwordValid := not (String.IsNullOrWhiteSpace e.Vars.Password.Value)
            emailValid := not (String.IsNullOrWhiteSpace e.Vars.Email.Value)

            if passwordValid.Value && emailValid.Value then
                JS.Alert (sprintf "Your email is %s" e.Vars.Email.Value)
            e.Event.PreventDefault()
        )
    ```
    > Note how `e` in the login handler enables you to **access all the input values** through `e.Vars`. You can also use these to **set their values on the UI** - two-way binding in WebSharper UI really rocks.

4. And last, seal these and bind your logic to the SPA:
    ```fsharp
        .Bind()
    ```
And boom, you are done.

### 5. Further improvements

Your login app is 30 lines of F# code, and even for a short tutorial like this one, you can pull off one more trick. Say, you wanted to add **error messages** below the input boxes that are failing validation. Bulma's way is to add these to the markup after each input control. In addition, all you need as extra is another `ws-attr` attribute:

```html
<div class="field">
  <div class="control">
    <input ...>
  </div>
  <p ws-attr="AttrEmailMessage" class="is-danger help">Please enter an email address</p>
</div>
```

The last bit is handling the showing/hiding of this error message in `Client.fs` by applying the `hidden` class you added to the template earlier:

```fsharp
    .AttrEmailMessage(Attr.ClassPred "hidden" emailValid.V)
```

As a bonus exercise, you can add a similar error message for the password field as well, and just a  hint: it will look exactly like what you did above.

### Source code and try the app

You can fork [this SPA project](https://github.com/websharper-samples/LoginWithBulma) via GitHub, it's a whopping 35 LOC F# in its entirety - enjoy! You can also [try out a slightly adapted version](http://try.websharper.com/snippet/adam.granicz/0000Jn) live on Try WebSharper.

Happy coding!
