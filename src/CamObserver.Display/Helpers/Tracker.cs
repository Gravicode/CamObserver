﻿using DepthAI.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamObserver.Display.Helpers
{
    public record RecordedObject(long No, DateTime Waktu, string Jenis);
    public class TrackedObject
    {
        static Random rnd = new Random(Environment.TickCount);
        public Color Col { get; set; }
        public string Id { get; set; }
        public PointF Location { get; set; }
        public string Label { get; set; }
        public List<PointF> Trails { get; set; }
        public bool HasBeenCounted { get; set; } = false;

        public DateTime LastUpdate { get; set; }

        public TrackedObject()
        {
            Col = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
            Id = Guid.NewGuid().ToString();
            Trails = new List<PointF>();
            LastUpdate= DateTime.Now;   
        }

        public void Update(PointF location)
        {
            Trails.Add(this.Location);
            this.Location = location;
            this.LastUpdate = DateTime.Now;
        }
    }
    public class Tracker
    {
        public static int DistanceLimit = 100; //in pixel
        public static int TimeLimit = 5; //in seconds
        public List<TrackedObject> TrackedList;
        ConcurrentBag<RecordedObject> table;
        //DataTable table = new DataTable("counter");
        public Tracker()
        {
            TrackedList = new List<TrackedObject>();
            table = new();
            /*
            table.Columns.Add("No");
            table.Columns.Add("Waktu");
            table.Columns.Add("Jenis");
            table.AcceptChanges();*/
        }
        public void ClearLogTable()
        {
            table.Clear();
            //table.Rows.Clear();
        }
        public List<RecordedObject> GetLogTable()
        {
            return table.ToList();
        }
        public void SaveToLog()
        {
            
            string FileName = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/log_{DateTime.Now.ToString("yyyy_MM_dd")}.csv";
            Logger.SaveAsCsv(table.ToList(), FileName);
        }

        public void Process(IReadOnlyList<ObjectInfo> Targets,Rectangle SelectArea)
        {
            HashSet<string> Existing = new HashSet<string>();
            bool IsAdded = false;
            foreach (var newItem in Targets)
            {
                //P1.X;// BoundingBox[0];
                //P1.Y;//BoundingBox[1];
                //P2.X;//BoundingBox[2];
                //P2.Y;//BoundingBox[3];
                IsAdded = true;
                //get center point of an object
                var pos = new PointF(newItem.P1.X + Math.Abs((newItem.P1.X - newItem.P2.X) /2), newItem.P1.Y + Math.Abs((newItem.P2.Y - newItem.P1.Y) / 2));
                //search the closest tracked objects with the new point
                var selTarget = TrackedList.Where(x=> Distance(pos, x.Location)<DistanceLimit && !Existing.Contains(x.Id) && newItem.Label == x.Label).FirstOrDefault();
                if (selTarget != null)
                {
                    //tambah ke existing and update
                    Existing.Add(selTarget.Id);
                    selTarget.Update(pos);
                    IsAdded = false;

                }else
                {
                    //treat as new tracked object
                    var newObj = new TrackedObject() { Location = pos, Label = newItem.Label };
                    TrackedList.Add(newObj);
                    Existing.Add(newObj.Id);
                }
                
            }
            //remove unmoved/static object, object that doesn't move more than time limit
            var now = DateTime.Now;
            var removes = TrackedList.Where(x=>TimeGapInSecond(now,x.LastUpdate)>TimeLimit).ToList();
            foreach(var item in removes)
            {
                TrackedList.Remove(item);
            }
            //count, if object is inside the select area, give it flag
            foreach(var item in TrackedList)
            {
                if(SelectArea.Contains(new Point((int) item.Location.X, (int)item.Location.Y)) && !item.HasBeenCounted)
                {
                    /*
                    var newRow = table.NewRow();
                    newRow["No"] = table.Rows.Count + 1;
                    newRow["Waktu"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    newRow["Jenis"] = item.Label;
                    table.Rows.Add(newRow);
                    */
                    var newRow = new RecordedObject(table.Count + 1, DateTime.Now, item.Label);
                   
                    table.Add(newRow);
                    item.HasBeenCounted = true;
                }
            }
        }

        double TimeGapInSecond(DateTime dt1,DateTime dt2)
        {
            var ts = dt1 - dt2;
            return ts.TotalSeconds;
        }

        double Distance(PointF p1, PointF p2)
        {
            var distance = Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
            return distance;
        }
    }
}
