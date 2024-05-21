using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Data.SqlClient;

namespace Pedi_Management_System
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=D:\\S C# WORK\\PEDI MS\\Pedi Management System\\db file\\trial001.db;Version=3;";

        public Form1()
        {
            InitializeComponent();

            // Set the passwordTextBox to show '*' for each character
            passwordTextBox.PasswordChar = '*';
            // Set the MaxLength for the passwordTextBox
            passwordTextBox.MaxLength = 12;
        }


        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string input1 = usernameTextBox.Text;
            string input2 = passwordTextBox.Text;
            
            usernameTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;

        }

        private void label4_Click(object sender, EventArgs e)
        {
            signUp signUp = new signUp();
            signUp.ShowDialog();

            /*
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Check if the users table exists, if not, create it
                using (SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT, Password TEXT)", connection))
                {
                    command.ExecuteNonQuery();
                }

                // Insert user data into the Users table
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", connection))
                {
                    command.Parameters.AddWithValue("@Username", usernameTextBox.Text);
                    command.Parameters.AddWithValue("@Password", passwordTextBox.Text);

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("User signed up successfully!");
            }

            */
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Check if the user with the given username exists
            if (UserDoesNotExist(usernameTextBox.Text))
            {
                MessageBox.Show("User does not exist. Please sign up first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Validate password and username 
            if (IsValidPassword(passwordTextBox.Text) && IsValidUsername(usernameTextBox.Text))
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Retrieve user data from the Users table for login
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Users WHERE Username=@Username AND Password=@Password", connection))
                {
                    command.Parameters.AddWithValue("@Username", usernameTextBox.Text);
                    command.Parameters.AddWithValue("@Password", passwordTextBox.Text);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Successful login
                            MessageBox.Show("Login successful!");

                            // Open MenuForm and close the current login form
                            MenuForm menuForm = new MenuForm();
                            menuForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            // Failed login
                            MessageBox.Show("Invalid username or password!");
                        }
                    }
                }
            }
            }

            else
            {
                MessageBox.Show("Invalid username or password format!" +
                    "     Username (Only Text and Digits)" +
                    "     Password (Only text) to have max 12 characters");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                passwordTextBox.UseSystemPasswordChar = false;
            }
            else {
                passwordTextBox.UseSystemPasswordChar = true;
            }

        }
        private bool IsValidUsername(string username)
        {
            // Username should only contain words and digits
            return !string.IsNullOrWhiteSpace(username) && username.All(char.IsLetterOrDigit);
        }
        private bool IsValidPassword(string password)
        {
            // Password should not be blank, have a 12 character limit, and contain only string characters
            return !string.IsNullOrWhiteSpace(password) && password.Length <= 12 && password.All(char.IsLetterOrDigit);
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            passwordTextBox.UseSystemPasswordChar = true;
        }
        private bool UserDoesNotExist(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Check if the user with the given username exists in the Users table
                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username=@Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count == 0;
                }
            }
        }
    }
}
