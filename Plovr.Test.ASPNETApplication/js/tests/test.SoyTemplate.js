goog.provide('test.SoyTemplate');

goog.require('soy');
goog.require('test.SoyTemplate.Test1');


test.SoyTemplate.init = function () {
	var ele = test.SoyTemplate.Test1.helloWorld();
	$(ele).appendTo("body");
};

$(test.SoyTemplate.init);