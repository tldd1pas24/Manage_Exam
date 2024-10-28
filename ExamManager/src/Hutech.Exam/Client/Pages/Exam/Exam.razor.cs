using Hutech.Exam.Client.DAL;
using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Hutech.Exam.Client.Authentication;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.SignalR.Client;

namespace Hutech.Exam.Client.Pages.Exam
{
    public partial class Exam
    {
        public static readonly int GIAY_CAP_NHAT = 30; // tự động lưu bài sau n (giây)
        [Inject]
        HttpClient? httpClient { get; set; }
        [Inject]
        ApplicationDataService? myData { get; set; }
        [Inject]
        AuthenticationStateProvider? authenticationStateProvider { get; set; }
        [Inject]
        NavigationManager? navManager { get; set; }
        [Inject]
        IJSRuntime? js { get; set; }
        private SinhVien? sinhVien { get; set; }
        private CaThi? caThi { get; set; }
        private static ChiTietCaThi? chiTietCaThi { get; set; }
        private List<CustomDeThi>? customDeThis { get; set; }
        private static List<ChiTietBaiThi>? chiTietBaiThis { get; set; }
        private static List<ChiTietBaiThi>? dsBaiThi_Update { get; set; } // lưu ds các câu sv vừa mới trả lời về server
        private static List<int>? cau_da_chons { get; set; } // lưu vết các đáp án đã khoanh trước đó
        private List<int>? cau_da_chons_tagA { get; set; }// lưu vết các đáp án đã khoanh trước đó cho tag Answer button
        private List<string>? alphabet { get; set; }
        public static List<int>? listDapAn { get; set; }// lưu vết các đáp án sinh viên chọn
        private System.Timers.Timer? timer { get; set; }
        private string? displayTime { get; set; }
        private HubConnection? hubConnection { get; set; } // cập nhật tình trạng đang thi, đã hoàn thành thi của thí sinh, ca thi
        private bool is_pause { get; set; } // cập nhật trạng thái dừng ca thi của thí sinh
        private List<bool>? isDisableAudio { get; set; }
        private async Task checkPage()
        {
            if ((myData == null || myData.chiTietCaThi == null || myData.sinhVien == null) && js != null)
            {
                await js.InvokeVoidAsync("alert", "Cách hoạt động trang trang web không hợp lệ. Vui lòng quay lại");
                navManager?.NavigateTo("/info");
                return;
            }
            if(myData != null && myData.chiTietCaThi != null)
            {
                khoiTaoBanDau();
                chiTietCaThi = myData.chiTietCaThi;
                caThi = myData.chiTietCaThi.MaCaThiNavigation;
                sinhVien = myData.sinhVien;
                await Start();
                Time(); // xử lí countdown
            }
        }
        protected override async Task OnInitializedAsync()
        {
            alphabet = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };
            //xác thực người dùng
            var customAuthStateProvider = (authenticationStateProvider != null) ? (CustomAuthenticationStateProvider)authenticationStateProvider : null;
            var token = (customAuthStateProvider != null) ? await customAuthStateProvider.GetToken() : null;
            if (!string.IsNullOrWhiteSpace(token) && httpClient != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
            else
            {
                navManager?.NavigateTo("/");
            }
            await checkPage();
            await base.OnInitializedAsync();
        }
        private async Task onClickNopBai()
        {
            if(js != null)
            {
                var result = await js.InvokeAsync<bool>("confirm", "Bạn có chắc chắn muốn nộp bài?");
                if (result)
                {
                    await UpdateChiTietBaiThi();
                    // Cập nhật cho quản trị viên biết sinh đã hoàn thành bài thi
                    if (isConnectHub() && chiTietCaThi != null && chiTietCaThi.MaCaThi != null)
                        await sendMessage((int)chiTietCaThi.MaCaThi);
                    if (myData != null)
                    {
                        myData.chiTietBaiThis = chiTietBaiThis;
                        myData.listDapAnKhoanh = listDapAn;
                    }
                    navManager?.NavigateTo("/result");
                }
            }
        }
        private async Task ketThucThoiGianLamBai()
        {
            await UpdateChiTietBaiThi();
            // Cập nhật cho quản trị viên biết sinh đã hoàn thành bài thi
            if (isConnectHub() && chiTietCaThi != null && chiTietCaThi.MaCaThi != null)
                await sendMessage((int)chiTietCaThi.MaCaThi);
            if (myData != null)
            {
                myData.chiTietBaiThis = chiTietBaiThis;
                myData.listDapAnKhoanh = listDapAn;
            }
            navManager?.NavigateTo("/result");
        }
        private void khoiTaoBanDau()
        {
            chiTietBaiThis = new List<ChiTietBaiThi>();
            sinhVien = new SinhVien();
            if (myData != null)
                sinhVien = myData.sinhVien;
            caThi = new CaThi();
            chiTietCaThi = new ChiTietCaThi();
            listDapAn = new List<int>();
            cau_da_chons = new List<int>();
            cau_da_chons_tagA = new List<int>();
            dsBaiThi_Update = new List<ChiTietBaiThi>();
            customDeThis = new List<CustomDeThi>();
        }
        private async Task Start()
        {
            is_pause = false;
            await InitialConnectionHub();
            if (myData != null && myData.chiTietCaThi != null)
            {
                chiTietCaThi = myData.chiTietCaThi;
                await getDeThi(chiTietCaThi.MaDeThi);
                await modifyNhomCauHoi();
            }
            chiTietBaiThis = new List<ChiTietBaiThi>();
            isDisableAudio = new List<bool>();
            // Cập nhật cho quản trị viên biết sinh viên đang thi
            if (isConnectHub() && chiTietCaThi != null && chiTietCaThi.MaCaThi != null)
                await sendMessage((int)chiTietCaThi.MaCaThi);
            // Nếu đã vào thi trước đó và treo máy tiếp tục thi thì chỉ lấy lại chi tiet bài thi, ko insert
            if (myData != null && myData.chiTietCaThi != null && myData.chiTietCaThi.DaThi)
            {
                await InsertChiTietBaiThi_DaVaoThiTruocDo();
                ProcessTiepTucThi();
            }
        }
        private void Time()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000; // 1000 = 1ms
            timer.AutoReset = true;
            timer.Enabled = true;
            int tong_so_giay = 0;
            // cập nhật thời gian thi còn lại cho sinh viên nếu bị out
            int? thoi_gian_con_lai = (int?)thoiGianConLaiLooseData();
            if (thoi_gian_con_lai != null)
            {
                // trừ khoảng thời gian sinh đã làm trước đó
                tong_so_giay -= (int)thoi_gian_con_lai;
            }
            if (caThi != null && chiTietCaThi != null)
            {
                tong_so_giay += caThi.ThoiGianThi * 60 + chiTietCaThi.GioCongThem * 60;
                displayTime = FormatTime(tong_so_giay);
            }
            int so_giay_hien_tai = tong_so_giay;
            timer.Elapsed += async (sender, e) =>
            {
                so_giay_hien_tai--;
                // cứ mỗi n giây thì hệ thống tự động lưu bài của SV
                if(so_giay_hien_tai % GIAY_CAP_NHAT == 0)
                {
                    await UpdateChiTietBaiThi();
                }
                if (so_giay_hien_tai >= 0)
                {
                    displayTime = FormatTime(so_giay_hien_tai);
                    await InvokeAsync(StateHasChanged); // Cập nhật giao diện người dùng
                }
                else
                {
                    timer.Stop(); // Dừng timer khi countdown kết thúc
                    await ketThucThoiGianLamBai();
                }
            };
        }

        // Xử lí việc thí sinh bị out ra khi đang làm bài
        private void ProcessTiepTucThi()
        {
            DateTime? thoi_gian = chiTietBaiThis?.Max(p => p.NgayCapNhat);
            thoi_gian = thoi_gian?.AddSeconds(GIAY_CAP_NHAT);
            if(chiTietBaiThis != null && thoi_gian != null)
            {
                foreach(var item in chiTietBaiThis)
                {
                    if(item.CauTraLoi != null && customDeThis != null && cau_da_chons != null && cau_da_chons_tagA != null)
                    {
                        cau_da_chons.Add((int)item.CauTraLoi);
                        int STT = 1;
                        foreach (var chiTietDeThi in customDeThis)
                        {
                            if(chiTietDeThi.MaNhom == item.MaNhom && chiTietDeThi.MaCauHoi == item.MaCauHoi)
                                cau_da_chons_tagA.Add(STT);

                            // cập nhật lại danh sách sinh viên đã khoanh
                            if (listDapAn != null)
                                listDapAn.Add((int)item.CauTraLoi);
                            STT++;
                        }
                    }
                }
            }
        }
        private double? thoiGianConLaiLooseData()
        {
            TimeSpan? result = null;
            if(chiTietCaThi != null && chiTietCaThi.DaHoanThanh == false && chiTietCaThi.DaThi == true && chiTietBaiThis?.Count != 0)
            {
                DateTime? thoi_gian_luu_lan_cuoi = chiTietBaiThis?.Max(p => p.NgayCapNhat);
                DateTime? thoi_gian_bat_dau_thi = chiTietBaiThis?.Min(p => p.NgayTao);
                result = thoi_gian_luu_lan_cuoi - thoi_gian_bat_dau_thi;
            }
            return result?.TotalSeconds;
        }
        private string FormatTime(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";// format từ giây sang phút/giây với D2 là 2 số nguyên
        }
        public void Dispose()
        {
            timer?.Dispose();
            hubConnection?.DisposeAsync();
        }
        private void dungThoiGian() => timer?.Stop();
        private void tiepTucThoiGian() => timer?.Start();

    }
}
