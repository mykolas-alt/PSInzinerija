using Frontend.Components;
using Frontend.Services;

using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpClient("BackendApi", options =>
{
    options.BaseAddress = new Uri("http://localhost:5211");
    options.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
}).AddHeaderPropagation();

builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add("Cookie");
});

builder.Services.AddScoped<HighScoreAPIService>();
builder.Services.AddScoped<WordListAPIService>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseHeaderPropagation();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
