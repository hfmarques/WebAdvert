﻿using System.IO;
using System.Threading.Tasks;

namespace Web.Services;

public interface IFileUploader
{
    Task<bool> UploadFileAsync(string fileName, Stream storageStream);
}