using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Common
{
    public interface IStorageService
    {
        public string GetFileUrl(string filename);
        public Task SaveFileAsync(Stream mediaBinaryStream, string fileName);
        public Task DeleteFileAsync(string fileName);
    }
}
