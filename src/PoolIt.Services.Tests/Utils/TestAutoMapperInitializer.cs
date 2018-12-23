namespace PoolIt.Services.Tests.Utils
{
    using AutoMapper;
    using Infrastructure.Mapping;

    public static class TestAutoMapperInitializer
    {
        private static bool isInitialized;

        public static void InitializeAutoMapper()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        }
    }
}
