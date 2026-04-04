using Volo.Abp.Modularity;

namespace saasLMS.NotificationService.Samples;

public abstract class SampleAppService_Tests<TStartupModule> : NotificationServiceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    //private readonly ISampleAppService _sampleAppService;

    protected SampleAppService_Tests()
    {
        //_sampleAppService = GetRequiredService<ISampleAppService>();
    }

    // [Fact]
    // public async Task Method1Async()
    // {
    //
    // }
}
