

namespace Books.Utilities
{
    public static class Roles
    {
        public enum RoleType
        {
            Admin,
            Employee,
            Individual,
            Company
        }

        public static List<RoleType> GetAllRoles { get; set; } = new List<RoleType>
        {
             RoleType.Admin,
             RoleType.Employee,
             RoleType.Individual,
             RoleType.Company
        };
    }
}
