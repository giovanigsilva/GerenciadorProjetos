using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Domain.Interfaces.Services;
using ProjectManagement.Infrastructure.Repositories;
using ProjectManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProjetoService, ProjetoService>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddScoped<ITarefaService, TarefaService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IComentarioService, ComentarioService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProjetoRepository, ProjetoRepository>();
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IComentarioRepository, ComentarioRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IHistoricoTarefaRepository, HistoricoTarefaRepository>();

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
