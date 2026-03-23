namespace saasLMS.LearningProgressService;

public static class LearningProgressServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "LearningProgressService";
}
