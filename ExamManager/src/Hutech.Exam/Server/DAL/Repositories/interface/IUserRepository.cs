using Hutech.Exam.Server.DAL.DataReader;
using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IUserRepository
    {
        public IDataReader SelectOne(Guid userId);
        public IDataReader SelectByLoginName(string loginName);
        public IDataReader Login(string loginName);
        public bool Update(Guid userId, string? loginName, string? username, string? email, string? name, bool? isDeleted, bool? isLockedOut,
            DateTime? lastActivityDate, DateTime? lastLoginDate, DateTime? lastLockedOutDate, int? failedPwdAttemptCount,
            DateTime? failedPwdAttemptWindowStart, string? comment);
    }
}
