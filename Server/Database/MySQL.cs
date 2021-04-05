using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AntStats.Avalonia.Database
{
    public class MySQL : IDatabase
    {

       private static void updateData(string connector,string ColumnName,int ColumnId,string Data,string nameTable)
        {
            new Thread(() =>
            {
           
                //Sometimes errors occur when trying to update data. This code is needed to minimize this.
                bool error = false;
                for (int i =0;error||i<10;i++)
                {
                    try
                    {
                        MySqlConnection mySqlConnection = new MySqlConnection(connector);
                        mySqlConnection.Open();
                        string Commondq = $"UPDATE {nameTable} SET {ColumnName}='{Data}' WHERE id = '{ColumnId}'";
                        MySqlCommand command = new MySqlCommand(Commondq,mySqlConnection);
                        command.ExecuteNonQuery();
                        mySqlConnection.Close();
                        error = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error MySQL");
                        error = true;
                    }
                    
                   
                }
                
            }).Start();
          
        }


       private bool tablePresenceInDatabase(string nameTable,string connector,string database)
       {
           try
           {
               MySqlConnection mySqlConnection = new MySqlConnection(connector);
               mySqlConnection.Open();
               MySqlCommand command = new MySqlCommand($"SELECT * FROM {database}."+nameTable,mySqlConnection);

               MySqlDataReader reader = command.ExecuteReader();
               mySqlConnection.Close();
           }
           catch (Exception e)
           {
    
               return false;
           }
         

           return true;
       }








       public void CreateTable(string connector,string nameTable,string database)
       {
           if (tablePresenceInDatabase(nameTable, connector, database) == false)
           {
               string table =
                   $"CREATE TABLE `{database}`.`{nameTable}` (" +
                   "`Chain` VARCHAR(45) NULL," +
                   "`Frequency` VARCHAR(45) NULL," +
                   "`Watts` VARCHAR(45) NULL," +
                   "`GHideal` VARCHAR(45) NULL," +
                   "`GHRT` VARCHAR(45) NULL, " +
                   "`HW` VARCHAR(45) NULL, " +
                   "`TempPCB` VARCHAR(45) NULL, " +
                   "`TempChip` VARCHAR(45) NULL, " +
                   "`Status` VARCHAR(45) NULL," +
                   "`id` INTEGER NOT NULL)";
               
               
               MySqlConnection mySqlConnection = new MySqlConnection(connector);
  
               mySqlConnection.Open();
               MySqlCommand command = new MySqlCommand(table,mySqlConnection);
               command.ExecuteNonQuery();
               mySqlConnection.Close();


               for (int i = 0; i <= 10; i++)
               {
                   string addColumn = $"INSERT INTO `{database}`.`{nameTable}` (`id`) VALUES ('{i}')";
                   mySqlConnection.Open();
                   new MySqlCommand(addColumn,mySqlConnection).ExecuteNonQuery();
                   mySqlConnection.Close();
               }
            
               
               
               
           }


       }



       public AsicStandartStatsObject GetAsicColumnData(string connector,string nameTable)
        {
            AsicStandartStatsObject asicsObject=new AsicStandartStatsObject();
            
            MySqlConnection mySqlConnection = new MySqlConnection(connector);
            
            mySqlConnection.Open();
            MySqlCommand command = new MySqlCommand("SELECT * FROM asic."+nameTable,mySqlConnection);

            MySqlDataReader reader = command.ExecuteReader();

            for (int i=0;reader.Read();i++)
            {   
                AsicColumnClass asicColumnClass = new AsicColumnClass();  
               
                       
                    
                    asicColumnClass.Chain=reader[0].ToString();
                    asicColumnClass.Frequency=reader[1].ToString();
                    asicColumnClass.Watts=reader[2].ToString();
                    asicColumnClass.GHideal=reader[3].ToString();
                    asicColumnClass.GHRT=reader[4].ToString();
                    asicColumnClass.HW= reader[5].ToString();
                    asicColumnClass.TempPCB=reader[6].ToString();
                    asicColumnClass.TempChip=reader[7].ToString();
                    asicColumnClass.Status=reader[8].ToString();

                    if (i == 9)
                    {
                        asicsObject.HashrateAVG = reader[4].ToString();
                        asicsObject.DateTime=reader[0].ToString();
                        asicsObject.ElapsedTime=reader[8].ToString();
                    }

                    
                


                asicsObject.LasicAsicColumnStats.Add(asicColumnClass);
            }
            
            
            mySqlConnection.Close();

            
            
            return asicsObject;
        }

        
        //SELECT * from asic_tabl
        
        
        
        
        
        
        
        
        public void SetAsicColumnData(string connectionString, AsicStandartStatsObject column,string table)
        {
            for (int i = 0; i <= 8; i++)
            {
             
                updateData(connectionString, "Chain", i,column.LasicAsicColumnStats[i].Chain,table);
                updateData(connectionString, "Frequency", i,column.LasicAsicColumnStats[i].Frequency,table);
                updateData(connectionString, "Watts", i,column.LasicAsicColumnStats[i].Watts,table);
                updateData(connectionString, "GHideal", i,column.LasicAsicColumnStats[i].GHideal,table);
                updateData(connectionString, "GHRT", i,column.LasicAsicColumnStats[i].GHRT,table);
                updateData(connectionString, "HW", i,column.LasicAsicColumnStats[i].HW,table);
                updateData(connectionString, "TempPCB", i,column.LasicAsicColumnStats[i].TempPCB,table);
                updateData(connectionString, "TempChip", i,column.LasicAsicColumnStats[i].TempChip,table);
                updateData(connectionString, "Status", i,column.LasicAsicColumnStats[i].Status,table);
              
            } 
            
        

       
            updateData(connectionString, "Status", 9,column.ElapsedTime,table);
            updateData(connectionString, "GHRT", 9,column.HashrateAVG,table);
            updateData(connectionString, "Chain", 9,column.DateTime,table);
            
            
            
        }
    }
}