using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Service
{
    public interface IDeptService : IAutoRegistered
    {
        Task<List<DepartmentEntity>> Query(string group = null);

        Task<_<DepartmentEntity>> Add(DepartmentEntity model);

        Task<_<DepartmentEntity>> Update(DepartmentEntity model);

        Task<bool> DeleteWhenNoChildren(string uid);
    }
}
