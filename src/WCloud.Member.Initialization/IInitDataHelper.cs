namespace WCloud.Member.Initialization
{
    public interface IInitDataHelper
    {
        void CreateAdminRole();
        void CreateAdmin();
        void SetAdminRoleForAdmin();
    }
}
