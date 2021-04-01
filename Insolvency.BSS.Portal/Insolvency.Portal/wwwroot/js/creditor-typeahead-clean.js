// utility functions
const debounce = (callback, time = 250, interval) =>
    (...args) =>
        clearTimeout(interval, interval = setTimeout(() => callback(...args), time));

const search = (input, callback) =>
    input.length < 1 ?
        callback([]) :
        $.getJSON(`https://${window.location.host}/ajax/creditor/search?query=${encodeURIComponent(input)}`, callback)


// elements and other globals
const resultsNode = document.getElementById("autocomplete-results");
const rootNode = document.getElementById("autocomplete");
const inputNode = document.getElementById("CreditorName-input");
const form = document.getElementById("creditorSearchForm");

let activeIndex = -1;
let resultsCount = 0;
let resultsMap = {};
let creditorId = null;

// logic
const setResultsHTML = results => {
    resultsMap = results.reduce((a, b) => { a[b.name] = b.id; return a; }, {});
    results = results.map(x => x.name);

    if (results.length === 0) {
        resultsNode.innerHTML = `
    <li
      id='autocomplete-result-0'
      class='autocomplete-result govuk-input selected-typeahead" }'
      role='option'
      aria-selected='true'"
    >
      No results found
    </li>
  `
    }
    else {
        resultsNode.innerHTML = results.map((result, index) => {
            const isSelected = index === 0;
            if (isSelected) {
                activeIndex = 0;
            }
            return `
      <li
        id='autocomplete-result-${index}'
        class='autocomplete-result govuk-input ${isSelected ? "selected-typeahead" : ""}'
        role='option'
        ${isSelected ? "aria-selected='true'" : ""}
      >
        ${result}
      </li>
    `;
        }).join("");
    }
    resultsNode.classList.remove("hidden");
    rootNode.setAttribute("aria-expanded", true);
    resultsCount = results.length;
}

const getItemAt = index => {
    return resultsNode.querySelector(`#autocomplete-result-${index}`);
}

const selectItem = node => {
    if (node) {
        const key = node.innerText.trim();
        inputNode.value = key;
        hideResults();
        creditorId = resultsMap[key];
    }
}

const hideResults = () => {
    activeIndex = -1;
    resultsNode.innerHTML = "";
    resultsNode.classList.add("hidden");
    rootNode.setAttribute("aria-expanded", "false");
    resultsCount = 0;
    inputNode.setAttribute("aria-activedescendant", "");
}

const checkSelection = () => {
    if (activeIndex < 0) return;
    selectItem(getItemAt(activeIndex));
}
// event implementations

const handleResultClick = event => {
    if (event.target && event.target.nodeName === 'LI') selectItem(event.target)
}

const handleKeyup = (event) => {
    const { key } = event;

    const inputValue = event.target.value;

    // if it's a control key skip default actions otherwise update results based on input
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

const handleKeydown = (event) => {
    const { key } = event;

    if (key === "Escape") {
        hideResults();
        inputNode.value = "";
        return;
    }

    if (resultsCount < 1) return;

    const prevActive = getItemAt(activeIndex);
    let activeItem;

    switch (key) {
        case "Up":
        case "ArrowUp":
            if (activeIndex <= 0) {
                activeIndex = resultsCount - 1;
            } else {
                activeIndex -= 1;
            }
            break;
        case "Down":
        case "ArrowDown":
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
    }   event.preventDefault();
    activeItem = getItemAt(activeIndex);
    activeIndex = activeIndex;

    if (prevActive) {
        prevActive.classList.remove("selected-typeahead");
        prevActive.setAttribute("aria-selected", "false");
    }

    if (activeItem) {
        inputNode.setAttribute(
            "aria-activedescendant",
            `autocomplete-result-${activeIndex}`
        );
        activeItem.classList.add("selected-typeahead");
        activeItem.setAttribute("aria-selected", "true");
    } else {
        inputNode.setAttribute("aria-activedescendant", "");
    }
}

const handleSubmit = (event) => {
    if (creditorId) {
        window.location.href = `https://${window.location.host}/ajax/creditor/submit?id=${encodeURIComponent(creditorId)}`;
    }
    else if (inputNode.value != null && inputNode.value.trim().length > 0) {
        window.location.href = "https://".concat(window.location.host, "/ajax/creditor/new?name=").concat(encodeURIComponent(inputNode.value));
    } else { // submit form if creditor name is empty for the validation to kick in
        return;
    }
    event.preventDefault();
}

const handleDocumentClick = event => {
    if (event.target === inputNode || rootNode.contains(event.target))
        return;

    hideResults();
}
//events binding
document.body.addEventListener('click', handleDocumentClick);
inputNode.addEventListener("keyup", debounce(handleKeyup, 160));
inputNode.addEventListener("keydown", handleKeydown);
resultsNode.addEventListener('click', handleResultClick);
form.addEventListener("submit", handleSubmit);