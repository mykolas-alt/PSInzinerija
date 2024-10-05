using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using PSInzinerija1.Components;
using PSInzinerija1.Services;
using PSInzinerija1.Services.ApplicationDbContext;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddHttpClient();
builder.Services.AddScoped<IHighScoreAPIService, HighScoreAPIService>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "High score API",
        Description = "An ASP.NET Core Web API for managing game high scores",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.MapSwagger();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
