using Laboratory3;
using System;
using System.Data.SqlClient; // ADDED: Needed for SQL Server interaction
using System.Drawing;
using System.Drawing.Drawing2D; // Needed for GraphicsPath
using System.Windows.Forms;

namespace Laboratory3
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

            // Ensure placeholder text is visible on load by moving focus to an inert control
            this.Shown += (s, e) => this.ActiveControl = label2;

            // --- textBox2 (Username) Placeholder Logic ---
            textBox2.Text = "Username";
            textBox2.ForeColor = Color.Gray;

            textBox2.GotFocus += (s, ev) =>
            {
                if (textBox2.Text == "Username")
                {
                    textBox2.Text = "";
                    textBox2.ForeColor = Color.Black;
                }
            };
            textBox2.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    textBox2.Text = "Username";
                    textBox2.ForeColor = Color.Gray;
                }
            };

            // --- textBox1 (Password) Placeholder Logic ---
            textBox1.Text = "Password";
            textBox1.ForeColor = Color.Gray;
            textBox1.UseSystemPasswordChar = false; // Must be false to show "Password" text

            textBox1.GotFocus += (s, ev) =>
            {
                if (textBox1.Text == "Password" && textBox1.ForeColor == Color.Gray)
                {
                    textBox1.Text = "";
                    textBox1.ForeColor = Color.Black;
                    // Apply password mask when the user starts typing
                    textBox1.UseSystemPasswordChar = true;
                }
            };
            textBox1.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    textBox1.Text = "Password";
                    textBox1.ForeColor = Color.Gray;
                    // Remove password mask to show the placeholder
                    textBox1.UseSystemPasswordChar = false;
                }
            };
        }

        // --- Core Form Handlers ---

        private void CenterPanel(Panel pnl)
        {
            pnl.Left = (this.ClientSize.Width - pnl.Width) / 2;
            pnl.Top = (this.ClientSize.Height - pnl.Height) / 2;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            CenterPanel(panel2);
        }

        private void Login_Resize(object sender, EventArgs e)
        {
            CenterPanel(panel2);
        }

        // --- Control Event Handlers ---

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Code for rounded panel corners
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int radius = 20;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(0, 0, radius, radius, 180, 90); // top-left
                path.AddArc(panel2.Width - radius, 0, radius, radius, 270, 90); // top-right
                path.AddArc(panel2.Width - radius, panel2.Height - radius, radius, radius, 0, 90); // bottom-right
                path.AddArc(0, panel2.Height - radius, radius, radius, 90, 90); // bottom-left
                path.CloseFigure();

                panel2.Region = new Region(path);

                // Draw background image inside the rounded region
                if (panel2.BackgroundImage != null)
                {
                    e.Graphics.SetClip(path);
                    e.Graphics.DrawImage(panel2.BackgroundImage, panel2.ClientRectangle);
                    e.Graphics.ResetClip();
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle the password mask only if the user has entered something (i.e., not the placeholder)
            if (textBox1.Text != "Password")
            {
                textBox1.UseSystemPasswordChar = !radioButton1.Checked;
            } 
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // ** LOGIN AUTHENTICATION LOGIC START **

            // IMPORTANT: Get values and handle the placeholder text from Laboratory2.Login.cs
            // textBox2 is Username (Placeholder: "Username")
            // textBox1 is Password (Placeholder: "Password")

            // Determine the actual username and password, treating placeholders as empty.
            string username = (textBox2.Text == "Username" || string.IsNullOrWhiteSpace(textBox2.Text))
                ? string.Empty : textBox2.Text.Trim();

            // Note: UseSystemPasswordChar is the best way to determine if the password box is active,
            // but for simplicity, we check the text content against the placeholder.
            string password = (textBox1.Text == "Password" || string.IsNullOrWhiteSpace(textBox1.Text))
                ? string.Empty : textBox1.Text.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter the username.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter the password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Connection details copied from your Test project
            string connectionString = "Data Source=DESKTOP-OCND0IM\\SQLEXPRESS; Initial Catalog=asplogin; Integrated Security=True; TrustServerCertificate=True";
            string sqlQuery = "SELECT COUNT(*) FROM login WHERE username = @username AND password = @password";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        // Use the extracted, non-placeholder values for parameters
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        await con.OpenAsync();

                        // ExecuteScalarAsync returns object, cast to int
                        int userCount = (int)await cmd.ExecuteScalarAsync();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Login Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Create an instance of the new form (e.g., Form2)
                            // NOTE: Ensure 'Form2' class exists in your Laboratory3 project.
                            Dashboard DashBoard = new Dashboard();

                            // Show the new form
                            DashBoard.Show();

                            // Hide the current login form
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Provide detailed error message, useful for debugging connection issues
                MessageBox.Show("A database connection error occurred. Please check your SQL connection string, SQL Server name, and ensure the database is accessible.\n\nError details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // ** LOGIN AUTHENTICATION LOGIC END **
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Logic to open the Registration Form
        }

        // Login.cs: This code is correct
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // The class name here is correct: New_Password
            New_Password forgotPassword = new New_Password();
            forgotPassword.Show();
            this.Hide();
        }

        // --- Empty Event Handlers referenced in Designer (kept for completeness) ---

        private void label1_Click_2(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged_1(object sender, EventArgs e) { }
        private void panel4_Paint_1(object sender, PaintEventArgs e) { }
    }
}
