using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WCloud.Framework.Database.Abstractions.Service
{
    public interface IBasicService<T>
    {
        Task<T> GetByUID(string uid);

        Task Add(T data);

        Task Delete(params string[] uids);

        Task<IEnumerable<T>> QueryTop(int count);

        Task<IEnumerable<T>> QueryTop<SortField>(int count, Expression<Func<T, SortField>> orderby, bool desc);

        Task<IEnumerable<T>> QueryByMaxID(int max_id, int count);

        Task<IEnumerable<T>> QueryByMinID(int? min_id, int count);

        Task<PagerData<T>> Query(string q, int page, int pagesize);
        
        Task<PagerData<T>> Query<SortField>(string q, int page, int pagesize,
            Expression<Func<T, SortField>> orderby, bool desc);

        Task Update(T data);
    }
}
