/**
 * This is a full example of all the options that are currently supported with the .NET version of Plovr. The goal is to support
 * the Plovr tool as much as posible for seamless use with .NET development. 
 * NOTE: Whenever referencing any of the paths below, you may use the .NET shorthand style of "~/" to reference the web root of 
 * the project that is being compiled.
 */
{
	/**
	 * Where the base paths is for your javascript. 
	 * This parameter is ~/ friendly.
	 * NOTE: /goog/base.js must be in the first path within the basePaths array.
	 * @type Array
	 */
	"basePaths": [ "~/js" ],
	
	/**
	 * the namespaces that you wish to compile, this is an array
	 * @type Array
	 */
	"namespaces": [ "test.Calendar" ],
	
	/**
	 * any extern files you want to pass to the closure compiler, if you're using jQuery on a project, you'll want to use the
	 * jQuery externs file or whatever external library you may be using.
	 * This parameter is ~/ friendly.
	 * @type Array
	  "externs": [ "~/js/jquery-1.6.1-externs.js" ],
	 */ 
	
	
	/**
	 * the closure compilation mode, options are WHITESPACE, SIMPLE and ADVANCED
	 * @see http://code.google.com/closure/compiler/docs/compilation_levels.html
	 * @type String
	 */
	"mode": "ADVANCED",
	
	/**
	 * The output file that should be saved. 
	 * This parameter is ~/ friendly.
	 * @type String
	 */
	"outputFile": "~/js/test-calendar-compiled.js"
}