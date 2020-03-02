using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ModbusAppMKK
{
    class MySQLTable
    {
        string Date;
        string connect = "database=zad_111; server=192.168.99.154; user=user; password= asutpuser";
        public MySQLTable(string date)
        {
            Date = date;
        }
        public List<string[]> SetQuery()
        {
            string command = "SELECT * FROM firstprogectC WHERE DTime BETWEEN '" + Date + "' AND '" + Date + "' + INTERVAL 1 DAY";
            MySqlConnection connection = new MySqlConnection(connect);
            connection.Open();
            MySqlCommand myCommand = new MySqlCommand(command, connection);
            MySqlDataReader reader = myCommand.ExecuteReader();
            int i = 1;
            Form1 Table = new Form1();

            List<String[]> data = new List<string[]>();
            while (reader.Read())
            {
                data.Add(new string[3]);
                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
            }
            reader.Close();
            connection.Close();
            //foreach(string[] s in data)
            //Table.dataGridView1.Rows.Add(s);
            return data;
        }
    }
}
