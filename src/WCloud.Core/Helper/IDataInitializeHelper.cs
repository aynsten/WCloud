using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WCloud.Core.Helper
{
    public interface IDataInitializeHelper : Lib.ioc.IAutoRegistered
    {
        Task CreateDatabase();

        Task InitSeedData();
    }
}
