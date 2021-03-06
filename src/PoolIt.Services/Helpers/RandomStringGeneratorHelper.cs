namespace PoolIt.Services.Helpers
{
    using System;
    using Contracts;

    public class RandomStringGeneratorHelper : IRandomStringGeneratorHelper
    {
        private Random random;

        public string GenerateRandomString(int length)
        {
            if (this.random == null)
            {
                this.random = new Random();
            }

            var arr = new char[length];

            for (int i = 0; i < length; i++)
            {
                arr[i] = (char)('a' + this.random.Next(26));
            }

            return new string(arr);
        }
    }
}