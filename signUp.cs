using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pedi_Management_System
{
    public partial class signUp : Form
    {
        private string connectionString = "Data Source=D:\\S C# WORK\\PEDI MS\\Pedi Management System\\db file\\trial001.db;Version=3;";

        public static string ConnectionString { get; private set; }

        private static void ExecuteNonQuery(string queryString)
        {
            using (var connection = new SQLiteConnection(
                       ConnectionString))
            {
                using (var command = new SQLiteCommand(queryString, connection))
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        public signUp()
        {
            InitializeComponent();
            // Set the passwordTextBox and confirmPasswordTextBox to show '*' for each character
            passwordTextBox.PasswordChar = '*';
            confirmPasswordTextBox.PasswordChar = '*';
            // Set the MaxLength for the passwordTextBox and confirmPasswordTextBox
            passwordTextBox.MaxLength = 12;
            confirmPasswordTextBox.MaxLength = 12;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Clear the textboxes
            usernameTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;
            confirmPasswordTextBox.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrWhiteSpace(usernameTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text) || string.IsNullOrWhiteSpace(confirmPasswordTextBox.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if the passwords match
                if (passwordTextBox.Text != confirmPasswordTextBox.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if the password meets the specified criteria
                if (!IsValidPassword(passwordTextBox.Text))
                {
                    MessageBox.Show("Invalid password format. (Only text and Digits) Max 12 characters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if the username meets the specified criteria
                if (!IsValidUsername(usernameTextBox.Text))
                {
                    MessageBox.Show("Invalid username format. (Only Text and Digits)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if the user with the given username already exists
                if (UserExists(usernameTextBox.Text))
                {
                    MessageBox.Show("Username already exists. Please choose a different username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Check if the Users table exists, if not, create it
                    if (!TableExists(connection, "Users"))
                    {
                        using (SQLiteCommand createTableCommand = new SQLiteCommand("CREATE TABLE Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT, Password TEXT)", connection))
                        {
                            createTableCommand.ExecuteNonQuery();
                        }
                    }

                    // Insert user data into the Users table
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", connection))
                    {
                        command.Parameters.AddWithValue("@Username", usernameTextBox.Text);
                        command.Parameters.AddWithValue("@Password", passwordTextBox.Text);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("User signed up successfully! Now Login!");
                    this.Close(); // Close the SignUpForm after successful signup
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool UserExists(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Check if the user with the given username exists in the Users table
                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username=@Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private bool IsValidPassword(string password)
        {
            // Password should not be blank, have a 12 character limit, and contain only string characters
            return !string.IsNullOrWhiteSpace(password) && password.Length <= 12 && password.All(char.IsLetterOrDigit);
        }

        private bool IsValidUsername(string username)
        {
            // Username should only contain words and digits
            return !string.IsNullOrWhiteSpace(username) && username.All(char.IsLetterOrDigit);
        }

        private bool TableExists(SQLiteConnection connection, string tableName)
        {
            using (SQLiteCommand command = new SQLiteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
            {
                return command.ExecuteScalar() != null;
            }
        }
    }
}
