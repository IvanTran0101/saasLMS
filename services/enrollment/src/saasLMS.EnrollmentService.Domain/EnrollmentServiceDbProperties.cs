namespace saasLMS.EnrollmentService;

public static class EnrollmentServiceDbProperties
{
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "EnrollmentService";
}
