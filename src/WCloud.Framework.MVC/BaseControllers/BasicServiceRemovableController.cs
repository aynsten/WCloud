using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.Framework.MVC.BaseController
{
    public abstract class BasicServiceRemovableController<ServiceType, EntityType> : BasicServiceController<ServiceType, EntityType>
        where ServiceType : IBasicServiceRemovable<EntityType>
        where EntityType : BaseEntity, ILogicalDeletion
    {
        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> Remove([FromForm]string uid)
        {
            await this._service.RemoveByUIDs(uid);

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> Recover([FromForm]string uid)
        {
            await this._service.RecoverByUIDs(uid);

            return SuccessJson();
        }
    }
}
