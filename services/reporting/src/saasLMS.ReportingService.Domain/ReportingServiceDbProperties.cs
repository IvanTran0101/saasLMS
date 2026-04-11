namespace saasLMS.ReportingService;

public static class ReportingServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "ReportingService";
}
