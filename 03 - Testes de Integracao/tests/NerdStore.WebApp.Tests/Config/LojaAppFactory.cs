using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

//necessário instalar os pacotes:
//Microsoft.AspNetCore.Mvc.Testing
//Microsoft.AspNetCore.App
namespace NerdStore.WebApp.Tests.Config
{
    public class LojaAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TStartup>();
            builder.UseEnvironment("Testing");
        }
    }
}