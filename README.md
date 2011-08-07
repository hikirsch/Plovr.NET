# Plovr.NET

Plovr.NET is a direct port of [Plovr](http://www.plovr.com) aimed at trying to make Closure development a little easier. The goal is to support all the options and features available within the real Plovr project in this in one but native to a .NET project.

Currently, only the Closure Compiler and Closure Library are supported. Support for the SOY template system will be done for the next release. Any bugs found should be reported on the [Plovr.NET GitHub issues tracker](https://github.com/Ogilvy/Plovr.NET/issues).

## Getting Started 

Plovr.NET works in either an ASP.NET website or as an MVC project.There are a few configurations additions that you will need to add your web.config. A complete example [web.config](https://github.com/Ogilvy/Plovr.NET/blob/master/Plovr.Test.ASPNETApplication/Web.config) is available for now. Including the Plovr.dll and the closure compiler jar, you may 

### Ongoing tasks
1. Documentation!
2. Support for SOY templates.
3. Full support for all options within the real Plovr.