goog.provide('test.ToolTip');

goog.require('goog.ui.Tooltip');

test.ToolTip.element_ = null;

test.ToolTip.init = function () {
	test.ToolTip.create();
	test.ToolTip.set();
};

test.ToolTip.create = function () {
	test.ToolTip.element_ = goog.dom.createDom('input', { 'id': 'whatever', 'type': 'button', 'value': 'Tooltip Demo' });

	var parentEle = goog.dom.createDom('div');

	goog.dom.appendChild(parentEle, test.ToolTip.element_);

	goog.dom.appendChild(document.body, parentEle);
};

test.ToolTip.set = function () {
	var tooltip = new goog.ui.Tooltip(test.ToolTip.element_);
	tooltip.className = 'tooltip-test';
	tooltip.setHtml(
		"This is message two, using a different class name for the tooltip and " +
		"<strong>HTML</strong> <em>markup</em>.<br>"
	);
};

test.ToolTip.init();