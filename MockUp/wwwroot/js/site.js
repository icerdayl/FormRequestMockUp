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

    const requestIdInput = document.getElementById("approvalRequestId");
    const decisionInput = document.getElementById("approvalDecision");
    const message = document.getElementById("approvalMessage");
    const confirmButton = document.getElementById("approvalConfirmButton");

    if (!requestIdInput || !decisionInput) {
        return;
    }

    // Event delegation on document, not per-button listeners -
    // HelpDesk's list page replaces the table (and its buttons) via
    // AJAX after searching/filtering, so listeners attached directly
    // to the original buttons would stop working after any refresh.
    document.addEventListener("click", function (e) {

        const button = e.target.closest(".approval-btn");

        if (!button) {
            return;
        }

        const requestId = button.dataset.id;
        const decision = button.dataset.action;

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