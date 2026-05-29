namespace Ecommerce.API.Helpers
{
    public enum FileType
    {
        ProductMainImg,
        ProductSubImg,
        ReviewImg
    }

    public class FileHandler : IFileHandler
    {
        public async Task<string> CreateFileAsync(IFormFile Img, FileType fileType = FileType.ProductMainImg)
        {
            var fileName =
                    $"{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}-{Guid.NewGuid().ToString()}{Path.GetExtension(Img.FileName)}";
            // 31290-fjkdsfhsd-32131.png

            var filePath = string.Empty;


            switch(fileType)
            {
                case FileType.ProductMainImg:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", fileName);
                    break;
                case FileType.ProductSubImg:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\SubImgs", fileName);
                    break;
                case FileType.ReviewImg:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\reviews", fileName);
                    break;
            }

            using (var stream = System.IO.File.Create(filePath))
            {
                await Img.CopyToAsync(stream);
            }

            return fileName;
        }

        public string GetOldFilePath(string oldFileName, FileType fileType = FileType.ProductMainImg)
        {
            var filePath = string.Empty;

            switch(fileType)
            {
                case FileType.ProductMainImg:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", oldFileName);
                    break;
                case FileType.ProductSubImg:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\SubImgs", oldFileName);
                    break;
                case FileType.ReviewImg:
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\reviews", oldFileName);
                    break;
            }

            return filePath;
        }

        public bool DeleteOldFile(string oldFileName, FileType fileType = FileType.ProductMainImg)
        {
            var oldFilePath = GetOldFilePath(oldFileName, fileType);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
                return true;
            }
            return false;
        }
    }
}
