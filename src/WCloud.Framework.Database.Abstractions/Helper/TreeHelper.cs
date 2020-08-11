using FluentAssertions;
using Lib.extension;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Extension;

namespace WCloud.Framework.Database.Abstractions.Helper
{
    public static class TreeHelper
    {
        public static IEnumerable<object> BuildAntTreeStructure<T>(IEnumerable<T> list,
            Func<T, string> title_selector,
            Func<T, object> raw_data = null)
            where T : TreeEntityBase
        {
            list.Any(x => ValidateHelper.IsEmpty(x.Id)).Should().BeFalse("每个节点都需要id");
            title_selector.Should().NotBeNull();

            raw_data ??= (x => x);

            var repeat = new List<string>();

            object BindChildren(T node)
            {
                repeat.AddOnceOrThrow(node.Id, "树存在错误");
                var children = list.GetChildrenOf(node).ToArray();
                return new
                {
                    key = node.Id,
                    title = title_selector.Invoke(node),
                    children = children.Select(x => BindChildren(x)),
                    raw_data = raw_data.Invoke(node)
                };
            }

            var data = list.Where(x => x.IsFirstLevel()).ToArray();

            var res = data.Select(x => BindChildren(x)).ToArray();

            return res;
        }
    }
}
