// This is the translated version of the ES6 code I had written - this was compiled with Babel targeting IE10

"use strict";

// utility functions
var debounce = function debounce(callback) {
    var time = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : 250;
    var interval = arguments.length > 2 ? arguments[2] : undefined;
    return function () {
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
            args[_key] = arguments[_key];
        }

        return clearTimeout(interval, interval = setTimeout(function () {
            return callback.apply(void 0, args);
        }, time));
    };
};

var search = function search(input, callback) {
    return input.length < 1 ? callback([]) : $.getJSON("https://".concat(window.location.host, "/ajax/creditor/search?query=").concat(encodeURIComponent(input)), callback);
}; // elements and other globals


var resultsNode = document.getElementById("autocomplete-results");
var rootNode = document.getElementById("autocomplete");
var inputNode = document.getElementById("CreditorName-input");
var form = document.getElementById("creditorSearchForm");
var debtIdElement = document.getElementById("debt-id");
var journeyKey = document.getElementById("journey-key");
var parentJourneyKey = document.getElementById("parent-journey-key");
var activeIndex = -1;
var resultsCount = 0;
var resultsMap = {};
var creditorId = null;

var setResultsHTML = function setResultsHTML(results) {
    resultsMap = results.reduce(function (a, b) {
        a[b.name] = b.id;
        return a;
    }, {});
    results = results.map(function (x) {
        return x.name;
    });

    if (results.length === 0) {
        resultsNode.innerHTML = "\n    <li\n      id='autocomplete-result-0'\n      class='autocomplete-result govuk-input selected-typeahead\" }'\n      role='option'\n      aria-selected='true'\"\n    >\n      No results found\n    </li>\n  ";
    } else {
        resultsNode.innerHTML = results.map(function (result, index) {
            var isSelected = index === 0;

            if (isSelected) {
                activeIndex = 0;
            }

            return "\n      <li\n        id='autocomplete-result-".concat(index, "'\n        class='autocomplete-result govuk-input ").concat(isSelected ? "selected-typeahead" : "", "'\n        role='option'\n        ").concat(isSelected ? "aria-selected='true'" : "", "\n      >\n        ").concat(result, "\n      </li>\n    ");
        }).join("");
    }

    resultsNode.classList.remove("hidden");
    rootNode.setAttribute("aria-expanded", true);
    resultsCount = results.length;
};

var getItemAt = function getItemAt(index) {
    return resultsNode.querySelector("#autocomplete-result-".concat(index));
};

var selectItem = function selectItem(node) {
    if (node) {
        var key = node.innerText.trim();
        inputNode.value = key;
        hideResults();
        creditorId = resultsMap[key];
    }
};

var hideResults = function hideResults() {
    activeIndex = -1;
    resultsNode.innerHTML = "";
    resultsNode.classList.add("hidden");
    rootNode.setAttribute("aria-expanded", "false");
    resultsCount = 0;
    inputNode.setAttribute("aria-activedescendant", "");
};

var checkSelection = function checkSelection() {
    if (activeIndex < 0) return;
    selectItem(getItemAt(activeIndex));
}; // event implementations


var handleResultClick = function handleResultClick(event) {
    if (event.target && event.target.nodeName === 'LI') selectItem(event.target);
};

var handleKeyup = function handleKeyup(event) {
    var key = event.key;
    var inputValue = event.target.value; // if it's a control key skip default actions otherwise update results based on input

    switch (key) {
        case "ArrowUp":
        case "Up":
        case "ArrowDown":
        case "Down":
        case "Escape":
        case "Enter":
            event.preventDefault();
            return;

        default:
            search(inputValue, setResultsHTML);
    }
};

var handleKeydown = function handleKeydown(event) {
    var key = event.key;

    if (key === "Backspace") {
        creditorId = null;
        return;
    }

    if (key === "Escape") {
        hideResults();
        inputNode.value = "";
        return;
    }

    if (resultsCount < 1) return;
    var prevActive = getItemAt(activeIndex);
    var activeItem;

    switch (key) {
        case "ArrowUp":
        case "Up":
            if (activeIndex <= 0) {
                activeIndex = resultsCount - 1;
            } else {
                activeIndex -= 1;
            }

            break;

        case "ArrowDown":
        case "Down":
            if (activeIndex === -1 || activeIndex >= resultsCount - 1) {
                activeIndex = 0;
            } else {
                activeIndex += 1;
            }

            break;

        case "Enter":
            activeItem = getItemAt(activeIndex);
            selectItem(activeItem);
            event.preventDefault();
            return;

        case "Tab":
            checkSelection();
            hideResults();
            return;

        default:
            return;
    }

    event.preventDefault();
    activeItem = getItemAt(activeIndex);
    activeIndex = activeIndex;

    if (prevActive) {
        prevActive.classList.remove("selected-typeahead");
        prevActive.setAttribute("aria-selected", "false");
    }

    if (activeItem) {
        inputNode.setAttribute("aria-activedescendant", "autocomplete-result-".concat(activeIndex));
        activeItem.classList.add("selected-typeahead");
        activeItem.setAttribute("aria-selected", "true");
    } else {
        inputNode.setAttribute("aria-activedescendant", "");
    }
};

var handleSubmit = function handleSubmit(event) {
    var redirectHref = null;

    if (creditorId) {
        redirectHref = "https://".concat(window.location.host, "/ajax/creditor/submit?id=").concat(encodeURIComponent(creditorId))
            .concat("&journeyKey=", encodeURIComponent(journeyKey.value))
            .concat("&parentJourneyKey=", encodeURIComponent(parentJourneyKey.value));
    }
    else if (inputNode.value != null && inputNode.value.trim().length > 0) {
        redirectHref = "https://".concat(window.location.host, "/ajax/creditor/new?name=").concat(encodeURIComponent(inputNode.value))
            .concat("&journeyKey=", encodeURIComponent(journeyKey.value))
            .concat("&parentJourneyKey=", encodeURIComponent(parentJourneyKey.value));
    } else { // submit form if creditor name is empty for the validation to kick in
        return;
    }

    var urlParam = new URLSearchParams(window.location.href);
    var debtId = urlParam.get('debtId');
    var returnAction = urlParam.get('returnAction');
    var edit = urlParam.get('edit');

    if (returnAction) {
        redirectHref = redirectHref.concat("&returnAction=").concat(encodeURIComponent(returnAction));
    }

    if (debtId) {
        redirectHref = redirectHref.concat("&debtId=").concat(encodeURIComponent(debtId));
    }

    if (edit) {
        redirectHref = redirectHref.concat("&edit=").concat(encodeURIComponent(edit));
    }

    window.location.href = redirectHref;

    event.preventDefault();
};

var handleDocumentClick = function handleDocumentClick(event) {
    if (event.target === inputNode || rootNode.contains(event.target)) return;
    hideResults();
}; //events binding

document.body.addEventListener('click', handleDocumentClick);
inputNode.addEventListener("keyup", debounce(handleKeyup, 160));
inputNode.addEventListener("keydown", handleKeydown);
resultsNode.addEventListener('click', handleResultClick);
form.addEventListener("submit", handleSubmit, false);