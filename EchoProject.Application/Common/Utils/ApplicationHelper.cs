namespace EchoProject.Application.Common.Utils
{
    public static class ApplicationHelper
    {
        public static bool IsAValidBase64String(string base64String)
        {
        
            if (string.IsNullOrEmpty(base64String)) return true;

            var base64Data = base64String.Contains(',') ? base64String.Split(',')[1] : base64String;

            try
            {
                Convert.FromBase64String(base64Data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}