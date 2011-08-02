goog.provide('test.AnotherTest');

test.AnotherTest.init = function () {
	$('<p>Yay! from AnotherClass</p>').appendTo('body');
};

$(test.AnotherTest.init);