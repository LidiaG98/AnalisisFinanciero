using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_Informes_de_Analisis_Financieros.Models;

[assembly: HostingStartup(typeof(Sistema_de_Informes_de_Analisis_Financieros.Areas.Identity.IdentityHostingStartup))]
namespace Sistema_de_Informes_de_Analisis_Financieros.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}