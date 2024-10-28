using Hutech.Exam.Server.DAL.Repositories;
using Hutech.Exam.Shared.Models;
using System.Data;

namespace Hutech.Exam.Server.BUS
{
    public class ChiTietBaiThiService
    {
        private readonly IChiTietBaiThiRepository _chiTietBaiThiRepository;
        public ChiTietBaiThiService(IChiTietBaiThiRepository chiTietBaiThiRepository)
        {
            _chiTietBaiThiRepository = chiTietBaiThiRepository;
        }
        private ChiTietBaiThi getProperty(IDataReader dataReader)
        {
            ChiTietBaiThi chiTietBaiThi = new ChiTietBaiThi();
            chiTietBaiThi.MaChiTietBaiThi = dataReader.GetInt64(0);
            chiTietBaiThi.MaChiTietCaThi = dataReader.GetInt32(1);
            chiTietBaiThi.MaDeHv = dataReader.GetInt64(2);
            chiTietBaiThi.MaNhom = dataReader.GetInt32(3);
            chiTietBaiThi.MaCauHoi = dataReader.GetInt32(4);
            chiTietBaiThi.CauTraLoi = dataReader.IsDBNull(5) ? null : dataReader.GetInt32(5);
            chiTietBaiThi.NgayTao = dataReader.GetDateTime(6);
            chiTietBaiThi.NgayCapNhat = dataReader.IsDBNull(7) ? null : dataReader.GetDateTime(7);
            chiTietBaiThi.KetQua = dataReader.IsDBNull(8) ? null : dataReader.GetBoolean(8);
            chiTietBaiThi.ThuTu = dataReader.GetInt32(9);
            return chiTietBaiThi;
        }
        public void Insert(int ma_chi_tiet_ca_thi, long MaDeHV, int MaNhom, int MaCauHoi, DateTime NgayTao, int ThuTu)
        {
            try
            {
                _chiTietBaiThiRepository.Insert(ma_chi_tiet_ca_thi, MaDeHV, MaNhom, MaCauHoi, NgayTao, ThuTu);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(long MaChiTietBaiThi, int CauTraLoi, DateTime NgayCapNhat, bool KetQua)
        {
            if(!_chiTietBaiThiRepository.Update(MaChiTietBaiThi, CauTraLoi, NgayCapNhat, KetQua))
            {
                throw new Exception("Can not update ChiTietBaiThi");
            }
        }
        public List<ChiTietBaiThi> SelectBy_ma_chi_tiet_ca_thi(int ma_chi_tiet_ca_thi)
        {
            List<ChiTietBaiThi> result = new List<ChiTietBaiThi>();
            using(IDataReader dataReader = _chiTietBaiThiRepository.SelectBy_ma_chi_tiet_ca_thi(ma_chi_tiet_ca_thi))
            {
                while(dataReader.Read())
                {
                    ChiTietBaiThi chiTietBaiThi = getProperty(dataReader);
                    result.Add(chiTietBaiThi);
                }
            }
            return result;
            
        }
        public void insertChiTietBaiThis_SelectByChiTietDeThiHV(List<CustomDeThi>? customDeThis, int ma_chi_tiet_ca_thi, long ma_de_hoan_vi)
        {
            List<ChiTietBaiThi> result = new List<ChiTietBaiThi>();
            int stt = 0;
            if (customDeThis == null)
                return;
            foreach(var item in customDeThis)
            {
                this.Insert(ma_chi_tiet_ca_thi, ma_de_hoan_vi, item.MaNhom, item.MaCauHoi, DateTime.Now, ++stt);
            }
        }
        public void updateChiTietBaiThis(List<ChiTietBaiThi> chiTietBaiThis)
        {
            foreach(var item in chiTietBaiThis)
            {
                if(item.CauTraLoi != null && item.KetQua != null)
                {
                    long ma_chi_tiet_bai_thi = this.SelectOne_v2(item.MaChiTietCaThi, item.MaDeHv, item.MaNhom, item.MaCauHoi).MaChiTietBaiThi;
                    this.Update(ma_chi_tiet_bai_thi, (int)item.CauTraLoi, DateTime.Now, (bool)item.KetQua);
                }
            }
        }
        public void insertChiTietBaiThis(List<ChiTietBaiThi> chiTietBaiThis)
        {
            foreach(var item in chiTietBaiThis)
            {
                this.Insert(item.MaChiTietCaThi, item.MaDeHv, item.MaNhom, item.MaCauHoi, DateTime.Now, item.ThuTu);
            }
        }
        public void Delete(long ma_chi_tiet_bai_thi)
        {
            try
            {
                _chiTietBaiThiRepository.Delete(ma_chi_tiet_bai_thi);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ChiTietBaiThi SelectOne_v2(int ma_chi_tiet_ca_thi, long ma_de_hv, int ma_nhom, int ma_cau_hoi)
        {
            ChiTietBaiThi chiTietBaiThi = new ChiTietBaiThi();
            using (IDataReader dataReader = _chiTietBaiThiRepository.SelectOne_v2(ma_chi_tiet_ca_thi, ma_de_hv, ma_nhom, ma_cau_hoi))
            {
                if (dataReader.Read())
                {
                    chiTietBaiThi = getProperty(dataReader);
                }
            }
            return chiTietBaiThi;
        }
    }
}
