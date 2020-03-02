using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ModbusAppMKK
{
    public class MysqlTitle
    {
        
        string values;
        string connect = "database=zad_111; server=192.168.99.154; user=user; password= asutpuser";
        public MysqlTitle(string values)
        {
            this.values = values;
        }
        
        public MysqlTitle() { }
        public void Query()
        {
            string command = "INSERT IGNORE INTO `firstprogectC` (`values`) VALUES (" + values + ")";

            MySqlConnection connection = new MySqlConnection(connect);
            MySqlCommand myCommand = new MySqlCommand(command, connection);
            connection.Open();
            myCommand.ExecuteNonQuery();
            connection.Close();
        }
       
    }
}
