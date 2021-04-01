form.removeEventListener("submit", handleSubmit, false);
var handleSubmitOverwrite = function (event) {
    if (creditorId) {
        window.location.href = "https://".concat(window.location.host, "/BreathingSpace/DebtorDebtSold/").concat(encodeURIComponent(creditorId))
            .concat("?journeyKey=", encodeURIComponent(journeyKey.value))
            .concat("&parentjourneyKey=", encodeURIComponent(parentJourneyKey.value));
    }
    else if (inputNode.value != null && inputNode.value.trim().length > 0) {
        window.location.href = "https://".concat(window.location.host, "/ajax/creditor/new?name=")
                                         .concat(encodeURIComponent(inputNode.value))
                                         .concat("&debtId=", encodeURIComponent(debtIdElement.value))
                                         .concat("&isDebtTransferred=true")
                                         .concat("&journeyKey=", encodeURIComponent(journeyKey.value))
                                         .concat("&parentjourneyKey=", encodeURIComponent(parentJourneyKey.value));
    }
    else { // submit form if creditor name is empty for the validation to kick in
        return;
    }

    event.preventDefault();
};
form.addEventListener("submit", handleSubmitOverwrite);