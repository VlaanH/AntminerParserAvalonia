using System;
using System.Threading;
using System.Threading.Tasks;
using AntStats.Avalonia.Database;

namespace AntStats.Avalonia
{
    public class AsicStats
    {
        private SettingsClass Settings { get; set; }

        public AsicStats(SettingsClass settings)
        {
            Settings = settings;
        }


        public bool CreateDataBaseTable()
        {
          
        
            string connector=$"Server={Settings.DatabaseIP};port={Settings.Port};Database={Settings.DataBaseName};Uid={Settings.DatabaseUser};pwd={Settings.DatabasePass};charset=utf8";

            
           
            Database.MySQL mySql = new MySQL();
                
            return mySql.CreateTable(connector,Settings.NameTable,Settings.DataBaseName,ref ProgressBarCreatingData.CreatingTable);


      
            
        }
        
        
        
        
        
        public AsicStandartStatsObject GetLocalhost()
        {
            var html = ParsingAuthorizationWeb.DownloadString($"http://{Settings.IP}/cgi-bin/minerStatus.cgi", Settings.User,
                Settings.Pass);
            
                
             return Html_In_AsicStandartStatsObject._Convert(html);
            
        }
        public AsicStandartStatsObject GetDataBase()
        {
          
            MySQL mySql = new MySQL();

            string connector=$"Server={Settings.DatabaseIP};port={Settings.Port};Database={Settings.DataBaseName};Uid={Settings.DatabaseUser};pwd={Settings.DatabasePass};charset=utf8";
            
                
            return mySql.GetAsicColumnData(connector,Settings.NameTable);
            
        }

      
        
        public void SetDataBase(AsicStandartStatsObject statsObject)
        {
          
            MySQL mySql = new MySQL();

            string connector=$"Server={Settings.DatabaseIP};port={Settings.Port};Database={Settings.DataBaseName};Uid={Settings.DatabaseUser};pwd={Settings.DatabasePass};charset=utf8";
            
            
          
            mySql.SetAsicColumnData(connector,statsObject,Settings.NameTable,ref ProgressBarCreatingData.CreatingTable);
          
      
            
        }
        
        
        
    }
}