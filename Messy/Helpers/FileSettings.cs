namespace Messy.Helpers;

public static class FileSettings
{
    private static IConfiguration _configuration;

    public static void initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public static long MaxFileSize => _configuration.GetValue<long>("File:MaxFileSize");
    public static long MaxAssignedFilesSize => _configuration.GetValue<long>("File:MaxAssignedFilesSize");
}