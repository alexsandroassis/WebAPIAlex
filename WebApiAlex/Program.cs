using WebApiAlex.Services.Interfaces;
using WebApiAlex.Services;
using WebApiAlex.Models;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.OperationFilter<AddResponseHeadersFilter>();

    // Configurações de segurança JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT como: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


// Serviços em memória
builder.Services.AddSingleton<ProdutoService>();
builder.Services.AddSingleton<IProdutoService>(provider => provider.GetRequiredService<ProdutoService>());
builder.Services.AddSingleton<IServiceBase<Produto>>(provider => provider.GetRequiredService<ProdutoService>());

builder.Services.AddSingleton<GarantiaService>();
builder.Services.AddSingleton<IGarantiaService>(provider => provider.GetRequiredService<GarantiaService>());
builder.Services.AddSingleton<IServiceBase<Garantia>>(provider => provider.GetRequiredService<GarantiaService>());

builder.Services.AddSingleton<IServiceBase<Venda>, VendaService>();

// Configuração de autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "WebApiAlex",
        ValidAudience = "WebApiAlex",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("gCK>|>Gz^r8e;yg8j;Z=d]J<P!vlWpx{GX*mL"))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// **IMPORTANTE: ordem correta**
app.UseAuthentication(); // <-- JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
