using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eID.PDEAU.Service.FilesManager;

public class FilesManager
{
    private readonly FilesManagerOptions _settings;
    private readonly ILogger<FilesManager> _logger;

    public FilesManager(ILogger<FilesManager> logger, IOptions<FilesManagerOptions> settings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _settings = settings?.Value ?? new FilesManagerOptions();
        _settings.Validate();
    }

    public async Task<string> SaveAsync(string directoryName, string fileName, string fileExtension, Stream fileStream)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
        {
            throw new ArgumentException($"'{nameof(directoryName)}' cannot be null or whitespace.", nameof(directoryName));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
        }

        if (fileStream is null)
        {
            throw new ArgumentNullException(nameof(fileStream));
        }

        string fullPath = string.Empty;
        try
        {
            var directory = Path.Combine(_settings.UploadDirectoryPath, directoryName);
            Directory.CreateDirectory(directory);

            fullPath = Path.GetFullPath(Path.Combine(directory, fileName + fileExtension));

            // Get unique name. Pattern: fileName (n).ext
            var counter = 1;
            while (File.Exists(fullPath))
            {
                fullPath = Path.GetFullPath(Path.Combine(directory, $"{fileName} ({counter}){fileExtension}"));
                counter++;
            }

            using (FileStream fs = File.Create(fullPath))
            {
                await fileStream.CopyToAsync(fs);
            }
            _logger.LogInformation("Successfully saved {FullPath}", fullPath);

            return fullPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during save of {FullPath}", fullPath);
            return string.Empty;
        }
    }

    public FileStream Read(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
        }

        try
        {
            return new FileStream(filePath,
                                  FileMode.Open,
                                  FileAccess.Read,
                                  FileShare.Read,
                                  bufferSize: 64 * 1024, // 64KB buffer
                                  useAsync: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during reading of {FullPath}", filePath);
            throw;
        }
    }

    public static bool FileExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }
        return File.Exists(filePath);
    }

    public IEnumerable<string> GetFolderFileNameList(string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
        {
            throw new ArgumentException($"'{nameof(directoryName)}' cannot be null or whitespace.", nameof(directoryName));
        }

        var directory = Path.Combine(_settings.UploadDirectoryPath, directoryName);
        try
        {
            Directory.CreateDirectory(directory);
            var files = Directory.GetFiles(directory);
            // Return files name only
            return files.Select(f => Path.GetFileName(f));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during get files of {FullPath}", directory);
            throw;
        }
    }

    public void RemoveFiles(IEnumerable<string> fullFilePathList)
    {
        if (fullFilePathList is null)
        {
            throw new ArgumentNullException(nameof(fullFilePathList));
        }

        foreach (var fullFilePath in fullFilePathList)
        {
            if (!File.Exists(fullFilePath))
            {
                _logger.LogWarning("File {FullFilePath} can not be deleted. It does not exist", fullFilePath);
                continue;
            }
            try
            {
                File.Delete(fullFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error {Error} during delete file {FullFilePath} ", ex.Message, fullFilePath);
            }
        }
    }
}
