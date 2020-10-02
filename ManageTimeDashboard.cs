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
using Timetable_Management_System.src;
using System.Data.SqlClient;
using Timetable_Management_System.src.Models;
using System.Globalization;
using System.Runtime.InteropServices;


namespace Timetable_Management_System
{
  public partial class ManageTimeDashboard : Form
  {
    public ManageTimeDashboard()
    {
      InitializeComponent();
      tabControl1.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);
      this.WindowState = FormWindowState.Maximized;
      fillCombo();
    }


    public class Slot
    {
      public String startTime { get; set; }

      public String endTime { get; set; }

      public String fullSlot { get; set; }
    }


    SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-KEB3VJ8\SQLEXPRESS;Initial Catalog=onsys_testing;Integrated Security=True");

    String monday = "";
    String tuesday = "";
    String wednesday = "";
    String thursday = "";
    String friday = "";
    String saturday = "";
    String sunday = "";

    int mondaychk = 0;
    int tuesdaychk = 0;
    int wednesdaychk = 0;
    int thursdaychk = 0;
    int fridaychk = 0;
    int saturdaychk = 0;
    int sundaychk = 0;

    public List<Slot> P1 = new List<Slot>();
    public List<String> day = new List<String>();
    public List<String> startTime = new List<String>();
    public List<String> endTime = new List<String>();
    public List<String> combineTime = new List<String>();



    private void ManageTimeDashboard_Load(object sender, EventArgs e)
    {

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

    private void ParallelSessions_Click(object sender, EventArgs e)
    {

    }

    private void txtTableType_TextChanged(object sender, EventArgs e)
    {

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (chkMon.Checked)
      {
        monday = "Monday";
        day.Add(monday);
        mondaychk = 1;
      }
      if (chkTue.Checked)
      {
        tuesday = "Tuesday";
        day.Add(tuesday);
        tuesdaychk = 1;
      }
      if (chkWed.Checked)
      {
        wednesday = "Wednesday";
        day.Add(wednesday);
        wednesdaychk = 1;
      }
      if (chkThur.Checked)
      {
        thursday = "Thursday";
        day.Add(thursday);
        thursdaychk = 1;
      }
      if (chkFri.Checked)
      {
        friday = "Friday";
        day.Add(friday);
        fridaychk = 1;
      }
      if (chkSat.Checked)
      {
        saturday = "Saturday";
        day.Add(saturday);
        saturdaychk = 1;
      }
      if (chkSun.Checked)
      {
        sunday = "Sunday";
        day.Add(sunday);
        sundaychk = 1;
      }

      string combindedString = string.Join(",", day);
      string sTime = string.Join(",", startTime);
      string eTime = string.Join(",", endTime);

      /*
      SqlCommand cmd = new SqlCommand("insert into Time_table1(tableType, noDays,days, workingTime,startTime,EndTime) values(@tableType,@noDays,@days,@workingTime,@sTime,@eTime)", con);
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.AddWithValue("@tableType", txtTableType.Text);
      cmd.Parameters.AddWithValue("@noDays", txtNoDays.Text);
      cmd.Parameters.AddWithValue("@days", combindedString);
      cmd.Parameters.AddWithValue("@workingTime", txtTotalTime.Text);
      cmd.Parameters.AddWithValue("@sTime", sTime);
      cmd.Parameters.AddWithValue("@eTime", eTime);

      con.Open();
      cmd.ExecuteNonQuery();
      con.Close();
      */

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      using var cmd = new SQLiteCommand(con);

      cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS Time_table1 (
                                tableID INTEGER PRIMARY KEY AUTOINCREMENT,
                                tableType TEXT,
                                noDays INTEGER,
                                days TEXT,
                                workingTime TEXT,
                                startTime TEXT,
                                EndTime TEXT      
                )";
      cmd.ExecuteNonQuery();

      cmd.CommandText = "INSERT INTO  Time_table1( tableType, noDays, days, workingTime, startTime, EndTime)" +
          "VALUES(@tableType, @noDays, @days, @workingTime, @startTime, @EndTime)";

      cmd.Parameters.AddWithValue("@tableType", txtTableType.Text);
      cmd.Parameters.AddWithValue("@noDays", txtNoDays.Text);
      cmd.Parameters.AddWithValue("@days", combindedString);
      cmd.Parameters.AddWithValue("@workingTime", txtTotalTime.Text);
      cmd.Parameters.AddWithValue("@startTime", sTime);
      cmd.Parameters.AddWithValue("@EndTime", eTime);


      cmd.Prepare();

      cmd.ExecuteNonQuery();

      //last row id
      cmd.CommandText = "select last_insert_rowid()";

      // The row ID is a 64-bit value - cast the Command result to an Int64.
      //
      Int64 LastRowID64 = (Int64)cmd.ExecuteScalar();

      // Then grab the bottom 32-bits as the unique ID of the row.
      //
      int LastRowID = (int)LastRowID64;
      //last row id

      cmd.CommandText = @"CREATE TABLE IF NOT EXISTS TimeSlots(
                                slotID     INTEGER PRIMARY KEY AUTOINCREMENT, 
                                fullTime TEXT,
                                tableID     INTEGER,
                                FOREIGN KEY(tableID) REFERENCES Time_table1(tableID)
     
                )";
      cmd.ExecuteNonQuery();

      int length = combineTime.Count;

      for (int i = 0; i < length; i++)
      {
        cmd.CommandText = "INSERT INTO  TimeSlots( fullTime, tableID)" +
        "VALUES(@fullTime, @tableID)";

        cmd.Parameters.AddWithValue("@fullTime", combineTime[i]);
        cmd.Parameters.AddWithValue("@tableID", LastRowID);

        cmd.Prepare();
        cmd.ExecuteNonQuery();
      }


      combineTime.Clear();

      int countchk = combineTime.Count;

      cmd.CommandText = @"CREATE TABLE IF NOT EXISTS TimetableDaychk(
                                checkID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                Monday INTEGER,
                                Tuesday INTEGER,
                                Wednesday INTEGER,
                                Thursday INTEGER,
                                Friday INTEGER,
                                Saturday INTEGER,
                                Sunday INTEGER,
                                tableID     INTEGER,
                                FOREIGN KEY(tableID) REFERENCES Time_table1(tableID)
     
                )";
      cmd.ExecuteNonQuery();

      cmd.CommandText = "INSERT INTO  TimetableDaychk( Monday, Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,tableID)" +
          "VALUES(@Monday, @Tuesday,@Wednesday, @Thursday,@Friday, @Saturday,@Sunday, @tableID)";

      cmd.Parameters.AddWithValue("@Monday", mondaychk);
      cmd.Parameters.AddWithValue("@Tuesday", tuesdaychk);
      cmd.Parameters.AddWithValue("@Wednesday", wednesdaychk);
      cmd.Parameters.AddWithValue("@Thursday", thursdaychk);
      cmd.Parameters.AddWithValue("@Friday", fridaychk);
      cmd.Parameters.AddWithValue("@Saturday", saturdaychk);
      cmd.Parameters.AddWithValue("@Sunday", sundaychk);
      cmd.Parameters.AddWithValue("@tableID", LastRowID);

      cmd.Prepare();
      cmd.ExecuteNonQuery();

      Console.WriteLine("row inserted");
      clearday();
      cleanForm1();

      MessageBox.Show("Time table info is successfuly inserted in the database", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);


      con.Close();
    }

    public void clearday()
    {
      mondaychk = 0;
      tuesdaychk = 0;
      wednesdaychk = 0;
      thursdaychk = 0;
      fridaychk = 0;
      saturdaychk = 0;
      sundaychk = 0;
    }

    public void cleanForm1()
    {
      txtTableType.Text = "";
      txtNoDays.Text = "";
      txtTotalTime.Text = "";
      txtSTime.Text = "";
      txtETime.Text = "";

      chkMon.Checked = false;
      chkTue.Checked = false;
      chkWed.Checked = false;
      chkThur.Checked = false;
      chkFri.Checked = false;
      chkSat.Checked = false;
      chkSun.Checked = false;

      P1.Clear();
    }

    public void cleanForm2()
    {

      txtNoDays2.Text = "";
      txtTime2.Text = "";


      chkMon1.Checked = false;
      chkTue2.Checked = false;
      chkWed2.Checked = false;
      chkThu2.Checked = false;
      chkFri2.Checked = false;
      chkSat2.Checked = false;
      chkSun2.Checked = false;

      P1.Clear();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      dataGridView1.DataSource = null;
      string stime = txtSTime.Text;
      string etime = txtETime.Text;
      string fulltime = stime + "-" + etime;
      P1.Add(new Slot() { startTime = stime, endTime = etime, fullSlot = fulltime });
      dataGridView1.DataSource = P1;

      startTime.Add(txtSTime.Text);
      endTime.Add(txtETime.Text);
      combineTime.Add(fulltime);
    }

    private void comboTableType_SelectedIndexChanged(object sender, EventArgs e)
    {
      //SqlCommand cmd = new SqlCommand("select * from Time_table1 where tableType ='" + comboTableType.Text + "'", con);


      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();
      using var cmd = new SQLiteCommand(con);

      cmd.CommandText = "select * from Time_table1 where tableType ='" + comboTableType.Text + "'";
      cmd.ExecuteNonQuery();

      SQLiteDataReader sdr;

      cleanForm2();
      try
      {

        sdr = cmd.ExecuteReader();

        while (sdr.Read())
        {
          string workingDays = sdr.GetInt32(2).ToString();
          string day = sdr.GetString(3);
          string workingTime = sdr.GetString(4);
          string sTime = sdr.GetString(5);
          string eTime = sdr.GetString(6);

          Array workArray = day.Split(',');
          //Array startTime = sTime.Split(',');
          //Array endTime = eTime.Split(',');

          string[] startTime = sTime.Split(',');
          string[] endTime = sTime.Split(',');

          foreach (string value in workArray)
          {
            String check = value.ToString();
            if (check == "Monday")
            {
              chkMon1.Checked = true;
            }
            else if (check == "Tuesday")
            {
              chkTue2.Checked = true;
            }
            else if (check == "Wednesday")
            {
              chkWed2.Checked = true;
            }
            else if (check == "Thursday")
            {
              chkThu2.Checked = true;
            }
            else if (check == "Friday")
            {
              chkFri2.Checked = true;
            }
            else if (check == "Saturday")
            {
              chkSat2.Checked = true;
            }
            else if (check == "Sunday")
            {
              chkSun2.Checked = true;
            }

          }

          int length = startTime.Length;

          for (int i = 0; i < length; i++)
          {
            dataGridView2.DataSource = null;
            P1.Add(new Slot() { startTime = startTime[i], endTime = endTime[i] });
            dataGridView2.DataSource = P1;
          }



          txtNoDays2.Text = workingDays;
          txtTime2.Text = workingTime;
        }

      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      con.Close();
    }

    void fillCombo()
    {

      comboTableType.Items.Clear();

      //SqlCommand cmd = new SqlCommand("select * from Time_table1", con);

      //SQLiteDataReader sdr;

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);

      using var cmd = new SQLiteCommand("select * from Time_table1", con);
      SQLiteDataReader sdr;

      try
      {
        con.Open();
        sdr = cmd.ExecuteReader();

        while (sdr.Read())
        {
          string type = sdr.GetString(1);
          comboTableType.Items.Add(type);
        }
        con.Close();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }

    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
      if (chkMon1.Checked)
      {
        monday = "Monday";
        day.Add(monday);
      }
      if (chkTue2.Checked)
      {
        tuesday = "Tuesday";
        day.Add(tuesday);
      }
      if (chkWed2.Checked)
      {
        wednesday = "Wednesday";
        day.Add(wednesday);
      }
      if (chkThu2.Checked)
      {
        thursday = "Thursday";
        day.Add(thursday);
      }
      if (chkFri2.Checked)
      {
        friday = "Friday";
        day.Add(friday);
      }
      if (chkSat2.Checked)
      {
        saturday = "Saturday";
        day.Add(saturday);
      }
      if (chkSun2.Checked)
      {
        sunday = "Sunday";
        day.Add(sunday);
      }

      string combindedString = string.Join(",", day);

      /*
      SqlCommand cmd = new SqlCommand("UPDATE Time_table1 SET noDays = @noDays,days = @days,workingTime = @workingTime where tableType = @tableType", con);
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.AddWithValue("@tableType", comboTableType.Text);
      cmd.Parameters.AddWithValue("@noDays", txtNoDays2.Text);
      cmd.Parameters.AddWithValue("@days", combindedString);
      cmd.Parameters.AddWithValue("@workingTime", txtTime2.Text);

      con.Open();
      cmd.ExecuteNonQuery();
      con.Close();

      MessageBox.Show("Time table info is successfuly updated in the database", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
      */
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      using var cmd = new SQLiteCommand(con);



      cmd.CommandText = "UPDATE Time_table1 SET noDays = @noDays,days = @days,workingTime = @workingTime where tableType = @tableType";
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.AddWithValue("@tableType", comboTableType.Text);
      cmd.Parameters.AddWithValue("@noDays", txtNoDays2.Text);
      cmd.Parameters.AddWithValue("@days", combindedString);
      cmd.Parameters.AddWithValue("@workingTime", txtTime2.Text);


      cmd.Prepare();

      cmd.ExecuteNonQuery();
      con.Close();

      MessageBox.Show("Time table info is successfuly updated in the database", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnDel_Click(object sender, EventArgs e)
    {
      /*
      SqlCommand cmd = new SqlCommand("DELETE from Time_table1 where tableType = @tableType", con);
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.AddWithValue("@tableType", comboTableType.Text);


      con.Open();
      cmd.ExecuteNonQuery();
      con.Close();

      comboTableType.Items.Clear();
      fillCombo();

      MessageBox.Show("Time table is deleted from the table", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
      */

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();

      using var cmd = new SQLiteCommand(con);

      cmd.CommandText = "DELETE from Time_table1 where tableType = @tableType";
      cmd.CommandType = CommandType.Text;
      cmd.Parameters.AddWithValue("@tableType", comboTableType.Text);

      cmd.ExecuteNonQuery();
      con.Close();

      comboTableType.Items.Clear();
      fillCombo();

      MessageBox.Show("Time table is deleted from the table", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
      cleanForm2();
    }

    private void label11_Click(object sender, EventArgs e)
    {

    }

    private void tabControl1_Selected(object sender, TabControlEventArgs e)
    {
      if (tabControl1.SelectedTab == ParallelSessions)
      {

      }
      else if (tabControl1.SelectedTab == tabPage1)
      {
        fillCombo();
      }

    }

    private void imgLoggedUser_Click(object sender, EventArgs e)
    {
      this.Hide();
      Login obj = new Login();
      obj.Show();
    }

    private void txtNoDays2_TextChanged(object sender, EventArgs e)
    {

    }
  }
}
