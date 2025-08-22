$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
});

function success(message) {
    toastr["success"](message);
}

function info(message) {
    toastr["info"](message);
}

function warning(message) {
    toastr["warning"](message);
}

function error(message) {
    toastr["error"](message);
}

function log(message) {
    console.log(message);
}
function Call(name) {
    let fcObj = window[`${name}`];
    fcObj();
}

function SetEvent(event, element, action) {
    $("body").on(event, element, action);
}