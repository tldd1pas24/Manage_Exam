using Hutech.Exam.Shared.Models;
using Microsoft.JSInterop;

namespace Hutech.Exam.Client.Pages.Exam
{
    public partial class Exam
    {
        [JSInvokable] // Đánh dấu hàm để có thể gọi từ JavaScript
        public static Task<int> GetDapAnFromJavaScript(int vi_tri_cau_hoi, int ma_cau_tra_loi, int ma_nhom, int ma_cau_hoi)
        {
            // Xử lý giá trị được truyền từ JavaScript
            if (listDapAn != null)
                listDapAn.Add(ma_cau_tra_loi);

            ChiTietBaiThi? findChiTietBaiThi = chiTietBaiThis?.FirstOrDefault(p => p.MaNhom == ma_nhom && p.MaCauHoi == ma_cau_hoi);
            ChiTietBaiThi tempChiTietBaiThi = getPropertyCTBT(vi_tri_cau_hoi, ma_cau_tra_loi, ma_nhom, ma_cau_hoi);

            // trường hợp thí sinh sửa đáp án của câu đã trả lời trước đó
            if (findChiTietBaiThi != null && chiTietBaiThis != null)
            {
                findChiTietBaiThi.CauTraLoi = ma_cau_tra_loi;
                tempChiTietBaiThi.ThuTu = 0; // biến này để đánh dấu cho server biết câu này đã được insert, chỉ cần update
            }
            else
                chiTietBaiThis?.Add(tempChiTietBaiThi);

            // trường hợp sinh viên lại khoanh lại đáp án nhiều lần trong 1 lần lưu
            ChiTietBaiThi? chiTietBaiThi = dsBaiThi_Update?.FirstOrDefault(p => p.MaNhom == ma_nhom && p.MaCauHoi == ma_cau_hoi);
            if (chiTietBaiThi != null)
            {
                chiTietBaiThi.CauTraLoi = ma_cau_tra_loi;
            }
            else
            {
                dsBaiThi_Update?.Add(tempChiTietBaiThi);
            }

            return Task.FromResult<int>(ma_cau_tra_loi);
        }

        private static ChiTietBaiThi getPropertyCTBT(int vi_tri_cau_hoi, int ma_cau_tra_loi, int ma_nhom, int ma_cau_hoi)
        {
            ChiTietBaiThi chiTietBaiThi = new ChiTietBaiThi();
            if (chiTietCaThi != null && chiTietCaThi.MaDeThi != null)
            {
                chiTietBaiThi.CauTraLoi = ma_cau_tra_loi;
                chiTietBaiThi.MaCauHoi = ma_cau_hoi;
                chiTietBaiThi.MaNhom = ma_nhom;
                chiTietBaiThi.ThuTu = vi_tri_cau_hoi;
                chiTietBaiThi.MaChiTietCaThi = chiTietCaThi.MaChiTietCaThi;
                chiTietBaiThi.MaDeHv = (long)chiTietCaThi.MaDeThi;
                chiTietBaiThi.MaChiTietCaThiNavigation = chiTietCaThi;
            }
            return chiTietBaiThi;
        }
    }
}
