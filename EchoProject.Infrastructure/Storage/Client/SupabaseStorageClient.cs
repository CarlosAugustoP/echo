using Supabase;

namespace EchoProject.Infrastructure.Storage.Client
{
    public class SupabaseStorageClient(Supabase.Client supabase, string bucketName) : IStorageClient
    {
        private readonly Supabase.Client _supabase = supabase;
        private readonly string _bucketName = bucketName;


        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
    
            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            var data = ms.ToArray();

            await _supabase.Storage
                .From(_bucketName)
                .Upload(data, fileName);

            return _supabase.Storage
                .From(_bucketName)
                .GetPublicUrl(fileName);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            await _supabase.Storage
                .From(_bucketName)
                .Remove([fileName]);
        }
    }
}