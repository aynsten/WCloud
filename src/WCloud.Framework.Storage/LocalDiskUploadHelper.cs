using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Storage;
using WCloud.Framework.IO;

namespace WCloud.Framework.Storage.Qiniu_
{
    public class LocalDiskUploadHelper : IUploadHelper
    {
        private readonly string save_path;
        private readonly string static_file_server;

        public string StorageProvider => "local-disk";

        public LocalDiskUploadHelper(IConfiguration config)
        {
            this.save_path = config["static_save_path"];
            this.static_file_server = config["static_file_server"];
            this.save_path.Should().NotBeNullOrEmpty();
            this.static_file_server.Should().NotBeNullOrEmpty();

            new DirectoryInfo(this.save_path).CreateIfNotExist();
        }

        (string dir, string file, string[] url_path) __key_as_path__(string key)
        {
            var paths = key.Split('\\', '/').Select(x => x?.Trim()).WhereNotEmpty().ToArray();

            var file_name = paths[paths.Length - 1];

            var dirs = new List<string>() { this.save_path };
            if (paths.Length > 1)
            {
                var sub_path = paths.Take(paths.Length - 1).ToArray();
                dirs.AddList_(sub_path);
            }

            var dir_path = Path.Combine(dirs.ToArray());
            var file_path = Path.Combine(dir_path, file_name);

            return (dir_path, file_path, paths);
        }

        public async Task Delete(string key)
        {
            key.Should().NotBeNullOrEmpty();

            await Task.CompletedTask;

            var (dir, file, url_paths) = this.__key_as_path__(key);

            new FileInfo(file).DeleteIfExist();
        }

        public async Task<bool> KeyExists(string key)
        {
            key.Should().NotBeNullOrEmpty();

            await Task.CompletedTask;

            var (dir, file, url_paths) = this.__key_as_path__(key);

            return File.Exists(file);
        }

        public async Task<string> Upload(byte[] bs, string key)
        {
            key.Should().NotBeNullOrEmpty();

            await Task.CompletedTask;

            var (dir, file, url_paths) = this.__key_as_path__(key);

            if (!File.Exists(file))
            {
                new DirectoryInfo(dir).CreateIfNotExist();
                await File.WriteAllBytesAsync(file, bs);
            }

            return this.ConcatUrl(key);
        }

        public string ConcatUrl(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var (dir, file, url_paths) = this.__key_as_path__(key);

            var url_file_path = string.Join('/', url_paths);

            var res = $"{this.static_file_server.TrimEnd('\\', '/')}/{url_file_path}";

            return res;
        }

        public string DebugInfo()
        {
            return new
            {
                static_file_server,
                save_path
            }.ToJson();
        }
    }
}
