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
    public partial class UserControlRoom : UserControl
    {
        DataBase dataBase = new DataBase();
        private string roomNumber = "";
        public UserControlRoom()
        {
            InitializeComponent();
        }
        private void tabPageAddRoom_Leave(object sender, EventArgs e)
        {
            Clear();
        }

        private void tabPageSearchRoom_Leave(object sender, EventArgs e)
        {
            textBoxSearchRoom.Clear();
        }

        private void tabPageUpdateandDeleteRoom_Leave(object sender, EventArgs e)
        {
            Clear1();
        }

        private void tabControlRoom_Enter(object sender, EventArgs e)
        {
            ShowUpdatingsInDataGridViewTable();
        }

        public void Clear()
        {
            comboBoxType.SelectedIndex = -1;
            textBoxPhoneNumber.Clear();
            textBoxFree.Clear();
            tabControlRoom.SelectedTab = tabPageAddRoom;
        }
        private void Clear1()
        {
            comboBoxType1.SelectedIndex = -1;
            textBoxPhoneNumber1.Clear();
            textBoxFree1.Clear();
            textBoxOldPhone.Clear();
            roomNumber = "";
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (comboBoxType.SelectedIndex==-1 || textBoxPhoneNumber.Text.Trim() == string.Empty )
                MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                try
                {
                    DataTable table = new DataTable();
                    String roomType = comboBoxType.SelectedItem.ToString();
                    String roomPhoneNumer = textBoxPhoneNumber.Text;
                    String roomFree = textBoxFree.Text;
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    MySqlCommand commandToSelectDataFromDataBaseTable = new MySqlCommand("select * from `room_table` WHERE `room_type` = @type " +
                    "AND `room_phone` = @roomphone AND `room_free` = @isfree", dataBase.GetConnection());
                    commandToSelectDataFromDataBaseTable.Parameters.Add("@type", MySqlDbType.VarChar).Value = roomType;
                    commandToSelectDataFromDataBaseTable.Parameters.Add("@roomphone", MySqlDbType.VarChar).Value = roomPhoneNumer;
                    commandToSelectDataFromDataBaseTable.Parameters.Add("@isfree", MySqlDbType.VarChar).Value = roomFree;
                    adapter.SelectCommand = commandToSelectDataFromDataBaseTable;
                    adapter.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        MessageBox.Show("Room already exists!", "Room information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        dataBase.OpenConnection();
                        MySqlCommand commandToInsertDataIntoDataBaseTable = new MySqlCommand("insert into `room_table`" +
                        " (`room_type`,`room_phone`,`room_free`) " + "VALUES (@type,@roomphone,@isfree)", dataBase.GetConnection());
                        commandToInsertDataIntoDataBaseTable.Parameters.Add("@type", MySqlDbType.VarChar).Value = comboBoxType.SelectedItem.ToString();
                        commandToInsertDataIntoDataBaseTable.Parameters.Add("@roomphone", MySqlDbType.VarChar).Value = textBoxPhoneNumber.Text;
                        commandToInsertDataIntoDataBaseTable.Parameters.Add("@isfree", MySqlDbType.VarChar).Value = textBoxFree.Text;
                        if (commandToInsertDataIntoDataBaseTable.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Success! Room was added!");
                        }
                        else
                        {
                            MessageBox.Show("Error!");
                        }
                        dataBase.CloseConnection();
                        ShowUpdatingsInDataGridViewTable();
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dataBase.CloseConnection();
                }
            }
            
        }

        private void dataGridViewUser_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridViewRoom.Rows[e.RowIndex];
                roomNumber = row.Cells[0].Value.ToString();
                textBoxOldPhone.Text= row.Cells[2].Value.ToString();
                comboBoxType1.Text = row.Cells[1].Value.ToString();
                textBoxPhoneNumber1.Text = row.Cells[2].Value.ToString();
                textBoxFree1.Text = row.Cells[3].Value.ToString();
            }
        }
        private void textBoxSearchRoom_TextChanged(object sender, EventArgs e)
        {
            (dataGridViewRoom.DataSource as DataTable).DefaultView.RowFilter = string.Format("room_phone LIKE '{0}%'", textBoxSearchRoom.Text);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (roomNumber != "")
            {
                if (comboBoxType1.SelectedIndex == -1 || textBoxPhoneNumber1.Text.Trim() == string.Empty)
                    MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    try
                    {
                        String queryToUpdateRoom = "update room_table set room_type='" + this.comboBoxType1.SelectedItem.ToString() +
                            "' , room_phone='" + this.textBoxPhoneNumber1.Text + "' ,room_free='" + this.textBoxFree1.Text +
                            "' where room_phone='" + this.textBoxOldPhone.Text + " ';";
                        MySqlCommand commandToUpdateRoom = new MySqlCommand(queryToUpdateRoom, dataBase.GetConnection());
                        MySqlDataReader readerForUpdatingRoom;
                        dataBase.OpenConnection();
                        readerForUpdatingRoom = commandToUpdateRoom.ExecuteReader();
                        MessageBox.Show("Room information was updated");
                        while (readerForUpdatingRoom.Read())
                        {
                        }
                        dataBase.CloseConnection();
                        ShowUpdatingsInDataGridViewTable();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        dataBase.CloseConnection();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please first select room from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (roomNumber != "")
            {
                try
                {
                    dataBase.OpenConnection();
                    string queryToDeleteRoom = "delete from room_table where room_phone='" + this.textBoxOldPhone.Text + " ';";
                    MySqlCommand commandToDeleteRoom = new MySqlCommand(queryToDeleteRoom, dataBase.GetConnection());
                    MySqlDataReader readerForDeletingRoom;
                    
                    readerForDeletingRoom = commandToDeleteRoom.ExecuteReader();
                    MessageBox.Show("Room information was deleted");
                    while (readerForDeletingRoom.Read())
                    {
                    }
                    dataBase.CloseConnection();
                    ShowUpdatingsInDataGridViewTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    dataBase.CloseConnection();
                }
            }
            else
            {
                MessageBox.Show("Please first select room from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ShowUpdatingsInDataGridViewTable()
        {
            try
            {
                string queryToSelectFromDataBaseTable = "select * from room_table;";
                MySqlCommand commandToShowUpdatings = new MySqlCommand(queryToSelectFromDataBaseTable, dataBase.GetConnection());
                dataBase.OpenConnection();
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = commandToShowUpdatings;
                DataTable dataTable = new DataTable();
                MyAdapter.Fill(dataTable);
                dataGridViewRoom.DataSource = dataTable;
                dataBase.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dataBase.CloseConnection();
            }
        }


    }
}
