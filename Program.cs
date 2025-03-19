
using JwtAuthApp.Authorization;
using JwtAuthApp.Data;
using JwtAuthApp.Helpers;
using JwtAuthApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace JwtAuthApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setup =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            builder.Services.AddCors();

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
            {
                throw new Exception("JWT Settings not found");
            }
            builder.Services.AddSingleton<JwtSettings>(serviceProvider => { return jwtSettings; });
            builder.Services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.RequireHttpsMetadata = true;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.SecretKeyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, ScopePolicyProvider>();
            builder.Services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();

            // Injection
            builder.Services.AddSingleton<ICosmosHelper, CosmosHelper>();
            builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
            {
                SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler();
                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
                {
                    SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase },
                    HttpClientFactory = () => new HttpClient(socketsHttpHandler, disposeHandler: false)
                };
                return new CosmosClient(builder.Configuration["CosmosSettings:ConnectionString"], cosmosClientOptions);
            });
            builder.Services.AddSingleton<IAuthHelper, AuthHelper>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<IUserService, UserService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

#if DEBUG
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
#endif

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            SetupData(app);

            app.Run();
        }

        private static void SetupData(WebApplication app)
        {
            //DataLoader.LoadLanguages(app);
            //DataLoader.LoadCountries(app);
        }
    }
}
