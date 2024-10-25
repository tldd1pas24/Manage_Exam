namespace Hutech.Exam.Server.Installers
{
    public interface IInstaller
    {
        void InstallService(IServiceCollection services, IConfiguration configuration);
    }
}
