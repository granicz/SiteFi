---
title: "Upcoming CloudSharper features in Q3"
categories: "cloudsharper,f#"
abstract: "As we all gear up for an exciting summer, I wanted to share some insights into the upcoming CloudSharper enhancements we are rolling out in the next few months."
identity: "3924,77265"
---
As we all gear up for an exciting summer, I wanted to share some insights into the upcoming CloudSharper enhancements we are rolling out in the next few months.

Our biggest challenge continues to be offering and maintaining full compatibility and seamless roundtrip of projects amongst CloudSharper, Visual Studio, and Xamarin Studio. The current release does poorly wrt. to "just working out of the box" on Linux/Mac and relies on manual intervention (such as manually building projects instead of the entire solution, etc.)  We are definitely aware of these issues, and fixes are coming as we have them. If you are a Linux/Mac user wanting to experiment with running CloudSharper locally, I would suggest to keep an eye on this blog for status announcements.

Regarding upcoming new features, the following are amongst the main items in the release pipeline, in the planned order of release:

 * **Group sessions** - enabling conferences, workshops, trainings, academic courses, meetups, coding dojos, etc. to host live F# coding sessions, and use CloudSharper via a single "server" without requiring local installations on participant machines.

The workflow we are aiming for is as simple as 1) events administrators setting up a site server for their event, and 2) providing a single login URL to the participants who will then be able to participate, follow along, and work with/in the workspaces shared on the event, with nothing more than a modern browser needed on their machine.  We also will be able to enforce user and usage quotas to help organizers fend off malicious or uninvited users, and offer services/availability during the scheduled time frame only.

You will soon be able to test "installation-less" F# code editing in our upcoming webinars, just check the CloudSharper blog for upcoming events.

Also, we are investing heavily in applying CloudSharper for various e-learning scenarios, more on this and the collaboration we are doing in this space in future blog posts.


 * **Extension API** - you will soon be able to write your ow file viewers, editors, extensions, and even your full CloudSharper-embedded applications. We are also aiming to align the built-in F# editor support with the community-driven IDE and refactoring efforts underway, and will be looking for fellow contributors to help develop these new and enhanced CloudSharper coding tools for F# (and soon for other languages).

Custom CloudSharper code opens up an exciting new space for integrated solutions, and helps to engage the vibrant F# community in a multitude of new opportunities. We are thrilled to offer the initial version of the extensibility API with full documentation and samples.


 * **Cloud sharing of workspaces** - in addition to the [cloud sharing](http://cloudsharper.com/blog/3920/2014619-cloudsharper-0-9-13) of workspace snapshots we introduced recently, we are adding a gallery of tagged/cataloged community workspaces to help newcomers get the best of CloudSharper and F#.

We are also recreating all of the WebSharper samples on the [main WebSharper site](http://websharper.com/samples), and the other sister sites for various client-side libraries ([D3](http://intellifactory.github.io/websharper.d3/), [Three.js](http://intellifactory.github.io/websharper.threejs/), [Babylon.js](http://intellifactory.github.io/websharper.babylonjs/), [Web RTC](http://intellifactory.github.io/websharper.webrtc/), etc.) as CloudSharper workspaces for anyone to clone. This means that you can create, run, and make changes to the dozens of WebSharper samples with a single click in your browser, dropping you in a cloned workspace ready to work in.


 * **A fully cloud-hosted backend** - next to the free local-hosted, "mini-cloud" version available today, we will be rolling out a fully cloud-based offering for professional developers and teams of developers who want a secure cloud-hosted developer environment that is ubiquitously available from any device. We are planning some truly awesome and disruptive innovations for this offering, and are already working with a number of organizations to bring you the next generation of web-based development tools for a new level of programmer productivity.

Much-much more in future blog posts, and for now, expect a couple quick releases to clean up the Linux/Mac story and a few remaining issues before we gear up for the main release schedule for the summer.

Happy coding!