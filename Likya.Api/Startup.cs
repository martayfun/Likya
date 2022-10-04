using System;
using System.Text;
using System.Threading.Tasks;
using Likya.Business.Services;
using Likya.Core;
using Likya.Core.EntityFramework;
using Likya.Core.Models;
using Likya.Core.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Likya.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), m => m.MigrationsAssembly("Likya.Api")));
            RegisterScopedServices(services);
            RegisterTransientServices(services);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };

                options.Events.OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                    {
                        context.Token = context.Request.Cookies["X-Access-Token"];
                    }
                    return Task.CompletedTask;
                };
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5; //En az kaç karakterli olması gerektiğini belirtiyoruz.
                options.Password.RequireNonAlphanumeric = false; //Alfanumerik zorunluluğunu kaldırıyoruz.
                options.Password.RequireLowercase = false; //Küçük harf zorunluluğunu kaldırıyoruz.
                options.Password.RequireUppercase = false; //Büyük harf zorunluluğunu kaldırıyoruz.
                options.Password.RequireDigit = false; //0-9 arası sayısal karakter zorunluluğunu kaldırıyoruz.

                options.User.RequireUniqueEmail = true; //Email adreslerini tekilleştiriyoruz.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Likya.Api", Version = "v1" });
            });
        }

        private void RegisterTransientServices(IServiceCollection services)
        {
            services.AddTransient<IProductService, ProductService>();
        }

        private void RegisterScopedServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                SampleData.Initialize(app);
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Likya.Api v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
