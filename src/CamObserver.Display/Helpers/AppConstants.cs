using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CamObserver.Display.Helpers
{
    public static class AppConstants
    {
        public static string GrpcUrl { get; set; }
        public static string Gateway { get; set; }
        public static string Lokasi { get; set; }

        public static string Label { get; set; }
        public static string Weights { get; set; }
        public static string Cfg { get; set; }

        public static string Cctv1 { set; get; }
        public static string CCTVName { set; get; } = "CCTV-001";

        public static int SyncDelay = 1000;
        public static bool AutoStart = false;
        public static List<ObjekStatistik> InfoStat { set; get; } = new();
    }

    public class ObjekStatistik
    {
        public int No { get; set; }
        public string Objek { get; set; }
        public long Jumlah { get; set; }
    }
}
