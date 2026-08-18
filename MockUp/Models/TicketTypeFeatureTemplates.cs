namespace RequestForm.Models
{
    // Keyed by TicketType.TicketTypeName (must match the seeded
    // values in ApplicationDbContext exactly).
    public static class TicketTypeFeatureTemplates
    {
        public static Dictionary<string, List<FeatureTemplate>> Templates => new()
        {
            ["Bug"] = new List<FeatureTemplate>
            {
                new FeatureTemplate(
                    "Bug Investigation & Fix",
                    "As a developer, I want to investigate and resolve the reported bug so that the system behaves correctly.",
                    new List<string>
                    {
                        "Reproduce the issue",
                        "Identify root cause",
                        "Implement fix",
                        "Test the fix"
                    })
            },

            ["Feature Request"] = new List<FeatureTemplate>
            {
                new FeatureTemplate(
                    "New Feature Implementation",
                    "As a user, I want the new feature implemented so that I can use the additional functionality.",
                    new List<string>
                    {
                        "Design UI/UX",
                        "Implement backend logic",
                        "Implement frontend",
                        "Test the feature"
                    })
            },

            ["Enhancement"] = new List<FeatureTemplate>
            {
                new FeatureTemplate(
                    "Enhancement Implementation",
                    "As a user, I want the existing functionality enhanced so that it better meets my needs.",
                    new List<string>
                    {
                        "Review current implementation",
                        "Implement enhancement",
                        "Test enhancement"
                    })
            },

            ["Maintenance"] = new List<FeatureTemplate>
            {
                new FeatureTemplate(
                    "System Maintenance",
                    "As an administrator, I want routine maintenance performed so that the system remains stable and secure.",
                    new List<string>
                    {
                        "Review system health",
                        "Apply updates/patches",
                        "Verify system stability"
                    })
            },

            ["Technical Support"] = new List<FeatureTemplate>
            {
                new FeatureTemplate(
                    "Technical Support Resolution",
                    "As a user, I want technical support so that my issue is resolved.",
                    new List<string>
                    {
                        "Diagnose issue",
                        "Provide resolution/workaround",
                        "Confirm resolution with user"
                    })
            },

            ["Change Request"] = new List<FeatureTemplate>
            {
                new FeatureTemplate(
                    "Change Implementation",
                    "As a stakeholder, I want the requested change implemented so that business requirements are met.",
                    new List<string>
                    {
                        "Analyze impact of change",
                        "Implement change",
                        "Test change"
                    })
            }
        };
    }

    public class FeatureTemplate
    {
        public string Title { get; }
        public string Description { get; }
        public List<string> SubTaskTitles { get; }

        public FeatureTemplate(string title, string description, List<string> subTaskTitles)
        {
            Title = title;
            Description = description;
            SubTaskTitles = subTaskTitles;
        }
    }
}
