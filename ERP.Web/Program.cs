using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using ERP.DataAccess.Data;
using ERP.BusinessLogic;
using ERP.BusinessLogic.Services;
using ERP.Web;
using ERP.Web.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
})
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

builder.Services.AddHttpClient<IAssistantService, AssistantService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(120);
});

builder.Services.AddHttpClient<IPriceComparisonService, PriceComparisonService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedAsync(
        services.GetRequiredService<ApplicationDbContext>(),
        services.GetRequiredService<ICategoryService>(),
        services.GetRequiredService<ISupplierService>(),
        services.GetRequiredService<IProductService>(),
        services.GetRequiredService<ICustomerService>(),
        services.GetRequiredService<IOrderService>());
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

var localizationOptions = new RequestLocalizationOptions
{
    // Formatting/parsing culture is pinned to en-US regardless of UI language, so
    // <input type="number"> decimal parsing on forms never depends on selected language.
    DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en"),
    SupportedCultures = new[] { new CultureInfo("en-US") },
    SupportedUICultures = new[] { new CultureInfo("en"), new CultureInfo("bg") }
};
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider> { new UiCultureCookieProvider() };

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
