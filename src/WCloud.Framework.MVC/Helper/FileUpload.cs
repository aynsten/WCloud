using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Lib.io;
using Microsoft.AspNetCore.Http;

namespace WCloud.Framework.MVC.Helper
{
    /// <summary>
    /// 用来上传文件的类
    /// authour:WJ
    /// </summary>
    public class FileUpload
    {
        async Task<UploadFileModel> __prepare_file__(string file_name, string save_path)
        {
            var extension = Path.GetExtension(file_name) ?? string.Empty;
            var now = DateTime.UtcNow;

            var model = new UploadFileModel(save_path)
            {
                SubPaths = new[] { now.Year, now.Month, now.Day, now.Hour }.Select(x => x.ToString()).ToArray()
            };

            new DirectoryInfo(model.GetSavePath()).CreateIfNotExist();

            var i = 0;
            while (++i < 10)
            {
                model.SavedFileNameWithExtension = $"{Com.GetUUID()}.{extension.Trim('.')}";
                var path = model.GetSavePath();
                if (!File.Exists(path))
                {
                    return model;
                }
            }

            await Task.CompletedTask;
            throw new TimeoutException("过多尝试");
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="http_file"></param>
        /// <param name="save_path"></param>
        /// <returns></returns>
        public async Task<UploadFileModel> UploadSingleFile(IFormFile http_file, string save_path)
        {
            http_file.Should().NotBeNull();
            save_path.Should().NotBeNullOrEmpty();

            var bs = http_file.GetAllBytes();
            var file_name = http_file.FileName;

            var res = await this.UploadSingleFile(bs, file_name, save_path);

            return res;
        }

        public async Task<UploadFileModel> UploadSingleFile(byte[] bs, string file_name, string save_path)
        {
            bs.Should().NotBeNullOrEmpty();
            file_name.Should().NotBeNullOrEmpty();
            save_path.Should().NotBeNullOrEmpty();

            //获取一个新的文件模型
            var model = await __prepare_file__(file_name, save_path);

            var final_save_path = model.GetSavePath();

            using (var fs = new FileStream(final_save_path, FileMode.Create))
            {
                await fs.WriteAsync(bs, 0, bs.Length);
                await fs.FlushAsync();
            }

            return model;
        }
    }

    /// <summary>
    /// 上传结果
    /// </summary>
    public class UploadFileModel
    {
        public UploadFileModel(string save_path)
        {
            this.SavePath = save_path;
        }

        public string SavePath { get; set; }
        public string[] SubPaths { get; set; }
        public string SavedFileNameWithExtension { get; set; }

        public string GetSavePath()
        {
            var paths = new List<string>()
                .AddItem_(this.SavePath)
                .AddList_(this.SubPaths)
                .AddItem_(this.SavedFileNameWithExtension)
                .Where(x => x?.Length > 0).ToArray();

            var final_save_path = paths.Aggregate((x, y) => Path.Combine(x, y));

            return final_save_path;
        }


        public string GetUrl(string host_prefix)
        {
            var paths = new List<string>()
                .AddItem_(host_prefix)
                .AddList_(this.SubPaths)
                .AddItem_(this.SavedFileNameWithExtension)
                .Where(x => x?.Length > 0)
                .Select(x => x.Trim('/').Trim('\\')).ToArray();

            var url = paths.Aggregate((x, y) => $"{x}/{y}");

            return url;
        }
    }
}

