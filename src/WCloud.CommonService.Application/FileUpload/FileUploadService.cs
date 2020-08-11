using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using WCloud.Core.Storage;
using WCloud.Framework.Database.Abstractions.Extension;

namespace WCloud.CommonService.Application.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ILogger _logger;
        private readonly ICommonServiceRepository<FileUploadEntity> _uploadRepo;
        private readonly ICommonServiceRepository<FileOwnerEntity> _fileOwnerRepo;
        private readonly IUploadHelper _uploadHelper;

        public FileUploadService(
            ILogger<FileUploadService> _logger,
            ICommonServiceRepository<FileUploadEntity> _uploadRepo,
            ICommonServiceRepository<FileOwnerEntity> _fileOwnerRepo,
            IUploadHelper uploadHelper)
        {
            this._logger = _logger;
            this._uploadRepo = _uploadRepo;
            this._fileOwnerRepo = _fileOwnerRepo;
            this._uploadHelper = uploadHelper;
        }

        string __extension__(string file_name)
        {
            try
            {
                var res = Path.HasExtension(file_name) ?
                    Path.GetExtension(file_name) :
                    string.Empty;

                return res.StartsWith(".") ?
                    res :
                    $".{res}";
            }
            catch
            {
                return string.Empty;
            }
        }

        string __file_hash__(byte[] bs)
        {
            var res = SecureHelper.GetMD5(bs);
            return res;
        }

        FileUploadEntity __map__(string file_name, byte[] bs, string catalog)
        {
            var res = new FileUploadEntity()
            {
                OriginFileName = file_name,
                FileSize = bs.Length,
                Extension = this.__extension__(file_name),
                FileHash = this.__file_hash__(bs),
                StorageProvider = this._uploadHelper.StorageProvider ?? "shared-qiniu",
                Catalog = catalog ?? string.Empty
            };

            var QiniuKey = res.QiniuKey;
            res.Url = this._uploadHelper.ConcatUrl(QiniuKey);
            res.ExtraData = new
            {
                debug_info = this._uploadHelper.DebugInfo(),
                QiniuKey
            }.ToJson();

            return res;
        }

        /// <summary>
        /// 七牛错误码
        /// https://developer.qiniu.com/kodo/api/3928/error-responses
        /// </summary>
        /// <param name="qiniu_key"></param>
        /// <returns></returns>
        async Task<bool> __file_exist_in_qiniu__(string qiniu_key)
        {
            var res = await this._uploadHelper.KeyExists(qiniu_key);

            return res;
        }

        async Task __upload_to_qiniu__(FileUploadEntity model, byte[] bs)
        {
            var qiniu_key = model.QiniuKey;

            if (await this.__file_exist_in_qiniu__(qiniu_key))
            {
                return;
            }
            var res = await this._uploadHelper.Upload(bs, qiniu_key);
            res.Should().NotBeNullOrEmpty();
        }

        public async Task<_<FileUploadEntity>> Upload(byte[] bs, string file_name, string catalog = null)
        {
            bs.Should().NotBeNullOrEmpty("upload bytes");
            file_name.Should().NotBeNullOrEmpty();

            var res = new _<FileUploadEntity>();

            var model = this.__map__(file_name, bs, catalog);
            //如果之前有人上传过就直接返回
            var previous_uploaded = await this._uploadRepo.QueryOneAsNoTrackAsync(x =>
            x.FileHash == model.FileHash &&
            x.Catalog == model.Catalog &&
            x.StorageProvider == model.StorageProvider);

            if (previous_uploaded != null)
            {
                if (await this.__file_exist_in_qiniu__(previous_uploaded.QiniuKey))
                {
                    this._logger.LogInformation($"文件{previous_uploaded.ToJson()}已经在服务器存在，直接返回数据");
                    return res.SetSuccessData(previous_uploaded);
                }
                else
                {
                    this._logger.LogWarning($"文件{previous_uploaded.ToJson()}已经在服务器存在，但是不存在于七牛，将删除记录并重新上传");
                    await this._uploadRepo.DeleteByIds(new string[] { previous_uploaded.Id });
                }
            }
            //上传到七牛
            await this.__upload_to_qiniu__(model, bs);
            //保存到数据库
            model.InitSelf();
            await this._uploadRepo.InsertAsync(model);

            return res.SetSuccessData(model);
        }
    }
}
