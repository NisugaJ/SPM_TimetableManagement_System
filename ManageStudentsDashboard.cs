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

namespace Timetable_Management_System
{
    public partial class ManageStudentsDashboard : Form
    {
        public ManageStudentsDashboard()
        {
            InitializeComponent();
            tabControl1.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);
            this.WindowState = FormWindowState.Maximized;
        }

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();

        private void ManageStudentsDashboard_Load(object sender, EventArgs e)
        {
            createYearSemesterTableIfEmpty();

            LoadYear();
            LoadSemester();
            LoadYearSemester();
           // LoadPrograme();
            LoadProgrameList();

            LoadGroup();
          //  LoadGroupList();
          //  LoadSubGroupList();
            LoadSubGroup();
            
      

        }
        // Create "year_semester" table if not exists
        private void createYearSemesterTableIfEmpty()
        {
            string cs = @"URI=file:.\timetableManagementSystemDB.db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS year_semester (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                year TEXT,
                                semester TEXT,
                                programe TEXT,
                                group_no INTEGER,
                                subgroup_no INTEGER,
                                group_id TEXT,
                                subgroup_id TEXT
                )";
            cmd.ExecuteNonQuery();
        }
        //set connection
        private void setConnection()
        {
            sql_con = new SQLiteConnection(@"URI=file:.\timetableManagementSystemDB.db");
        }

        //set executequery
        private void ExecuteQuery(string txtQuery)
        {
            setConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        //set year_semester
        private void LoadYearSemester()
        {
            setConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "SELECT id,year,semester FROM year_semester";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView1.DataSource = DT;
            sql_con.Close();
        }

        //set programe
        private void LoadPrograme()
        {
            setConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "SELECT id,year,semester,programe FROM year_semester";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView2.DataSource = DT;
            sql_con.Close();
        }

        //set group
        private void LoadGroupList()
        {
            setConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "SELECT id,year,semester,programe,group_no,group_id FROM year_semester";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView3.DataSource = DT;
            sql_con.Close();
        }//set group
        private void LoadSubGroupList()
        {
            setConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "SELECT * FROM year_semester";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView4.DataSource = DT;
            sql_con.Close();
        }

        //Load year
        private void LoadYear()
        {
            for (int i = 1; i <= 4; i++)
            {
                comboBox1.Items.Add("Year " + i);
            }
        }

        //Load semester
        private void LoadSemester()
        {
            for (int i = 1; i <= 2; i++)
            {
                comboBox2.Items.Add("Semester " + i);
            }
        }

        //Load programe
        private void LoadProgrameList()
        {

            String[] programes = { "IT", "SE", "CS", "IM", "DS" };

            for (int i = 0; i < 5; i++)
            {
                comboBox3.Items.Add(programes[i]);
            }
        }

        //Load group
        private void LoadGroup()
        {
            for (int i = 1; i <= 25; i++)
            {
                comboBox5.Items.Add(i);
            }
        }
        //Load sub Group
        private void LoadSubGroup()
        {
            for (int i = 1; i <= 2; i++)
            {
                comboBox7.Items.Add(i);
            }
        }


        //add ys
        private void button1_Click(object sender, EventArgs e)
        {
            string txtQuery = "INSERT INTO year_semester (year,semester) VALUES('" + comboBox1.Text + "','" + comboBox2.Text + "')";
            ExecuteQuery(txtQuery);
            LoadYearSemester();
        }

        //edit ys
        private void button3_Click(object sender, EventArgs e)
        {
            string txtQuery = "UPDATE year_semester SET year='" + comboBox1.Text + "', semester='" + comboBox2.Text + "' WHERE id ='" + Convert.ToInt32(label8.Text) + "'";
            ExecuteQuery(txtQuery);
            LoadYearSemester();
        }
        //del ys
        private void button2_Click(object sender, EventArgs e)
        {
            string txtQuery = "DELETE FROM year_semester WHERE id='" + Convert.ToInt32(label8.Text) + "'";
            ExecuteQuery(txtQuery);
            LoadYearSemester();
        }

        //add programe
        private void button6_Click(object sender, EventArgs e)
        {
            string txtQuery = "UPDATE year_semester SET programe='" + comboBox3.Text + "' WHERE id ='" + Convert.ToInt32(label9.Text) + "'";
            ExecuteQuery(txtQuery);
            LoadPrograme();
        }


        //update programe
        private void button4_Click(object sender, EventArgs e)
        {
            string txtQuery = "UPDATE year_semester SET programe='" + comboBox3.Text + "' WHERE id ='" + Convert.ToInt32(label9.Text) + "'";
            ExecuteQuery(txtQuery);
            LoadPrograme();
        }

        //genarateGroupID
        //del programe
        private void button5_Click(object sender, EventArgs e)
        {
            string txtQuery = "UPDATE year_semester SET programe='" + null + "' WHERE id ='" + Convert.ToInt32(label9.Text) + "'";
            ExecuteQuery(txtQuery);
            LoadPrograme();
        }


        //genarate id
        private String genarateGroupID(String y, String s, String p, String g) {
            String id = "";
            if (y == "Year 1")
            {
                id = "Y1.";
            }
            else if (y == "Year 2")
            {
                id = "Y2.";
            }
            else if (y == "Year 3")
            {
                id = "Y3.";
            }
            else if (y == "Year 3")
            {
                id = "Y4.";
            }

            if (s == "Semester 1")
            {
                id = id + "S1.";
            }
            else if (s == "Semester 2")
            {
                id = id + "S2.";
            }

            id = id + p +"."+ g;

            return id;
        }
        //add group
        private void button9_Click(object sender, EventArgs e)
        {
            String gid = genarateGroupID(label12.Text, label5.Text, label13.Text, comboBox5.Text);
            string txtQuery = "UPDATE year_semester SET group_no='" + comboBox5.Text.ToString() + "',group_id='" + gid + "' WHERE id ='" + label11.Text.ToString() + "'";
            ExecuteQuery(txtQuery);
            LoadGroupList();
        }
        //edit group
        private void button7_Click(object sender, EventArgs e)
        {
            String gid = genarateGroupID(label12.Text, label5.Text, label13.Text, comboBox5.Text);
            string txtQuery = "UPDATE year_semester SET group_no='" + comboBox5.Text.ToString() + "',group_id='" + gid + "' WHERE id ='" + label11.Text.ToString() + "'";
            ExecuteQuery(txtQuery);
            LoadGroupList();
        }
        //delete group
        private void button8_Click(object sender, EventArgs e)
        {
            string txtQuery = "UPDATE year_semester SET group_no='" + null + "',group_id='" + null + "' WHERE id ='" + label11.Text.ToString() + "'";
            ExecuteQuery(txtQuery);
            LoadGroupList();
        }
        //add sub group
        private void button12_Click(object sender, EventArgs e)
        {
            String sgid = label18.Text +"."+ comboBox7.Text;
            string txtQuery = "UPDATE year_semester SET subgroup_no='" + comboBox7.Text.ToString() + "',subgroup_id='" + sgid + "' WHERE id ='" + label15.Text.ToString() + "'";
            ExecuteQuery(txtQuery);
            LoadSubGroupList();
        }
        //edit sub group
        private void button10_Click(object sender, EventArgs e)
        {
            String sgid = label18.Text + "." + comboBox7.Text;
            string txtQuery = "UPDATE year_semester SET subgroup_no='" + comboBox7.Text.ToString() + "',subgroup_id='" + sgid + "' WHERE id ='" + label15.Text.ToString() + "'";
            ExecuteQuery(txtQuery);
            LoadSubGroupList();
        }
        //delete sub group
        private void button11_Click(object sender, EventArgs e)
        {
            string txtQuery = "UPDATE year_semester SET subgroup_no='" + null + "',subgroup_id='" + null + "' WHERE id ='" + label15.Text.ToString() + "'";
            ExecuteQuery(txtQuery);
            LoadSubGroupList();
        }

        //set year and semester
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label8.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                comboBox1.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                comboBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select the corner");
            }
        }

        //show year,semester and program
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label9.Text = dataGridView2.SelectedRows[0].Cells[0].Value.ToString();
                label3.Text = dataGridView2.SelectedRows[0].Cells[1].Value.ToString();
                label10.Text = dataGridView2.SelectedRows[0].Cells[2].Value.ToString();
                comboBox3.Text = dataGridView2.SelectedRows[0].Cells[3].Value.ToString();
            }catch(Exception ex)
            {
                MessageBox.Show("Please select the corner");
            }
        }

        //show year,semester,program and group no
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

            label11.Text = dataGridView3.SelectedRows[0].Cells[0].Value.ToString();
            label12.Text = dataGridView3.SelectedRows[0].Cells[1].Value.ToString();
            label5.Text = dataGridView3.SelectedRows[0].Cells[2].Value.ToString();
            label13.Text = dataGridView3.SelectedRows[0].Cells[3].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select the corner");
            }
        }
        //show year,semester,program,group no and sub group no
        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

            label15.Text = dataGridView4.SelectedRows[0].Cells[0].Value.ToString();
            label16.Text = dataGridView4.SelectedRows[0].Cells[1].Value.ToString();
            label14.Text = dataGridView4.SelectedRows[0].Cells[2].Value.ToString();
            label7.Text = dataGridView4.SelectedRows[0].Cells[3].Value.ToString();
            label17.Text = dataGridView4.SelectedRows[0].Cells[4].Value.ToString();
            label18.Text = dataGridView4.SelectedRows[0].Cells[6].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select the corner");
            }
        }

        private void tabControl1_DrawItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl1.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {

                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.Red);
                g.FillRectangle(Brushes.Gray, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", 10.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }



        private void imgManageStudent_Click(object sender, EventArgs e)
        {
            ManageStudentsDashboard manageStudentsDashboardObj = new ManageStudentsDashboard();
            manageStudentsDashboardObj.Show();
            this.Hide();
        }

        private void ImgManageLecturers_Click(object sender, EventArgs e)
        {
            ManageLecturerDashboard lecturerDashboardObj = new ManageLecturerDashboard();
            lecturerDashboardObj.Show();
            this.Hide();
        }

        private void imgManageSubjects_Click(object sender, EventArgs e)
        {
            ManageSubjectsDashboard manageSubjectsDashboardObj = new ManageSubjectsDashboard();
            manageSubjectsDashboardObj.Show();
            this.Hide();
        }

        private void imgTime_Click(object sender, EventArgs e)
        {
            ManageTimeDashboard manageTimeDashboardObj = new ManageTimeDashboard();
            manageTimeDashboardObj.Show();
            this.Hide();
        }

        private void imgLocations_Click(object sender, EventArgs e)
        {
            ManageLocationsDashboard manageLocationsDashboardObj = new ManageLocationsDashboard();
            manageLocationsDashboardObj.Show();
            this.Hide();
        }

        private void imgManageTags_Click(object sender, EventArgs e)
        {
            ManageTagsDashboard manageTagsDashboardObj = new ManageTagsDashboard();
            manageTagsDashboardObj.Show();
            this.Hide();
        }

        private void imgStatistics_Click(object sender, EventArgs e)
        {
            ManageStatisticsDashboard manageStatisticsDashboardObj = new ManageStatisticsDashboard();
            manageStatisticsDashboardObj.Show();
            this.Hide();
        }

        private void imgGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateReportDashboard generateReportDashboardObj = new GenerateReportDashboard();
            generateReportDashboardObj.Show();
            this.Hide();
        }

        private void YearAndSemester_Click(object sender, EventArgs e)
        {

        }

        private void lblYearAndSemester_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void YearAndSemester_Click_1(object sender, EventArgs e)
        {

        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }


        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SubGroupNumbers_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab == YearAndSemester)
            {
                LoadYearSemester();
            }
            else if (tabControl1.SelectedTab == Program)
            {
                LoadPrograme();
            }
            else if (tabControl1.SelectedTab == GroupNumbers)
            {
                LoadGroupList();
            }
            else if (tabControl1.SelectedTab == SubGroupNumbers)
            {
                LoadSubGroupList();
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void imgLoggedUser_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login obj = new Login();
            obj.Show();
        }
    }
}
