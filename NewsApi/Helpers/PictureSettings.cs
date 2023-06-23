namespace News.Helpers
{
    public static class PictureSettings
    {
        public static string UploadFile(IFormFile formFile, string folderName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);

            string fileName = $"{Guid.NewGuid()}_{formFile.FileName}";

            string filePath = Path.Combine(folderPath, fileName);

            var fileStream = new FileStream(filePath, FileMode.Create);
            formFile.CopyTo(fileStream);

            return Path.Combine("images", folderName, fileName);
        }
        public static bool DeleteFile( string fileName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
