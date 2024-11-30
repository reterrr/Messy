namespace Messy.Helpers;

public static class ConfigAccesser
{
    public static IConfiguration Configuration { get; set; }

    public static void Init(IConfiguration config)
    {
        Configuration = config;
    }
}