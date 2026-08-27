const RequestFeaturesBuilder = (function () {

    let state = [];
    let builderContainer = null;
    let hiddenInput = null;
    let isInitialized = false;

    function escapeHtml(str) {
        return (str === null || str === undefined ? "" : String(str))
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function getTodayString() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, "0");
        const day = String(today.getDate()).padStart(2, "0");
        return `${year}-${month}-${day}`;
    }

    function render() {

        if (!builderContainer) {
            return;
        }

        if (state.length === 0) {
            builderContainer.innerHTML = '<p class="text-muted small">No features added yet.</p>';
            return;
        }

        let html = "";

        state.forEach(function (feature, fIndex) {

            const subTaskRows = feature.subTasks.map(function (subTask, sIndex) {
                const calculatedManDays = calculateManDays(
                    subTask.startDate,
                    subTask.dueDate);

                if (calculatedManDays !== null) {
                    subTask.estimatedManDays = calculatedManDays;
                }

                return `
                    <tr>
                        <td style="min-width:160px;">
                            <input type="text" class="form-control form-control-sm subtask-title"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   value="${escapeHtml(subTask.title)}" />
                        </td>
                        <td>
                            <input type="date" class="form-control form-control-sm subtask-start"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   min="${getTodayString()}"
                                   value="${subTask.startDate || ""}" />
                        </td>
                        <td>
                            <input type="date" class="form-control form-control-sm subtask-due"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   min="${getTodayString()}"
                                   value="${subTask.dueDate || ""}" />
                        </td>
                        <td style="width:90px;">
                            <input type="text" class="form-control form-control-sm subtask-mandays"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   value="${subTask.estimatedManDays === null || subTask.estimatedManDays === undefined ? "" : subTask.estimatedManDays}"
                                   readonly />
                        </td>
                        <td>
                            <button type="button" class="btn btn-outline-danger btn-sm remove-subtask-btn"
                                    data-feature-index="${fIndex}" data-subtask-index="${sIndex}" title="Remove subtask">
                                &times;
                            </button>
                        </td>
                    </tr>`;
            }).join("");

            html += `
                <div class="card mb-3">
                    <div class="card-body">

                        <div class="d-flex justify-content-between align-items-start mb-2">
                            <div class="flex-grow-1 me-3">
                                <label class="form-label small">Feature Title</label>
                                <input type="text" class="form-control form-control-sm feature-title"
                                       data-feature-index="${fIndex}" value="${escapeHtml(feature.title)}" />
                            </div>
                            <button type="button" class="btn btn-outline-danger btn-sm remove-feature-btn mt-4"
                                    data-feature-index="${fIndex}" title="Remove feature">
                                &times;
                            </button>
                        </div>

                        <div class="mb-2">
                            <label class="form-label small">Description</label>
                            <textarea class="form-control form-control-sm feature-description"
                                      data-feature-index="${fIndex}" rows="2">${escapeHtml(feature.description)}</textarea>
                        </div>
                                             
                        <label class="form-label small fw-bold mt-2">Subtasks</label>

                        <div class="table-responsive">
                            <table class="table table-sm align-middle mb-2">
                                <thead>
                                    <tr>
                                        <th>Title</th>
                                        <th>Start</th>
                                        <th>Due</th>
                                        <th>Man-Days</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    ${subTaskRows}
                                </tbody>
                            </table>
                        </div>

                        <button type="button" class="btn btn-outline-secondary btn-sm add-subtask-btn" data-feature-index="${fIndex}">
                            + Add Subtask
                        </button>

                    </div>
                </div>`;
        });

        builderContainer.innerHTML = html;
        syncSubtaskDateConstraints();
    }

    function syncSubtaskDateConstraints() {
        if (!builderContainer) {
            return;
        }

        const requestStartInput = document.getElementById("StartDate");
        const requestDueInput = document.getElementById("PreferredCompletionDate");
        const today = getTodayString();

        const minDate = requestStartInput && requestStartInput.value
            ? (requestStartInput.value > today ? requestStartInput.value : today)
            : today;

        const maxDate = requestDueInput && requestDueInput.value
            ? requestDueInput.value
            : "";

        builderContainer.querySelectorAll(".subtask-start, .subtask-due")
            .forEach(function (input) {
                input.min = minDate;
                input.max = maxDate;
            });
    }

    function addFeature(title, description, subTaskTitles) {
        state.push({
            title: title || "",
            description: description || "",
            subTasks: (subTaskTitles || []).map(function (t) {
                return { title: t, startDate: "", dueDate: "", estimatedManDays: null };
            })
        });
    }

    function applyTemplate(ticketTypeName, templates) {
        const featureTemplates = templates[ticketTypeName];

        if (!featureTemplates) {
            return;
        }

        featureTemplates.forEach(function (ft) {
            addFeature(ft.title, ft.description, ft.subTaskTitles);
        });
    }

    function serialize() {
        return state
            .filter(function (f) { return f.title && f.title.trim() !== ""; })
            .map(function (f) {
                return {
                    title: f.title,
                    description: f.description,                    
                    subTasks: f.subTasks
                        .filter(function (s) { return s.title && s.title.trim() !== ""; })
                        .map(function (s) {
                            return {
                                title: s.title,
                                startDate: s.startDate || null,
                                dueDate: s.dueDate || null,
                                estimatedManDays: (s.estimatedManDays === "" || s.estimatedManDays === null || s.estimatedManDays === undefined)
                                    ? null
                                    : parseFloat(s.estimatedManDays)
                            };
                        })
                };
            });
    }

    // Public method - called from request-validation.js right
    // before it does form.submit(), since calling .submit()
    // programmatically does NOT fire the form's "submit" event,
    // so a normal submit listener here would never run.
    function syncBeforeSubmit() {
        if (hiddenInput) {
            hiddenInput.value = JSON.stringify(serialize());
        }
    }

    // Inclusive calendar-day count between two yyyy-mm-dd strings.
    // Does not exclude weekends (no business-day rule established).
    function calculateManDays(startDateStr, dueDateStr) {

        if (!startDateStr || !dueDateStr) {
            return null;
        }

        const start = new Date(startDateStr);
        const due = new Date(dueDateStr);
        const diffDays = Math.round((due - start) / (1000 * 60 * 60 * 24)) + 1;

        return diffDays > 0 ? diffDays : null;
    }

    function syncFieldToState(target) {

        const fIndexRaw = target.dataset.featureIndex;

        if (fIndexRaw === undefined) {
            return;
        }

        const feature = state[parseInt(fIndexRaw, 10)];

        if (!feature) {
            return;
        }

        const sIndexRaw = target.dataset.subtaskIndex;

        if (sIndexRaw !== undefined) {

            const subTask = feature.subTasks[parseInt(sIndexRaw, 10)];

            if (!subTask) {
                return;
            }

            if (target.classList.contains("subtask-title")) subTask.title = target.value;
            else if (target.classList.contains("subtask-start")) subTask.startDate = target.value;
            else if (target.classList.contains("subtask-due")) subTask.dueDate = target.value;
            // Man-days are derived automatically from Start/Due dates.

            // Automatically calculate man-days from the date span whenever
            // either date changes.
            if (target.classList.contains("subtask-start") || target.classList.contains("subtask-due")) {

                const suggested = calculateManDays(subTask.startDate, subTask.dueDate);

                if (suggested !== null) {

                    subTask.estimatedManDays = suggested;

                    const manDaysInput = builderContainer.querySelector(
                        `.subtask-mandays[data-feature-index="${fIndexRaw}"][data-subtask-index="${sIndexRaw}"]`);

                    if (manDaysInput) {
                        manDaysInput.value = suggested;
                    }

                }

            }

        } else {

            if (target.classList.contains("feature-title")) feature.title = target.value;
            else if (target.classList.contains("feature-description")) feature.description = target.value;

        }
    }

    function init(options) {

        // Defends against the builder accidentally being initialized
        // twice on the same page (e.g. a stray duplicate script
        // include), which would double up every event listener and
        // could look like duplicate/blank feature cards appearing.
        if (isInitialized) {
            return;
        }

        isInitialized = true;

        const ticketTypeSelect = document.getElementById(options.ticketTypeSelectId);
        builderContainer = document.getElementById(options.builderContainerId);
        const addFeatureButton = document.getElementById(options.addFeatureButtonId);
        hiddenInput = document.getElementById(options.hiddenInputId);
        const templates = options.templates || {};
        state = Array.isArray(options.initialFeatures) ? options.initialFeatures : [];

        if (ticketTypeSelect) {

            ticketTypeSelect.addEventListener("change", function () {

                const selectedOption = this.options[this.selectedIndex];
                const ticketTypeName = selectedOption ? selectedOption.dataset.name : null;

                // Changing the Ticket Type replaces whatever is in
                // the builder with that type's suggestions - any
                // features/subtasks from the previous selection
                // (suggested or manually added) are cleared first.
                state = [];

                if (ticketTypeName) {
                    applyTemplate(ticketTypeName, templates);
                }

                render();

            });
        }

        const requestStartInput = document.getElementById("StartDate");
        const requestDueInput = document.getElementById("PreferredCompletionDate");

        if (requestStartInput) {
            requestStartInput.addEventListener("change", syncSubtaskDateConstraints);
        }

        if (requestDueInput) {
            requestDueInput.addEventListener("change", syncSubtaskDateConstraints);
        }

        if (addFeatureButton) {
            addFeatureButton.addEventListener("click", function () {
                addFeature("", "", []);
                render();
            });
        }

        if (builderContainer) {

            // Structural changes (add/remove rows) need a re-render.
            builderContainer.addEventListener("click", function (e) {

                const removeFeatureBtn = e.target.closest(".remove-feature-btn");

                if (removeFeatureBtn) {
                    const fIndex = parseInt(removeFeatureBtn.dataset.featureIndex, 10);
                    state.splice(fIndex, 1);
                    render();
                    return;
                }

                const addSubtaskBtn = e.target.closest(".add-subtask-btn");

                if (addSubtaskBtn) {
                    const fIndex = parseInt(addSubtaskBtn.dataset.featureIndex, 10);
                    state[fIndex].subTasks.push({
                        title: "", startDate: "", dueDate: "", estimatedManDays: null
                    });
                    render();
                    return;
                }

                const removeSubtaskBtn = e.target.closest(".remove-subtask-btn");

                if (removeSubtaskBtn) {
                    const fIndex = parseInt(removeSubtaskBtn.dataset.featureIndex, 10);
                    const sIndex = parseInt(removeSubtaskBtn.dataset.subtaskIndex, 10);
                    state[fIndex].subTasks.splice(sIndex, 1);
                    render();
                    return;
                }

            });

            // Field edits update state in place with no re-render —
            // re-rendering on every keystroke would steal focus out
            // of whatever the person is currently typing into.
            builderContainer.addEventListener("input", function (e) {
                syncFieldToState(e.target);
            });

            builderContainer.addEventListener("change", function (e) {
                syncFieldToState(e.target);
            });

        }

        render();
    }

    function getTotalManDays() {
        return state.reduce(function (sum, feature) {
            return sum + feature.subTasks.reduce(function (subTotal, subTask) {
                const value = parseFloat(subTask.estimatedManDays);
                return subTotal + (isNaN(value) ? 0 : value);
            }, 0);
        }, 0);
    }

    // Checks every subtask's dates fall within the overall
    // request's Start Date / Due Date window. Plain string
    // comparison works here since dates are yyyy-mm-dd (ISO),
    // which sorts identically to chronological order.
    function getLatestSubtaskDueDate() {
        let latest = null;

        state.forEach(function (feature) {
            feature.subTasks.forEach(function (subTask) {
                if (!subTask.dueDate) {
                    return;
                }

                if (!latest || subTask.dueDate > latest) {
                    latest = subTask.dueDate;
                }
            });
        });

        return latest;
    }

    function getDateRangeViolations(requestStart, requestDue) {

        const violations = [];
        const today = getTodayString();

        state.forEach(function (feature) {

            feature.subTasks.forEach(function (subTask) {

                const label = subTask.title && subTask.title.trim() !== ""
                    ? subTask.title
                    : "Untitled subtask";

                if (subTask.startDate && subTask.startDate < today) {
                    violations.push(
                        `"${label}" cannot start before today.`);
                }

                if (subTask.dueDate && subTask.dueDate < today) {
                    violations.push(
                        `"${label}" cannot be due before today.`);
                }

                if (!subTask.startDate || !subTask.dueDate) {
                    violations.push(
                        `"${label}" must have both a start date and a due date.`);
                }

                if (subTask.startDate &&
                    subTask.dueDate &&
                    subTask.dueDate < subTask.startDate) {
                    violations.push(
                        `"${label}" cannot have a due date earlier than its start date.`);
                }

                if (requestStart && subTask.startDate && subTask.startDate < requestStart) {
                    violations.push(
                        `"${label}" cannot start before the request's Start Date (${requestStart}).`);
                }

                if (requestDue && subTask.dueDate && subTask.dueDate > requestDue) {
                    violations.push(
                        `"${label}" cannot be due after the request's Completion Date (${requestDue}).`);
                }

            });

        });

        return violations;
    }

    const api = {
        init: init,
        syncBeforeSubmit: syncBeforeSubmit,
        getTotalManDays: getTotalManDays,
        getLatestSubtaskDueDate: getLatestSubtaskDueDate,
        getDateRangeViolations: getDateRangeViolations
    };

    // Expose the builder on window because request-validation.js
    // checks window.RequestFeaturesBuilder before synchronizing
    // FeaturesJson on submit. A top-level `const` is not a window
    // property in a classic script.
    window.RequestFeaturesBuilder = api;

    return api;

})();
