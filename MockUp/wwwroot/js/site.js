// SIDEBAR TOGGLE

document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.querySelector(".sidebar");

    const button = document.querySelector(".menu-toggle");

    if (button) {

        button.addEventListener("click", function () {

            sidebar.classList.toggle("show");

        });

    }

});

// APPROVAL MODAL

document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".approval-btn");

    const requestIdInput = document.getElementById("approvalRequestId");
    const decisionInput = document.getElementById("approvalDecision");
    const message = document.getElementById("approvalMessage");
    const confirmButton = document.getElementById("approvalConfirmButton");

    if (!requestIdInput || !decisionInput) {
        return;
    }

    buttons.forEach(function (button) {

        button.addEventListener("click", function () {

            const requestId = this.dataset.id;
            const decision = this.dataset.action;

            requestIdInput.value = requestId;
            decisionInput.value = decision;

            if (message) {

                message.textContent =
                    "Are you sure you want to " +
                    decision.toLowerCase() +
                    " this request?";

            }

            if (confirmButton) {

                if (decision === "Approved") {

                    confirmButton.textContent = "Approve";
                    confirmButton.className = "btn btn-success";

                } else {

                    confirmButton.textContent = "Reject";
                    confirmButton.className = "btn btn-danger";

                }

            }

        });

    });

});