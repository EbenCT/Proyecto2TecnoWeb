using Aforo255.Cross.Http.Src;
using Aforo255.Cross.Token.Src;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSVenta.Venta.Repositories;
using MSVenta.Venta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using System.Text.Json.Serialization;

namespace MSVenta.Venta
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddDbContext<ContextDatabase>(
               opt =>
               {
                   opt.UseNpgsql(Configuration["postgres:cn"]);
               });
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IAlmecenService, AlmacenService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IMarcaService, MarcaService>();
            services.AddScoped<ProductoAlmacenService>();
            services.AddScoped<AjusteInventarioService>();

            services.AddProxyHttp();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddHttpClient<IMarcaHttpClient, MarcaHttpClient>(client =>
            {
                // URL del microservicio de Ventas
                client.BaseAddress = new Uri("http://localhost:5002/"); // Ajusta el puerto según tu configuración
            });
            services.AddHttpClient<ICategoriaHttpClient, CategoriaHttpClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5002/");
            });
            services.AddHttpClient<IProductoHttpClient, ProductoHttpClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5002/");
            });
            services.AddHttpClient<IAlmacenHttpClient, AlmacenHttpClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5002/");
            });
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
