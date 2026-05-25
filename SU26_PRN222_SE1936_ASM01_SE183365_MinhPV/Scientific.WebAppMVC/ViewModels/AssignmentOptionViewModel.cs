namespace Scientific.WebAppMVC.ViewModels
{
    public class AssignmentOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
    }
}
