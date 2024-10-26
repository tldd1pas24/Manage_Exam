using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IMonHocRepository
    {
        public IDataReader SelectOne(int ma_mon_hoc);
    }
}
