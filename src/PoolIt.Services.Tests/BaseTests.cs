namespace PoolIt.Services.Tests
{
    using Utils;

    public abstract class BaseTests
    {
        protected BaseTests()
        {
            TestAutoMapperInitializer.InitializeAutoMapper();
        }
    }
}
