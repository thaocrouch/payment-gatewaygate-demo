using WebClient.Components;
using WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents().AddCircuitOptions(option =>
    {
        option.DetailedErrors = true;
    });

builder.Services.AddHttpClient("backend", (option) =>
{
    option.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "");
    option.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddTransient<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();