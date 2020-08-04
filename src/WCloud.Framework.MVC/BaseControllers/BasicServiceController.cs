using FluentAssertions;
using Lib.core;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.Framework.MVC.BaseController
{
    [Obsolete]
    abstract class BasicServiceController<T> : BasicServiceController<IBasicService<T>, T>
       where T : EntityBase
    {
        protected BasicServiceController(IBasicService<T> service) : base(_service: service) { }
    }

    public abstract class BasicServiceController<ServiceType, EntityType> : WCloudBaseController
        where ServiceType : IBasicService<EntityType>
        where EntityType : EntityBase
    {
        protected readonly _<string> SUCCESS = new _<string>().SetSuccessData(string.Empty);

        protected readonly Lazy_<ServiceType> _lazyService;

        protected BasicServiceController() : this(func: null) { }

        protected BasicServiceController(ServiceType _service) : this(func: () => _service) { }

        protected BasicServiceController(Func<ServiceType> func)
        {
            var res = func ?? (() => this.HttpContext.RequestServices.Resolve_<ServiceType>());

            this._lazyService = new Lazy_<ServiceType>(res);
        }

        protected ServiceType _service
        {
            get
            {
                var res = this._lazyService.Value;

                res.Should().NotBeNull();

                return res;
            }
        }

        protected virtual object __parse__(EntityType x) => x;

        /// <summary>
        /// 根据记录UID获取单条记录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> GetByUID([FromForm]string uid)
        {
            var data = await this._service.GetByUID(uid);

            if (data == null)
                throw new MsgException("数据不存在");

            var res = this.__parse__(data);

            return SuccessJson(res);
        }

        /// <summary>
        /// 获取所有记录，数据量大慎用
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> QueryAll()
        {
            var data = await this._service.QueryTop(5000);

            var res = data.Select(this.__parse__);

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> QueryByMaxID([FromForm]int? max_id, [FromForm]int? count)
        {
            var size = count ?? this.PageSize;
            size.Should().BeLessOrEqualTo(5000);

            var data = await this._service.QueryByMaxID(max_id ?? 0, size);

            var res = data.Select(this.__parse__);

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> QueryByMinID([FromForm]int? min_id, [FromForm]int? count)
        {
            var size = count ?? this.PageSize;
            size.Should().BeLessOrEqualTo(5000);

            var data = await this._service.QueryByMinID(min_id, size);

            var res = data.Select(this.__parse__);

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> Query([FromForm]string q, [FromForm]int? page)
        {
            page = this.CheckPage(page);

            var data = await this._service.Query(q, page.Value, this.PageSize);

            var res = data.PagerDataMapper_(this.__parse__);

            return SuccessJson(res);
        }

        protected virtual _<string> ValidInsertModel(EntityType model) => this.SUCCESS;

        protected virtual _<string> ValidUpdateModel(EntityType model) => this.SUCCESS;

        protected virtual Task<EntityType> PrepareInsertData(EntityType model) => Task.FromResult(model);

        protected virtual Task<EntityType> PrepareUpdateData(EntityType model) => Task.FromResult(model);

        protected Func<bool, Task> AfterSave;

        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> Save([FromForm]string data)
        {
            var model = this.JsonToEntity_<EntityType>(data);

            var save_callback = this.AfterSave ?? (x => Task.CompletedTask);

            if (ValidateHelper.IsNotEmpty(model.UID))
            {
                model = await this.PrepareUpdateData(model);

                var valid = this.ValidUpdateModel(model);
                valid.ThrowIfNotSuccess();

                await this._service.Update(model);

                await save_callback.Invoke(true);
            }
            else
            {
                model = await this.PrepareInsertData(model);

                var valid = this.ValidInsertModel(model);
                valid.ThrowIfNotSuccess();

                await this._service.Add(model);

                await save_callback.Invoke(false);
            }

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public virtual async Task<IActionResult> Delete([FromForm]string uid)
        {
            await this._service.Delete(uid);

            return SuccessJson();
        }
    }
}
