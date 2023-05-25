window.JSInteropPlus = {

    SubscribeToWindowEvent: function (eventToSubscribe, id, eventCallType, eventCallTimeMilliseconds) {

        var callback;
        let controller = new AbortController();

        if (eventCallType == null || eventCallType === 0) {
            callback = eventCallback(id, controller);
        }
        else if (eventCallType === 1) {
            callback = throttle(eventCallback(id, controller), eventCallTimeMilliseconds);
        }
        else if (eventCallType === 2) {
            callback = debounce(eventCallback(id, controller), eventCallTimeMilliseconds);
        }

        window.addEventListener(eventToSubscribe, callback, { signal: controller.signal });
    },

    SubscribeToElementEvent: function (element, eventToSubscribe, id, eventCallType, eventCallTimeMilliseconds) {

        var callback;
        let controller = new AbortController();

        if (eventCallType == null || eventCallType === 0) {
            callback = eventCallback(id, controller);
        }
        else if (eventCallType === 1) {
            callback = throttle(eventCallback(id, controller), eventCallTimeMilliseconds);
        }
        else if (eventCallType === 2) {
            callback = debounce(eventCallback(id, controller), eventCallTimeMilliseconds);
        }

        element.addEventListener(eventToSubscribe, callback, { signal: controller.signal });
    },

    CallWindowFunction: function (functionToCall, ...args) {

        const func = window[functionToCall];

        if (typeof func === "function")
            return func.apply(window, ...args)
    },

    CallElementFunction: function (functionToCall, element, ...args) {

        const func = element[functionToCall];

        if (typeof func === "function")
            return func.apply(element, ...args)
    },

    SetWindowProperty: function (property, value) {
        window[property] = value;
    },

    GetWindowProperty: function (property) {
        return window[property];
    },

    SetElementProperty: function (element, property, value) {
        element[property] = value;
    },

    GetElementProperty: function (element, property) {
        return element[property];
    }
}

function eventCallback(id, abortController) {
    return function () {
        DotNet.invokeMethodAsync('Blazor.JSInteropPlus', 'OnJSEvent', id).then(wasSuccessfull => {
            if (wasSuccessfull === false) {
                abortController.abort();
            }
        });
    }
}

function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
};

function throttle(func, wait) {
    var context, args, result;
    var timeout = null;
    var previous = 0;
    var later = function () {
        previous = Date.now();
        timeout = null;
        result = func.apply(context, args);
        if (!timeout) context = args = null;
    };
    return function () {
        var now = Date.now();
        if (!previous) previous = now;
        var remaining = wait - (now - previous);
        context = this;
        args = arguments;

        if (!timeout) {
            timeout = setTimeout(later, remaining);
        }
        return result;
    };
};