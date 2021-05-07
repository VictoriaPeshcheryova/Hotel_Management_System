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
using System.Data.SqlClient;
using System.Diagnostics;

namespace Hotel_Management_System
{
    public partial class UserControlReservation : UserControl
    {
        DataBase dataBase = new DataBase();
        private string reservationID = "";
        public UserControlReservation()
        {
            InitializeComponent();
        }
        private void UserControlReservation_Load(object sender, EventArgs e)
        {
            comboBoxType.SelectedIndex = -1;
            comboBoxNo.SelectedIndex = -1;
            comboBoxType1.SelectedIndex = -1;
            comboBoxNo1.SelectedIndex = -1;
        }

        private void TabPageSearchReservation_Enter(object sender, EventArgs e)
        {
            ShowUpdatingsInDataGridViewTable();
        }
        private void TabPageAddReservation_Leave(object sender, EventArgs e)
        {
            Clear();
        }
        private void TabPageUpdateAndCancelReservation_Leave(object sender, EventArgs e)
        {
            Clear1();
        }

        private void TabPageSearchReservation_Leave(object sender, EventArgs e)
        {
            textBoxSearchClientNo.Clear();
        }
        public void Clear()
        {
            comboBoxType.SelectedIndex = -1;
            comboBoxNo.SelectedIndex = -1;
            textBoxClientNo.Clear();
            dateTimePickerIN.Value = DateTime.Now;
            dateTimePickerOUT.Value = DateTime.Now;
            tabControlReservation.SelectedTab = tabPageAddReservation;
        }
        private void Clear1()
        {
            comboBoxType1.SelectedIndex = -1;
            comboBoxNo1.SelectedIndex = -1;
            textBoxCLientNoToUpdate.Clear();
            dateTimePickerIN1.Value = DateTime.Now;
            dateTimePickerOUT1.Value = DateTime.Now;
            textBoxOldClientNo.Clear();
            reservationID = "";
        }
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (comboBoxType.SelectedIndex == -1 || comboBoxNo.SelectedIndex == -1
                || textBoxClientNo.Text.Trim() == string.Empty)
                MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    String room_type = comboBoxType.SelectedItem.ToString();
                    string room_number = comboBoxNo.SelectedItem.ToString();
                    String date_in = dateTimePickerIN.Text;
                    String date_out = dateTimePickerOUT.Text;
                    String client_No = textBoxClientNo.Text;
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    string queryToSelectFromTheTable = "select * from `reservation_table` WHERE `reservation_room_type` = @roomType " +
                    "AND `reservation_room_number` = @roomNum  AND `reservation_in` = @resIn " +
                    " AND `reservation_out` = @resOut  AND `client_no` = @phoneNo";
                    MySqlCommand commandToSelectReservation = new MySqlCommand(queryToSelectFromTheTable, dataBase.GetConnection());
                    commandToSelectReservation.Parameters.Add("@roomType", MySqlDbType.VarChar).Value = room_type;
                    commandToSelectReservation.Parameters.Add("@roomNum", MySqlDbType.VarChar).Value = room_number;
                    commandToSelectReservation.Parameters.Add("@resIn", MySqlDbType.VarChar).Value = date_in;
                    commandToSelectReservation.Parameters.Add("@resOut", MySqlDbType.VarChar).Value = date_out;
                    commandToSelectReservation.Parameters.Add("@phoneNo", MySqlDbType.VarChar).Value = client_No;
                    adapter.SelectCommand = commandToSelectReservation;
                    adapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        MessageBox.Show("We already have this reservation active in database",
                        "Client information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        dataBase.OpenConnection();
                        string queryToAddIntoTable = "insert into `reservation_table` (`reservation_room_type`,`reservation_room_number`," +
                        "`reservation_in`,`reservation_out`,`client_no`) " +
                        "VALUES (@roomType,@roomNum,@resIn, @resOut, @phoneNo)";
                        MySqlCommand commandToAddReservation = new MySqlCommand(queryToAddIntoTable, dataBase.GetConnection());
                        commandToAddReservation.Parameters.Add("@roomType", MySqlDbType.VarChar).Value = room_type;
                        commandToAddReservation.Parameters.Add("@roomNum", MySqlDbType.VarChar).Value = room_number;
                        commandToAddReservation.Parameters.Add("@resIn", MySqlDbType.VarChar).Value = date_in;
                        commandToAddReservation.Parameters.Add("@resOut", MySqlDbType.VarChar).Value = date_out;
                        commandToAddReservation.Parameters.Add("@phoneNo", MySqlDbType.VarChar).Value = client_No;
                        if (commandToAddReservation.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Success! Reservation was added");
                        }
                        else
                        {
                            MessageBox.Show("Error");
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
       private void TextBoxSearchClientNo_TextChanged(object sender, EventArgs e)
        {
            (dataGridViewReservation.DataSource as DataTable).DefaultView.RowFilter = string.Format("client_no LIKE '{0}%'", textBoxSearchClientNo.Text);
        }
        private void dataGridViewReservation_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridViewReservation.Rows[e.RowIndex];
                reservationID = row.Cells[0].Value.ToString();
                comboBoxType1.Text = row.Cells[1].Value.ToString();
                comboBoxNo1.Text = row.Cells[2].Value.ToString();
                dateTimePickerIN1.Text = row.Cells[3].Value.ToString();
                dateTimePickerOUT1.Text = row.Cells[4].Value.ToString();
                textBoxOldClientNo.Text = row.Cells[5].Value.ToString();
                textBoxCLientNoToUpdate.Text = row.Cells[5].Value.ToString();
            }
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (reservationID != "")
            {
                if (comboBoxType1.SelectedIndex == -1 || comboBoxNo1.SelectedIndex == -1)
                    MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    try
                    {
                        dataBase.OpenConnection();
                        String queryToUpdateReservation = "update reservation_table set reservation_room_type='" + this.comboBoxType1.SelectedItem.ToString() +
                        "',reservation_room_number='" + this.comboBoxNo1.Text +
                        "',reservation_in='" + this.dateTimePickerIN1.Text + "',reservation_out='" + this.dateTimePickerOUT1.Text +
                        "',client_no='" + this.textBoxCLientNoToUpdate.Text+
                        "' where client_no='" + this.textBoxOldClientNo.Text + "';";
                        MySqlCommand commandToUpdateReservation = new MySqlCommand(queryToUpdateReservation, dataBase.GetConnection());
                        MySqlDataReader readerForUpdatingReservation;
                        readerForUpdatingReservation = commandToUpdateReservation.ExecuteReader();
                        MessageBox.Show("Reservation was updated");
                        while (readerForUpdatingReservation.Read())
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
                MessageBox.Show("Please first select row from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (reservationID != "")
            {
                try
                {
                    dataBase.OpenConnection();
                    String queryToDeleteReservation = "delete from reservation_table where client_no='" + this.textBoxOldClientNo.Text + "';";
                    MySqlCommand commandToDeleteReservation = new MySqlCommand(queryToDeleteReservation, dataBase.GetConnection());
                    MySqlDataReader readerForUpdatingReservation;
                    readerForUpdatingReservation = commandToDeleteReservation.ExecuteReader();
                    MessageBox.Show("Reservation was cancelled");
                    while (readerForUpdatingReservation.Read())
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
                MessageBox.Show("Please first select row from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        private void ComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxNo.Items.Clear();
            try
            {
                SelectFreeRoomsToShowINComboBoxForAdding();
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
        private void ComboBoxType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxNo1.Items.Clear();
            try
            {
                SelectFreeRoomsToShowINComboBoxForUpdatingOrCanceling();
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
        public void SelectFreeRoomsToShowINComboBoxForAdding()
        {
            try
            {
                dataBase.OpenConnection();
                string queryToAddToComboBox = "select room_number from room_table where room_type='" + comboBoxType.Text + "'" +
                "AND room_free='yes' order by room_number";
                MySqlCommand commandToSelectItemsFromDataBaseTable = new MySqlCommand(queryToAddToComboBox, dataBase.GetConnection());
                MySqlDataReader DR = commandToSelectItemsFromDataBaseTable.ExecuteReader();
                while (DR.Read())
                {
                    comboBoxNo.Items.Add(DR[0]);
                }
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
        public void SelectFreeRoomsToShowINComboBoxForUpdatingOrCanceling()
        {
            try
            {
                dataBase.OpenConnection();
                string queryToAddToComboBox = "select room_number from room_table where room_type='" + comboBoxType1.Text + "'" +
                "AND room_free='yes' order by room_number";
                MySqlCommand commandToSelectItemsFromDataBaseTable = new MySqlCommand(queryToAddToComboBox, dataBase.GetConnection());
                MySqlDataReader DR = commandToSelectItemsFromDataBaseTable.ExecuteReader();
                while (DR.Read())
                {
                    comboBoxNo1.Items.Add(DR[0]);
                }
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
        public void ShowUpdatingsInDataGridViewTable()
        {
            try
            {
                dataBase.OpenConnection();
                string Query = "select * from reservation_table;";
                MySqlCommand commandToShowUpdatings = new MySqlCommand(Query, dataBase.GetConnection());
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = commandToShowUpdatings;
                DataTable dataTable = new DataTable();
                MyAdapter.Fill(dataTable);
                dataGridViewReservation.DataSource = dataTable;
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
