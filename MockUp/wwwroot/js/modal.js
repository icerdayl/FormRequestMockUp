let confirmCallback = null;

function showConfirm(message, callback) {

    document.getElementById("confirmMessage").textContent = message;

    confirmCallback = callback;

    const modal = new bootstrap.Modal(
        document.getElementById("confirmModal")
    );

    modal.show();

}

document
    .getElementById("confirmYes")
    .addEventListener("click", function () {

        bootstrap.Modal
            .getInstance(document.getElementById("confirmModal"))
            .hide();

        if (confirmCallback)
            confirmCallback();

    });

function showMessage(message) {

    document.getElementById("messageText").textContent = message;

    new bootstrap.Modal(
        document.getElementById("messageModal")
    ).show();

}