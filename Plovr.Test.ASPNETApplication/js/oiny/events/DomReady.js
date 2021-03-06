﻿goog.provide('oiny.Events.DomReady');

oiny.Events.DomReady = (function(){

var fns = [], ol, fn, f = false,
	doc = goog.global['document'],
    testEl = doc.documentElement,
    hack = testEl.doScroll,
    domContentLoaded = 'DOMContentLoaded',
    addEventListener = 'addEventListener',
    onreadystatechange = 'onreadystatechange',
    loaded = /^loade|c/.test(doc.readyState);

function flush(i) {
	loaded = 1;
	while (i = fns.shift()) { i() }
}

doc[addEventListener] && doc[addEventListener](domContentLoaded, fn = function () {
	doc.removeEventListener(domContentLoaded, fn, f);
	flush();
}, f);

hack && doc.attachEvent(onreadystatechange, (ol = function () {
	if (/^c/.test(doc.readyState)) {
		doc.detachEvent(onreadystatechange, ol);
		flush();
	}
}));

return hack ?
function (fn) {
    self != top ?
    loaded ? fn() : fns.push(fn) :
    function () {
        try {
        	testEl.doScroll('left');
        } catch (e) {
        	return setTimeout(function () { oiny.Events.DomReady(fn); }, 50);
        }
        fn();
    } ()
} :
function (fn) {
    loaded ? fn() : fns.push(fn);
};

})();