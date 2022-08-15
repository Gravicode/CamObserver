using CamObserver.Models;
using Microsoft.EntityFrameworkCore;

namespace CamObserver.Web.Data
{
    public class WeatherDataService : ICrud<WeatherData>
    {
        CamObserverDB db;

        public WeatherDataService()
        {
            if (db == null) db = new CamObserverDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.WeatherDatas.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.WeatherDatas.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<WeatherData> FindByKeyword(string Keyword)
        {
            var data = from x in db.WeatherDatas
                       where x.Tanggal.ToString().Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<WeatherData> GetAllData()
        {
            return db.WeatherDatas.ToList();
        }

        public WeatherData GetDataById(object Id)
        {
            return db.WeatherDatas.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(WeatherData data)
        {
            try
            {
                db.WeatherDatas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(WeatherData data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.WeatherDatas.Max(x => x.Id);
        }
    }

}