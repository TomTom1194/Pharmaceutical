using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pharmaceutical.Data;
using Pharmaceutical.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PharmaceuticalDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("PharmConnection")));

builder.Services.AddControllers();

builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITabletSpecificationService, TabletSpecificationService>();
builder.Services.AddScoped<ICapsuleSpecificationService, CapsuleSpecificationService>();
builder.Services.AddScoped<ILiquidFillingSpecificationService, LiquidFillingSpecificationService>();
builder.Services.AddScoped<IQuoteRequestService, QuoteRequestService>();

//Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IContentPageService, ContentPageService>();
builder.Services.AddScoped<IPositionService, PositionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
