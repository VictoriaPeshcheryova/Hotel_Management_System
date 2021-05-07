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
    public partial class UserControlSetting : UserControl
    {
        DataBase dataBase = new DataBase();
        private string userID = "";
        public UserControlSetting()
        {
            InitializeComponent();
        } 
        private void tabControlUser_Enter(object sender, EventArgs e)
        {
            ShowUpdatingsInDataGridViewTable();
        }
        private void tabPageAddUser_Leave(object sender, EventArgs e)
        {
            Clear();
        }

        private void tabPageSearchUser_Leave(object sender, EventArgs e)
        {
            textBoxSearchUsername.Clear();
        }

        private void tabPageUpdateAndDeleteUser_Leave(object sender, EventArgs e)
        {
            Clear1();
        }
      
        public void Clear()
        {
            textBoxUsername.Clear();
            textBoxPassword.Clear();
            tabControlUser.SelectedTab = tabPageAddUser;
        }

        private void Clear1()
        {
            textBoxUsername1.Clear();
            textBoxPassword1.Clear();
            textBoxOldUsername.Clear();
            userID = "";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxUsername.Text.Trim() == string.Empty || textBoxPassword.Text.Trim() == string.Empty)
                MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                try
                {
                    DataTable table = new DataTable();
                    String userName = textBoxUsername.Text;
                    String userPassword = textBoxPassword.Text;
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    MySqlCommand commandToSelectFromDataBaseTable = new MySqlCommand("select * from `user_table` WHERE `username` = @name OR" +
                        " `password` = @pass", dataBase.GetConnection());
                    commandToSelectFromDataBaseTable.Parameters.Add("@name", MySqlDbType.VarChar).Value = userName;
                    commandToSelectFromDataBaseTable.Parameters.Add("@pass", MySqlDbType.VarChar).Value = userPassword;
                    adapter.SelectCommand = commandToSelectFromDataBaseTable;
                    adapter.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        MessageBox.Show("Username was already taken!", "Username", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MySqlCommand commandToInsertNewDataToDataBaseTable = new MySqlCommand("insert into `user_table` (`username`,`password`)" +
                            " VALUES (@name,@pass)", dataBase.GetConnection());
                        commandToInsertNewDataToDataBaseTable.Parameters.Add("@name", MySqlDbType.VarChar).Value = textBoxUsername.Text;
                        commandToInsertNewDataToDataBaseTable.Parameters.Add("@pass", MySqlDbType.VarChar).Value = textBoxPassword.Text;
                        dataBase.OpenConnection();
                        if (commandToInsertNewDataToDataBaseTable.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Success! You have been registered!");
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
                DataGridViewRow row = dataGridViewUser.Rows[e.RowIndex];
                userID = row.Cells[0].Value.ToString();
                textBoxOldUsername.Text = row.Cells[1].Value.ToString();
                textBoxUsername1.Text = row.Cells[1].Value.ToString();
                textBoxPassword1.Text= row.Cells[2].Value.ToString();

            }
        }

        private void textBoxSearchUsername_TextChanged(object sender, EventArgs e)
        {
            (dataGridViewUser.DataSource as DataTable).DefaultView.RowFilter = string.Format("username LIKE '%{0}%'", textBoxSearchUsername.Text);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (userID != "")
            {
                if (textBoxUsername1.Text.Trim() == string.Empty || textBoxPassword1.Text.Trim() == string.Empty)
                    MessageBox.Show("Please fill every field!", "Required field", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    try
                    {
                        String queryToUpdateUser = "update user_table set username='" + this.textBoxUsername1.Text + "'," +
                            "password='" + this.textBoxPassword1.Text + "' where username='" + this.textBoxOldUsername.Text + "';";
                        MySqlCommand commandToUpdateUser = new MySqlCommand(queryToUpdateUser, dataBase.GetConnection());
                        MySqlDataReader readerForUpdatingUser;
                        dataBase.OpenConnection();
                        readerForUpdatingUser = commandToUpdateUser.ExecuteReader();
                        MessageBox.Show("User was updated successfully!");
                        while (readerForUpdatingUser.Read())
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
                MessageBox.Show("Please first select user from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (userID != "")
            {
                try
                {
                    string queryToDeleteUser = "delete from user_table where username='" + this.textBoxOldUsername.Text + "';";
                    MySqlCommand commandToDeleteUser = new MySqlCommand(queryToDeleteUser, dataBase.GetConnection());
                    MySqlDataReader readerForDeletingUser;
                    dataBase.OpenConnection();
                    readerForDeletingUser = commandToDeleteUser.ExecuteReader();
                    MessageBox.Show("User was deleted successfully!");
                    while (readerForDeletingUser.Read())
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
                MessageBox.Show("Please first select user from table", "Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ShowUpdatingsInDataGridViewTable()
        {
            try
            {
                string queryToPasteInfoFromDataBaseToDataGrid = "select * from user_table;";
                MySqlCommand commandToShowUpdatingsInDataGridViewTable = 
                    new MySqlCommand(queryToPasteInfoFromDataBaseToDataGrid, dataBase.GetConnection());
                dataBase.OpenConnection();
                MySqlDataAdapter myAdapter = new MySqlDataAdapter();
                myAdapter.SelectCommand = commandToShowUpdatingsInDataGridViewTable;
                DataTable dataTable = new DataTable();
                myAdapter.Fill(dataTable);
                dataGridViewUser.DataSource = dataTable;
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
