using MissionControl.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<OpenClawDataService>();
builder.Services.AddSingleton<WorkLogService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<MissionControl.Components.App>().AddInteractiveServerRenderMode();

app.Run();
