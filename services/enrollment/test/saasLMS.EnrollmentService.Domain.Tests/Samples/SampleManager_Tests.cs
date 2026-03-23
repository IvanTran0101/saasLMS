using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService.Samples;

public abstract class SampleManager_Tests<TStartupModule> : EnrollmentServiceDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    //private readonly SampleManager _sampleManager;

    protected SampleManager_Tests()
    {
        //_sampleManager = GetRequiredService<SampleManager>();
    }

    // [Fact]
    // public async Task Method1Async()
    // {
    //
    // }
}
