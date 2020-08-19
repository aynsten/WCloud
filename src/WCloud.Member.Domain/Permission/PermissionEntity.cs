using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain.Permission
{
    public enum PermissionSide : int
    {
        Both = 0,
        Admin = 1,
        Tanant = 2
    }

    public class PermissionEntity : TreeEntityBase, IMemberShipDBTable
    {
        public string PermissionName { get; set; }

        public string PermissionKey { get; set; }

        public string Desc { get; set; }

        public int Side { get; set; } = (int)PermissionSide.Both;
    }

}
