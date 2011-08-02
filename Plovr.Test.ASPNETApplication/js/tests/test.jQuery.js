goog.provide('test.jQuery');

test.jQuery.init = function () {
	test.jQuery.addIt('<div>Yay! jQuery Works!</div>', 'body');
	test.jQuery.addIt('<div>Yay! jQuery Wahoo!!</div>', 'body');
};

test.jQuery.addIt = function(html, destination) {
	var x = $(html);
	x.appendTo(destination);
};

var xxxxx = {
	'a': 'a',
	'b': 'b',
	'c': 'c',
	'd': 'd',
	'e': 'e'
};


$(test.jQuery.init);