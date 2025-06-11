namespace MedishcMVCProject.Utilities.Extensions
{
    public static class FileValidator
    {
        public static bool ValidateType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
        public static bool ValidateSize(this IFormFile file, FileType fileSize, int size)
        {
            switch (fileSize)
            {
                case FileType.KB:
                    return file.Length <= size * 1024;
                case FileType.MB:
                    return file.Length <= size * 1024 * 1024;
                case FileType.GB:
                    return file.Length <= size * 1024 * 1024 * 1024;
            }
            return false;
        }

        public static async Task<string> CreateFileAsync(this IFormFile file, params string[] roots)
        {
            string fileName = string.Concat(Guid.NewGuid().ToString(), file.FileName.Substring(file.FileName.LastIndexOf(".")));
            string path = _getPath(roots);
            path = Path.Combine(path, fileName);


            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);

            }
            return fileName;

        }

        public static void DeleteFile(this string fileName, params string[] roots)
        {
            string path = _getPath(roots);
            path = Path.Combine(path, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static string _getPath(params string[] roots)
        {
            string path = string.Empty;

            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }
            return path;
        }


    }
}
