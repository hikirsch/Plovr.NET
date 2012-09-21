{
	"id": "test-Soy-no-closure", 
	"paths": "js-no-closure",
	"inputs": "js-no-closure/tests/test.SoyTemplate.js", 
	"mode": "RAW",
	"externs": "~/js2/jquery-1.6.1-externs.js",
	"compilerCustomParams" : "--output_wrapper='(function(){%output%})();' --warning_level=VERBOSE --jscomp_off=checkTypes"
}