using Hutech.Exam.Shared.Models;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace Hutech.Exam.Client.Pages.Exam
{
    public partial class Exam
    {
        private async Task getDeThi(long? ma_de_hoan_vi)
        {
            HttpResponseMessage? response = null;
            if (httpClient != null)
                response = await httpClient.GetAsync($"api/Exam/GetDeThi?ma_de_thi_hoan_vi={ma_de_hoan_vi}");
            if (response != null && response.IsSuccessStatusCode && myData != null)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                customDeThis = myData.customDeThis = JsonSerializer.Deserialize<List<CustomDeThi>>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
        }
        // insert các các dòng dữ liệu chiTietBaiThi, và lấy dữ liệu của chiTietbaiThi
        private async Task InsertChiTietBaiThi()
        {
            HttpResponseMessage? response = null;
            if (httpClient != null && chiTietCaThi != null && myData != null && myData.chiTietCaThi != null)
                response = await httpClient.PostAsync($"api/Exam/InsertChiTietBaiThi?ma_chi_tiet_ca_thi={chiTietCaThi.MaChiTietCaThi}&ma_de_thi_hoan_vi={myData.chiTietCaThi.MaDeThi}", null);
            if (response != null && response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                chiTietBaiThis = JsonSerializer.Deserialize<List<ChiTietBaiThi>>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            if (chiTietBaiThis != null && chiTietCaThi != null)
            {
                foreach (var item in chiTietBaiThis)
                    item.MaChiTietCaThiNavigation = chiTietCaThi;
            }
        }
        private async Task InsertChiTietBaiThi_DaVaoThiTruocDo()
        {
            HttpResponseMessage? response = null;
            if (httpClient != null && chiTietCaThi != null && myData != null && myData.chiTietCaThi != null)
                response = await httpClient.GetAsync($"api/Exam/InsertCTBT_DaVaoThi?ma_chi_tiet_ca_thi={chiTietCaThi.MaChiTietCaThi}&ma_de_thi_hoan_vi={myData.chiTietCaThi.MaDeThi}");
            if (response != null && response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                chiTietBaiThis = JsonSerializer.Deserialize<List<ChiTietBaiThi>>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            if (chiTietBaiThis != null && chiTietCaThi != null)
            {
                foreach (var item in chiTietBaiThis)
                    item.MaChiTietCaThiNavigation = chiTietCaThi;
            }
        }
        private async Task UpdateChiTietBaiThi()
        {
            var jsonString = JsonSerializer.Serialize(dsBaiThi_Update);
            dsBaiThi_Update?.Clear(); // xóa toàn bộ các phần tử khi đã được update, tiếp tục lưu những phần tử sv làm
            if (httpClient != null)
                await httpClient.PostAsync("api/Exam/UpdateChiTietBaiThi", new StringContent(jsonString, Encoding.UTF8, "application/json"));
        }
        private async Task<bool> isActiveCaThi()
        {
            HttpResponseMessage? response = null;
            if (httpClient != null)
                response = await httpClient.GetAsync($"api/Exam/IsActiveCaThi?ma_ca_thi={chiTietCaThi?.MaCaThi}");
            if (response != null && response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            return true;
        }
        private async Task<int> getSoLanNghe(int ma_chi_tiet_ca_thi, string filename)
        {
            HttpResponseMessage? response = null;
            if (httpClient != null)
                response = await httpClient.GetAsync($"api/Exam/AudioListendCount?ma_chi_tiet_ca_thi={ma_chi_tiet_ca_thi}&filename={filename}");
            if (response != null && response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            return 0;
        }
        private async Task addOrUpdateListen(int ma_chi_tiet_ca_thi, string filename)
        {
            if (httpClient != null)
                await httpClient.GetAsync($"api/Exam/AddOrUpdateAudio?ma_chi_tiet_ca_thi={ma_chi_tiet_ca_thi}&filename={filename}");
        }
    }
}
