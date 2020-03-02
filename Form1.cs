using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModbusAppMKK.Properties;

namespace ModbusAppMKK
{
    public partial class Form1 : Form
    {
        ushort num;
        bool button = false;
        string vares;
        string message { get; set; }
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = Settings.Default["ipAdd"].ToString();
            textBox2.Text = Settings.Default["tcpP"].ToString();
        }
        public Form1(string mess)
        {
            message = mess;
        }
        public void Alarm(string alarm)
        {
            label5.Text = alarm;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            Thread thread = new Thread(process);
            thread.Start();
            
            
        }
       

        private void button1_Click(object sender, EventArgs e)
        {
            button = !button;
            if (button) button1.BackColor = Color.LimeGreen;
            else button1.BackColor = Color.Gray;
        }
        private void process()
        {
           
                if (button)
                {
                    
                      Action action = () =>
                      {
                      try
                      {
                        ModbusMy RT = new ModbusMy(textBox1.Text, Int32.Parse(textBox2.Text));
                        RT.btStart();
                        num = RT.Run();
                        label3.Text = num.ToString();  
                      }
                      catch (Exception ex)
                      {
                      label5.Text = "Нет подключения " + ex.Message;
                      }
                      };
                    Invoke(action);
                  if (vares != label3.Text)
                  {
                    vares = label3.Text;
                    MysqlTitle Cd = new MysqlTitle(label3.Text);
                    Cd.Query();
                  }
                }
                
            
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.Default["ipAdd"] = textBox1.Text;
            Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Settings.Default["tcpP"] = textBox2.Text;
            Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            List<string[]> Table = new List<string[]>();
            MySQLTable Seter = new MySQLTable(dateTimePicker1.Text);
            Table = Seter.SetQuery();
            foreach (string[] s in Table)
                dataGridView1.Rows.Add(s);
            label6.Text = dateTimePicker1.Text;
        }
    }
}
