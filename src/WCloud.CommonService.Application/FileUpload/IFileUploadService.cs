using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WCloud.CommonService.Application.FileUpload
{
    public interface IFileUploadService : IAutoRegistered
    {
        Task<_<FileUploadEntity>> Upload(byte[] bs, string file_name, string catalog = null);
    }
}
