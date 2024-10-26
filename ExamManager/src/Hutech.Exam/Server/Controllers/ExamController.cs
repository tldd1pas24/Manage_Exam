using Hutech.Exam.Server.Attributes;
using Hutech.Exam.Server.BUS;
using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;

namespace Hutech.Exam.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamController : Controller
    {
        private readonly ChiTietDeThiHoanViService _chiTietDeThiHoanViService;
        private readonly ChiTietBaiThiService _chiTietBaiThiService;
        private readonly AudioListenedService _audioListenedService;
        private readonly CustomDeThiService _customDeThiService;
        private readonly CaThiService _caThiService;
        public ExamController(ChiTietDeThiHoanViService chiTietDeThiHoanViService, ChiTietBaiThiService chiTietBaiThiService, 
            AudioListenedService audioListenedService, CustomDeThiService customDeThiService, CaThiService caThiService)
        {
            _chiTietDeThiHoanViService = chiTietDeThiHoanViService;
            _chiTietBaiThiService = chiTietBaiThiService;
            _audioListenedService = audioListenedService;
            _customDeThiService = customDeThiService;
            _caThiService = caThiService;
        }
        [HttpGet("GetDeThi")]
        [Cache]
        public ActionResult<List<CustomDeThi>> GetDeThi([FromQuery] long ma_de_thi_hoan_vi)
        {
            List<CustomDeThi> result = _customDeThiService.handleDeThi(ma_de_thi_hoan_vi);
            return result;
        }
        public async Task<List<int>?> GetListDapAnWithCacheAsync(long maDeHv)
        {
            var cacheKey = $"/api/Exam/GetDeThi|DapAn-{maDeHv}";
            var cacheService = HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            // Kiểm tra xem có dữ liệu trong cache không
            var cachedData = await cacheService.GetCacheResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Nếu có, deserialization và trả về dữ liệu
                return JsonSerializer.Deserialize<List<int>>(cachedData);
            }

            // Nếu không có, thực hiện logic lấy dữ liệu từ database
            List<TblChiTietDeThiHoanVi> chiTietDeThiHoanVis = _chiTietDeThiHoanViService.SelectBy_MaDeHV(maDeHv);
            List<int> listDapAn = new List<int>();
            foreach (var item in chiTietDeThiHoanVis)
            {
                if (item.DapAn != null)
                    listDapAn.Add((int)item.DapAn);
            }

            // Lưu vào cache
            await cacheService.SetCacheResponseAsync(cacheKey, listDapAn, TimeSpan.FromMinutes(120));

            return listDapAn;
        }
        [HttpGet("IsActiveCaThi")]
        public ActionResult<bool> IsActiveCaThi([FromQuery] int ma_ca_thi)
        {
            CaThi caThi = _caThiService.SelectOne(ma_ca_thi);
            return (caThi.IsActivated);
        }
        // Insert (có trả về list bài thi) giúp cho sinh viên tiếp tục thi trong trường hợp treo máy

        [HttpPost("InsertChiTietBaiThi")]
        public ActionResult<List<ChiTietBaiThi>> InsertChiTietBaiThi([FromQuery] int ma_chi_tiet_ca_thi ,[FromQuery] long ma_de_thi_hoan_vi)
        {
            List<CustomDeThi>? customDeThis = this.GetDeThi(ma_de_thi_hoan_vi).Value;
            _chiTietBaiThiService.insertChiTietBaiThis_SelectByChiTietDeThiHV(customDeThis, ma_chi_tiet_ca_thi, ma_de_thi_hoan_vi);

            // tránh trường hợp lấy đề của những môn khác
            List<ChiTietBaiThi> result = _chiTietBaiThiService.SelectBy_ma_chi_tiet_ca_thi(ma_chi_tiet_ca_thi);

            return result;
        }
        [HttpGet("InsertCTBT_DaVaoThi")] // khi này sinh viên đã thi trước đó và tiếp tục thi nên không cần insert chi tiet bai thi nữa mà chỉ lấy -> tối ưu API
        public ActionResult<List<ChiTietBaiThi>> InsertCTBT_DaVaoThi([FromQuery] int ma_chi_tiet_ca_thi, [FromQuery] long ma_de_thi_hoan_vi)
        {
            // tránh trường hợp lấy đề của những môn khác
            return _chiTietBaiThiService.SelectBy_ma_chi_tiet_ca_thi(ma_chi_tiet_ca_thi);
        }

        [HttpPost("UpdateChiTietBaiThi")]
        public async Task<ActionResult> UpdateChiTietBaiThi([FromBody] List<ChiTietBaiThi> chiTietBaiThis)
        {
            if (chiTietBaiThis.Count == 0)
                return Ok();
            List<int>? listDapAn = await this.GetListDapAnWithCacheAsync(chiTietBaiThis[0].MaDeHv);
            // nếu thứ tự là 0 là đã insert trước đó, chỉ update và ngược lại thì insert và update
            foreach(var item in chiTietBaiThis)
            {
                if (item.ThuTu != 0)
                    _chiTietBaiThiService.Insert(item.MaChiTietCaThi, item.MaDeHv, item.MaNhom, item.MaCauHoi, DateTime.Now, item.ThuTu);
                if (listDapAn != null && item.CauTraLoi != null)
                    item.KetQua = (listDapAn.Contains((int)item.CauTraLoi)) ? true : false;
            }
            _chiTietBaiThiService.updateChiTietBaiThis(chiTietBaiThis);
            return Ok();
        }

        [HttpGet("AudioListendCount")]
        public ActionResult<int> AudioListendCount([FromQuery] int ma_chi_tiet_ca_thi, [FromQuery] string filename)
        {
            return _audioListenedService.SelectOne(ma_chi_tiet_ca_thi, filename);
        }
        [HttpGet("AddOrUpdateAudio")]
        public ActionResult AddOrUpdateAudio([FromQuery] int ma_chi_tiet_ca_thi, [FromQuery] string filename)
        {
            _audioListenedService.Save(ma_chi_tiet_ca_thi, filename);
            return Ok();
        }

    }
}
