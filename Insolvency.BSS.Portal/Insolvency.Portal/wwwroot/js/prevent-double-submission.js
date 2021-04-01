if (document.forms.length == 1) {
    var form = document.forms[0];

    var onSubmitHandler = function (event) {
        let button = event.submitter;

        button.setAttribute("disabled", "disabled");
        button.classList.add('govuk-button--disabled');
    };

    form.onsubmit = onSubmitHandler;
}