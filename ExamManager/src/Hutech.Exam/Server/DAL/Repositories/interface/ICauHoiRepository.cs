using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface ICauHoiRepository
    {
        public IDataReader SelectOne(int ma_cau_hoi);
        public IDataReader SelectDapAn(int ma_cau_hoi);
    }
}
