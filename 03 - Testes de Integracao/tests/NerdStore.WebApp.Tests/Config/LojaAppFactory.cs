using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

//necessário instalar os pacotes:
//Microsoft.AspNetCore.Mvc.Testing
//Microsoft.AspNetCore.App
namespace NerdStore.WebApp.Tests.Config
{
    //WebApplicationFactory -> Trata-se de uma factory responsável por subir uma aplicação web
    //TStartup -> é uma classe genérica que terá as configurações necessárias
    public class LojaAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TStartup>();
            builder.UseEnvironment("Testing");
        }
    }
}