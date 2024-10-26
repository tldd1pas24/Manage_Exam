using Hutech.Exam.Server.DAL.DataReader;
using Hutech.Exam.Shared.Models;
using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public class DeThiHoanViRepository : IDeThiHoanViRepository
    {
        public IDataReader SelectOne(long ma_de_hoan_vi)
        {
            DatabaseReader sql = new DatabaseReader("tbl_DeThiHoanVi_SelectOne");
            sql.SqlParams("@MaDeHV", SqlDbType.BigInt, ma_de_hoan_vi);
            return sql.ExcuteReader();
        }
        public IDataReader SelectBy_MaDeThi(int ma_de_thi)
        {
            DatabaseReader sql = new DatabaseReader("tbl_DeThiHoanVi_SelectBy_MaDeThi");
            sql.SqlParams("@MaDeThi", SqlDbType.Int, ma_de_thi);
            return sql.ExcuteReader();
        }
    }
}
