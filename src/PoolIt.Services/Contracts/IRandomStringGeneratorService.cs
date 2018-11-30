namespace PoolIt.Services.Contracts
{
    public interface IRandomStringGeneratorService
    {
        string GenerateRandomString(int length);
    }
}