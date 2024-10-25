
using Hutech.Exam.Server.BUS;
using Hutech.Exam.Server.Configurations;
using Hutech.Exam.Server.DAL.Repositories;
using StackExchange.Redis;

namespace Hutech.Exam.Server.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguarion = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguarion);

            services.AddSingleton(redisConfiguarion);

            if (!redisConfiguarion.Enabled)
                return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguarion.ConnectionString));
            services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguarion.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
