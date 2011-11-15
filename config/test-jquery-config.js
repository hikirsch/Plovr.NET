{
	"id": "test-jquery",
	"basePath": "~/js",
	"namespaces": "test.jQuery",
	"mode": "raw",
	"closureExternFiles": "~/js/jquery-1.6.1-externs.js",
	"compilerCustomParams": "--output_wrapper='(function($){%output%})(jQuery);' --warning_level=VERBOSE --jscomp_off=checkTypes"
}