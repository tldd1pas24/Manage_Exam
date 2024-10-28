using Hutech.Exam.Client.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace Hutech.Exam.Client.Pages.Exam
{
    public partial class Exam
    {
        private async Task InitialConnectionHub()
        {
            if (navManager != null && myData != null)
            {
                // mở để cập nhật trạng thái thi của sinh viên
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(navManager.ToAbsoluteUri("/ChiTietCaThiHub"))
                .Build();
                // trường hợp dừng ca thi
                hubConnection.On<int>("ReceiveMessageStatusCaThi", (ma_ca_thi) =>
                {
                    if(chiTietCaThi != null && ma_ca_thi == chiTietCaThi.MaCaThi)
                    {
                        callLoadData();
                        StateHasChanged();
                    }
                });
                hubConnection.On<long>("ReceiveMessageResetLogin", (ma_so_sv) =>
                {
                    if (sinhVien != null && ma_so_sv == sinhVien.MaSinhVien)
                        resetLogin();
                });
                await hubConnection.StartAsync();
            }
        }
        private bool isConnectHub() => hubConnection?.State == HubConnectionState.Connected;


        private async Task sendMessage(int ma_ca_thi)
        {
            if (hubConnection != null)
                await hubConnection.SendAsync("SendMessageMCT", ma_ca_thi);
        }
        private async Task sendMessage(long ma_sinh_vien)
        {
            if (hubConnection != null)
                await hubConnection.SendAsync("SendMessageMSV", ma_sinh_vien);
        }
        private void callLoadData()
        {
            Task.Run(async () =>
            {
                await UpdateChiTietBaiThi();
                bool result = await isActiveCaThi();
                if (!result)
                {
                    js?.InvokeVoidAsync("alert", "Quản trị viên đang tạm thời dừng ca thi này. Thí sinh vui lòng chờ trong giây lát");
                    navManager?.NavigateTo("/info");
                }
            });
        }
        private void resetLogin()
        {
            Task.Run(async () =>
            {
                if (authenticationStateProvider != null)
                {
                    var customAuthStateProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
                    await customAuthStateProvider.UpdateAuthenticationState(null);
                    navManager?.NavigateTo("/", true);
                }
            });
        }
    }
}
