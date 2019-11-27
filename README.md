# LibSvnSharp

C# wrapper around Subversion based on SharpSvn

## Purpose

We have been using the wonderful [SharpSvn](https://sharpsvn.open.collab.net/) library to do communication between our software and Subversion (SVN) servers for quite a while.

Unfortunately, SharpSvn comes short in fulfilling the following requirements:

* SharpSvn has not been updated for quite a while. More specifically, Subversion 1.9 is the latest version SharpSvn supports, and it's not supported anymore.

* SharpSvn also statically links SVN library, which means we don't even get most patches.

* SharpSvn is not compatible with .NET Core, meaning it would be hard for us to go forward with new versions of .NET.

* SharpSvn is written in C++/CLI, which means it is only compatible with Windows, while we provide some tools that work cross-platform (e.g. mxbuild).

To cover those shortcomings, we decided to rebuild SharpSvn using a different approach.

We decided to give a new name to this library because we don't want to shadow the original SharpSvn library, as we don't plan to reimplement it in full.

## Implementation

We implemented LibSvnSharp as a port of SharpSvn to managed code. Because of that, you will see a lot of mentions of SharpSvn in the code.

We use [CppSharp](https://github.com/mono/CppSharp) project to generate bindings for native Subversion library APIs, and then we take SharpSvn code and rewrite it in C#. Simple! :)

This allows us to build Subversion separately from LibSvnSharp, so to update it to a newer version would mean to simply build a newer version of the native library. Because of this approach, we need to redistribute both the managed DLL and the native one.

## TODO

This project is still a work in progress. We are far from done yet, and we don't plan to port all the APIs of the original SharpSvn library.

We would like to have the following:

* Finish what has been started (exceptions, long file paths etc.)

* Build scripts (for now Windows-only)

* Adopt some of the tests from original SharpSvn library to test LibSvnSharp

## Contributing

This document is a work in progress and will be posted separately in this repository.
