using LadyFirstShop.Data.AutoMapper;
using LadyFirstShop.Data.EF;
using LadyFirstShop.Data.Entities;
using LadyFirstShop.Data.Infrastructure;
using LadyFirstShop.Data.Repositories.ProductRepository;
using LadyFirstShop.Data.Services.Brands;
using LadyFirstShop.Data.Services.Image;
using LadyFirstShop.Data.Services.Orders;
using LadyFirstShop.Data.Services.Products;
using LadyFirstShop.Data.Services.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LadyFirstShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LadyFirstShopDb"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}


builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<LadyFirstShopDbContext>()
    .AddDefaultTokenProviders();

// add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Swagger",
    });
    // cấu hình Authozire
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
        }
                    });
});
// Cấu hình authorize
string issuer = builder.Configuration.GetValue<string>("Tokens:Issuer");
string signingKey = builder.Configuration.GetValue<string>("Tokens:Key");
byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = issuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = System.TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
    };
});
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					// chưa login thì trả về login page
					options.LoginPath = "/Login/Index/";
					options.AccessDeniedPath = "/User/Forbidden/";
				});
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
	);
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
