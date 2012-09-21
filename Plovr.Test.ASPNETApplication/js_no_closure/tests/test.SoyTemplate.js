goog.provide('test.SoyTemplate');

goog.require('test.SoyTemplate.Test1');
goog.require('test.SoyTemplate.Test2');

test.SoyTemplate.init = function () {
	var ele = test.SoyTemplate.Test1.helloWorld();
	$(ele).appendTo("body");

	var ele2 = test.SoyTemplate.Test2.helloWorld();
	$(ele2).appendTo("body");
	
	$("<p>TEST</p>").appendTo("body");
};

$(test.SoyTemplate.init);