using trabalho2.Repositories;
using trabalho2.Services;

namespace trabalho2.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services) // pra nao ter que fazer isso no Program.cs, e deixar o Program.cs mais limpo
        {
            // repositories
            services.AddScoped<UserRepository>();
            services.AddScoped<UserLogRepository>();

            // services
            services.AddScoped<UserService>();
            services.AddScoped<UserLogService>();

            return services;
        }
    }
}