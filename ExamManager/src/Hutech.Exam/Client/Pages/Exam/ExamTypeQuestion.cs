using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Hutech.Exam.Client.Pages.Exam
{
    public partial class Exam
    {
        // xử lí các dạng kiểu dữ liệu
        private async Task modifyNhomCauHoi()
        {
            int thu_tu_ma_nhom = 0;
            if (customDeThis != null)
            {
                foreach (var item in customDeThis)
                {
                    if (item.MaNhom != thu_tu_ma_nhom && item.NoiDungCauHoiNhom != null)
                    {
                        thu_tu_ma_nhom = item.MaNhom;

                        // xử lí audio
                        if (item.NoiDungCauHoiNhom.Contains("<audio") && chiTietCaThi != null)
                        {
                            string fileName = handleAudioSource(item.NoiDungCauHoiNhom);
                            int so_lan_nghe = (chiTietCaThi.DaThi) ? await getSoLanNghe(chiTietCaThi.MaChiTietCaThi, fileName) : 0;
                            item.GhiChu = so_lan_nghe.ToString();
                        }
                    }
                }
            }
        }
        private List<string> handleLatex(string text)
        {
            List<string> result = new List<string>();
            if (!text.Contains("latex"))
                return new List<string> { text };
            string[] parts = text.Split("<latex>");
            // xử lí phần đầu chắc chắn không có latex hoặc là thuần latex
            if(parts.Length > 1)
                result.Add(parts[0]);
            for (int i = 1; i < parts.Length; i++)
            {
                // phần cắt này chỉ có 2 phần duy nhất
                string[] parts2 = parts[i].Split("</latex>");

                // xử lí phần đầu chắc chắn là latex
                result.Add("$$" + parts2[0]);

                // phần còn lại là chữ hoặc không có nếu là thuần latex
                if(parts2.Length > 1)
                    result.Add(parts2[1]);
            }
            return result;
        }
        private string handleDienKhuyet(string text, int STT)
        {
            if(!text.Contains("(*)"))
                return text;
            return Regex.Replace(text, @"\(\*\)", m => "(" + (STT++).ToString() + ")");
        }
        private string handleAudioSource(string text)
        {
            text = text.Trim();
            int index_source = text.IndexOf("src=\"");
            int length = "src=\"".Length;
            string source = text.Substring(index_source + length);
            int end_source = source.IndexOf("\"/>");
            source = source.Substring(0, end_source);
            int index_audio = text.IndexOf("<audio");
            return source;
        }
        private string handleBeforeAudio(CustomDeThi customDeThi, string text, int ma_audio)
        {
            int index_audio = text.IndexOf("<audio");
            if (int.TryParse(customDeThi.GhiChu, out int temp) && chiTietCaThi != null)
            {
                if(temp == 3)
                    isDisableAudio?.Insert(ma_audio, true);
                else
                    isDisableAudio?.Insert(ma_audio, false);
            }
            return text.Substring(0, index_audio);
        }
        private async Task onPlayAudio(CustomDeThi customDeThi, int ma_audio, string fileName, string elementId)
        {
            // tăng số lần nghe lên
            if (int.TryParse(customDeThi.GhiChu, out int so_lan_nghe) && chiTietCaThi != null && js != null)
            {
                await js.InvokeVoidAsync("playAudio", elementId, so_lan_nghe);
                if (so_lan_nghe < 3 && isDisableAudio != null)
                {
                    customDeThi.GhiChu = (++so_lan_nghe).ToString();
                    await addOrUpdateListen(chiTietCaThi.MaChiTietCaThi, fileName);
                }
            }
            StateHasChanged();
        }
    }
}
