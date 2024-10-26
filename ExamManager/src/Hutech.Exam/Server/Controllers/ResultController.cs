using Microsoft.AspNetCore.Mvc;
using Hutech.Exam.Server.BUS;
using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Hutech.Exam.Server.Attributes;


namespace Hutech.Exam.Server.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]        
    public class ResultController : Controller
    {
    private readonly SinhVienService _sinhVienService;
    private readonly CaThiService _caThiService;
    private readonly ChiTietDeThiHoanViService _chiTietDeThiHoanViService;
    private readonly CauTraLoiService _cauTraLoiService;
    private readonly ChiTietCaThiService _chiTietCaThiService;
    private readonly ChiTietBaiThiService _chiTietBaiThiService;
    public ResultController(SinhVienService sinhVienService, ChiTietDeThiHoanViService chiTietDeThiHoanViService, CaThiService caThiService,
        CauTraLoiService cauTraLoiService, ChiTietCaThiService chiTietCaThiService, ChiTietBaiThiService chiTietBaiThiService)
    {
        _sinhVienService = sinhVienService;
        _caThiService = caThiService;
        _chiTietDeThiHoanViService = chiTietDeThiHoanViService;
        _cauTraLoiService = cauTraLoiService;
        _chiTietCaThiService = chiTietCaThiService;
        _chiTietBaiThiService = chiTietBaiThiService;
    }
    [HttpGet("GetThongTinSinhVien")]
    public ActionResult<SinhVien> GetThongTinSinhVien([FromQuery] long ma_sinh_vien)
    {
        return _sinhVienService.SelectOne(ma_sinh_vien);
    }
    [HttpGet("GetThongTinCaThi")]
    public ActionResult<CaThi> GetThongTinCaThi([FromQuery] int ma_ca_thi)
    {
        return _caThiService.SelectOne(ma_ca_thi);
    }
    [HttpGet("GetChiTietCaThiSelectBy_SinhVien")]
    // lấy chi tiết các thông tin của 1 sinh viên thi vào 1 ca giờ cụ thể (đề thi hoán vị)
    public ActionResult<ChiTietCaThi> GetChiTietCaThiSelectBy_SinhVien([FromQuery] int ma_ca_thi, [FromQuery] long ma_sinh_vien)
    {
        return _chiTietCaThiService.SelectBy_MaCaThi_MaSinhVien(ma_ca_thi, ma_sinh_vien);
    }
    [HttpGet("GetListDungSai")]
    public ActionResult<int> GetListDungSai([FromQuery] int ma_chi_tiet_ca_thi, int tong_so_cau)
    {
        List<bool?> result = new List<bool?>();
        List<ChiTietBaiThi> chiTietBaiThis = _chiTietBaiThiService.SelectBy_ma_chi_tiet_ca_thi(ma_chi_tiet_ca_thi).OrderBy(p => p.ThuTu).ToList();
        for(int i = 1; i <= tong_so_cau; i++)
        {
            bool? ketQua = chiTietBaiThis?.FirstOrDefault(p => p.ThuTu == i)?.KetQua;
            result.Add(ketQua);
        }
        return Ok(result);
    }
    [HttpPost("UpdateKetThuc")]
    public ActionResult UpdateKetThuc([FromBody] ChiTietCaThi chiTietCaThi)
    {
        _chiTietCaThiService.UpdateKetThuc(chiTietCaThi);
        return Ok();
    }
}

