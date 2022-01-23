

namespace Books.Utilities
{
    public static class Roles
    {
        public enum RoleType
        {
            Admin,
            Manager,
            Employee,
            User
        }

        public static List<RoleType> GetAllRoles { get; set; } = new List<RoleType>
        {
             RoleType.Admin,
             RoleType.Manager,
             RoleType.Employee ,
             RoleType.User
        };
    }
}
