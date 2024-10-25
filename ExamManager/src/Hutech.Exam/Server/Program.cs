using Hutech.Exam.Server.Authentication;
using Hutech.Exam.Server.BUS;
using Hutech.Exam.Server.DAL.Repositories;
using Hutech.Exam.Server.Hubs;
using Hutech.Exam.Server.Installers;
using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Them cac dich vu service
ConfigureServices(builder.Services, builder.Configuration);


var app = builder.Build();
Configure(app);

app.Run();

static void Configure(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseResponseCompression();
    app.UseHttpsRedirection();
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();


    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");
    app.MapHub<ChiTietCaThiHub>("/ChiTietCaThiHub");

}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.InstallerServicesInAssembly(configuration);

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
}