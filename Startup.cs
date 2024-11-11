using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Método para adicionar serviços ao contêiner
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Configuração do Tweetinvi com as credenciais do appsettings.json
        services.AddSingleton<ITwitterClient>(sp =>
        {
            var twitterConfig = Configuration.GetSection("Twitter");
            return new TwitterClient(
                twitterConfig["APIKey"],
                twitterConfig["APISecretKey"],
                twitterConfig["AccessToken"],
                twitterConfig["AccessTokenSecret"]
            );
        });

        // Configuração de CORS para permitir chamadas do frontend
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    // Método para configurar o pipeline de requisições HTTP
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors(); // Habilita CORS

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}