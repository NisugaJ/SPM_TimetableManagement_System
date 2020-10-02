using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timetable_Management_System.src;
using Timetable_Management_System.src.Models;
using System.Data.SqlClient;

namespace Timetable_Management_System
{
  public partial class GenerateReportDashboard : Form
  {

    List<Lecturer> selectedLecturersListForCreateSession;
    bool dashboardLoaded = false;

    public GenerateReportDashboard()
    {
      createParallelSessionTable();
      InitializeComponent();
      tabControl1.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);
      this.WindowState = FormWindowState.Maximized;

            createSessionIfNotExist();
            //Create Session
            fillInitialComboboxes();
      cleanCreateSession();

      //Manage Session
      fillSessionManagementCmb();
      setSummaryVisibleOrHidden_ManageSession(false);

      //genarate report
      fillCombo();
    }
    //Lists
    public List<String> ListDays = new List<String>();
    public List<String> ListTime = new List<String>();
    public List<String> ListSession = new List<String>();
    public List<String> ListSlots = new List<String>();

    private SQLiteConnection sql_con;
    private SQLiteCommand sql_cmd;
    private SQLiteDataAdapter DB;
    private DataSet DS = new DataSet();
    private DataTable DT = new DataTable();



    public class Lecture
    {
      public String? name { get; set; }

      public int id { get; set; }

    }
    private void GenerateReportDashboard_Load(object sender, EventArgs e)
    {

      selectedLecturersListForCreateSession = new List<Lecturer>();

      fillCmbLecturerList();

      //this should be always last statement in GenerateReportDashboard_Load
      dashboardLoaded = true;
    }

    private void setConnection()
    {
      sql_con = new SQLiteConnection(@"URI=file:.\timetableManagementSystemDB.db");
    }

    //create parallel sessions
    private void createParallelSessionTable()
    {
      string cs = @"URI=file:.\timetableManagementSystemDB.db";

      using var con = new SQLiteConnection(cs);
      con.Open();

      using var cmd = new SQLiteCommand(con);

      cmd.CommandText = @"CREATE TABLE IF NOT EXISTS parallel_sessions (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                subgroup_id TEXT,
                                subject_code STRING,
                                lecturer_id TEXT,
                                time_slot TEXT,
                                session_id STRING,
                                FOREIGN KEY (subgroup_id) REFERENCES year_semester(subgroup_id),
                                FOREIGN KEY (subject_code) REFERENCES subjects(subjectCode),
                                FOREIGN KEY (session_id) REFERENCES session(sessionId),
                                FOREIGN KEY (lecturer_id) REFERENCES lecturers(lecturerID)
                                )";
      cmd.ExecuteNonQuery();
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

        //load parallel sessions
        private void LoadParallelSessions()
        {
            setConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "SELECT * FROM parallel_sessions";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView1.DataSource = DT;
            sql_con.Close();
        }

        //get group id from year_semester table

        private void loadSubGroupId()
    {
      SQLiteCommand sql_cmd;
      SQLiteDataAdapter DB;
      DataSet DS = new DataSet();
      setConnection();
      sql_con.Open();
      sql_cmd = sql_con.CreateCommand();
      string CommandText = "SELECT subgroup_id FROM year_semester";
      DB = new SQLiteDataAdapter(CommandText, sql_con);
      DS.Reset();
      DB.Fill(DS);
      DT = DS.Tables[0];
      comboBox6.DisplayMember = "subgroup_id";
      comboBox6.ValueMember = "subgroup_id";
      comboBox6.DataSource = DT;
      sql_con.Close();
    }

    private void loadSubjectCode()
    {
      SQLiteCommand sql_cmd;
      SQLiteDataAdapter DB;
      DataSet DS = new DataSet();
      setConnection();
      sql_con.Open();
      sql_cmd = sql_con.CreateCommand();
      string CommandText = "SELECT subjectCode FROM subjects";
      DB = new SQLiteDataAdapter(CommandText, sql_con);
      DS.Reset();
      DB.Fill(DS);
      DT = DS.Tables[0];
      comboBox2.DisplayMember = "subjectCode";
      comboBox2.ValueMember = "subjectCode";
      comboBox2.DataSource = DT;
      sql_con.Close();
    }

    private void loadLecturerId()
    {
      SQLiteCommand sql_cmd;
      SQLiteDataAdapter DB;
      DataSet DS = new DataSet();
      setConnection();
      sql_con.Open();
      sql_cmd = sql_con.CreateCommand();
      string CommandText = "SELECT lecturerID FROM lecturers";
      DB = new SQLiteDataAdapter(CommandText, sql_con);
      DS.Reset();
      DB.Fill(DS);
      DT = DS.Tables[0];
      comboBox3.DisplayMember = "lecturerID";
      comboBox3.ValueMember = "lecturerID";
      comboBox3.DataSource = DT;
      sql_con.Close();
    }

    private void loadSessionId()
    {
      SQLiteCommand sql_cmd;
      SQLiteDataAdapter DB;
      DataSet DS = new DataSet();
      setConnection();
      sql_con.Open();
      sql_cmd = sql_con.CreateCommand();
      string CommandText = "SELECT sessionId FROM session";
      DB = new SQLiteDataAdapter(CommandText, sql_con);
      DS.Reset();
      DB.Fill(DS);
      DT = DS.Tables[0];
      comboBox4.DisplayMember = "sessionId";
      comboBox4.ValueMember = "sessionId";
      comboBox4.DataSource = DT;
      sql_con.Close();
    }

    private void loadTimeSlot()
    {
            comboBox5.Items.Add("08.30");
            comboBox5.Items.Add("09.30");
            comboBox5.Items.Add("10.30");
            comboBox5.Items.Add("11.30");
            comboBox5.Items.Add("12.30");
            comboBox5.Items.Add("13.30");
            comboBox5.Items.Add("14.30");
            comboBox5.Items.Add("15.30");
            comboBox5.Items.Add("16.30");
            comboBox5.Items.Add("17.30");
            comboBox5.Items.Add("18.30");
            comboBox5.Items.Add("19.30");
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

    List<Lecturer> foundLecturersListForDisplay = new List<Lecturer>();

    private void fillCmbLecturerList()
    {
      //No items in the list. All lecturers needs be added to dropdown
      if (selectedLecturersListForCreateSession.Count == 0)
      {
        cmbLecturerAddSession.Items.Clear();

        foundLecturersListForDisplay = getAllLecturers();

        foreach (Lecturer lecObj in foundLecturersListForDisplay) // Loop through List with foreach
        {
          cmbLecturerAddSession.Items.Add(lecObj.lecturerID + " - " + lecObj.name);
        }
      }
      //Lecturers already selected
      else
      {
        cmbLecturerAddSession.Items.Clear();
        foundLecturersListForDisplay = refreshDataInLecturerCmb();
        foreach (Lecturer lecObj in foundLecturersListForDisplay) // Loop through List with foreach
        {
          cmbLecturerAddSession.Items.Add(lecObj.lecturerID + " - " + lecObj.name);
        }
      }

    }

    private List<Lecturer> getAllLecturers()
    {
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();
      string stm = "";

      stm = "SELECT * FROM lecturers";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      List<Lecturer> foundLecturersListForDisplay = new List<Lecturer>();

      while (rdr.Read())
      {
        Lecturer lecObj = new Lecturer();
        // Console.WriteLine($@"{rdr.GetInt32(0),-3} {rdr.GetString(1),-8} {rdr.GetInt32(2),8}");
        lecObj.lecturerID = Int32.Parse($@"{rdr.GetInt32(0),-3}");
        lecObj.title = $@"{ rdr.GetString(1),-8}".Trim();
        lecObj.name = $@"{ rdr.GetString(2),-8}".Trim();
        lecObj.faculty = $@"{ rdr.GetString(3),-8}".Trim();
        lecObj.department = $@"{ rdr.GetString(4),-8}".Trim();
        lecObj.center = $@"{ rdr.GetString(5),-8}".Trim();
        lecObj.building = $@"{ rdr.GetString(6),-8}".Trim();
        lecObj.employeeLevel = Int32.Parse($@"{rdr.GetInt32(7),-3}");
        lecObj.photoPath = $@"{ rdr.GetString(8),-8}".Trim();

        foundLecturersListForDisplay.Add(lecObj);
      }
      return foundLecturersListForDisplay;
    }

    private void cmbLecturerAddSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      string selectComboBoxText = this.cmbLecturerAddSession.GetItemText(this.cmbLecturerAddSession.SelectedItem) + "";
      //string[] authorsList = selectComboBoxText.Split(" - ");

      // Split the string on line breaks.
      string[] lines = selectComboBoxText.Split(new string[] { " - " }, StringSplitOptions.None);

      //lines[0] = { lecturer ID}
      //lines[1] = { lecturer name}

      List<string> temp = new List<string>();
      temp.Add(lines[0]);

      List<Lecturer> listOfLecObj = new List<Lecturer>();
      listOfLecObj = getLecturerObjectListByProvindingLecturerIDList(temp);
      foreach (Lecturer element in listOfLecObj)
      {
        selectedLecturersListForCreateSession.Add(element);
      }

      //refresh data in lecturer Combobox
      foundLecturersListForDisplay = refreshDataInLecturerCmb();

      fillCmbLecturerList();


      UpdateSelectedLecturerGrid();

      RefreshImagesWindows_CreateSession();

      generateAndDisplayLecturersList_CreateSession();
    }

    private void RefreshImagesWindows_CreateSession()
    {
      this.imgDataGridView_CreateSession.Rows.Clear();

      if (this.imgDataGridView_CreateSession.Columns.Count < 3)
      {
        DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
        imgCol.DefaultCellStyle.NullValue = null;
        imgDataGridView_CreateSession.Columns.Add(imgCol);
        imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
      }

      imgDataGridView_CreateSession.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

      List<Object> temp = new List<object>();
      for (int i = 0; i < selectedLecturersListForCreateSession.Count; i++)
      {
        string photoPath = selectedLecturersListForCreateSession[i].photoPath;
        Image img = Image.FromFile(@"" + photoPath);
        temp.Add(img);
        if ((i + 1) % 3 == 0)
        {
          Object[] row = temp.Cast<object>().ToArray();
          imgDataGridView_CreateSession.Rows.Add(row);
          temp = new List<object>();
        }
      }
      if (temp.Count > 0)
      {
        imgDataGridView_CreateSession.Rows.Add(temp.Cast<object>().ToArray());
      }

      /*

      List<ImageLec> imagesList = new List<ImageLec>(); 
      foreach (Lecturer element in selectedLecturersListForCreateSession)
      {
          ImageLec obj = new ImageLec();
          obj.isDisplayed = false;
          obj.lecturerID = element.lecturerID;
          obj.lecturerName = element.name;
          obj.photoPath = element.photoPath;
          imagesList.Add(obj);
      }


      List<Object> temp = new List<object>();

      int row_var = 0;
      int col_var = 0;
      foreach (DataGridViewRow row in imgDataGridView_CreateSession.Rows)
      {
          foreach (DataGridViewCell cell in row.Cells)
          {
              FROM lecturers WHERE
              foreach (ImageLec element in imagesList)
              {
                  if(element.isDisplayed == false)
                  {
                      //  selectedLecturersListForCreateSession[0].name
                      string photoPath = element.photoPath;
                    //  Image img1 = Image.FromFile(@"" + photoPath);
                    //  temp.Add(img1);


                      Bitmap bmp = (Bitmap)Bitmap.FromFile(@"" + photoPath);

                      DataGridViewImageCell iCell = new DataGridViewImageCell();
                      iCell.Value = bmp;
                      imgDataGridView_CreateSession[row_var, col_var] = iCell;

                      element.isDisplayed = true;
                      break;
                  }

              }
          col_var++;

          }
       row_var++;
      }

      */


    }

    private void UpdateSelectedLecturerGrid()
    {
      selectedLecturersGridView.Columns.Clear();

      selectedLecturersGridView.ColumnCount = 2;
      selectedLecturersGridView.Columns[0].Name = "Lecturer ID";
      selectedLecturersGridView.Columns[0].Width = 100;
      selectedLecturersGridView.Columns[1].Name = "Lecturer Name";
      selectedLecturersGridView.Columns[1].Width = 150;

      foreach (Lecturer element in selectedLecturersListForCreateSession)
      {
        string[] row1 = new string[] { element.lecturerID + "", element.name };
        selectedLecturersGridView.Rows.Add(row1);
      }

      DataGridViewButtonColumn dgvBtn = new DataGridViewButtonColumn();
      dgvBtn.UseColumnTextForButtonValue = true;
      dgvBtn.HeaderText = "    X";
      dgvBtn.Name = "X";
      dgvBtn.Text = "X";

      dgvBtn.Width = 50;

      selectedLecturersGridView.Columns.Add(dgvBtn);


    }

    private void selectedLecturersGridView_CellClick(object sender, DataGridViewCellEventArgs e)
    {


      int rowindex = selectedLecturersGridView.CurrentCell.RowIndex;
      int columnindex = selectedLecturersGridView.CurrentCell.ColumnIndex;


      string lecName = selectedLecturersGridView.Rows[rowindex].Cells[columnindex - 1].Value.ToString();
      string lecId = selectedLecturersGridView.Rows[rowindex].Cells[columnindex - 2].Value.ToString();

      find_And_Remove_Relevant_Lecturer_In_selectedLecturersListForCreateSession_List(lecId);
      UpdateSelectedLecturerGrid();
      Console.WriteLine(selectedLecturersListForCreateSession);
      fillCmbLecturerList();
      generateAndDisplayLecturersList_CreateSession();
      RefreshImagesWindows_CreateSession();
    }

    private void find_And_Remove_Relevant_Lecturer_In_selectedLecturersListForCreateSession_List(string lecId)
    {
      List<Lecturer> temp = new List<Lecturer>();

      foreach (Lecturer element in selectedLecturersListForCreateSession)
      {
        if (!lecId.Equals(element.lecturerID.ToString()))
        {
          temp.Add(element);
        }
      }
      selectedLecturersListForCreateSession = temp;
    }

    private List<Lecturer> refreshDataInLecturerCmb()
    {
      //List<int> lecIdList = new List<int>();
      string fullLecIdString = "";

      int counter = 0;
      foreach (Lecturer element in selectedLecturersListForCreateSession)
      {
        if (counter != 0)
        {
          fullLecIdString += ",";
        }
        fullLecIdString += element.lecturerID;
        counter++;
      }

      string cs = @"URI=file:.\" + Utils.dbName + ".db";
      using var con = new SQLiteConnection(cs);
      con.Open();

      string stm = "";
      stm = "SELECT * FROM lecturers WHERE lecturerID NOT IN(" + fullLecIdString + ")";
      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      List<Lecturer> lecList = new List<Lecturer>();
      while (rdr.Read())
      {
        Lecturer lecObj = new Lecturer();
        // Console.WriteLine($@"{rdr.GetInt32(0),-3} {rdr.GetString(1),-8} {rdr.GetInt32(2),8}");
        lecObj.lecturerID = Int32.Parse($@"{rdr.GetInt32(0),-3}");
        lecObj.title = $@"{ rdr.GetString(1),-8}".Trim();
        lecObj.name = $@"{ rdr.GetString(2),-8}".Trim();
        lecObj.faculty = $@"{ rdr.GetString(3),-8}".Trim();
        lecObj.department = $@"{ rdr.GetString(4),-8}".Trim();
        lecObj.center = $@"{ rdr.GetString(5),-8}".Trim();
        lecObj.building = $@"{ rdr.GetString(6),-8}".Trim();
        lecObj.employeeLevel = Int32.Parse($@"{rdr.GetInt32(7),-3}");
        lecObj.photoPath = $@"{ rdr.GetString(8),-8}".Trim();

        lecList.Add(lecObj);
      }

      con.Close();
      return lecList;

    }

    private List<Lecturer> getLecturerObjectListByProvindingLecturerIDList(List<string> listOfLecturerIDs)
    {
      List<Lecturer> fillCmbLecturerList = new List<Lecturer>();

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      foreach (string lecId in listOfLecturerIDs)
      {
        string stm = "";
        stm = "SELECT * FROM lecturers WHERE lecturerID='" + lecId + "'";
        using var cmd = new SQLiteCommand(stm, con);
        using SQLiteDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
          Lecturer lecObj = new Lecturer();
          // Console.WriteLine($@"{rdr.GetInt32(0),-3} {rdr.GetString(1),-8} {rdr.GetInt32(2),8}");
          lecObj.lecturerID = Int32.Parse($@"{rdr.GetInt32(0),-3}");
          lecObj.title = $@"{ rdr.GetString(1),-8}".Trim();
          lecObj.name = $@"{ rdr.GetString(2),-8}".Trim();
          lecObj.faculty = $@"{ rdr.GetString(3),-8}".Trim();
          lecObj.department = $@"{ rdr.GetString(4),-8}".Trim();
          lecObj.center = $@"{ rdr.GetString(5),-8}".Trim();
          lecObj.building = $@"{ rdr.GetString(6),-8}".Trim();
          lecObj.employeeLevel = Int32.Parse($@"{rdr.GetInt32(7),-3}");
          lecObj.photoPath = $@"{ rdr.GetString(8),-8}".Trim();

          fillCmbLecturerList.Add(lecObj);
        }
      }
      con.Close();

      return fillCmbLecturerList;
    }

    private void fillTags_CreateSession()
    {
      cmbTag_CreateSession.Items.Clear();

      cmbTag_CreateSession.Items.Add("Lecture");
      cmbTag_CreateSession.Items.Add("Tutorial");
      cmbTag_CreateSession.Items.Add("Lab");
      cmbTag_CreateSession.Items.Add("Evaluation");
    }
    private void fillYear_CreateSession()
    {
      cmbYear_CreateSession.Items.Add(1);
      cmbYear_CreateSession.Items.Add(2);
      cmbYear_CreateSession.Items.Add(3);
      cmbYear_CreateSession.Items.Add(4);
    }
    private void fillSemester_CreateSession()
    {
      cmbSemester_CreateSession.Items.Add(1);
      cmbSemester_CreateSession.Items.Add(2);
    }

    private void fillProgram_CreateSession()
    {
      cmbProgram_CreateSession.Items.Add("SE");
      cmbProgram_CreateSession.Items.Add("IT");
      cmbProgram_CreateSession.Items.Add("CS");
      cmbProgram_CreateSession.Items.Add("IM");
      cmbProgram_CreateSession.Items.Add("DS");
    }

    private void fillInitialComboboxes()
    {
      fillTags_CreateSession();
      fillYear_CreateSession();
      fillSemester_CreateSession();
      fillProgram_CreateSession();
      fillGroup_CreateSession();
      loadSubGroupId();
      loadSubjectCode();
      loadLecturerId();
      loadSessionId();
      loadTimeSlot();
      LoadParallelSessions();
    }

    private void fillGroup_CreateSession()
    {

    }

    private void generateAndDisplayLecturersList_CreateSession()
    {
      string fullLecList = "";
      foreach (Lecturer element in selectedLecturersListForCreateSession)
      {
        if (fullLecList.Equals(""))
        {
          fullLecList = fullLecList + element.name;
        }
        else
        {
          fullLecList = fullLecList + " , " + element.name;
        }
      }
      lblLecturerList_CreateSession.Text = fullLecList;
    }

    private void cleanCreateSession()
    {
      lblLecturerList_CreateSession.Text = "";
      lblLecturerList_CreateSession.Text = "";
      lblTag_CreateSession_Summary.Text = "";
      lblSessionID_CreateSession_Summary.Text = "";
      lblSubject_CreateSession_summary.Text = "";
      lblStudentCreateSessionSummary_CreateSession.Text = "";
      lblCountAndHrs_CreateSession.Text = "";


    }


    private void btnReset_CreateSession_Click(object sender, EventArgs e)
    {
      this.Hide();
      GenerateReportDashboard obj = new GenerateReportDashboard();
      obj.Show();
    }


    private void btnCreateSession_Click(object sender, EventArgs e)
    {

      string sessionId = txtSessionID_CreateSession.Text;

      string selectedTag = this.cmbTag_CreateSession.GetItemText(this.cmbTag_CreateSession.SelectedItem);
      string selectedYear = this.cmbYear_CreateSession.GetItemText(this.cmbYear_CreateSession.SelectedItem);
      string selectedSemester = this.cmbSemester_CreateSession.GetItemText(this.cmbSemester_CreateSession.SelectedItem);
      string selectedProgram = this.cmbProgram_CreateSession.GetItemText(this.cmbProgram_CreateSession.SelectedItem);
      string selectedGroup = this.cmbGroup_CreateSession.GetItemText(this.cmbGroup_CreateSession.SelectedItem);
      string selectedSubGroup = this.cmbSubGroup_CreateSession.GetItemText(this.cmbSubGroup_CreateSession.SelectedItem);
      string selectedSubjectId = this.cmbSubject_CreateSession.GetItemText(this.cmbSubject_CreateSession.SelectedItem);


      string noOfStudents = txtNoOfStudents_CreateSession.Text;
      string sessionDuration = txtDuration_CreateSession.Text;


      double numberDur;
      int numberStd;

      if (sessionId.Equals(""))
      {
        MessageBox.Show("Please enter session ID");
      }
      else if (selectedTag.Equals(""))
      {
        MessageBox.Show("Please select a tag");
      }
      else if (selectedYear.Equals(""))
      {
        MessageBox.Show("Please select the year");
      }
      else if (selectedSemester.Equals(""))
      {
        MessageBox.Show("Please select the semester");
      }
      else if (selectedProgram.Equals(""))
      {
        MessageBox.Show("Please select the program");
      }
      else if (selectedGroup.Equals(""))
      {
        MessageBox.Show("Please select the group");
      }
      else if (selectedSubGroup.Equals(""))
      {
        MessageBox.Show("Please select the sub group");
      }
      else if (selectedSubjectId.Equals(""))
      {
        MessageBox.Show("Please select the subject ");
      }
      else if (noOfStudents.Equals(""))
      {
        MessageBox.Show("Please enter number of students");
      }
      else if (!Int32.TryParse(noOfStudents, out numberStd))
      {
        MessageBox.Show("Number of students must be a valid number");
      }
      else if (sessionDuration.Equals(""))
      {
        MessageBox.Show("Please enter session duration");
      }
      else if (!Double.TryParse(sessionDuration, out numberDur))
      {
        MessageBox.Show("Session duration must be a number");
      }
      else
      {
        //Add to db
        addSessionToDB();
      }
    }

    private void addSessionToDB()
    {
      string sessionId = txtSessionID_CreateSession.Text;

      string tag = this.cmbTag_CreateSession.GetItemText(this.cmbTag_CreateSession.SelectedItem);
      string year = this.cmbYear_CreateSession.GetItemText(this.cmbYear_CreateSession.SelectedItem);
      string semester = this.cmbSemester_CreateSession.GetItemText(this.cmbSemester_CreateSession.SelectedItem);
      string program = this.cmbProgram_CreateSession.GetItemText(this.cmbProgram_CreateSession.SelectedItem);
      string group = this.cmbGroup_CreateSession.GetItemText(this.cmbGroup_CreateSession.SelectedItem);
      string subGroup = this.cmbSubGroup_CreateSession.GetItemText(this.cmbSubGroup_CreateSession.SelectedItem);
      string subjectId = this.cmbSubject_CreateSession.GetItemText(this.cmbSubject_CreateSession.SelectedItem);


      string[] lines = subjectId.Split(new string[] { " - " }, StringSplitOptions.None);

      subjectId = lines[0];

      string noOfStudents = txtNoOfStudents_CreateSession.Text;
      string sessionDuration = txtDuration_CreateSession.Text;

      //selectedLecturersListForCreateSession

      int yearNum;
      int semesterNum;
      int groupNum;
      int subGroupNum;
      int noOfStudentsNum;
      double sessionDurationNum;

      bool success1 = Int32.TryParse(year, out yearNum);
      bool success2 = Int32.TryParse(semester, out semesterNum);
      bool success3 = Int32.TryParse(group, out groupNum);
      bool success4 = Int32.TryParse(subGroup, out subGroupNum);
      bool success5 = Int32.TryParse(noOfStudents, out noOfStudentsNum);
      bool success6 = Double.TryParse(sessionDuration, out sessionDurationNum);


      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();

      //Create session table if not exist
      using var cmd = new SQLiteCommand(con);

      cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS session (
                                    sessionId               STRING PRIMARY KEY,
	                                tag                     TEXT,
	                                year                    INTEGER,
	                                semester                INTEGER,
                                    program                 TEXT,
                                    groupId                   INTEGER,
                                    subGroupId                INTEGER,
                                    subjectId               TEXT,
                                    noOfStudents            INTEGER,
                                    sessionDuration         DOUBLE,
                                    isUsed                  BOOLEAN
                )";
      cmd.ExecuteNonQuery();

      using var cmd1 = new SQLiteCommand(con);

      cmd1.CommandText = "INSERT INTO session VALUES" +
          "(@sessionId, @tag, @year, @semester, @program, @groupId, @subGroupId, @subjectId, @noOfStudents, @sessionDuration,@isUsed)";

      cmd1.Parameters.AddWithValue("@sessionId", sessionId);
      cmd1.Parameters.AddWithValue("@tag", tag);
      cmd1.Parameters.AddWithValue("@year", yearNum);
      cmd1.Parameters.AddWithValue("@semester", semesterNum);
      cmd1.Parameters.AddWithValue("@program", program);
      cmd1.Parameters.AddWithValue("@groupId", groupNum);
      cmd1.Parameters.AddWithValue("@subGroupId", subGroupNum);
      cmd1.Parameters.AddWithValue("@subjectId", subjectId);
      cmd1.Parameters.AddWithValue("@noOfStudents", noOfStudentsNum);
      cmd1.Parameters.AddWithValue("@sessionDuration", sessionDurationNum);
      cmd1.Parameters.AddWithValue("@isUsed", false);


      cmd1.Prepare();
      bool addSessionOK = true;
      try
      {
        cmd1.ExecuteNonQuery();
      }
      catch (SQLiteException ex)
      {
        if (ex.ErrorCode == 19)
        {
          MessageBox.Show("Session Already created");
          addSessionOK = false;
        }
      }


      //Adding Lecturers

      if (addSessionOK == true)
      {
        //Create session table if not exist
        using var cmdLec = new SQLiteCommand(con);

        cmdLec.CommandText = @"CREATE TABLE  IF NOT EXISTS session_lecturers (
                                    sessionId   TEXT,
	                                lecturerID  INTEGER
                                    
                )";
        cmdLec.ExecuteNonQuery();

        foreach (Lecturer element in selectedLecturersListForCreateSession)
        {

          using var cmdLecAdd = new SQLiteCommand(con);

          cmdLecAdd.CommandText = "INSERT INTO session_lecturers VALUES" +
              "(@sessionId, @lecturerID)";

          cmdLecAdd.Parameters.AddWithValue("@sessionId", sessionId);
          cmdLecAdd.Parameters.AddWithValue("@lecturerID", element.lecturerID);

          cmdLecAdd.Prepare();
          cmdLecAdd.ExecuteNonQuery();
        }
        MessageBox.Show("Session Created Successfully");
        this.Hide();
        GenerateReportDashboard obj = new GenerateReportDashboard();
        obj.Show();
      }



      con.Close();



    }

    private void cmbProgram_CreateSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      string selectedYear = this.cmbYear_CreateSession.GetItemText(this.cmbYear_CreateSession.SelectedItem);
      string selectedSemester = this.cmbSemester_CreateSession.GetItemText(this.cmbSemester_CreateSession.SelectedItem);

      string selectedProgram = this.cmbProgram_CreateSession.GetItemText(this.cmbProgram_CreateSession.SelectedItem);

      fillSubjectAccordingToProgram_Year_Semester(selectedProgram, selectedYear, selectedSemester);
      //fillGroupNumbers(selectedProgram, selectedYear, selectedSemester);
    }

    private void fillGroupNumbers(string selectedProgram, string selectedYear, string selectedSemester)
    {

      string year = "Year " + selectedYear;
      string semester = "Semester " + selectedSemester;

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();
      string stm = "select " +
              "s.group_no AS Group_No " +
              " from year_semester s" +
              " where s.year = '" + year + "' AND s.semester = '" + semester + "' AND s.programe = '" + selectedProgram + "'";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        string groupNo = $@"{ rdr.GetString(0),-8}";

        cmbGroup_CreateSession.Items.Add(groupNo);
      }

      con.Close();
    }

    private void fillSubjectAccordingToProgram_Year_Semester(string selectedProgram, string selectedYear, string selectedSemester)
    {

      int selectedYearNum;
      int selectedSemesterNum;

      bool success1 = Int32.TryParse(selectedYear, out selectedYearNum);
      bool success2 = Int32.TryParse(selectedSemester, out selectedSemesterNum);

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();
      string stm = "select " +
              "s.subjectCode AS Subject_Code ," +
              "s.subjectName AS Subject_Name " +
              " from subjects s" +
              " where s.program = '" + selectedProgram + "' AND s.offeredYear = " + selectedYearNum + " AND s.offeredSemester = " + selectedSemesterNum + "";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();
      cmbSubject_CreateSession.Items.Clear();

      while (rdr.Read())
      {
        string subjectCode = $@"{ rdr.GetString(0),-8}";
        string subjectName = $@"{ rdr.GetString(1),-8}";

        string generatedCode = subjectCode.Trim() + " - " + subjectName.Trim(); ;
        cmbSubject_CreateSession.Items.Add(generatedCode);
      }

      //#####

      string yr = "Year " + selectedYear;
      string sem = "Semester " + selectedSemester;

      string stm1 = "select DISTINCT " +
             "s.group_no AS Group_No " +
             " from year_semester s" +
             " where s.year = '" + yr + "' AND s.semester = '" + sem + "' AND s.programe = '" + selectedProgram + "' ORDER BY s.group_no ";

      using var cmd1 = new SQLiteCommand(stm1, con);
      using SQLiteDataReader rdr1 = cmd1.ExecuteReader();

      cmbGroup_CreateSession.Items.Clear();
      while (rdr1.Read())
      {
        int group = Int32.Parse($@"{rdr1.GetInt32(0),-3}");
        cmbGroup_CreateSession.Items.Add(group.ToString());
      }


      //  cmbSubGroup_CreateSession.Items.Add("1");
      //  cmbSubGroup_CreateSession.Items.Add("2");

      con.Close();
    }

    private void btnGenerateSummary_Click(object sender, EventArgs e)
    {

      string sessionId = txtSessionID_CreateSession.Text;

      string selectedTag = this.cmbTag_CreateSession.GetItemText(this.cmbTag_CreateSession.SelectedItem);
      string selectedYear = this.cmbYear_CreateSession.GetItemText(this.cmbYear_CreateSession.SelectedItem);
      string selectedSemester = this.cmbSemester_CreateSession.GetItemText(this.cmbSemester_CreateSession.SelectedItem);
      string selectedProgram = this.cmbProgram_CreateSession.GetItemText(this.cmbProgram_CreateSession.SelectedItem);
      string selectedGroup = this.cmbGroup_CreateSession.GetItemText(this.cmbGroup_CreateSession.SelectedItem);
      string selectedSubGroup = this.cmbSubGroup_CreateSession.GetItemText(this.cmbSubGroup_CreateSession.SelectedItem);
      string selectedSubjectId = this.cmbSubject_CreateSession.GetItemText(this.cmbSubject_CreateSession.SelectedItem);


      string noOfStudents = txtNoOfStudents_CreateSession.Text;
      string sessionDuration = txtDuration_CreateSession.Text;


      double numberDur;
      int numberStd;

      if (sessionId.Equals(""))
      {
        MessageBox.Show("Please enter session ID");
      }
      else if (selectedTag.Equals(""))
      {
        MessageBox.Show("Please select a tag");
      }
      else if (selectedYear.Equals(""))
      {
        MessageBox.Show("Please select the year");
      }
      else if (selectedSemester.Equals(""))
      {
        MessageBox.Show("Please select the semester");
      }
      else if (selectedProgram.Equals(""))
      {
        MessageBox.Show("Please select the program");
      }
      else if (selectedGroup.Equals(""))
      {
        MessageBox.Show("Please select the group");
      }
      else if (selectedSubGroup.Equals(""))
      {
        MessageBox.Show("Please select the sub group");
      }
      else if (selectedSubjectId.Equals(""))
      {
        MessageBox.Show("Please select the subject ");
      }
      else if (noOfStudents.Equals(""))
      {
        MessageBox.Show("Please enter number of students");
      }
      else if (!Int32.TryParse(noOfStudents, out numberStd))
      {
        MessageBox.Show("Number of students must be a valid number");
      }
      else if (sessionDuration.Equals(""))
      {
        MessageBox.Show("Please enter session duration");
      }
      else if (!Double.TryParse(sessionDuration, out numberDur))
      {
        MessageBox.Show("Session duration must be a number");
      }
      else if (selectedLecturersListForCreateSession.Count == 0)
      {
        MessageBox.Show("Please add lecturers");
      }
      else
      {
        //set session id
        lblSessionID_CreateSession_Summary.Text = txtSessionID_CreateSession.Text;

        //set subject
        string[] lines = this.cmbSubject_CreateSession.GetItemText(this.cmbSubject_CreateSession.SelectedItem).ToString().Split(new string[] { " - " }, StringSplitOptions.None);
        string finalStr = lines[1] + " ( " + lines[0] + " ) ";
        lblSubject_CreateSession_summary.Text = finalStr;

        //Set tag
        lblTag_CreateSession_Summary.Text = this.cmbTag_CreateSession.GetItemText(this.cmbTag_CreateSession.SelectedItem);

        //Generate Count and Hours
        string countAndHours = txtNoOfStudents_CreateSession.Text + " ( " + txtDuration_CreateSession.Text + " ) ";
        lblCountAndHrs_CreateSession.Text = countAndHours;

        //Generate Summary of group

        string year = this.cmbYear_CreateSession.GetItemText(this.cmbYear_CreateSession.SelectedItem);
        string semester = this.cmbSemester_CreateSession.GetItemText(this.cmbSemester_CreateSession.SelectedItem);
        string prog = this.cmbProgram_CreateSession.GetItemText(this.cmbProgram_CreateSession.SelectedItem);
        string gr = this.cmbGroup_CreateSession.GetItemText(this.cmbGroup_CreateSession.SelectedItem);

        string fullString = "Y" + year + ".S" + semester + "." + prog + "." + gr;
        lblStudentCreateSessionSummary_CreateSession.Text = fullString;
      }

    }

    private void btnRefresh_SessionManagement_Click(object sender, EventArgs e)
    {
      refreshManageSessionGrid();
    }

    private void refreshManageSessionGrid()
    {
      createSessionIfNotExist();
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);

      System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(
          "select session.sessionId AS SessionId ,session.tag AS Tag ,session.year AS Year ,session.semester AS Semester ,session.program AS Program ,session.groupId AS Group_Id ,session.subGroupId AS SubGroup_Id ,session.subjectId AS Subject_Id ,session.noOfStudents AS No_of_Students ,session.sessionDuration AS Hours, session.isUsed AS IS_Used from session"
          );

      cmd.Connection = conn;

      conn.Open();

      System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd);
      System.Data.DataSet ds = new System.Data.DataSet();

      DataTable dt = new DataTable();
      da.Fill(dt);

      manageSessionGridView.DataSource = dt;



      using var cmdCreateSubject = new SQLiteCommand(conn);

      cmdCreateSubject.CommandText = @"CREATE TABLE  IF NOT EXISTS session (
                                    sessionId               STRING PRIMARY KEY,
	                                tag                     TEXT,
	                                year                    INTEGER,
	                                semester                INTEGER,
                                    program                 TEXT,
                                    groupId                   INTEGER,
                                    subGroupId                INTEGER,
                                    subjectId               TEXT,
                                    noOfStudents            INTEGER,
                                    sessionDuration         DOUBLE,
                                    isUsed                  BOOLEAN
                )";
      cmdCreateSubject.ExecuteNonQuery();



      //Create session table if not exist
      using var cmdLec = new SQLiteCommand(conn);

      cmdLec.CommandText = @"CREATE TABLE  IF NOT EXISTS session_lecturers (
                                    sessionId   TEXT,
	                                lecturerID  INTEGER
                                    
                )";
      cmdLec.ExecuteNonQuery();
      conn.Close();
    }

    private void createSessionIfNotExist()
    {
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);
      using var cmdCreateSubject = new SQLiteCommand(conn);

      conn.Open();

      cmdCreateSubject.CommandText = @"CREATE TABLE  IF NOT EXISTS session (
                                    sessionId               STRING PRIMARY KEY,
	                                tag                     TEXT,
	                                year                    INTEGER,
	                                semester                INTEGER,
                                    program                 TEXT,
                                    groupId                   INTEGER,
                                    subGroupId                INTEGER,
                                    subjectId               TEXT,
                                    noOfStudents            INTEGER,
                                    sessionDuration         DOUBLE,
                                    isUsed                  BOOLEAN
                )";
      cmdCreateSubject.ExecuteNonQuery();
      conn.Close();
    }

    private void manageSessionGridView_SelectionChanged(object sender, EventArgs e)
    {
      int rowindex = manageSessionGridView.CurrentCell.RowIndex;
      int columnindex = manageSessionGridView.CurrentCell.ColumnIndex;

      string a = manageSessionGridView.Rows[rowindex].Cells[columnindex].Value.ToString();
    }

    private void manageSessionGridView_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      int rowindex = manageSessionGridView.CurrentCell.RowIndex;

      string sessionId = manageSessionGridView.Rows[rowindex].Cells[0].Value.ToString();
      sessionId = sessionId.Trim();
      loadLecturersAndFillImageGrid_SessionManagement(sessionId);
      setSummaryVisibleOrHidden_ManageSession(true);

      fillsetSummaryManageSession(sessionId);
      setSummaryVisibleOrHidden_ManageSession(true);
    }

    private void fillsetSummaryManageSession(string sessionId)
    {
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();
      string stm = "";

      stm = "select session.sessionId AS SessionId ,session.tag AS Tag ,session.year AS Year ,session.semester AS Semester ,session.program AS Program ,session.groupId AS Group_Id ,session.subGroupId AS SubGroup_Id ,session.subjectId AS Subject_Id ,session.noOfStudents AS No_of_Students ,session.sessionDuration AS Hours from session WHERE session.sessionId='" + sessionId + "'";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        // Console.WriteLine($@"{rdr.GetInt32(0),-3} {rdr.GetString(1),-8} {rdr.GetInt32(2),8}");
        string SessionId = $@"{ rdr.GetString(0),-8}".Trim();
        string tag = $@"{ rdr.GetString(1),-8}".Trim();
        int year = Int32.Parse($@"{rdr.GetInt32(2),-3}");
        int semester = Int32.Parse($@"{rdr.GetInt32(3),-3}");
        string program = $@"{ rdr.GetString(4),-8}".Trim();
        int groupId = Int32.Parse($@"{rdr.GetInt32(5),-3}");
        int subGroupId = Int32.Parse($@"{rdr.GetInt32(6),-3}");
        string subjectId = $@"{ rdr.GetString(7),-8}".Trim();
        int noOfStudents = Int32.Parse($@"{rdr.GetInt32(8),-3}");
        double duration = Double.Parse($@"{rdr.GetDouble(9),-3}");



        lblSessionId_ManageSession.Text = SessionId;
        lblTag_ManageSession.Text = tag;
        lblYear_ManageSession.Text = year.ToString();
        lblSemester_ManageSession.Text = semester.ToString();
        lblProgram_ManageSession.Text = program;
        lblGroupId_ManageSession.Text = groupId.ToString();
        lblSubGroupId_ManageSession.Text = subGroupId.ToString();
        lblSubjectId_ManageSession.Text = subjectId;
        lblNoOfStudents_ManageSession.Text = noOfStudents.ToString();
        lblDuration_ManageSession.Text = duration.ToString();

      }
      con.Close();
    }

    private void loadLecturersAndFillImageGrid_SessionManagement(string sessionId)
    {
      List<string> lecList = new List<string>();
      lecList = loadLecturerIdsBysessionID(sessionId);

      List<Lecturer> lecListFull = new List<Lecturer>();

      lecListFull = getLecturerObjectListByProvindingLecturerIDList(lecList);

      drawLecturersInGridView_ManageSession(lecListFull);
    }

    private void drawLecturersInGridView_ManageSession(List<Lecturer> lecListFull)
    {
      this.imgLecGridView.Rows.Clear();
      List<Object> temp = new List<object>();
      Object[] row = null;

      List<DataGridViewTextBoxCell> tempStr = new List<DataGridViewTextBoxCell>();
      DataGridViewTextBoxCell[] rowStr = null;

      foreach (Lecturer element in lecListFull)
      {
        DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
        imgCol.DefaultCellStyle.NullValue = null;
        imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
        imgLecGridView.Columns.Add(imgCol);

        string photoPath = element.photoPath;
        Image img = Image.FromFile(@"" + photoPath);
        temp.Add(img);



      }
      imgLecGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

      row = temp.Cast<object>().ToArray();
      imgLecGridView.Rows.Add(row);



      /*
      if (this.imgDataGridView_CreateSession.Columns.Count < 3)
      {
          DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
          imgCol.DefaultCellStyle.NullValue = null;
          imgDataGridView_CreateSession.Columns.Add(imgCol);
          imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
      }

      imgDataGridView_CreateSession.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

      List<Object> temp = new List<object>();
      for (int i = 0; i < selectedLecturersListForCreateSession.Count; i++)
      {
          string photoPath = selectedLecturersListForCreateSession[i].photoPath;
          Image img = Image.FromFile(@"" + photoPath);
          temp.Add(img);
          if ((i + 1) % 3 == 0)
          {
              Object[] row = temp.Cast<object>().ToArray();
              imgDataGridView_CreateSession.Rows.Add(row);
              temp = new List<object>();
          }
      }
      if (temp.Count > 0)
      {
          imgDataGridView_CreateSession.Rows.Add(temp.Cast<object>().ToArray());
      }

      */




    }

    private List<string> loadLecturerIdsBysessionID(string sessionId)
    {
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();
      string stm = "";

      stm = "SELECT lecturerID FROM session_lecturers WHERE sessionId='" + sessionId + "'";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      List<string> foundLecturers = new List<string>();

      while (rdr.Read())
      {

        // Console.WriteLine($@"{rdr.GetInt32(0),-3} {rdr.GetString(1),-8} {rdr.GetInt32(2),8}");
        string lecID = ($@"{ rdr.GetInt32(0),-8 }");


        foundLecturers.Add(lecID);
      }
      con.Close();
      return foundLecturers;
    }

    private void cmbSubject_ManageSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dashboardLoaded == true)
      {
        filterSessions_ManageSession(
       this.cmbSubject_ManageSession.GetItemText(this.cmbSubject_ManageSession.SelectedItem),
       this.cmbLecturer_ManageSession.GetItemText(this.cmbLecturer_ManageSession.SelectedItem),
       this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem),
       this.cmbSubGroup_ManageSession.GetItemText(this.cmbSubGroup_ManageSession.SelectedItem),
       this.cmbTag_ManageSession.GetItemText(this.cmbTag_ManageSession.SelectedItem),
       this.cmbIsUsed_ManageSession.GetItemText(this.cmbIsUsed_ManageSession.SelectedItem).Trim()
    );
      }



    }

    private void cmbLecturer_ManageSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dashboardLoaded == true)
      {
        filterSessions_ManageSession(
        this.cmbSubject_ManageSession.GetItemText(this.cmbSubject_ManageSession.SelectedItem),
        this.cmbLecturer_ManageSession.GetItemText(this.cmbLecturer_ManageSession.SelectedItem),
        this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem),
        this.cmbSubGroup_ManageSession.GetItemText(this.cmbSubGroup_ManageSession.SelectedItem),
        this.cmbTag_ManageSession.GetItemText(this.cmbTag_ManageSession.SelectedItem),
        this.cmbIsUsed_ManageSession.GetItemText(this.cmbIsUsed_ManageSession.SelectedItem).Trim()
    );
      }


    }

    private void cmbGroup_ManageSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dashboardLoaded == true)
      {
        string groupText = this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem);
        if (!groupText.Equals("Any"))
        {
          refreshSubGroupAccordingToGroup(this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem));
        }

        filterSessions_ManageSession(
            this.cmbSubject_ManageSession.GetItemText(this.cmbSubject_ManageSession.SelectedItem),
            this.cmbLecturer_ManageSession.GetItemText(this.cmbLecturer_ManageSession.SelectedItem),
            this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem),
            this.cmbSubGroup_ManageSession.GetItemText(this.cmbSubGroup_ManageSession.SelectedItem),
            this.cmbTag_ManageSession.GetItemText(this.cmbTag_ManageSession.SelectedItem),
            this.cmbIsUsed_ManageSession.GetItemText(this.cmbIsUsed_ManageSession.SelectedItem).Trim()
            );
      }


    }

    private void cmbSubGroup_ManageSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dashboardLoaded == true)
      {
        filterSessions_ManageSession(
            this.cmbSubject_ManageSession.GetItemText(this.cmbSubject_ManageSession.SelectedItem),
            this.cmbLecturer_ManageSession.GetItemText(this.cmbLecturer_ManageSession.SelectedItem),
            this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem),
            this.cmbSubGroup_ManageSession.GetItemText(this.cmbSubGroup_ManageSession.SelectedItem),
            this.cmbTag_ManageSession.GetItemText(this.cmbTag_ManageSession.SelectedItem),
            this.cmbIsUsed_ManageSession.GetItemText(this.cmbIsUsed_ManageSession.SelectedItem).Trim()
    );
      }

    }



    private void cmbTag_ManageSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dashboardLoaded == true)
      {
        filterSessions_ManageSession(
       this.cmbSubject_ManageSession.GetItemText(this.cmbSubject_ManageSession.SelectedItem),
       this.cmbLecturer_ManageSession.GetItemText(this.cmbLecturer_ManageSession.SelectedItem),
       this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem),
       this.cmbSubGroup_ManageSession.GetItemText(this.cmbSubGroup_ManageSession.SelectedItem),
       this.cmbTag_ManageSession.GetItemText(this.cmbTag_ManageSession.SelectedItem),
       this.cmbIsUsed_ManageSession.GetItemText(this.cmbIsUsed_ManageSession.SelectedItem).Trim()
        );
      }

    }

    private void cmbIsUsed_ManageSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dashboardLoaded == true)
      {
        filterSessions_ManageSession(
       this.cmbSubject_ManageSession.GetItemText(this.cmbSubject_ManageSession.SelectedItem),
       this.cmbLecturer_ManageSession.GetItemText(this.cmbLecturer_ManageSession.SelectedItem),
       this.cmbGroup_ManageSession.GetItemText(this.cmbGroup_ManageSession.SelectedItem),
       this.cmbSubGroup_ManageSession.GetItemText(this.cmbSubGroup_ManageSession.SelectedItem),
       this.cmbTag_ManageSession.GetItemText(this.cmbTag_ManageSession.SelectedItem),
       this.cmbIsUsed_ManageSession.GetItemText(this.cmbIsUsed_ManageSession.SelectedItem).Trim()
        );
      }
    }

    private void refreshSubGroupAccordingToGroup(string selectedGroup)
    {

      cmbSubGroup_ManageSession.Items.Clear();
      cmbSubGroup_ManageSession.Items.Add("Any");

      if (!selectedGroup.Equals("Any") || !selectedGroup.Trim().Equals(""))
      {
        string cs = @"URI=file:.\" + Utils.dbName + ".db";

        using var con = new SQLiteConnection(cs);
        con.Open();

        using var cmdSubGrp = new SQLiteCommand(con);

        cmdSubGrp.CommandText = @"CREATE TABLE  IF NOT EXISTS year_semester (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    year TEXT,
                                    semester TEXT,
                                    programe TEXT,
                                    group_no INTEGER,
                                    subgroup_no INTEGER,
                                    group_id TEXT,
                                    subgroup_id TEXT
                    )";
        cmdSubGrp.ExecuteNonQuery();

        int selectedGroupNum;
        bool success = Int32.TryParse(selectedGroup, out selectedGroupNum);

        string stm = "SELECT DISTINCT y.subgroup_no FROM year_semester y WHERE y.group_no = " + selectedGroupNum + " ORDER BY y.subgroup_no ";


        using var cmd = new SQLiteCommand(stm, con);
        using SQLiteDataReader rdr = cmd.ExecuteReader();

        while (rdr.Read())
        {
          int subGroupId = Int32.Parse($@"{rdr.GetInt32(0),-3}");
          cmbSubGroup_ManageSession.Items.Add(subGroupId.ToString());
        }
        con.Close();
      }
      else
      {

      }

      cmbSubGroup_ManageSession.SelectedIndex = 0;

    }

    private void fillSessionManagementCmb()
    {
      fillSubjectCmb_ManageSession();
      fillLecturerCmb_ManageSession();
      fillGroupCmb_ManageSession();
      //fillSubGroupCmb_ManageSession();
      refreshSubGroupAccordingToGroup("Any");
      fillTagCmb_ManageSession();
      fillUsedCmb_ManageSession();
    }

    private void fillSubjectCmb_ManageSession()
    {
      cmbSubject_ManageSession.Items.Clear();
      cmbSubject_ManageSession.Items.Add("Any");

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();

      using var cmdSub = new SQLiteCommand(con);

      cmdSub.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects (
                                    subjectCode STRING PRIMARY KEY,
	                                subjectName TEXT,
	                                offeredYear INTEGER,
	                                offeredSemester INTEGER,
                                    program TEXT,
	                                isParallel BOOLEAN,
	                                category TEXT
                )";
      cmdSub.ExecuteNonQuery();



      string stm = "SELECT s.subjectCode FROM subjects s";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        string subjectCode = $@"{ rdr.GetString(0),-8}".Trim();
        cmbSubject_ManageSession.Items.Add(subjectCode);
      }
      con.Close();


      cmbSubject_ManageSession.SelectedIndex = 0;
    }

    private void fillLecturerCmb_ManageSession()
    {
      cmbLecturer_ManageSession.Items.Clear();
      cmbLecturer_ManageSession.Items.Add("Any");

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      using var cmdSub = new SQLiteCommand(con);

      cmdSub.CommandText = @"CREATE TABLE  IF NOT EXISTS lecturers (
                                lecturerID INTEGER PRIMARY KEY,
                                title TEXT,
                                name TEXT,
                                faculty TEXT,
                                department TEXT,
                                center TEXT,
                                building TEXT,
                                employeeLevel INT,
                                photoPath TEXT        
                )";
      cmdSub.ExecuteNonQuery();

      string stm = "SELECT l.lecturerID FROM lecturers l";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        // string lecturerId = $@"{ rdr.GetString(0),-8}";
        int lecturerId = Int32.Parse($@"{rdr.GetInt32(0),-3}");
        //string lecturerId = "aaa";

        cmbLecturer_ManageSession.Items.Add(lecturerId.ToString());
      }
      con.Close();


      cmbLecturer_ManageSession.SelectedIndex = 0;
    }

    private void fillUsedCmb_ManageSession()
    {
      cmbIsUsed_ManageSession.Items.Clear();
      cmbIsUsed_ManageSession.Items.Add("Any");
      cmbIsUsed_ManageSession.Items.Add("Used");
      cmbIsUsed_ManageSession.Items.Add("Not Used");
      cmbIsUsed_ManageSession.SelectedIndex = 0;
    }

    private void fillGroupCmb_ManageSession()
    {
      cmbGroup_ManageSession.Items.Clear();
      cmbGroup_ManageSession.Items.Add("Any");

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      using var cmdGro = new SQLiteCommand(con);

      cmdGro.CommandText = @"CREATE TABLE  IF NOT EXISTS year_semester (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                year TEXT,
                                semester TEXT,
                                programe TEXT,
                                group_no INTEGER,
                                subgroup_no INTEGER,
                                group_id TEXT,
                                subgroup_id TEXT
                )";
      cmdGro.ExecuteNonQuery();

      string stm = "SELECT DISTINCT  ys.group_no FROM year_semester ys ORDER BY ys.group_no";

      using var cmd = new SQLiteCommand(stm, con);
      using SQLiteDataReader rdr = cmd.ExecuteReader();

            try
            {

            
                  while (rdr.Read())
                  {
                            // string lecturerId = $@"{ rdr.GetString(0),-8}";
                            int groupNo = Int32.Parse($@"{rdr.GetInt32(0),-3}");
                
                            cmbGroup_ManageSession.Items.Add(groupNo.ToString());
                  }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Please fill all fields in year semester table");
                this.Hide();
                ManageStudentsDashboard obj = new ManageStudentsDashboard();
                obj.Show();
            }

            con.Close();

      cmbGroup_ManageSession.SelectedIndex = 0;
    }

    private void fillSubGroupCmb_ManageSession()
    {
      cmbSubGroup_ManageSession.Items.Clear();

      cmbSubGroup_ManageSession.Items.Add("Any");

    }

    private void fillTagCmb_ManageSession()
    {
      cmbTag_ManageSession.Items.Clear();

      cmbTag_ManageSession.Items.Add("Any");
      cmbTag_ManageSession.Items.Add("Lecture");
      cmbTag_ManageSession.Items.Add("Tutorial");
      cmbTag_ManageSession.Items.Add("Lab");
      cmbTag_ManageSession.Items.Add("Evaluation");

      cmbTag_ManageSession.SelectedIndex = 0;
    }

    private void setSummaryVisibleOrHidden_ManageSession(bool status)
    {
      sessionGroupBox_ManageSession.Visible = status;

      lblTitle_SessionId_ManageSession.Visible = status;
      lblTitle_Tag_ManageSession.Visible = status;
      lblYear_SessionId_ManageSession.Visible = status;
      lblTitle_Semester_ManageSession.Visible = status;
      lblTitle_Program_ManageSession.Visible = status;
      lblTitle_GroupId_ManageSession.Visible = status;
      lblTitle_SubGroupId_ManageSession.Visible = status;
      lblTitle_SubjectId_ManageSession.Visible = status;
      lblTitle_NoOfStudents_ManageSession.Visible = status;
      lblTitle_Duration_ManageSession.Visible = status;

      lblSessionId_ManageSession.Visible = status;
      lblTag_ManageSession.Visible = status;
      lblYear_ManageSession.Visible = status;
      lblSemester_ManageSession.Visible = status;
      lblProgram_ManageSession.Visible = status;
      lblGroupId_ManageSession.Visible = status;
      lblSubGroupId_ManageSession.Visible = status;
      lblSubjectId_ManageSession.Visible = status;
      lblNoOfStudents_ManageSession.Visible = status;
      lblDuration_ManageSession.Visible = status;

      btnRemoveSession_ManageSession.Visible = status;
    }

    private void btnResetManageSession_Click(object sender, EventArgs e)
    {
      this.Hide();
      GenerateReportDashboard obj = new GenerateReportDashboard();
      obj.Show();
      obj.tabControl1.SelectedIndex = 1;
    }

    private void tabControl1_Selected(object sender, TabControlEventArgs e)
    {
      if (tabControl1.SelectedTab == SessionManagement)
      {
        refreshManageSessionGrid();
      }
    }

    private void btnRemoveSession_ManageSession_Click(object sender, EventArgs e)
    {
      removeSession_ManageSession();
    }

    private void removeSession_ManageSession()
    {
      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      //delete from session lecturers table
      using var cmd = new SQLiteCommand(con);

      cmd.CommandText = @"DELETE FROM session_lecturers WHERE sessionId = '" + lblSessionId_ManageSession.Text.Trim() + "'";
      cmd.ExecuteNonQuery();

      //delete from session lecturers table
      using var cmd1 = new SQLiteCommand(con);

      cmd1.CommandText = @"DELETE FROM session WHERE sessionId = '" + lblSessionId_ManageSession.Text.Trim() + "'";
      cmd1.ExecuteNonQuery();


      con.Close();
      MessageBox.Show("Session deleted successfully");
      this.Hide();
      GenerateReportDashboard obj = new GenerateReportDashboard();
      obj.Show();
      obj.tabControl1.SelectedIndex = 1;

    }

    private void filterSessions_ManageSession(string subject, string lecturer, string group, string subGroup, string tag, string isUsed)
    {

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      System.Data.SQLite.SQLiteConnection conn1 = new System.Data.SQLite.SQLiteConnection(cs);
      using var cmdCreateSubject = new SQLiteCommand(conn1);

      cmdCreateSubject.Connection = conn1;

      conn1.Open();


      cmdCreateSubject.CommandText = @"CREATE TABLE  IF NOT EXISTS session (
                                    sessionId               STRING PRIMARY KEY,
	                                tag                     TEXT,
	                                year                    INTEGER,
	                                semester                INTEGER,
                                    program                 TEXT,
                                    groupId                   INTEGER,
                                    subGroupId                INTEGER,
                                    subjectId               TEXT,
                                    noOfStudents            INTEGER,
                                    sessionDuration         DOUBLE,
                                    isUsed                  BOOLEAN
                )";
      cmdCreateSubject.ExecuteNonQuery();

      //Create session table if not exist
      using var cmdLec = new SQLiteCommand(conn1);



      cmdLec.CommandText = @"CREATE TABLE  IF NOT EXISTS session_lecturers (
                                    sessionId   TEXT,
	                                lecturerID  INTEGER
                                    
                )";
      cmdLec.ExecuteNonQuery();

      conn1.Close();

      System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);

      //Filtering start
      string query = "";

      if (lecturer.Equals("Any"))
      {
        query = "select " +
       "session.sessionId AS SessionId ," +
       "session.tag AS Tag ," +
       "session.year AS Year ," +
       "session.semester AS Semester ," +
       "session.program AS Program ," +
       "session.groupId AS Group_Id ," +
       "session.subGroupId AS SubGroup_Id ," +
       "session.subjectId AS Subject_Id ," +
       "session.noOfStudents AS No_of_Students ," +
       "session.sessionDuration AS Hours ," +
       "session.isUsed AS isUsed " +
       "from session ";
        bool whereClauseAdded = false;
        bool havePrevCondition = false;

        if (!subject.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }

          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
            whereClauseAdded = true;
          }


          query = query + "session.subjectId = '" + subject + "' ";
          havePrevCondition = true;
        }
        if (!group.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
            whereClauseAdded = true;
          }

          query = query + "session.groupId = '" + group + "' ";
          havePrevCondition = true;

        }
        if (!subGroup.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
            whereClauseAdded = true;
          }

          query = query + "session.subGroupId = '" + subGroup + "' ";
          havePrevCondition = true;

        }
        if (!tag.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
            whereClauseAdded = true;
          }

          query = query + "session.tag = '" + tag + "' ";
          havePrevCondition = true;
        }
        if (!isUsed.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
            whereClauseAdded = true;
          }

          bool boolIsUsed = false;
          if (isUsed.Equals("Used"))
          {
            boolIsUsed = true;
          }
          else if (isUsed.Equals("Not Used"))
          {
            boolIsUsed = false;
          }



          query = query + "session.isUsed = " + boolIsUsed + " ";
          havePrevCondition = true;
        }

        Console.WriteLine(query);
      }
      else
      {
        query = "select " +
   "s.sessionId AS SessionId ," +
   "s.tag AS Tag ," +
   "s.year AS Year ," +
   "s.semester AS Semester ," +
   "s.program AS Program ," +
   "s.groupId AS Group_Id ," +
   "s.subGroupId AS SubGroup_Id ," +
   "s.subjectId AS Subject_Id ," +
   "s.noOfStudents AS No_of_Students ," +
   "s.sessionDuration AS Hours " +
   "from session s , session_lecturers l WHERE s.sessionId= l.sessionId AND l.lecturerID = '" + lecturer + "'";
        bool whereClauseAdded = false;
        bool havePrevCondition = false;

        if (!subject.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }

          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
          }


          query = query + "session.subjectId = '" + subject + "' ";
          havePrevCondition = true;
        }
        if (!group.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
          }

          query = query + "session.groupId = '" + group + "' ";
          havePrevCondition = true;

        }
        if (!subGroup.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
          }

          query = query + "session.subGroupId = '" + subGroup + "' ";
          havePrevCondition = true;

        }
        if (!tag.Equals("Any"))
        {
          if (havePrevCondition == true)
          {
            query = query + " AND ";
          }
          if (whereClauseAdded == false)
          {
            query = query + " WHERE ";
          }

          query = query + "session.tag = '" + tag + "' ";
          havePrevCondition = true;
        }

        Console.WriteLine(query);
      }

      System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(query);

      cmd.Connection = conn;

      conn.Open();

      System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd);
      System.Data.DataSet ds = new System.Data.DataSet();

      DataTable dt = new DataTable();
      da.Fill(dt);

      manageSessionGridView.DataSource = dt;
      conn.Close();

      //Filter End


    }

    private void cmbGroup_CreateSession_SelectedIndexChanged(object sender, EventArgs e)
    {
      string selectedYear = this.cmbYear_CreateSession.GetItemText(this.cmbYear_CreateSession.SelectedItem);
      string selectedSemester = this.cmbSemester_CreateSession.GetItemText(this.cmbSemester_CreateSession.SelectedItem);

      string selectedProgram = this.cmbProgram_CreateSession.GetItemText(this.cmbProgram_CreateSession.SelectedItem);

      string selectedGroup = this.cmbGroup_CreateSession.GetItemText(this.cmbGroup_CreateSession.SelectedItem);

      fillSubGroupAccordingToProgram_Year_Semester_Group(selectedProgram, selectedYear, selectedSemester, selectedGroup);
    }

    private void fillSubGroupAccordingToProgram_Year_Semester_Group(string selectedProgram, string selectedYear, string selectedSemester, string selectedGroup)
    {


      int selectedYearNum;
      int selectedSemesterNum;

      bool success1 = Int32.TryParse(selectedYear, out selectedYearNum);
      bool success2 = Int32.TryParse(selectedSemester, out selectedSemesterNum);

      string cs = @"URI=file:.\" + Utils.dbName + ".db";

      using var con = new SQLiteConnection(cs);
      con.Open();


      //#####

      string yr = "Year " + selectedYear;
      string sem = "Semester " + selectedSemester;


      string stm1 = "select DISTINCT " +
             "s.subgroup_no AS SubGroup_No " +
             " from year_semester s" +
             " where s.year = '" + yr + "' AND s.semester = '" + sem + "' AND s.programe = '" + selectedProgram + "' AND s.group_no= '" + selectedGroup + "' ORDER BY s.subgroup_no ";

      using var cmd1 = new SQLiteCommand(stm1, con);
      using SQLiteDataReader rdr1 = cmd1.ExecuteReader();

      cmbSubGroup_CreateSession.Items.Clear();
      while (rdr1.Read())
      {
        int subgroup = Int32.Parse($@"{rdr1.GetInt32(0),-3}");
        cmbSubGroup_CreateSession.Items.Add(subgroup.ToString());
      }


      //  cmbSubGroup_CreateSession.Items.Add("1");
      //  cmbSubGroup_CreateSession.Items.Add("2");

      con.Close();




    }

    private void imgLoggedUser_Click(object sender, EventArgs e)
    {
      this.Hide();
      Login obj = new Login();
      obj.Show();
    }

    private void tabPage1_Click(object sender, EventArgs e)
    {
            
     }

    private void RoomManagement_Click(object sender, EventArgs e)
    {

    }

    private void lblRoomManagement_Click(object sender, EventArgs e)
    {

    }

    private void label10_Click(object sender, EventArgs e)
    {

    }

    //add ps
    private void button1_Click(object sender, EventArgs e)
    {
            string txtQuery = "INSERT INTO parallel_sessions (subgroup_id,subject_code,lecturer_id,time_slot,session_id) VALUES('" + comboBox6.Text + "','" + comboBox2.Text + "','" + comboBox3.Text + "','" + comboBox5.Text + "','" + comboBox4.Text + "')";
            ExecuteQuery(txtQuery);
            LoadParallelSessions();
        }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    private void lblGenerateTimetable_Click(object sender, EventArgs e)
    {

    }
        //Ravindu changes
        void fillCombo()
        {

            comboTableType.Items.Clear();

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

        public string value;
        private void btnGenarate_Click(object sender, EventArgs e)
        {
            string tableType = comboTableType.Text;

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select tableID from Time_table1 where tableType = '" + tableType + "'", con);
            con.Open();

            SQLiteDataReader sdr;
            sdr = cmd.ExecuteReader();
            sdr.Read();
            value = sdr.GetValue(0).ToString();
            sdr.Close();
            con.Close();

            using var cmd1 = new SQLiteCommand("SELECT TimeSlots.fullTime, TimetableDaychk.Monday, TimetableDaychk.Tuesday,TimetableDaychk.Wednesday," +
                "TimetableDaychk.Thursday,TimetableDaychk.Friday,TimetableDaychk.Saturday,TimetableDaychk.Sunday " +
                "FROM TimeSlots INNER JOIN TimetableDaychk ON TimeSlots.tableID = TimetableDaychk.tableID WHERE TimeSlots.tableID = '" + value + "' ", con);
            /*
            cmd.CommandText = "SELECT TimeSlots.fullTime, TimetableDaychk.Monday, TimetableDaychk.Tuesday,TimetableDaychk.Wednesday," +
                "TimetableDaychk.Thursday,TimetableDaychk.Friday,TimetableDaychk.Saturday,TimetableDaychk.Sunday " +
                "FROM TimeSlots INNER JOIN TimetableDaychk ON TimeSlots.tableID = TimetableDaychk.tableID WHERE TimeSlots.tableID = '" + value + "' ";
            */
            /*
            System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd1);
            System.Data.DataSet ds = new System.Data.DataSet();

            DataTable dt = new DataTable();
            da.Fill(dt);

            simpleGrid.DataSource = dt;
            con.Close();
            */
            con.Open();
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd1);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            simpleGrid.DataSource = dt;
            con.Close();

            con.Open();
            using var cmd2 = new SQLiteCommand("SELECT Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday" +
                " from TimetableDaychk" +
                " WHERE tableID = '" + value + "' ", con);
            SQLiteDataReader sdr3 = cmd2.ExecuteReader();

            try
            {
                while (sdr3.Read())
                {
                    if (sdr3.GetInt32(0) == 1)
                    {
                        ListDays.Add("Monday");
                    }
                    if (sdr3.GetInt32(1) == 1)
                    {
                        ListDays.Add("Tuesday");
                    }
                    if (sdr3.GetInt32(2) == 1)
                    {
                        ListDays.Add("Wednesday");
                    }
                    if (sdr3.GetInt32(3) == 1)
                    {
                        ListDays.Add("Thursday");
                    }
                    if (sdr3.GetInt32(4) == 1)
                    {
                        ListDays.Add("Friday");
                    }
                    if (sdr3.GetInt32(5) == 1)
                    {
                        ListDays.Add("Saturday");
                    }
                    if (sdr3.GetInt32(6) == 1)
                    {
                        ListDays.Add("Sunday");
                    }

                    //lengthdays = ListDays.Count;
                }

                sdr3.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            int lengthdays = ListDays.Count;


            con.Open();
            cmd.CommandText = "SELECT fullTime from TimeSlots WHERE tableID = '" + value + "'";
            SQLiteDataReader sdr2 = cmd.ExecuteReader();


            try
            {
                while (sdr2.Read())
                {
                    string time = sdr2.GetString(0);
                    ListTime.Add(time);
                }

                sdr2.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            con.Open();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS fullSlot(
                                fullSlotID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                slotNo INTEGER,
                                Day TEXT,
                                time TEXT,
                                tableID     INTEGER,
                                FOREIGN KEY(tableID) REFERENCES Time_table1(tableID)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS sessionAssign(
                                saID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                sessID INTEGER,
                                slotNo INTEGER
                )";
            cmd.ExecuteNonQuery();

            int lengthTime = ListTime.Count;
            int count = 1;

            for (int y = 0; y < lengthdays; y++)
            {
                for (int x = 0; x < lengthTime; x++)
                {
                    cmd.CommandText = "INSERT INTO  fullSlot( slotNo, Day, time, tableID)" +
                    "VALUES(@slotNo, @Day,@time, @tableID)";

                    cmd.Parameters.AddWithValue("@slotNo", count);
                    cmd.Parameters.AddWithValue("@Day", ListDays[y]);
                    cmd.Parameters.AddWithValue("@time", ListTime[x]);
                    cmd.Parameters.AddWithValue("@tableID", value);

                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    count++;
                }
            }
            getOrderedSession();

            /*
            con.Open();
            int countSlot;
            using var cmd4 = new SQLiteCommand("select count(tableID) from fullSlot", con);
            sdr = cmd.ExecuteReader();

            while (sdr.Read())
            {
                countSlot = sdr.GetInt32(0);
            }
            con.Close();
            */

            getSlotNo();
            int countSlot = ListSlots.Count;
            int countSession = ListSession.Count;
            con.Close();

            con.Open();


            for (int i = 1; i <= countSlot; i++)
            {
                if (i <= countSession)
                {
                    cmd.CommandText = "INSERT INTO  sessionAssign( sessID, slotNo)" +
                   "VALUES(@sessID, @slotNo)";

                    int j = i - 1;
                    cmd.Parameters.AddWithValue("@sessID", ListSession[j]);
                    cmd.Parameters.AddWithValue("@slotNo", ListSlots[j]);

                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();

            fillComboLec();//filling lecture combo
            fillComboGroup();//filling group combo
            fillComboRoom();//filling room combo
        }

        void getOrderedSession()
        {

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select * from testSessions", con);
            SQLiteDataReader sdr;

            try
            {
                con.Open();
                sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    string session = Convert.ToString(sdr.GetInt32(1));
                    ListSession.Add(session);
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        void getSlotNo()
        {

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select * from fullSlot WHERE tableID = '" + value + "'", con);
            SQLiteDataReader sdr;

            try
            {
                con.Open();
                sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    string slotNo = Convert.ToString(sdr.GetInt32(0));
                    ListSlots.Add(slotNo);
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        void fillComboLec()
        {

            combolecName.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select * from testLec", con);
            SQLiteDataReader sdr;

            try
            {
                con.Open();
                sdr = cmd.ExecuteReader();

                combolecName.Text = "--select Lec Name --";
                while (sdr.Read())
                {
                    //string lecID = Convert.ToString(sdr.GetInt32(2));
                    string lecName = sdr.GetString(1);

                    //combolecName.DataTextField = "Text";
                    //combolecName.Items.Insert();
                    //combolecName.Items.Insert(lecID, lecName);

                    //combolecName.DataSource = comboBoxList;
                    //combolecName.DisplayMember = lecName;
                    //combolecName.ValueMember = lecID;
                    combolecName.Items.Add(lecName);
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        void fillComboGroup()
        {

            comboGroup.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select groupId,groupName from testSessions GROUP by groupId", con);
            SQLiteDataReader sdr;

            try
            {
                con.Open();
                sdr = cmd.ExecuteReader();

                comboGroup.Text = "--select Group Name --";
                while (sdr.Read())
                {
                    //string groupID = Convert.ToString(sdr.GetInt32(0));
                    string groupName = sdr.GetString(1);
                    comboGroup.Items.Add(groupName);
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        void fillComboRoom()
        {

            comboRoom.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select roomName from testSessions GROUP by roomName", con);
            SQLiteDataReader sdr;

            try
            {
                con.Open();
                sdr = cmd.ExecuteReader();

                comboRoom.Text = "--select Room Name --";
                while (sdr.Read())
                {
                    //string groupID = Convert.ToString(sdr.GetInt32(0));
                    string roomName = sdr.GetString(0);
                    comboRoom.Items.Add(roomName);
                }
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void combolecName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lecname = combolecName.SelectedItem.ToString();
            //string lecid = ((System.Web.UI.WebControls.ListItem)combolecName.SelectedItem).Value;


            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select lecNo from testLec where lecName = '" + lecname + "'", con);
            con.Open();

            SQLiteDataReader sdr;
            sdr = cmd.ExecuteReader();
            sdr.Read();
            string lecid = sdr.GetValue(0).ToString();
            sdr.Close();
            con.Close();

            using var cmd2 = new SQLiteCommand("SELECT DISTINCT(testSessions.sessionID), sessionAssign.slotNo,fullSlot.Day," +
                "fullSlot.time,testSessions.groupName,testSessions.roomName FROM testSessions INNER JOIN sessionAssign ON testSessions.sessionId = sessionAssign.sessID " +
                "LEFT JOIN fullSlot ON sessionAssign.slotNo = fullSlot.fullSlotID WHERE testSessions.lecId = '" + lecid + "'", con);



            con.Open();
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd2);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            simpleGrid.DataSource = dt;
            con.Close();


        }

        

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void CreateSession_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label16.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                comboBox6.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                comboBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                comboBox3.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                comboBox5.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                comboBox4.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select the corner");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string txtQuery = "UPDATE parallel_sessions SET subgroup_id='" + comboBox6.Text + "', subject_code='" + comboBox2.Text + "', lecturer_id='" + comboBox3.Text + "', time_slot='" + comboBox5.Text + "', session_id='" + comboBox4.Text + "' WHERE id ='" + label6.Text.ToString() + "'";
                ExecuteQuery(txtQuery);
                LoadParallelSessions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string txtQuery = "DELETE FROM parallel_sessions WHERE id='" + label6.Text.ToString() + "'";
                ExecuteQuery(txtQuery);
                LoadParallelSessions();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void comboGroup_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string groupname = comboGroup.SelectedItem.ToString();
            //string groupID = ((System.Web.UI.WebControls.ListItem)comboGroup.SelectedItem).Value;

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("select groupId from testSessions where groupName = '" + groupname + "'", con);
            con.Open();

            SQLiteDataReader sdr;
            sdr = cmd.ExecuteReader();
            sdr.Read();
            string groupID = sdr.GetValue(0).ToString();
            sdr.Close();
            con.Close();

            using var cmd2 = new SQLiteCommand("SELECT DISTINCT(testSessions.sessionID), sessionAssign.slotNo,fullSlot.Day," +
                "fullSlot.time,testLec.lecName,testSessions.roomName FROM testSessions INNER JOIN sessionAssign ON testSessions.sessionId = sessionAssign.sessID " +
                "LEFT JOIN fullSlot ON sessionAssign.slotNo = fullSlot.fullSlotID LEFT JOIN testLec ON testLec.lecNo = testSessions.lecID WHERE testSessions.groupId = '" + groupID + "'", con);



            con.Open();
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd2);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            simpleGrid.DataSource = dt;
            con.Close();


        }

        private void comboRoom_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string roomname = comboRoom.SelectedItem.ToString();
            //string groupID = ((System.Web.UI.WebControls.ListItem)comboGroup.SelectedItem).Value;

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);

            using var cmd = new SQLiteCommand("SELECT DISTINCT(testSessions.sessionID), sessionAssign.slotNo,fullSlot.Day," +
                "fullSlot.time,testLec.lecName,testSessions.groupName FROM testSessions INNER JOIN sessionAssign ON testSessions.sessionId = sessionAssign.sessID " +
                "LEFT JOIN fullSlot ON sessionAssign.slotNo = fullSlot.fullSlotID LEFT JOIN testLec ON testLec.lecNo = testSessions.lecID WHERE testSessions.roomName = '" + roomname + "'", con);



            con.Open();
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            simpleGrid.DataSource = dt;
            con.Close();
        }
    }
}
