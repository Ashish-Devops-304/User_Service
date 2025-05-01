using Contracts.IRepository;
using Contracts.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Repository;
using Services;
using System.Text;
using User_Service.Helpers;

namespace UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) // Configure services 
        {
            services.AddHttpContextAccessor();

            // services.AddTransient<ExceptionHandlingMiddleware>();

            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            });

            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service", Version = "v1" });
            });

            services.AddAutoMapper(typeof(UserProfile));

            // Registers the service for "CommonUserService" 

            services.AddScoped<ICommonService, CommonService>();

            // Registers the service for "UserAccount" - service/repository

            services.AddScoped<IUserAccountService, UserAccountService>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();

            // Registers the service for "UserAddress" - service/repository

            services.AddScoped<IUserAddressService, UserAddressService>();
            services.AddScoped<IUserAddressRepository, UserAddressRepository>();

            // Registers the service for "UserPayment" - service/repository

            services.AddScoped<IUserPaymentService, UserPaymentService>();
            services.AddScoped<IUserPaymentRepository, UserPaymentRepository>();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) // All the middleware in the application can be configured in this method
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
