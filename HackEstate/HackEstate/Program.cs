using HackEstate.Authentication;
using HackEstate.Managers;
using HackEstate.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.Configure<TokenAuthentication>(builder.Configuration.GetSection("TokenAuthentication"));
builder.Services.AddScoped<TokenValidationParametersFactory>();
builder.Services.AddScoped<SignInManager>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenProviderOptionsFactory>();
builder.Services.AddScoped<MailManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // Add authentication middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");

app.Run();
