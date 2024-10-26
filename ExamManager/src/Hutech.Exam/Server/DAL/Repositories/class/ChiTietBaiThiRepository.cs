using Hutech.Exam.Server.DAL.DataReader;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public class ChiTietBaiThiRepository : IChiTietBaiThiRepository
    {
        public bool Insert(int ma_chi_tiet_ca_thi, long MaDeHV, int MaNhom, int MaCauHoi, DateTime NgayTao, int ThuTu)
        {
            DatabaseReader sql = new DatabaseReader("chi_tiet_bai_thi_Insert");
            sql.SqlParams("@ma_chi_tiet_ca_thi", SqlDbType.Int, ma_chi_tiet_ca_thi);
            sql.SqlParams("@MaDeHV", SqlDbType.BigInt, MaDeHV);
            sql.SqlParams("@MaNhom", SqlDbType.Int, MaNhom);
            sql.SqlParams("@MaCauHoi", SqlDbType.Int, MaCauHoi);
            sql.SqlParams("@NgayTao", SqlDbType.DateTime, NgayTao);
            sql.SqlParams("@ThuTu", SqlDbType.Int, ThuTu);
            return sql.ExcuteNonQuery() != -1;
        }
        public bool Update(long MaChiTietBaiThi, int CauTraLoi, DateTime NgayCapNhat, bool KetQua)
        {
            DatabaseReader sql = new DatabaseReader("chi_tiet_bai_thi_Update");
            sql.SqlParams("@MaChiTietBaiThi", SqlDbType.BigInt, MaChiTietBaiThi);
            sql.SqlParams("@CauTraLoi", SqlDbType.Int, CauTraLoi);
            sql.SqlParams("@NgayCapNhat", SqlDbType.DateTime, NgayCapNhat);
            sql.SqlParams("@KetQua", SqlDbType.Bit, KetQua);
            return sql.ExcuteNonQuery() != 0;
        }
        public IDataReader SelectBy_ma_chi_tiet_ca_thi(int ma_chi_tiet_ca_thi)
        {
            DatabaseReader sql = new DatabaseReader("chi_tiet_bai_thi_SelectBy_ma_chi_tiet_ca_thi");
            sql.SqlParams("@ma_chi_tiet_ca_thi", SqlDbType.Int, ma_chi_tiet_ca_thi);
            return sql.ExcuteReader();
        }
        public bool Delete(long ma_chi_tiet_bai_thi)
        {
            DatabaseReader sql = new DatabaseReader("chi_tiet_bai_thi_Delete");
            sql.SqlParams("@MaChiTietBaiThi", SqlDbType.BigInt, ma_chi_tiet_bai_thi);
            return sql.ExcuteNonQuery() != 0;
        }
        public IDataReader SelectOne_v2(int ma_chi_tiet_ca_thi, long ma_de_hv, int ma_nhom, int ma_cau_hoi)
        {
            DatabaseReader sql = new DatabaseReader("chi_tiet_bai_thi_SelectOne_v2");
            sql.SqlParams("@ma_chi_tiet_ca_thi", SqlDbType.Int, ma_chi_tiet_ca_thi);
            sql.SqlParams("@MaDeHV", SqlDbType.BigInt, ma_de_hv);
            sql.SqlParams("@MaNhom", SqlDbType.Int, ma_nhom);
            sql.SqlParams("@MaCauHoi", SqlDbType.Int, ma_cau_hoi);
            return sql.ExcuteReader();
        }
    }
}
