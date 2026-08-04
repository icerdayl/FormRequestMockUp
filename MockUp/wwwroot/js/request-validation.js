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

// Description Validation

const description = document.getElementById("Description");
const descriptionCounter =
    document.getElementById("descriptionCounter");

const descriptionError =
    document.getElementById("descriptionError");

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

            alert("Only PDF, Excel, Word, JPG and PNG files are allowed.");

            this.value = "";

            return;

        }

        if (file.size > 3 * 1024 * 1024) {

            alert("Maximum file size is 3 MB.");

            this.value = "";

        }

    });

}

const form = document.querySelector("form");

form.addEventListener("submit", function (e) {

    let valid = true;

    titleError.textContent = "";
    descriptionError.textContent = "";

    if (countWords(title.value) > 15) {

        titleError.textContent =
            "Title must not exceed 15 words.";

        valid = false;

    }

    if (countWords(description.value) > 50) {

        descriptionError.textContent =
            "Description must not exceed 50 words.";

        valid = false;

    }

    if (!valid) {

        e.preventDefault();

        return;

    }

    if (!confirm("Are you sure you want to submit this request?")) {

        e.preventDefault();

    }

});