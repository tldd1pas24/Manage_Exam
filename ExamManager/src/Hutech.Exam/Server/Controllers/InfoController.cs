using Hutech.Exam.Server.BUS;
using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.DependencyResolver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hutech.Exam.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InfoController : ControllerBase
    {
        private readonly SinhVienService _sinhVienService;
        private readonly ChiTietCaThiService _chiTietCaThiService;
        private readonly CaThiService _caThiService;
        private readonly ChiTietDotThiService _chiTietDotThiService;
        private readonly LopAoService _lopAoService;
        private readonly MonHocService _monHocService;
        private readonly DotThiService _dotThiService;
        public InfoController(SinhVienService sinhVienService, ChiTietCaThiService chiTietCaThiService,
            CaThiService caThiService, ChiTietDotThiService chiTietDotThiService, LopAoService lopAoService, MonHocService monHocService, DotThiService dotThiService)
        {
            _sinhVienService = sinhVienService;
            _chiTietCaThiService = chiTietCaThiService;
            _caThiService = caThiService;
            _chiTietDotThiService = chiTietDotThiService;
            _lopAoService = lopAoService;
            _monHocService = monHocService;
            _dotThiService = dotThiService;
        }
        [HttpGet("GetThongTinChiTietCaThi")]
        public ActionResult<List<ChiTietCaThi>> GetThongTinChiTietCaThi([FromQuery] long ma_sinh_vien)
        {
            List<ChiTietCaThi> result = _chiTietCaThiService.SelectBy_MaSinhVienThi(ma_sinh_vien, DateTime.Now);
            foreach (var item in result)
            {
                item.MaCaThiNavigation = (item.MaCaThi != null) ? getThongTinCaThi((int)item.MaCaThi) : null;
                item.MaSinhVienNavigation = getThongTinSV(ma_sinh_vien);
            }
            //TH thí sinh không có ca thi
            if(result.Count == 0)
            {
                ChiTietCaThi newChiTietCaThi = new ChiTietCaThi();
                newChiTietCaThi.MaSinhVienNavigation = getThongTinSV(ma_sinh_vien);
                result.Add(newChiTietCaThi);
            }
            return result;
        }
        private SinhVien? getThongTinSV(long ma_sinh_vien)
        {
            return _sinhVienService.SelectOne(ma_sinh_vien);
        }
        private CaThi getThongTinCaThi(int ma_ca_thi)
        {
            CaThi caThi = _caThiService.SelectOne(ma_ca_thi);
            caThi.MaChiTietDotThiNavigation = getThongTinChiTietDotThi(caThi.MaChiTietDotThi);
            return caThi;
        }
        private ChiTietDotThi getThongTinChiTietDotThi(int ma_chi_tiet_dot_thi)
        {
            ChiTietDotThi chiTietDotThi = _chiTietDotThiService.SelectOne(ma_chi_tiet_dot_thi);
            chiTietDotThi.MaDotThiNavigation = getThongTinDotThi(chiTietDotThi.MaDotThi);
            chiTietDotThi.MaLopAoNavigation = getThongTinLopAo(chiTietDotThi.MaLopAo);
            return chiTietDotThi;
        }
        private DotThi getThongTinDotThi(int ma_dot_thi)
        {
            return _dotThiService.SelectOne(ma_dot_thi);
        }
        private LopAo getThongTinLopAo(int ma_lop_ao)
        {
            LopAo lopAo = _lopAoService.SelectOne(ma_lop_ao);
            lopAo.MaMonHocNavigation = getThongTinMonHoc(ma_lop_ao);
            return lopAo;
        }
        private MonHoc getThongTinMonHoc(int ma_mon_hoc)
        {
            return _monHocService.SelectOne(ma_mon_hoc);
        }
        [HttpPost("UpdateBatDauThi")]
        public ActionResult UpdateBatDauThi([FromBody] ChiTietCaThi chiTietCaThi)
        {
            _chiTietCaThiService.UpdateBatDau(chiTietCaThi);
            return Ok();
        }
    }
}
