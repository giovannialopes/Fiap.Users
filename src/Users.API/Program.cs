using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Users.Domain.Config;
using Users.Domain.Dependency;
using Users.Domain.Entity;
using Users.Domain.Enum;
using Users.Domain.Middleware;
using Users.Infrastructure.Data;
using Users.Infrastructure.Dependency;

var builder = WebApplication.CreateBuilder(args);

//Controllers.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger com JWT
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//RabbitMQ com MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var loggerFactory = context.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MassTransit");
        
        var host = configuration["RabbitMQ:Host"] ?? "localhost";
        var port = configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
        var username = configuration["RabbitMQ:Username"] ?? "guest";
        var password = configuration["RabbitMQ:Password"] ?? "guest";
        var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";

        logger.LogInformation("Configurando RabbitMQ: Host={Host}, Port={Port}, VirtualHost={VirtualHost}, Username={Username}", 
            host, port, virtualHost, username);

        cfg.Host(host, port, virtualHost, h =>
        {
            h.Username(username);
            h.Password(password);
        });

        // Configuração de reconexão automática com retry exponencial
        cfg.UseMessageRetry(r => r.Exponential(
            retryLimit: 5,
            minInterval: TimeSpan.FromSeconds(1),
            maxInterval: TimeSpan.FromSeconds(30),
            intervalDelta: TimeSpan.FromSeconds(2)
        ));

        // Configuração de circuit breaker para resiliência
        cfg.UseCircuitBreaker(cb => {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });

        // Configuração de outbox para garantir entrega
        cfg.UseInMemoryOutbox();
        
        cfg.ConfigureEndpoints(context);
    });
});

//Banco de Dados
builder.Services.AddDbContext<DbUser>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Valida��o FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

// JWT Auth
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Inje��o de Depend�ncias
builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ObservabilityMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ErrorsMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();


// Executar migrations (não crítico - aplicação continua mesmo se falhar)
try {
    using (var scope = app.Services.CreateScope()) {
        var dbContext = scope.ServiceProvider.GetRequiredService<DbUser>();
        await dbContext.Database.MigrateAsync();

        if (!dbContext.USERS.Any()) {
            var usuarioInicial = PerfilEnt.Criar("Admin", 
                "admin@fcg.com",
                "1234",
                PerfilEnum.Administrador);

            dbContext.USERS.Add(usuarioInicial);

            await dbContext.SaveChangesAsync();
        }
    }
} catch (Exception ex) {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Não foi possível conectar ao banco de dados na inicialização. A aplicação continuará rodando.");
}


await app.RunAsync();

