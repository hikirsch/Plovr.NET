goog.provide("test.AnotherTest");

test.AnotherTest.init = function () {
	$('<p>Yay! from AnotherTest</p>').appendTo('body');
};

$(test.AnotherTest.init);