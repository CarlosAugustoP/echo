namespace EchoProject.Infrastructure.Storage.Client
{
    public interface IStorageClient
    {
        Task<string> UploadFileAsync(string fileName, Stream fileStream);
        Task DeleteFileAsync(string fileName);
    }
}