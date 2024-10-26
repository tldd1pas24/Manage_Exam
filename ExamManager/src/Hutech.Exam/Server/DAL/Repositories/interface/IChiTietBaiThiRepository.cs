using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IChiTietBaiThiRepository
    {
        public bool Insert(int ma_chi_tiet_ca_thi, long MaDeHV, int MaNhom, int MaCauHoi, DateTime NgayTao, int ThuTu);
        public bool Update(long MaChiTietBaiThi, int CauTraLoi, DateTime NgayCapNhat, bool KetQua);
        public IDataReader SelectBy_ma_chi_tiet_ca_thi(int ma_chi_tiet_ca_thi);
        public bool Delete(long ma_chi_tiet_bai_thi);
        public IDataReader SelectOne_v2(int ma_chi_tiet_ca_thi, long ma_de_hv, int ma_nhom, int ma_cau_hoi);
    }
}
