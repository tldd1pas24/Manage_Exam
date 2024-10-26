using Hutech.Exam.Server.DAL.DataReader;
using Hutech.Exam.Shared.Models;
using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        public IDataReader SelectOne(Guid userId)
        {
            DatabaseReader sql = new DatabaseReader("User_SelectOne");
            sql.SqlParams("@UserId", SqlDbType.UniqueIdentifier, userId);
            return sql.ExcuteReader();
        }
        public IDataReader SelectByLoginName(string loginName)
        {
            DatabaseReader sql = new DatabaseReader("User_SelectByLoginName");
            sql.SqlParams("@LoginName", SqlDbType.NVarChar, loginName);
            return sql.ExcuteReader();
        }
        public IDataReader Login(string loginName)
        {
            DatabaseReader sql = new DatabaseReader("User_Login");
            sql.SqlParams("@LoginName", SqlDbType.NVarChar, loginName);
            return sql.ExcuteReader();
        }
        public bool Update(Guid userId,string? loginName, string? username, string? email, string? name, bool? isDeleted, bool? isLockedOut, 
            DateTime? lastActivityDate, DateTime? lastLoginDate, DateTime? lastLockedOutDate, int? failedPwdAttemptCount, 
            DateTime? failedPwdAttemptWindowStart, string? comment)
        {
            DatabaseReader sql = new DatabaseReader("User_Update");
            sql.SqlParams("@UserId", SqlDbType.UniqueIdentifier, userId);
            sql.SqlParams("@LoginName", SqlDbType.NVarChar, (loginName == null) ? DBNull.Value : loginName);
            sql.SqlParams("@Email", SqlDbType.NVarChar, (username == null) ? DBNull.Value : username);
            sql.SqlParams("Name", SqlDbType.NVarChar, (name == null) ? DBNull.Value : name);
            sql.SqlParams("IsDeleted", SqlDbType.Bit, (isDeleted == null) ? DBNull.Value : isDeleted);
            sql.SqlParams("@IsLockedOut", SqlDbType.Bit, (isLockedOut == null) ? DBNull.Value : isLockedOut);
            sql.SqlParams("@LastActiviyDate", SqlDbType.DateTime, (lastActivityDate == null) ? DBNull.Value : lastActivityDate);
            sql.SqlParams("@LastLoginDate", SqlDbType.DateTime, (lastLoginDate == null) ? DBNull.Value : lastLoginDate);
            sql.SqlParams("@LastLockedOutDate", SqlDbType.DateTime, (lastLockedOutDate == null) ? DBNull.Value : lastLockedOutDate);
            sql.SqlParams("@FailedPwdAttemptCount", SqlDbType.Int, (failedPwdAttemptCount == null) ? DBNull.Value : failedPwdAttemptCount);
            sql.SqlParams("@FailedPwdAttemptWindowStart", SqlDbType.DateTime, (failedPwdAttemptWindowStart == null) ? DBNull.Value : failedPwdAttemptWindowStart);
            sql.SqlParams("Comment", SqlDbType.NText, (comment == null) ? DBNull.Value : comment);
            return sql.ExcuteNonQuery() != 0;
        }
    }
}
