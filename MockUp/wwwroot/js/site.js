document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.querySelector(".sidebar");

    const button = document.querySelector(".menu-toggle");

    if (button) {

        button.addEventListener("click", function () {

            sidebar.classList.toggle("show");

        });

    }

});

document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".approval-btn");

    buttons.forEach(btn => {

        btn.addEventListener("click", function () {

            document.getElementById("RequestId").value =
                this.dataset.id;

            document.getElementById("status").value =
                this.dataset.action;

            document.getElementById("Remarks").value =
                document.querySelector("textarea[name='Remarks']").value;

            document.getElementById("approvalMessage").innerText =
                "Are you sure you want to " +
                this.dataset.action.toLowerCase() +
                " this request?";
        });

    });

});