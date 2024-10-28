using Hutech.Exam.Server.DAL.Repositories;
using Hutech.Exam.Shared.Models;
using System;
using System.Data;

namespace Hutech.Exam.Server.BUS
{
    public class AudioListenedService
    {
        private readonly IAudioListenedRepository _audioListenedRepository;
        public AudioListenedService(IAudioListenedRepository audioListenedRepository)
        {
            _audioListenedRepository = audioListenedRepository;
        }
        private TblAudioListened getProperty(IDataReader dataReader)
        {
            TblAudioListened audioListened = new TblAudioListened();
            audioListened.ListenId = dataReader.GetInt64(0);
            audioListened.MaChiTietCaThi = dataReader.GetInt32(1);
            audioListened.FileName = dataReader.GetString(2);
            audioListened.ListenedCount = dataReader.GetInt32(3);
            return audioListened;
        }
        public int SelectOne(int ma_chi_tiet_ca_thi, string fileName)
        {
            int listenedCount = 0;
            using (IDataReader dataReader = _audioListenedRepository.SelectOne(ma_chi_tiet_ca_thi, fileName))
            {
                if (dataReader.Read())
                {
                    listenedCount = dataReader.GetInt32(0);
                }
            }
            return listenedCount;
        }
        public void Save(int ma_chi_tiet_ca_thi, string fileName)
        {
            try
            {
                _audioListenedRepository.Save(ma_chi_tiet_ca_thi, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
