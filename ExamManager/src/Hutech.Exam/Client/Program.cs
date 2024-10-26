using Blazored.SessionStorage;
using Hutech.Exam.Client;
using Hutech.Exam.Client.Authentication;
using Hutech.Exam.Client.DAL;
using Hutech.Exam.Client.Pages.Admin.DAL;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddHttpClient("Hutech.Exam.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Hutech.Exam.ServerAPI"));
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
// bien toan cuc
builder.Services.AddSingleton<AdminDataService>();
builder.Services.AddSingleton<ApplicationDataService>();
await builder.Build().RunAsync();
