document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.querySelector(".sidebar");

    const button = document.querySelector(".menu-toggle");

    if (button) {

        button.addEventListener("click", function () {

            sidebar.classList.toggle("show");

        });

    }

});