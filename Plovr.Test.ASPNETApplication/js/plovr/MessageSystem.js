// Copyright 2011 Ogilvy & Mather. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS-IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

/**
 * @fileoverview The Plovr.NET Message System will basically take all the messages from the closure compiler and display them either 
 * in a custom console that is created on the fly or will use the console that already exists. console.error, console.warn, and 
 * console.info are used.
 */
goog.provide('plovr.MessageSystem');

goog.require('goog.dom');
goog.require('goog.events');
goog.require('goog.events.EventHandler');
goog.require('goog.events.EventType');
goog.require('goog.style');

/**
* The main container element of our custom logger.
* @type {HtmlElement} 
*/
plovr.MessageSystem.element = null;

/**
 * The main element to append more custom log entries into.
 * @type {HtmlElement} 
 */
plovr.MessageSystem.logsElement = null;

/**
 * Show's some sort of identifcation on the top of the console so the user knows what is showing.
 * @type {string}
 */
plovr.MessageSystem.Identify = 'Plovr.NET - Closure Compiler Messages';

/**
* map the enum types to console member functions to report on.
* @enum {string}
*/
plovr.MessageSystem.MessageTypes = {
	'0': 'error',
	'1': 'warn',
	'2': 'info'
};

/**
* A base reset.
* @type {string}
*/
plovr.MessageSystem.BaseLogStyle = 'font-family: Courier new; font-size: 13px; line-height: 16px; padding: 0; margin: 0; background-color: transparent; color: #000; border: 0;';

/**
* All the styles for the custom log.
* @type {object}
*/
plovr.MessageSystem.LogStyles = {
	// treating styles like CSS by using these as a 'className'
	'header': plovr.MessageSystem.BaseLogStyle + 'background-color: #c6c1ba; height: 16px; padding: 4px 2px; overflow: hidden;',
	'headerTitle': plovr.MessageSystem.BaseLogStyle +  'font-size: 13px; font-family: Arial; float: left; color: #000; font-size: 12px; padding: 1px 0 1px 3px;',
	'container': plovr.MessageSystem.BaseLogStyle + 'background-color: #fff; position: fixed; bottom: 0; left: 0; right: 0; height: 200px;',
	'closeButton': plovr.MessageSystem.BaseLogStyle + 'color: #fff; background-color: #a6524d; height: 16px; width: 16px; font-size: 11px; text-align: center; text-decoration: none; font-family: Verdana; font-weight: bold; line-height: 13px; float: right; background-repeat: no-repeat;',
	'logContainer': plovr.MessageSystem.BaseLogStyle + 'overflow: scroll; height: 176px;',
	'defaultLog': plovr.MessageSystem.BaseLogStyle + 'border-bottom: 1px solid #d7d7d7; padding: 2px 0 2px 5px; background-repeat: no-repeat; background-position: 5px 2px;',
	
	// individual log styles
	'info': '',
	'warn': 'background-color: #fffdc6;',
	'error': 'color: #ff1f5a; background-color: #ffebeb;'
};

/**
 * Init the messaging system by showing each message. We will figure out how we do that.
 * @param {Array} plovrMessages all of the plovr messages
 */
plovr.MessageSystem.init = function (plovrMessages) {
	for (var i = 0, len = plovrMessages.length; i < len; i += 1) {
		plovr.MessageSystem.showMessage(plovrMessages[i]);
	}
};

/**
 * Show's a message, which is a IPlovrMessage, into the console. We figure out which mode it needs to be in and send it to log.
 * @param {Object} message a single message object
 */
plovr.MessageSystem.showMessage = function (message) {
	// show in accustomed closure form.
	var messageString = message['FilePath'] + ':' + message['LineNumber'] + ':' + message['ErrorMessage'];

	// the line of code that is in error.
	if (message['CodeLine'].length > 0) {
		messageString += '\n' + message['CodeLine'].replace(/ /g, ' ');
	}

	// our start index is the character the closure compiler is complaining about, we highlight it to the user some how.
	if (message['ErrorStartIndex'] > 0) {
		var fakeSpace = '';
		for (var i = 1; i < message['ErrorStartIndex']; i++) { fakeSpace += ' '; }
		messageString += '\n' + fakeSpace + '^';
	}

	// lookup what kind of log function we should use for this message type.
	var consoleFunction = plovr.MessageSystem.convertMessageTypeToConsoleFunction(message['Type']);
	
	// ok log it now
	plovr.MessageSystem.log(consoleFunction, messageString);
};

/**
 * Convert a message type, which is an integer serialized from PlovrMessageType. 
 * @param {Number} a message type's enum value
 * @returns {string} a string representation of the function to call within console.
 */
plovr.MessageSystem.convertMessageTypeToConsoleFunction = function (messageType) {
	// default to info
	var loggerType = 3;

	// if the log type we have is in our mapper, then set it
	if (messageType in plovr.MessageSystem.MessageTypes) {
		loggerType = messageType;
	}

	// return the mapping
	return plovr.MessageSystem.MessageTypes[loggerType];
};

/**
 * Log a message to the console. If the real console doesn't exist, we create our own if we don't know of it and log our message.
 */
plovr.MessageSystem.log = function (consoleFunction, line) {
	// log to the main console if it exists, the console function should line up.
	if (plovr.MessageSystem.doesConsoleExist()) {
		goog.global['console'][consoleFunction](line);
	} else {
		// if we don't have our custom console already, create it
		if (plovr.MessageSystem.element == null) {
			plovr.MessageSystem.createConsole();
		}

		// we use HTML to format our logger, its easier
		var htmlLine = line.replace(/ /g, "&nbsp;");
		htmlLine = htmlLine.replace(/\n/g, "<br />");
		line = htmlLine;

		// create our custom log entry and add it
		var logEntry = goog.dom.createDom('div',
			{ style: plovr.MessageSystem.LogStyles['defaultLog'] + plovr.MessageSystem.LogStyles[consoleFunction] },
			goog.dom.htmlToDocumentFragment(line)
		);

		goog.dom.appendChild(plovr.MessageSystem.logsElement, logEntry);
	}
};

/**
 * Check to see if the built-in console that's in the browser now exists, and if it does if it has our functions to call.
 * @returns {bool} whether or not the console exists
 */
plovr.MessageSystem.doesConsoleExist = function () {
	var hasAllComponents = false;

	if ('console' in goog.global) {
		hasAllComponents = ( goog.isFunction( goog.global['console']['log'] ) ) && ( goog.isFunction( goog.global['console']['error'] ) ) &&
						   ( goog.isFunction( goog.global['console']['info'] ) ) && ( goog.isFunction( goog.global['console']['warn'] ) );
	}

	return hasAllComponents;
};

/**
 * Create a custom console, pretty basic stuff. It uses a class name instead for style so there's no need for an external CSS file.
 */
plovr.MessageSystem.createConsole = function () {
	// figure out if we already have this element. if we do then we don't need to recreate it, we can steal the logsElement
	plovr.MessageSystem.element = goog.dom.getElement('plovr-logger');

	// if we didn't find anything, then we haven't created one yet, if we did then we will find logsElement and set it.
	if (plovr.MessageSystem.element == null) {
		// our main wrapper element, everything is inside here
		plovr.MessageSystem.element = goog.dom.createDom('div',
			{ 'style': plovr.MessageSystem.LogStyles['container'], 'id': 'plovr-logger' }
		);

		// the logger element is the only thing that scrolls.
		plovr.MessageSystem.logsElement = goog.dom.createDom('div',
			{ 'style': plovr.MessageSystem.LogStyles['logContainer'] }
		);

		// the header contains the close button and some text
		var headerEle = goog.dom.createDom('div',
			{ 'style': plovr.MessageSystem.LogStyles['header'] }
		);

		var closeButton = goog.dom.createDom('a',
			{ 'style': plovr.MessageSystem.LogStyles['closeButton'], 'href': 'javascript:void(0)' },
			'x'
		);

		var headerTitle = goog.dom.createDom('p',
			{ 'style': plovr.MessageSystem.LogStyles['headerTitle'] },
			plovr.MessageSystem.Identify
		);

		// attach close button behavior
		goog.events.listen(closeButton, goog.events.EventType.CLICK, function () {
			goog.dom.removeNode(plovr.MessageSystem.element);
			plovr.MessageSystem.element = null;
		});

		// put the header together
		goog.dom.appendChild(headerEle, headerTitle);
		goog.dom.appendChild(headerEle, closeButton);
		goog.dom.appendChild(plovr.MessageSystem.element, headerEle);

		// add the logger element
		goog.dom.appendChild(plovr.MessageSystem.element, plovr.MessageSystem.logsElement);

		goog.dom.appendChild(document.body, plovr.MessageSystem.element);

		goog.style.setStyle(document.body, 'padding-bottom', '200px');

		plovr.MessageSystem.ConsoleSupportsHtml = true;
	} else {
		plovr.MessageSystem.logsElement = goog.dom.getLastElementChild(plovr.MessageSystem.element);
	}
};

// if we have messages waiting to be shown, now would be a good time to show them.
if (goog.global['plovrMessages'] != null && goog.global['plovrMessages'].length > 0) {
	plovr.MessageSystem.init(goog.global['plovrMessages']);
}