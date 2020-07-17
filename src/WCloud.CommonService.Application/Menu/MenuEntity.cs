using Lib.core;
using Lib.extension;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WCloud.CommonService.Application;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Application.Entity
{
    /// <summary>
    /// 导航/边栏/轮播都属于菜单
    /// </summary>
    [Table("tb_menu")]
    public class MenuEntity : TreeEntityBase, ICommonServiceEntity, IUpdateTime
    {
        private readonly Lazy_<string[]> PermissionValues;

        public MenuEntity()
        {
            string[] ParsePermission()
            {
                var res = this.PermissionJson?.JsonToEntity<string[]>(throwIfException: false);
                return res ?? new string[] { };
            }

            this.PermissionValues = new Lazy_<string[]>(ParsePermission);
        }

        public virtual string NodeName { get; set; }

        public virtual string Description { get; set; }

        public virtual string Url { get; set; }

        public virtual string ImageUrl { get; set; }

        public virtual string IconCls { get; set; }

        public virtual string IconUrl { get; set; }

        public virtual string HtmlId { get; set; }

        public virtual string HtmlCls { get; set; }

        public virtual string HtmlStyle { get; set; }

        public virtual string HtmlOnClick { get; set; }

        public virtual int Sort { get; set; }

        public virtual string PermissionJson { get; set; }

        /// <summary>
        /// 解析json，拿到权限名
        /// </summary>
        [NotMapped]
        public virtual string[] PermissionNames => this.PermissionValues.Value;

        public virtual DateTime? UpdateTimeUtc { get; set; }
    }
}
