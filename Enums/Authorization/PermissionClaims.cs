namespace Enums.Authorization
{
    public static class PermissionClaims
    {
        public const string Permission = "Permission";

        public const string ReadUsers = "ReadUsers";
        public const string ManageUsers = "ManageUsers";
        public const string DeleteUsers = "DeleteUsers";

        public const string ReadRoles = "ReadRoles";
        public const string ManageRoles = "ManageRoles";
        public const string DeleteRoles = "DeleteRoles";

        public const string ReadBooks = "ReadBooks";
        public const string ManageBooks = "ManageBooks";
        public const string DeleteBooks = "DeleteBooks";


        public static readonly List<string> claimList = new List<string>
    {
        # region User claims
        ReadBooks, ManageBooks, DeleteBooks,
        #endregion
        
        # region Role claims
        ReadRoles, ManageRoles, DeleteRoles,
        #endregion

        # region Book claims
        ReadUsers, ManageUsers, DeleteUsers
        #endregion        
    };
    }

}
