{
	"id": "test-jquery",
	"paths": "js",
	"inputs": "js/tests/test.jQuery.js",
	"mode": "raw",
	"externs": "~/js/jquery-1.6.1-externs.js",
	"compilerCustomParams": "--output_wrapper='(function($){%output%})(jQuery);' --warning_level=VERBOSE --jscomp_off=checkTypes"
}