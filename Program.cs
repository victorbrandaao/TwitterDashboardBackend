using Microsoft.Extensions.Configuration;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

// Criação do builder
var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();

// Configuração do Tweetinvi com as credenciais do appsettings.json
var twitterConfig = builder.Configuration.GetSection("Twitter");
var client = new TwitterClient(
    twitterConfig["APIKey"],
    twitterConfig["APISecretKey"],
    twitterConfig["AccessToken"],
    twitterConfig["AccessTokenSecret"]
);

// Registra o ITwitterClient como um serviço singleton
builder.Services.AddSingleton<ITwitterClient>(client);

// Configuração de CORS para permitir chamadas do frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Construção da aplicação
var app = builder.Build();

// Configuração do pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(); // Habilita CORS
app.UseAuthorization();
app.MapControllers(); // Mapeia os controladores
app.MapFallbackToFile("index.html"); // Serve o index.html da pasta wwwroot
app.Run();