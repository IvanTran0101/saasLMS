namespace saasLMS.AssessmentService;

public static class AssessmentServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "AssessmentService";
}
