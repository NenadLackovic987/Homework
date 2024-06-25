using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(options =>
{
    // these must be set other ASP.NET Core will throw exception that no
    // default authentication scheme or default challenge scheme is set.
    options.DefaultAuthenticateScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => 
                        { 
                            options.LoginPath = "/Login";
                            options.ExpireTimeSpan = TimeSpan.FromHours(8);
                            options.SlidingExpiration = true;
                        }
            );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
