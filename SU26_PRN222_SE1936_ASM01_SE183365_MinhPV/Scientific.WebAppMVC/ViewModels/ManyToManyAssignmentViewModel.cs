namespace Scientific.WebAppMVC.ViewModels
{
    public class ManyToManyAssignmentViewModel
    {
        public int EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string PageTitle { get; set; } = string.Empty;
        public string OptionLabel { get; set; } = string.Empty;
        public string BackController { get; set; } = string.Empty;
        public string BackAction { get; set; } = "Details";
        public List<int> SelectedIds { get; set; } = new();
        public List<AssignmentOptionViewModel> Options { get; set; } = new();
        public bool RequireAtLeastOne { get; set; }
    }
}
