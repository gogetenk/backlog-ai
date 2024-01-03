using AgileMind.Application.Domain;
using AgileMind.Infrastructure;
using AgileMind.Infrastructure.Gpt;
using AgileMind.Web.Client.Pages;
using AgileMind.Web.Components;
using AgileMind.Web.Components.Services;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

builder.Services.AddInfrastructure();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UserStory>());

builder.Services.AddHttpClient<IApiService, ApiService>(client => client.BaseAddress = new Uri("https://localhost:7035"));
builder.Services.AddSingleton(new OpenAIClient("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"));
builder.Services.AddSingleton<JsonProcessor>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(UserPrompt).Assembly);

app.UseRouting();
app.UseAntiforgery();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<BacklogHub>("/backlogHub");
});

app.UseAntiforgery();
app.MapControllers();

app.Run();
