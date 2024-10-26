using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IDeThiRepository
    {
        public IDataReader SelectOne(int ma_de_thi);
    }
}
