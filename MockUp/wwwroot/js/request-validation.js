function countWords(text) {

    return text
        .trim()
        .split(/\s+/)
        .filter(word => word.length > 0)
        .length;

}
// Title Validation
const title = document.getElementById("Title");
const titleCounter = document.getElementById("titleCounter");
const titleError = document.getElementById("titleError");

if (title) {

    title.addEventListener("input", function () {

        const words = countWords(this.value);

        titleCounter.textContent = words + " / 15 words";

        if (words > 15) {

            titleError.textContent =
                "Title must not exceed 15 words.";

        }
        else {

            titleError.textContent = "";

        }

    });

}

//Description Validation

const description = document.getElementById("Description");
const descriptionCounter = document.getElementById("descriptionCounter");

const descriptionError = document.getElementById("descriptionError");

if (description) {

    description.addEventListener("input", function () {

        const words = countWords(this.value);

        descriptionCounter.textContent =
            words + " / 50 words";

        if (words > 50) {

            descriptionError.textContent =
                "Description must not exceed 50 words.";

        }
        else {

            descriptionError.textContent = "";

        }

    });

}

// File Validation

const fileInput = document.getElementById("Attachment");

if (fileInput) {

    fileInput.addEventListener("change", function () {

        const file = this.files[0];

        if (!file)
            return;

        const allowed = [
            ".pdf",
            ".docx",
            ".xls",
            ".xlsx",
            ".png",
            ".jpg",
            ".jpeg"
        ];

        const extension = "." + file.name.split(".").pop().toLowerCase();

        if (!allowed.includes(extension)) {

            showMessage("Only PDF, Excel, Word, JPG and PNG files are allowed.");;

            this.value = "";

            return;

        }

        if (file.size > 3 * 1024 * 1024) {

            showMessage("Maximum file size is 3 MB.");;

            this.value = "";

        }

    });

}

// Start Date / Due Date order validation

const startDateInput = document.getElementById("StartDate");
const dueDateInput = document.getElementById("PreferredCompletionDate");
const dateOrderError = document.getElementById("dateOrderError");

function syncDueDateMin() {

    if (!startDateInput || !dueDateInput || !startDateInput.value)
        return;

    dueDateInput.min = startDateInput.value;

    if (dueDateInput.value && dueDateInput.value < startDateInput.value) {

        dueDateInput.value = startDateInput.value;

    }

}

if (startDateInput) {

    startDateInput.addEventListener("change", syncDueDateMin);

    syncDueDateMin();

}

const form = document.querySelector("form");

form.addEventListener("submit", function (e) {

    e.preventDefault();

    let valid = true;

    titleError.textContent = "";
    descriptionError.textContent = "";

    if (countWords(title.value) > 15) {

        titleError.textContent =
            "Title must not exceed 15 words.";

        valid = false;

    }

    if (countWords(title.value) === 0) {

        valid = false;

    }

    if (countWords(description.value) > 50) {

        descriptionError.textContent =
            "Description must not exceed 50 words.";

        valid = false;

    }

    if (countWords(description.value) === 0) {

        valid = false;

    }

    if (dateOrderError) {
        dateOrderError.textContent = "";
    }

    if (startDateInput && dueDateInput &&
        startDateInput.value && dueDateInput.value &&
        dueDateInput.value < startDateInput.value) {

        if (dateOrderError) {
            dateOrderError.textContent =
                "Completion date cannot be earlier than the start date.";
        }

        valid = false;

    }

    if (!valid) {

        return;

    }

    showConfirm(
        "Are you sure you want to submit this request?",
        function () {

            form.submit();

        });

});