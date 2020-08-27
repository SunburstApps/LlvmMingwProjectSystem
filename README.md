# Sunburst.LlvmMingw

This repository contains the source code to an MSBuild-based build
system that can build native code for Windows (a la vcxproj), even
if the build is running on non-Windows platforms. It uses the
[LLVM-MinGW](https://github.com/mstorsjo/llvm-mingw) project
to bootstrap the compilers.

You can download prebuilt versions of the Visual Studio for Mac
and Visual Studio for Windows (once they are implemented) from Releases.
The SDK will be published to this repo's Packages feed, so it
can be easily consumed by MSBuild.
