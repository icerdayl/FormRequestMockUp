namespace RequestForm.Models
{
    public static class CommonFeatures
    {
        public static List<CommonFeature> Templates => new()
        {
            new CommonFeature(
                "User Login / Authentication",
                "As a user, I want to securely log in so that I can access the system according to my role."),

            new CommonFeature(
                "User Registration",
                "As a user, I want to register an account so that I can access the system."),

            new CommonFeature(
                "CRUD / Data Management",
                "As an administrator, I want to manage records so that I can maintain accurate information."),

            new CommonFeature(
                "Search and Filtering",
                "As a user, I want to search and filter records so that I can quickly find information."),

            new CommonFeature(
                "Dashboard",
                "As a user, I want a dashboard so that I can monitor system information."),

            new CommonFeature(
                "File Attachment",
                "As a user, I want to attach files so that I can provide supporting documents."),

            new CommonFeature(
                "Approval Workflow",
                "As an approver, I want to approve or reject a request so that requests follow the organization's approval process."),

            new CommonFeature(
                "Developer Assignment",
                "As a Help Desk user, I want to assign a request to a developer so that work can begin."),

            new CommonFeature(
                "Task Management",
                "As a developer, I want to manage subtasks so that I can complete the requested work systematically."),

            new CommonFeature(
                "Reporting / Completion",
                "As a stakeholder, I want to see completed requests so that I can track finished work.")
        };
    }

    public class CommonFeature
    {
        public string Label { get; }
        public string Description { get; }

        public CommonFeature(string label, string description)
        {
            Label = label;
            Description = description;
        }
    }
}