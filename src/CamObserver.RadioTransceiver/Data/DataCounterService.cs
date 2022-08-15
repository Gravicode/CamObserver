﻿using CamObserver.Models;
using Microsoft.EntityFrameworkCore;
using CamObserver.RadioTransceiver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CamObserver.RadioTransceiver.Data
{
    public class DataCounterService : ICrud<DataCounter>
    {
        CamObserverDB db;

        public DataCounterService()
        {
            if (db == null) db = new CamObserverDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.DataCounters.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.DataCounters.Remove(selData);
            db.SaveChanges();
            return true;
        }


        public Models.CounterInfo GetPersonAndBicycle()
        {
            try
            {
                var person = db.DataCounters.Where(x => x.Objek == "person").Count();
                var bicycle = db.DataCounters.Where(x => x.Objek == "bicycle").Count();
                var newItem = new Models.CounterInfo() { Bicycle = bicycle, Person = person };
                return newItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine("get person fail:"+ex.ToString());
                return default;
            }
            
        }
        public List<DataCounter> FindByKeyword(string Keyword)
        {
            var data = from x in db.DataCounters
                       where x.Objek.Contains(Keyword) || x.Deskripsi.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<DataCounter> GetAllData()
        {
            return db.DataCounters.ToList();
        }

        public DataCounter GetDataById(object Id)
        {
            return db.DataCounters.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(DataCounter data)
        {
            try
            {
                db.DataCounters.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(DataCounter data)
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
            return db.DataCounters.Max(x => x.Id);
        }
    }

}