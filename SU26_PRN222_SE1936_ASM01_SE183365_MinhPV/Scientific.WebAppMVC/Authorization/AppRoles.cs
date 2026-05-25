namespace Scientific.WebAppMVC.Authorization
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Researcher = "Researcher";
        public const string Lecturer = "Lecturer";
        public const string Student = "Student";
        public const string LecturerStudent = "LecturerStudent";

        public const string AcademicUsers = Admin + "," + Researcher + "," + Lecturer + "," + Student + "," + LecturerStudent;
        public const string DataManagers = Admin + "," + Researcher;
    }
}
