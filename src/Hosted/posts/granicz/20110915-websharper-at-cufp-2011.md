---
title: "WebSharper at CUFP 2011"
categories: "workshop,cufp,f#,websharper"
abstract: "Back in 2004 when we founded IntelliFactory to do commercial F# development, there were just a few dozen people who have heard about F# worldwide. When pitching to various companies or talking informally about what we do I usually met with total utter disbelief - how could a company that specializes in something unknown ever succeed? [...]"
identity: "1917,74749"
---
Back in 2004 when we founded IntelliFactory to do commercial F# development, there were just a few dozen people who have heard about F# worldwide. When pitching to various companies or talking informally about what we do I usually met with total utter disbelief - how could a company that specializes in something unknown ever succeed?

Fast forward ca. 6 years: after over 130 internal and commercial projects/applications completed in F#, having presented at numerous conferences and workshops about our work, and tackling some of the most challenging and interesting problems on a daily basis, I have to confess that there have been nothing more fulfilling than doing what we do: thinking of something big, implementing it in F#, and seeing it benefit people.

Most of the time you hear about F# users coming from big banks and investment companies, but if you think that's all where they are you are definitely wrong. Personally, I strongly oppose the "F# as a complementary language to C#" idea, and I don't feel that F# is "best only in certain domains." We had been working hard to prove this in the web sphere: F#, coupled with a framework like [WebSharper](http://websharper.com/), is a fantastic tool set to develop client-oriented, rich internet applications, not only compatible with proven standards like ASP.NET but also yielding a huge developer productivity boost that no other language/web framework combo can match.

### Commercial Users of Functional Programming (CUFP)

[CUFP](http://cufp.org/) is the primary place to learn about commercial users and applications of functional programming in industry. We presented about F# there back in 2009, when we demoed a marketing automation stack we built for one of our partners in Silicon Valley. This project consisted of several large pieces: a data mart collecting marketing campaign data and responses; an analysis engine to run sophisticated queries; a web front-end to construct these queries, to create marketing communications, and to visualize their effectiveness over a larger campaign scope; an execution platform to deliver these campaigns and their waves; and a printing automation that managed and optimized the printing and delivery processes.

This year we are back to CUFP! This time we are presenting our work on WebSharper applied to build mobile applications with F#. From type-safe, composable mobile UIs to integrating third-party services, WebSharper Mobile is unmatched in giving mobile developers the kind of abstractions that make mobile development easier, much quicker, and much more fun!

The abstract of the talk is below:

>Native mobile applications enjoyed tremendous success in recent years,
>and looking at various mobile application stores such as Apple's App Store
>or Google's Android Market reveals a staggering number of native mobile
>applications. As technologies to build these applications mature and the
>market saturates, mobile OS vendors are struggling to find ways to keep
>up with and secure the exponential market growth. Inhibiting factors include
>platform-specific development environments, programming languages, and
>building blocks for applications; developer-unfriendly licensing, policies
>and subscriptions; and controlled channels of application distribution.
>
>These problems have given rise to many promising alternatives and
>technologies that aim to bridge across various mobile platforms and enable
>sharing some or all the code in between versions of applications for different
>mobile OSs. Two main directions unfolded: targeting mobile code generation
>from mainstream languages such as C# and Java, and embracing web
>applications with platform-independent UI abstractions and enhanced access
>to the capabilities of the underlying device. While the technologies that
>enabled the former are an interesting topic, we believe that the latter has
>implications not only for mobile applications but also for their desktop
>counterparts, and finding ways to utilize functional programming in the
>development of these web-based applications has an immense impact on
>mobiles and desktops alike.
>
>In this talk I will highlight some of the work we are doing at IntelliFactory to
>enable building platform-independent mobile applications in F#. This work
>leverages our commercially available WebSharper framework, the premiere
>functional web development framework for F# with thousands of active users
>and partner companies, and utilizes some key functional programming
>abstractions that enable modeling first-class, type-safe, composable web
>applications and user interfaces. I will briefly outline the metaprogramming
>infrastructure that enables us to enlist arbitrary JavaScript libraries into the
>type-safe discipline of F#, and the underlying CoreJS language that is more
>amenable to reasoning about and applying code transformations and
>optimizations.
>
>At the end of the talk, I will briefly touch upon our upcoming F# in the Cloud
>support and how that helps to seamlessly scale into the cloud desktop and
>mobile web applications with immense server computation needs.</quote>

It's still not too late to sign up to CUFP and attend the talk in Japan! We are definitely interested in hearing your feedback, please visit the [FPish forums](http://fpish.net/topics) (more on FPish in the coming days...) to ask or comment. Also, the drop to try out WebSharper Mobile will be available and announced here shortly. Happy mobile coding!
