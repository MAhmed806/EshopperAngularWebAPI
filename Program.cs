using EShopperAngular.Data;
using EShopperAngular.Models;
using EShopperAngular.Repositories.GenericRepository;
using EShopperAngular.Services.API.MYAPI;
using EShopperAngular.Stripe_Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
//JWT MIDDLEWARE
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
//JWT Middleware end
//stripe 
builder.Services.AddScoped<StripeService>();
builder.Services.AddScoped<CardService>();
//stripe
//cors added
builder.Services.AddCors();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//repository additions
builder.Services.AddScoped<IGenericRepository<Products>, GenericRepository<Products>>();
builder.Services.AddScoped<IGenericRepository<ProductTypes>, GenericRepository<ProductTypes>>();
builder.Services.AddScoped<IGenericRepository<Order>, GenericRepository<Order>>();
builder.Services.AddScoped<IGenericRepository<OrderDetails>, GenericRepository<OrderDetails>>();
builder.Services.AddScoped<IGenericRepository<Color>, GenericRepository<Color>>();
builder.Services.AddScoped<IGenericRepository<ProductColors>, GenericRepository<ProductColors>>();
builder.Services.AddScoped<IGenericRepository<ChargeAgainstOrder>, GenericRepository<ChargeAgainstOrder>>();
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

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
//CORS CODE
app.UseMiddleware<AddHeaderMiddleware>();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//CORS CODE END

app.UseHttpsRedirection();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllers();

app.Run();
