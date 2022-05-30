namespace Practice.Models.ViewModels
{
    public class ProjectManagerViewModel
    {
        public Dictionary<int, string> projects;
        public Dictionary<string, string> selectedProject;
        public List<Dictionary<string, string>> employees;

        public ProjectManagerViewModel() { }
    }
}
