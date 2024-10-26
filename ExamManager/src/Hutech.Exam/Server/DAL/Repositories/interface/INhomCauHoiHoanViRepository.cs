using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface INhomCauHoiHoanViRepository
    {
        public IDataReader SelectOne(long ma_de_hoan_vi, int ma_nhom);
        public IDataReader SelectBy_MaDeHV(long ma_de_hoan_vi);
    }
}
