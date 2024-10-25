using Microsoft.AspNetCore.SignalR;
namespace Hutech.Exam.Server.Hubs
{
    public class ChiTietCaThiHub : Hub
    {
        // hàm yêu cầu load lại dữ liệu
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage");
        }
        // value ma_sinh_vien để cập nhật thông tin sinh viên đã đăng nhập hay chưa đăng nhập
        public async Task SendMessageMSV(long ma_sinh_vien)
        {
            await Clients.All.SendAsync("ReceiveMessageMSV", ma_sinh_vien);
        }
        // value ma_chi_tiet_ca_thi để cập nhật thông tin của bất kì 1 thí sinh nào đang vào thi hãy đã thi hoàn tất
        public async Task SendMessageMCT(int ma_ca_thi)
        {
            await Clients.All.SendAsync("ReceiveMessageMCT", ma_ca_thi);
        }
        // trường hợp dừng ca thi, thông báo cho thí sinh biết hoặc load lại tình trạng ca thi khi có sự thay đổi tình trạng ca thi
        public async Task SendMessageStatusCaThi(int ma_ca_thi)
        {
            await Clients.All.SendAsync("ReceiveMessageStatusCaThi", ma_ca_thi);
        }
        // hàm kiểm soát khi người giám sát reset tài khoản trên các máy đang đang nhập 
        public async Task SendMessageResetLogin(long ma_sinh_vien)
        {
            await Clients.All.SendAsync("ReceiveMessageResetLogin", ma_sinh_vien);
        }
    }
}
