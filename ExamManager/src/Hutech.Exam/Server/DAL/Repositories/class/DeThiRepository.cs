﻿using Hutech.Exam.Server.DAL.DataReader;
using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public class DeThiRepository : IDeThiRepository
    {
        public IDataReader SelectOne(int ma_de_thi)
        {
            DatabaseReader sql = new DatabaseReader("tbl_DeThi_SelectOne");
            sql.SqlParams("@MaDeThi", SqlDbType.Int, ma_de_thi);
            return sql.ExcuteReader();
        }
    }
}
