using trabalho2.Repositories;
using trabalho2.Repositories.Interfaces;
using trabalho2.Repositories.Interfaces.Base;
using trabalho2.Services;

namespace trabalho2.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            // repositories
            services.AddScoped<UserRepository>();
            services.AddScoped<UserLogRepository>();

            // interfaces
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserLogRepository, UserLogRepository>();

            // services
            services.AddScoped<UserService>();
            services.AddScoped<UserLogService>();
            services.AddScoped<TarefaService>();

            return services;
        }
    }
}