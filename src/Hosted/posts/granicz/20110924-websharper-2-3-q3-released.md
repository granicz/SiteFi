---
title: "WebSharper 2.3 Q3 released"
categories: "mobile,f#,websharper"
abstract: "This release ships the preliminary support for mobile devices, a new WebSharper component we call WebSharper Mobile. [more...]"
identity: "1916,74748"
---
[This release](http://websharper.com/latest/ws2) (2.3.62+) ships the preliminary support for mobile devices, a new WebSharper component we call WebSharper Mobile (WM). (Be sure to update your extensions to their matching versions from the [Extensions page](http://websharper.com/extensions).) WM consists of the following:

 * The tooling for packaging applications for various mobile platforms. This component assumes the presence of the appropriate SDKs present on your system (see below for a summary).
 * Visual Studio templates for online (with server-side) and offline (without) mobile applications. These provide a convenient and fast way to get started and provide a short sample application for reference.
 * A mobile layer that exposes device-specific functionality to JavaScript. This is the core component of WM, and we use the name interchangeably or as "WM layer" when we talk about this component.

The WM layer is designed in two parts: a general API meant to convey core mobile functionality such as supplying accelerometer and GPS data or accessing the camera capabilities, and various platform-specific APIs. In this first iteration, only the core API is shipped. Subsequent releases will include an increasing number of platform-specific capabilities for each major platform, such as Bluetooth communication, calling API, etc.

<img src="/assets/figure3.jpg">

A jQuery Mobile formlet application running on Android is depicted on the figure to the left. You can follow through building this application in [this InfoQ article](http://www.infoq.com/articles/WebSharper).

### Prerequisites for Android development

If you are planning to use WebSharper Mobile for Android development, you will need the following in place:

 * JDK: from version 1.5 to 1.7 (which match Java 5 to 7). Note: must be the 32-bit version, since the Android SDK does not recognize the 64-bit versions.
 * Android SDK. When asked what packages to install, only install the platforms you wish to develop/test for. Installing a platform means that you can specify its API level later and that you can create an emulator for it. We recommend Android 2.2, optionally with Android 3.2. Tested: Android 2.x and Android 3.x. Android 1.x does not work, because WM is using functionality available for the 2.x+ series only. Note: You must not put the SDK in a folder whose path include spaces.
 * Adding the equivalents of `C:\Program Files\Java\jdk1.6.0_27\bin`, `C:\android-sdk\tools` and `C:\android-sdk\platform-tools` to the system path. You also have to set either the environment variable `ANDROID_HOME` or `ANDROID_SDK` to the equivalent of `C:\android-sdk\tools`.

#### Running Android applications in the emulator

The sequence of steps closely follows those normally done, with a couple minor additions:

 * Start the Android SDK and AVD Manager. If you do not know how, run android.bat from command line prompt (its path must be in scope as mentioned earlier).
 * Create a new AVD (Android Virtual Device). We recommend either Android 2.2 or 3.0, 512MB of RAM. You should enable camera (it is “off” by default). Run it. Note: Android 2.3.x emulators are broken, just ignore them.
 * First, Android apps are not built by default. This is because you need to specify more information for them. Start by finding out which SDK version you are developing to (you can find it from the Android SDK and AVD Manager window).
 * Copy your `.keystore` file (read below) to the project’s folder (that is, the mobile project itself, where your `mobile.config` is).
 * Open `mobile.config`, uncomment one of the build elements (there are two sample elements that you can use). Change the name of the output and the SDK version. Note: for this release of WebSharper mobile, you can only build for SDK 7, and it will work on both the 2.x and 3.x series. Note: the SDK version is sometimes also referred to as Android API level.
 * Configure the information about the `.keystore` (read below).
 * Build!
 * Browse with the command line to `bin\html`, run `adb install –r .apk`. Note: this will fail if the version of the device is lower than the version you specified, or if there are several emulators running.
 * The application is now in the application list.
 * Note: Sometimes, even though the emulator is running and all seems to be fine - the command `adb install ...` will fail and say the device is offline. This can be solved by running `adb kill-server` and then installing again.

#### Setting geolocation on Android

You can use the following to "hardcode" a given GPS coordinate into your Android emulator as your current position.

 * Enable the telnet client (skip if you are using Windows XP):

    * Open Control Panel.
    * In the Programs category, there is Turn Windows features on or off. Enable Telnet Client.

 * Find out which port your emulator is using by looking at the window’s title. For example, your emulator might be on port 5556, so the title is 5556:.
 * Run (command line) `telnet localhost`.
 * After the application has started (geolocation works on listeners, so it must be after the application registered its listener) enter: `geo fix`. Notice the order of arguments. The command might fail, just retype it.

#### `.keystore` files

You can create one using a tool called keytool. You can just run it, if you correctly set up your system path in an earlier step.

 * Type `keytool -genkey -v -keystore .keystore -alias -keyalg RSA -validity 10000` to generate a new one.
 * Just follow the instruction (it is an interactive tool). Note: if you started by working with a debug key, and then will try to use a “release” key, you will not be able to install the application with the new key on devices which have the previous version installed. You must uninstall the old version first.

### Requirements for WP7 development

You will need the following:

 * Windows Vista or Windows 7 (but not Starter Edition). Windows Server 2008 (also R2) was reported to sometimes work if you apply this trick: http://blogs.msdn.com/b/astebner/archive/2010/05/02/10005980.aspx. We can confirm that this works for some 2008 R2's.
 * VS2010 has to be installed, although doesn’t have to be used (if you really like VS2008 – which can happen if you have many extensions which are not compatible with VS2010).
 * Windows Phone SDK. If you have VS2010 without SP1 then SDK for WP7, if you have it with SP1 then SDK for WP7.1.

### Online vs offline applications

When starting a new WebSharper Visual Studio project you might wonder what the offline vs online application templates are. As mentioned earlier, online applications have a server-side, offline ones don't. For instance, a simple static HTML application can be modeled as an offline sitelet project. For online mobile applications, the following apply:

 * Create a new Visual Studio project based on a WebSharper online mobile template. In general, online applications work like regular sitelets with the limitations of offline sitelets (e.g. finite number of pages).
 * Your RPC server (the WebApplication’s output) must be hosted on a publicly accessible location, and that location must be set in the `mobile.config` file. (The online projects have a mobile.config with a sample server location, this needs to be overridden).
