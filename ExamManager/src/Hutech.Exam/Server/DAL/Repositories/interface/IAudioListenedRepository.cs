using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IAudioListenedRepository
    {
        public IDataReader SelectOne(int ma_chi_tiet_ca_thi, string filename);

        public bool Save(int ma_chi_tiet_ca_thi, string fileName);
    }
}
