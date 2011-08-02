CLOSURE_NO_DEPS = true;

(function () {
              	
var files = ['%FILES%'];

var path = '%INCLUDE_PATH%';

var scriptEl;
var doc = document;
var scripts = doc.getElementsByTagName('script');

for (var i = scripts.length - 1; i >= 0; --i) {
	var candidateScriptEl = scripts[i];
	var src = candidateScriptEl.src; 
	if (src.indexOf(path) >= 0) {
			scriptEl = candidateScriptEl;
			break;
	}
}

if (!scriptEl) { return; }

for (var i = 0; i < files.length; i++) {
		doc.write('<script src="' + files[i] + '"><\/script>');
}
              	
})();