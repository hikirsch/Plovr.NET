goog.provide('test.Calendar');

goog.require('goog.dom');
goog.require('goog.i18n.DateTimeFormat');
goog.require('goog.i18n.DateTimeParse');
goog.require('goog.ui.InputDatePicker');
goog.require('goog.ui.LabelInput');

test.Calendar.containerEle_ = null;
test.Calendar.labelEle_ = null;
test.Calendar.spanEle_ = null;

test.Calendar.init = function () {
	test.Calendar.createElements();
	test.Calendar.initElements();
};

test.Calendar.createElements = function () {
	test.Calendar.element_ = goog.dom.createDom( 'div' );
	test.Calendar.labelEle_ = goog.dom.createDom( 'label', { }, 'Date:' );
	test.Calendar.spanEle_ = goog.dom.createDom( 'span' );

	goog.dom.appendChild(test.Calendar.element_, test.Calendar.labelEle_);
	goog.dom.appendChild(test.Calendar.element_, test.Calendar.spanEle_);
	goog.dom.appendChild(document.body, test.Calendar.element_);
};

test.Calendar.initElements = function () {
	var pattern = "MM'/'dd'/'yyyy";
	var formatter = new goog.i18n.DateTimeFormat(pattern);
	var parser = new goog.i18n.DateTimeParse(pattern);

	// Use a LabelInput for this one:
	var fieldLabelInput = new goog.ui.LabelInput('MM/DD/YYYY');
	fieldLabelInput.render(test.Calendar.spanEle_);

	var idp3 = new goog.ui.InputDatePicker(formatter, parser);
	idp3.decorate(fieldLabelInput.getElement());
};

test.Calendar.init();