﻿using System.Data;

namespace Hutech.Exam.Server.DAL.Repositories
{
    public interface ICauTraLoiRepository
    {
        public IDataReader SelectOne(int ma_cau_tra_loi);
        public IDataReader SelectBy_MaCauHoi(int ma_cau_hoi);
    }
}
