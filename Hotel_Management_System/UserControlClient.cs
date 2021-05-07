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
    public partial class UserControlClient : UserControl
    {
        DataBase dataBase = new DataBase();
        private string clientID = "";
        public UserControlClient()
        {
            InitializeComponent();
        }
        private void tabPageSearchClient_Leave(object sender, EventArgs e)
        {
            textBoxSearchPhoneNumber.Clear();
        } 
        private void tabControlClient_Enter(object sender, EventArgs e)
        {
            ShowUpdatingsInDataGridViewTable();
        }
       
        private void tabPageAddClient_Leave(object sender, EventArgs e)
        {
            Clear();
        }

       
        private void tabPageUpdateAndDeleteClient_Leave(object sender, EventArgs e)
        {
            Clear1();
        } 
       
        public void Clear()
        {
            textBoxFirstName.Clear();
            textBoxLastName.Clear();
            textBoxPhoneNumber.Clear();
            textBoxResID.Clear();
            textBoxAddress.Clear();
            tabControlClient.SelectedTab = tabPageAddClient;

        }
        private void Clear1()
        {
            textBoxFirstName1.Clear();
            textBoxLastName1.Clear();
            textBoxPhoneNumber1.Clear();
            textBoxAddress1.Clear();
            textBoxResID1.Clear();
            textBoxOldClient.Clear();
            clientID = "";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxFirstName.Text.Trim() == string.Empty || textBoxLastName.Text.Trim() == string.Empty
                || textBoxPhoneNumber.Text.Trim() == string.Empty || textBoxAddress.Text.Trim() == string.Empty)
                MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                try 
                { 
                DataTable dataTable = new DataTable();
                String first_name = textBoxFirstName.Text;
                String last_name = textBoxLastName.Text;
                String phone_num = textBoxPhoneNumber.Text;
                String address = textBoxAddress.Text;
                String res_id = textBoxResID.Text;
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand commandToSelectClientsFromDataBaseTable = new MySqlCommand("select * from `client_table`" +
                " WHERE `firstname` = @firstname " +"AND `lastname` = @lastname AND" +
                " `phonenumber` = @phonenumber AND `address` = @address AND `reservation_id` = @id", dataBase.GetConnection());
                commandToSelectClientsFromDataBaseTable.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = first_name;
                commandToSelectClientsFromDataBaseTable.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = last_name;
                commandToSelectClientsFromDataBaseTable.Parameters.Add("@phonenumber", MySqlDbType.VarChar).Value = phone_num;
                commandToSelectClientsFromDataBaseTable.Parameters.Add("@address", MySqlDbType.VarChar).Value = address;
                commandToSelectClientsFromDataBaseTable.Parameters.Add("@id", MySqlDbType.VarChar).Value = res_id;
                adapter.SelectCommand = commandToSelectClientsFromDataBaseTable;
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    MessageBox.Show("We already have this client in database", "Client information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                { 
                    dataBase.OpenConnection();
                    MySqlCommand commandToInsertClientIntoDataBaseTable = new MySqlCommand("insert into `client_table` " +
                    "(`firstname`,`lastname`,`phonenumber`,`address`,`reservation_id`) " +
                    "VALUES (@firstname,@lastname,@phonenumber,@address,@id)", dataBase.GetConnection());
                    commandToInsertClientIntoDataBaseTable.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = first_name;
                    commandToInsertClientIntoDataBaseTable.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = last_name;
                    commandToInsertClientIntoDataBaseTable.Parameters.Add("@phonenumber", MySqlDbType.VarChar).Value = phone_num;
                    commandToInsertClientIntoDataBaseTable.Parameters.Add("@address", MySqlDbType.VarChar).Value = address;
                    commandToInsertClientIntoDataBaseTable.Parameters.Add("@id", MySqlDbType.VarChar).Value = res_id;
                    if (commandToInsertClientIntoDataBaseTable.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Client was added successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error with adding the client!");
                    }

                    dataBase.CloseConnection();
                    ShowUpdatingsInDataGridViewTable(); }
               
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
        private void textBoxSearchPhoneNumber_TextChanged(object sender, EventArgs e)
        {
            (dataGridViewClient.DataSource as DataTable).DefaultView.RowFilter = string.Format("phonenumber LIKE '{0}%'", textBoxSearchPhoneNumber.Text);
        }
        private void dataGridViewClient_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridViewClient.Rows[e.RowIndex];
                clientID = row.Cells[0].Value.ToString();
                textBoxOldClient.Text = row.Cells[1].Value.ToString();
                textBoxFirstName1.Text= row.Cells[1].Value.ToString();
                textBoxLastName1.Text= row.Cells[2].Value.ToString();
                textBoxPhoneNumber1.Text = row.Cells[3].Value.ToString();
                textBoxAddress1.Text = row.Cells[4].Value.ToString();
                textBoxResID1.Text = row.Cells[5].Value.ToString();
            }
        }
       
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (clientID != "")
            {
                if (textBoxFirstName1.Text.Trim() == string.Empty || textBoxLastName1.Text.Trim() == string.Empty
                    || textBoxAddress1.Text.Trim() == string.Empty || textBoxPhoneNumber1.Text.Trim() == string.Empty)
                    MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    try
                    {
                        dataBase.OpenConnection();
                        String queryToUpdateClient = "update client_table set firstname='" + this.textBoxFirstName1.Text +
                        "',lastname='" + this.textBoxLastName1.Text + "',phonenumber='" + this.textBoxPhoneNumber1.Text +
                        "',address='" + this.textBoxAddress1.Text +
                        "',reservation_ID='" + this.textBoxResID1.Text + "' where firstname='" + this.textBoxOldClient.Text + "';";
                        MySqlCommand commandToUpdateClient = new MySqlCommand(queryToUpdateClient, dataBase.GetConnection());
                        MySqlDataReader readerForUpdatingClient;
                        readerForUpdatingClient = commandToUpdateClient.ExecuteReader();
                        MessageBox.Show("Client was updated successfully!");
                        while (readerForUpdatingClient.Read())
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
                MessageBox.Show("Please first select client from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (clientID != "")
            {
                try
                {
                    dataBase.OpenConnection();
                    string queryToDeleteClient = "delete from client_table where firstname='" + this.textBoxOldClient.Text + "';";
                    MySqlCommand commandToDeleteClient = new MySqlCommand(queryToDeleteClient, dataBase.GetConnection());
                    MySqlDataReader readerForDeletingClient;
                    readerForDeletingClient = commandToDeleteClient.ExecuteReader();
                    MessageBox.Show("Client was deleted successfully!");
                    while (readerForDeletingClient.Read())
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
                MessageBox.Show("Please first select client from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void ShowUpdatingsInDataGridViewTable()
        {
            try
            {
                dataBase.OpenConnection(); 
                string queryToSelectFromDataBaseTable = "select * from client_table;";
                MySqlCommand commandToShowUpdatings = new MySqlCommand(queryToSelectFromDataBaseTable, dataBase.GetConnection());
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = commandToShowUpdatings;
                DataTable dataTable = new DataTable();
                MyAdapter.Fill(dataTable);
                dataGridViewClient.DataSource = dataTable;
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
