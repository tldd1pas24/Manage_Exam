﻿using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface IDotThiRepository
    {
        public IDataReader GetAll();
        public IDataReader SelectOne(int ma_dot_thi);
    }
}
