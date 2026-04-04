namespace saasLMS.CourseCatalogService;

public static class CourseCatalogServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "CourseCatalogService";
}
