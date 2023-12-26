using AgileMind.Web.Components.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddHttpClient<IApiService, ApiService>(client => client.BaseAddress = new Uri("https://localhost:7035"));

await builder.Build().RunAsync();
