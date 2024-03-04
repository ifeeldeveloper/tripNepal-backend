using AdminPanelTN.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<TripNepalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbString"))
    );

builder.Services.AddCors(Options =>
{
    Options.AddPolicy(name: "AllowOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:7082", "http://localhost:4200", "http://localhost:4201") //cross platform frontend and backend
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Configure the first issuer

builder.Services.AddAuthentication("Issuer1Scheme")
    .AddJwtBearer("Issuer1Scheme", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JWTSettings:Issuer1");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["ValidIssuer"],
            ValidAudience = jwtSettings["ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
        };
    });

// Configure the second issuer
builder.Services.AddAuthentication("Issuer2Scheme")
    .AddJwtBearer("Issuer2Scheme", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings:Issuer2");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["ValidIssuer"],
            ValidAudience = jwtSettings["ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
        };
    });

builder.Services.AddControllers()
   .AddJsonOptions(options =>
   options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();