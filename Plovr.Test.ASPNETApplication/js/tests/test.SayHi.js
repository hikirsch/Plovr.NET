goog.provide('test.SayHi');

goog.require('oiny.Events.DomReady');

goog.require('goog.dom');

test.SayHi = function() {
	var newHeader = goog.dom.createDom(
		'h1',
		{ 'style': 'background-color:#EEE' },
		'Hello world!'
	);

	goog.dom.appendChild(document.body, newHeader);
};

oiny.Events.DomReady(test.SayHi);