namespace EchoProject.Infrastructure.Storage.Client
{
    public class SupabaseStorageClient(Supabase.Client supabase, string bucketName) : IStorageClient
    {
        private readonly Supabase.Client _supabase = supabase;
        private readonly string _bucketName = bucketName;

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            var extension = GetFileTypeByStream(fileStream);
            
            var fullPath = $"{fileName}.{extension}";

            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            var data = ms.ToArray();

            await _supabase.Storage
                .From(_bucketName)
                .Upload(data, fullPath, new Supabase.Storage.FileOptions { Upsert = true });

            return _supabase.Storage
                .From(_bucketName)
                .GetPublicUrl(fullPath);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            await _supabase.Storage
                .From(_bucketName)
                .Remove([fileName]);
        }

        private static string GetFileTypeByStream(Stream st)
        {
            try 
            {
                byte[] buffer = new byte[8];
                long originalPosition = st.Position;
                st.Read(buffer, 0, 8);
                st.Position = originalPosition; 

                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                    return "png";

                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                    return "jpeg";

                throw new ArgumentException("Invalid format. Only PNG and JPEG are allowed.");
            }
            catch (Exception)
            {
                throw new ArgumentException("Error occurred while reading the file.");
            }

        }
    }
}