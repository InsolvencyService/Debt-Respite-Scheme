window.addEventListener("load", function () {
    var seconds = 10;
    var secondsElement = this.document.getElementById("countdown-seconds");
    var setRemainingSecods = function (seconds) {
        secondsElement.innerText = seconds;
    };
    var redirect = function () {
        var a = document.getElementById("complete-logout-link");
        if (a) {
            window.location = a.href;
        }
    };    

    setRemainingSecods(seconds);
    var countdownInterval = this.setInterval(function () {
        seconds = seconds - 1;
        setRemainingSecods(seconds);
        if (seconds == 0) {
            clearInterval(countdownInterval);
            redirect();
        }
    }, 1000);
});
