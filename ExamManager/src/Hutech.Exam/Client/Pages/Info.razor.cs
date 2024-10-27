using Hutech.Exam.Client.DAL;
using Hutech.Exam.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Hutech.Exam.Client.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.AspNetCore.SignalR.Client;

namespace Hutech.Exam.Client.Pages
{
    public partial class Info
    {
        private static int THOI_GIAN_TREN_DUOI_THI = 1000000; // 15 phút
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
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }
        private SinhVien? sinhVien { get; set; }
        private CaThi? caThi { get; set; }
        private MonHoc? monHoc { get; set; }
        private List<ChiTietCaThi>? chiTietCaThis { get; set; }
        string selectoption_cathi = "";
        private System.Timers.Timer? timer { get; set; }
        private string? displayTime { get; set; }
        private ChiTietCaThi? selectedCTCaThi { get; set; }
        private HubConnection? hubConnection { get; set; }
        protected override async Task OnInitializedAsync()
        {
            //xác thực người dùng
            var customAuthStateProvider = (authenticationStateProvider!= null) ? (CustomAuthenticationStateProvider)authenticationStateProvider: null;
            var token = (customAuthStateProvider!= null) ? await customAuthStateProvider.GetToken() : null;
            if (!string.IsNullOrWhiteSpace(token) && httpClient != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
            else
            {
                navManager?.NavigateTo("/");
            }
            await Start();
            Time();
            await base.OnInitializedAsync();
        }
        private async Task getThongTinChiTietCaThi(long ma_sinh_vien)
        {
            // kiểm tra tham số
            if (ma_sinh_vien == -1)
                return;
            HttpResponseMessage? response = null;
                if(httpClient != null)
                    response = await httpClient.GetAsync($"api/Info/GetThongTinChiTietCaThi?ma_sinh_vien={ma_sinh_vien}");
            if (response != null && response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                chiTietCaThis = JsonSerializer.Deserialize<List<ChiTietCaThi>>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if(chiTietCaThis != null && myData != null)
                    sinhVien = myData.sinhVien = chiTietCaThis[0]?.MaSinhVienNavigation;
            }
        }
        private void onClickCaThi(ChiTietCaThi chiTietCaThi)
        {
            if(myData != null)
            {
                myData.chiTietCaThi = selectedCTCaThi = chiTietCaThi;
            }

        }
        private async Task onClickDangXuat()
        {
            bool result = (js != null) && await js.InvokeAsync<bool>("confirm", "Bạn có chắc chắn muốn đăng xuất?");
            if (result && authenticationStateProvider != null)
            {
                await UpdateLogout();
                // Cập nhật cho quản trị viên biết sinh viên đã đăng xuất
                if (isConnectHub() && sinhVien != null)
                    await sendMessage(sinhVien.MaSinhVien);
                var customAuthStateProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
                await customAuthStateProvider.UpdateAuthenticationState(null);
                navManager?.NavigateTo("/", true);
            }
        }
        private async Task UpdateLogout()
        {
            if(httpClient != null && myData != null && myData.sinhVien != null)
                await httpClient.GetAsync($"api/User/UpdateLogout?ma_sinh_vien={myData.sinhVien.MaSinhVien}");
        }
        private async Task OnClickBatDauThi()
        {
            if (!CheckRadioButton() && js!= null)
            {
                await js.InvokeVoidAsync("alert", "Vui lòng chọn ca thi!");
                return;
            }
            if (caThi != null && (caThi.IsActivated == false || caThi.KetThuc == true) && js!= null)
            {
                await js.InvokeVoidAsync("alert", "Ca thi này hiện chưa được kích hoạt hoặc dừng tạm thời. Vui lòng liên hệ quản trị để kích hoạt ca thi");
                return;
            }
            DateTime currentTime = DateTime.Now;
            if (caThi != null && js != null && (caThi.ThoiGianBatDau.AddMinutes(THOI_GIAN_TREN_DUOI_THI) <= currentTime || caThi.ThoiGianBatDau.AddMinutes(-THOI_GIAN_TREN_DUOI_THI) >= currentTime))
            {
                await js.InvokeVoidAsync("alert", "Ca thi này hiện chưa đến thời gian làm bài hoặc đã quá giờ làm. Vui lòng thí sinh chờ đợi đến giờ thi");
                return;
            }
            await HandleUpdateBatDau();
            if (js != null)
                await js.InvokeVoidAsync("alert", "Bắt đầu thi.Chúc bạn sớm hoàn thành kết quả tốt nhất");
                navManager?.NavigateTo("/exam");
        }
        private async Task HandleUpdateBatDau()
        {
            if(selectedCTCaThi != null)
                selectedCTCaThi.ThoiGianBatDau = DateTime.Now;
            var jsonString = JsonSerializer.Serialize(selectedCTCaThi);
            if(httpClient != null)
                await httpClient.PostAsync("api/Info/UpdateBatDauThi", new StringContent(jsonString, Encoding.UTF8, "application/json"));
        }
        private async Task Start()
        {
            await initialHubConnection();
            sinhVien = new SinhVien();
            caThi = new CaThi();
            displayTime = DateTime.Now.ToString("hh:mm:ss tt");
            chiTietCaThis = new List<ChiTietCaThi>();
            selectedCTCaThi = new ChiTietCaThi();
            var authState = (authenticationState!= null) ? await authenticationState : null;
            // lấy thông tin mã sinh viên từ claim
            long ma_sinh_vien = -1;
            // chuyển đổi string thành long
            if(authState!= null && authState.User.Identity != null)
                long.TryParse(authState.User.Identity.Name, out ma_sinh_vien);
            await getThongTinChiTietCaThi(ma_sinh_vien);
        }
        private void Time()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000; // 1000 = 1ms
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += (sender, e) =>
            {
                displayTime = DateTime.Now.ToString("hh:mm:ss tt");
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            };
        }
        public void Dispose()
        {
            if(timer != null)
                timer.Dispose();
            hubConnection?.DisposeAsync();
        }
        private bool CheckRadioButton()
        {
            return !string.IsNullOrEmpty(selectoption_cathi);
        }
        private void RadioChanged(ChangeEventArgs e)
        {
            selectoption_cathi = "true";
        }
        private async Task initialHubConnection()
        {
            if (navManager != null)
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(navManager.ToAbsoluteUri("/ChiTietCaThiHub"))
                    .Build();
                hubConnection.On("ReceiveMessage", () =>
                {
                    callLoadData();
                    StateHasChanged();
                });
                hubConnection.On<long>("ReceiveMessageResetLogin", (ma_so_sv) =>
                {
                    if (sinhVien != null && ma_so_sv == sinhVien.MaSinhVien)
                        resetLogin();
                });
                await hubConnection.StartAsync();
            }
        }
        private void callLoadData()
        {
            Task.Run(async () =>
            {
                if(sinhVien != null)
                    await getThongTinChiTietCaThi(sinhVien.MaSinhVien);
                StateHasChanged();
            });
        }
        private void resetLogin()
        {
            Task.Run(async () =>
            {
                if(authenticationStateProvider != null)
                {
                    var customAuthStateProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
                    await customAuthStateProvider.UpdateAuthenticationState(null);
                    navManager?.NavigateTo("/", true);
                }
            });
        }
        private bool isConnectHub() => hubConnection?.State == HubConnectionState.Connected;

        private async Task sendMessage(long ma_sinh_vien)
        {
            if (hubConnection != null)
                await hubConnection.SendAsync("SendMessageMSV", ma_sinh_vien);
        }
    }
}
