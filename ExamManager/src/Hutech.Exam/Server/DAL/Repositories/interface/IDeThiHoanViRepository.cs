using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IDeThiHoanViRepository
    {
        public IDataReader SelectOne(long ma_de_hoan_vi);
        public IDataReader SelectBy_MaDeThi(int ma_de_thi);
    }

}
