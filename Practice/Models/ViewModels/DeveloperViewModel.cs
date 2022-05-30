namespace Practice.Models.ViewModels
{
    public class DeveloperViewModel
    {
        public Dictionary<int, string> projects;
        public Dictionary<string, string> selectedProject;
        public List<Dictionary<string, string>> tasks;

        public DeveloperViewModel() { }
    }
}
