using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Hotel_Management_System
{
    public partial class UserControlDashBoard : UserControl
    {
        DataBase dataBase = new DataBase();
        public UserControlDashBoard()
        {
            InitializeComponent();
        }
        public void UserAmountInDataBase()
        {

            dataBase.OpenConnection();
            string queryToSelectFromDataBaseTable = "select count(*) as countOfUsers from user_table;";
            MySqlCommand commandToCountUsers = new MySqlCommand(queryToSelectFromDataBaseTable, dataBase.GetConnection());
            int returnValue = int.Parse(commandToCountUsers.ExecuteScalar().ToString());
            labelUserCount.Text = returnValue.ToString();

        }
        public void ClientAmountInDataBase()
        {

            dataBase.OpenConnection();
            string queryToSelectFromDataBaseTable = "select count(*) as countOfClients from client_table;";
            MySqlCommand commandToCountClients = new MySqlCommand(queryToSelectFromDataBaseTable, dataBase.GetConnection());
            int returnValue = int.Parse(commandToCountClients.ExecuteScalar().ToString());
            labelClientCount.Text = returnValue.ToString();

        }
        public void RoomAmountInDataBase()
        {

            dataBase.OpenConnection();
            string queryToSelectFromDataBaseTable = "select count(*) as countOfRooms from room_table;";
            MySqlCommand commandToCountRooms = new MySqlCommand(queryToSelectFromDataBaseTable, dataBase.GetConnection());
            int returnValue = int.Parse(commandToCountRooms.ExecuteScalar().ToString());
            labelRoomCount.Text = returnValue.ToString();

        }

        private void UserControlDashBoard_Load(object sender, EventArgs e)
        {
            UserAmountInDataBase();
            ClientAmountInDataBase();
            RoomAmountInDataBase();
        }
    }

}
