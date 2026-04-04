namespace saasLMS.NotificationService;

public static class NotificationServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "NotificationService";
}
