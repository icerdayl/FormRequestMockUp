const RequestFeaturesBuilder = (function () {

    let state = [];
    let developers = [];
    let builderContainer = null;
    let hiddenInput = null;

    function escapeHtml(str) {
        return (str === null || str === undefined ? "" : String(str))
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function renderDeveloperOptions(selected) {
        let html = '<option value="">Unassigned</option>';

        developers.forEach(function (dev) {
            const isSelected = dev === selected ? "selected" : "";
            html += `<option value="${escapeHtml(dev)}" ${isSelected}>${escapeHtml(dev)}</option>`;
        });

        return html;
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
                return `
                    <tr>
                        <td style="min-width:160px;">
                            <input type="text" class="form-control form-control-sm subtask-title"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   value="${escapeHtml(subTask.title)}" />
                        </td>
                        <td style="min-width:140px;">
                            <select class="form-select form-select-sm subtask-assigned"
                                    data-feature-index="${fIndex}" data-subtask-index="${sIndex}">
                                ${renderDeveloperOptions(subTask.assignedTo)}
                            </select>
                        </td>
                        <td>
                            <input type="date" class="form-control form-control-sm subtask-start"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   value="${subTask.startDate || ""}" />
                        </td>
                        <td>
                            <input type="date" class="form-control form-control-sm subtask-due"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   value="${subTask.dueDate || ""}" />
                        </td>
                        <td style="width:90px;">
                            <input type="number" step="0.5" min="0" class="form-control form-control-sm subtask-mandays"
                                   data-feature-index="${fIndex}" data-subtask-index="${sIndex}"
                                   value="${subTask.estimatedManDays === null || subTask.estimatedManDays === undefined ? "" : subTask.estimatedManDays}" />
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

                        <div class="row mb-2">
                            <div class="col-md-8">
                                <label class="form-label small">Acceptance Criteria</label>
                                <input type="text" class="form-control form-control-sm feature-acceptance"
                                       data-feature-index="${fIndex}" value="${escapeHtml(feature.acceptanceCriteria)}" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label small">Priority</label>
                                <select class="form-select form-select-sm feature-priority" data-feature-index="${fIndex}">
                                    <option ${feature.priority === "Low" ? "selected" : ""}>Low</option>
                                    <option ${feature.priority === "Medium" ? "selected" : ""}>Medium</option>
                                    <option ${feature.priority === "High" ? "selected" : ""}>High</option>
                                </select>
                            </div>
                        </div>

                        <label class="form-label small fw-bold mt-2">Subtasks</label>

                        <div class="table-responsive">
                            <table class="table table-sm align-middle mb-2">
                                <thead>
                                    <tr>
                                        <th>Title</th>
                                        <th>Assigned To</th>
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
    }

    function addFeature(title, description, subTaskTitles) {
        state.push({
            title: title || "",
            description: description || "",
            acceptanceCriteria: "",
            priority: "Medium",
            subTasks: (subTaskTitles || []).map(function (t) {
                return { title: t, assignedTo: "", startDate: "", dueDate: "", estimatedManDays: null };
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
                    acceptanceCriteria: f.acceptanceCriteria,
                    priority: f.priority,
                    subTasks: f.subTasks
                        .filter(function (s) { return s.title && s.title.trim() !== ""; })
                        .map(function (s) {
                            return {
                                title: s.title,
                                assignedTo: s.assignedTo || null,
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
            else if (target.classList.contains("subtask-assigned")) subTask.assignedTo = target.value;
            else if (target.classList.contains("subtask-start")) subTask.startDate = target.value;
            else if (target.classList.contains("subtask-due")) subTask.dueDate = target.value;
            else if (target.classList.contains("subtask-mandays")) subTask.estimatedManDays = target.value;

        } else {

            if (target.classList.contains("feature-title")) feature.title = target.value;
            else if (target.classList.contains("feature-description")) feature.description = target.value;
            else if (target.classList.contains("feature-acceptance")) feature.acceptanceCriteria = target.value;
            else if (target.classList.contains("feature-priority")) feature.priority = target.value;

        }
    }

    function init(options) {

        const ticketTypeSelect = document.getElementById(options.ticketTypeSelectId);
        builderContainer = document.getElementById(options.builderContainerId);
        const addFeatureButton = document.getElementById(options.addFeatureButtonId);
        hiddenInput = document.getElementById(options.hiddenInputId);
        const templates = options.templates || {};
        developers = options.developers || [];

        let templateAppliedForType = null;

        if (ticketTypeSelect) {

            ticketTypeSelect.addEventListener("change", function () {

                const selectedOption = this.options[this.selectedIndex];
                const ticketTypeName = selectedOption ? selectedOption.dataset.name : null;

                if (!ticketTypeName) {
                    return;
                }

                // Only auto-fill once per ticket type per visit, so
                // switching back and forth doesn't keep duplicating
                // suggested features the user may have already
                // edited or removed.
                if (templateAppliedForType === ticketTypeName) {
                    return;
                }

                templateAppliedForType = ticketTypeName;

                applyTemplate(ticketTypeName, templates);
                render();

            });
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
                        title: "", assignedTo: "", startDate: "", dueDate: "", estimatedManDays: null
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

    return { init: init, syncBeforeSubmit: syncBeforeSubmit };

})();
