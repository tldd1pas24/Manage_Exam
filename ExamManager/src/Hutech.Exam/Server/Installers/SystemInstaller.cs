
using Microsoft.AspNetCore.ResponseCompression;

namespace Hutech.Exam.Server.Installers
{
    public class SystemInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddResponseCompression(option =>
            {
                option.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" 
                    });
            });
        }
    }
}
