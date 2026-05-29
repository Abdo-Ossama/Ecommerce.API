namespace Ecommerce.API.Helpers
{
    public interface IFileHandler
    {
        /// <summary>
        /// Creates a file from the provided IFormFile and returns the file path. The file type can be specified to determine the storage location and naming convention.
        /// </summary>
        /// <param name="Img">The IFormFile to be saved.</param>
        /// <param name="fileType">The type of file to determine the storage location and naming convention.</param>
        /// <returns>The path of the created file.</returns>
        Task<string> CreateFileAsync(IFormFile Img, FileType fileType = FileType.ProductMainImg);

        /// <summary>
        /// Gets the full file path for a specified old file name and file type.
        /// </summary>
        /// <param name="oldFileName">The name of the old file for which to retrieve the path. Cannot be null or empty.</param>
        /// <param name="fileType">The type of file to locate. Defaults to FileType.ProductMainImg if not specified.</param>
        /// <returns>A string containing the full path to the specified old file. Returns null if the file does not exist.</returns>
        string GetOldFilePath(string oldFileName, FileType fileType = FileType.ProductMainImg);

        /// <summary>
        /// Delete Old File: Deletes an old file based on the provided file name and file type. This method is used to remove outdated or unnecessary files from the storage. The file type parameter helps determine the specific location and naming convention for the file to be deleted.
        /// </summary>
        /// <param name="oldFileName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        bool DeleteOldFile(string oldFileName, FileType fileType = FileType.ProductMainImg);
    }
}
