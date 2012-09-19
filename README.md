# Plovr.NET

Plovr.NET is a direct port of [Plovr](http://www.plovr.com) aimed at trying to make Closure development a little easier for the .NET platform. The goal is to support all the options and features available within the real Plovr project in this in one but native to a .NET project.

Currently, only the Closure Compiler and the Soy Template Systems are supported (read: no Closure CSS support). Any bugs found should be reported on the [Plovr.NET GitHub issues tracker](https://github.com/Ogilvy/Plovr.NET/issues).


# Setup
1. Download the [Plovr.dll](http://www.github.com) and reference it in your web project.
2. Download the [Google Closure Compiler](http://closure-compiler.googlecode.com/files/compiler-latest.zip) and copy the jar into your project.
3. Download the [Soy to JS Compiler](http://closure-templates.googlecode.com/files/closure-templates-for-javascript-latest.zip) and copy the jar into your project.
4. Add the [required configuration](https://gist.github.com/3751374) to your web.config and customize for your project. You will need to ensure the Jar paths point to the Jars downloaded from Steps 1 and 2. See the  [web.config](https://github.com/Ogilvy/Plovr.NET/blob/master/Plovr.Test.ASPNETApplication/Web.config) in the [sample project](https://github.com/Ogilvy/Plovr.NET/tree/master/Plovr.Test.ASPNETApplication).
5. Add the script tag to your project.
 
	```
	<script src="<%= Plovr.Utilities.GetIncludePath() %>"></script>
	```
	
	or (for MVC Razor projects)
	
	```
	<script src="@Plovr.Utilities.GetIncludePath()"></script>
	```
6. Create a [Plovr configuration file](http://www.plovr.com/docs.html) and add it into your web.config in the Plovr section.
7. The Closure Library's [base.js](http://code.google.com/p/closure-library/source/browse/trunk/closure/goog/base.js) will need to be included in your project code as well. The file should be in the first path lookup available in the Plovr config.

# Other Notes
The Plovr.NET url works exactly like the official Plovr URL works in serve mode. You can override some of the config options using a URL token (such as the compilation mode).

# Limitations

1. Plovr.NET does NOT package the Google Closure library like the official version does. If you would like to use the Closure Library, you should download it and add a [path](#) in your plovr configuartion file that points to the library.
2. A build counterpart existed in an earlier version of this project and was dropped due to only constraints from the original version of Plovr. It is highly recommended to use the official Plovr jar to build a project.
3. Closure's base.js is required to be in your project.


# FAQ

** Q: Do I need to reference the Java jars in my project? **

If you want to use the Soy template system or test building in either "ADVANCED" or "SIMPLE" mode, yes you will need to reference the jars properly.

** Q: How do I start the Plovr.NET server? ** 

There isn't one! Plovr.NET creates a new [httpModule](http://msdn.microsoft.com/en-us/library/zec9k340\(v=vs.85\).aspx) and listens for any URL that starts with "/Plovr.NET". 

** Q: I'm done with my project, how do I compile? **

It's recommended to use the official Plovr release. See that [documentation](http://www.plovr.com/docs.html) on how to run Plovr in build mode. 

** Q: My project does not use uses .NET 3.5, how can I use Plovr.NET for a different version? **

Download the full source and change the Target framework to your preferred version and just build the Plovr project. Note, you will also need the version of Newtonsoft that matches your preferred version.

** Q: I'm using a different version of Newtonsoft.JSON, is there a way I can reference mine instead of yours? **

Unfortunately, you can't. Newtonsoft.JSON has a new feature to support custom JSON serializations. Some properties in the Plovr configuration can be specified as a string or as  an array of strings (such as the paths). If you need to reference both versions of Newtonsoft, please see this [MSDN article](http://msdn.microsoft.com/en-us/library/efs781xb\(v=vs.110\).aspx).

** Q: It's not working, what can I try? ** 

Visit the Plovr.NET url directly at /Plovr.NET and see the error that comes up (if any). If you are still unable to resolve, [create an issue](https://github.com/Ogilvy/Plovr.NET/issues).

** Q: When I'm developing, I need to reference the Plovr.NET/compile method but when I'm running it on another site, I have to change the URL that I point to.**

Plovr.NET's configuration comes with a setting called "includePath". A helper function also exists (Plovr.Utilities.GetIncludePath) to get you the path from the web.config. Typically, multiple web.config's exist for each environment and thus you can change the one that's not use for local development to point to the compiled path that's configured.

** Q: Do the jars have to be deployed to production? **

No, and it's recommended that you don't unless you want the raw source deployed. When building to production, you should use the official [Plovr](http://www.plovr.com) jar in build mode.