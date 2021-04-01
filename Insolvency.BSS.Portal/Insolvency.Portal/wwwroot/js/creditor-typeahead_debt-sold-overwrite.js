search = function search(input, callback) {
    return input.length < 1 ? callback([]) : $.getJSON("https://".concat(window.location.host, "/creditor/ajax/creditor/search?query=").concat(encodeURIComponent(input)), callback);
}; // elements and other globals


form.removeEventListener("submit", handleSubmit, false);
var handleSubmitOverwrite = function (event) {
    if (creditorId) {
        window.location.href = "https://".concat(window.location.host, "/Creditor/DebtSold/").concat(encodeURIComponent(creditorId)).concat("?journeyKey=", encodeURIComponent(journeyKey.value));
    }
    else if (inputNode.value != null && inputNode.value.trim().length > 0) {
        window.location.href = "https://".concat(window.location.host, "/Creditor/CreditorNewAdHocCreditor?name=").concat(encodeURIComponent(inputNode.value)).concat("&journeyKey=", encodeURIComponent(journeyKey.value));
    }
    else { // submit form if creditor name is empty for the validation to kick in
        return;
    }
    event.preventDefault();
};
form.addEventListener("submit", handleSubmitOverwrite);
if (inputNode.value) {
    search(inputNode.value, setResultsHTML);
}