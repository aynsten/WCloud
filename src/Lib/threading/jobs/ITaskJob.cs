using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lib.threading.jobs
{
    /// <summary>
    /// 为了方便持久化，运行数据应该尽量避免使用复杂类型
    /// </summary>
    public interface ITaskJob<T>
    {
        string Name { get; }
        string Group { get; }
        bool AutoStart { get; }
        string Cron { get; }
        Task Execute(T data);
    }

    public interface ITaskJob:ITaskJob<IDictionary<string, object>>
    {
        // 
    }
}
