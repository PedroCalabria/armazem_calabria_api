using ArmazemCalabria.CrossCutting;

namespace ArmazemCalabria.CrossCutting.Extensions
{
    public static class UserContextExtensions
    {
        public static void AddData<TValue>(this IUserContext userContext, string key, TValue data)
        {
            userContext.CreateKeyIfNotExists<TValue>(key);

            userContext.AdditionalData[key] = data;
        }

        private static void CreateKeyIfNotExists<TValue>(this IUserContext userContext, string key)
        {
            userContext.AdditionalData ??= [];

            if (!userContext.AdditionalData.ContainsKey(key))
            {
                userContext.AdditionalData.Add(key, default(TValue));
            }
        }
    }
}