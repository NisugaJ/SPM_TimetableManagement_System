using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using Timetable_Management_System.src;
using Timetable_Management_System.src.Models;
using System.Runtime.CompilerServices;

namespace Timetable_Management_System
{
    public partial class ManageSubjectsDashboard : Form
    {
        String[] tagsList;
        List<SubjectEditStatus> editTagsStatusList;

        Subject gblSearchFoundObj_Search;
        List<Subject_Tags> gblSubjectTagslist_Search;
        List<string> gblTagNames_Remove;
        List<string> gblTagNames_Search;
        List<string> gblTagNames_label_Edit;
        List<string> gblTagNames_text_Edit;
        List<string> gblTagNames_button_Edit;

        Dictionary<string, bool> closeButtonClickStatus_Add = new Dictionary<string, bool>();
        Dictionary<string, bool> closeButtonClickStatus_Edit = new Dictionary<string, bool>();
        List<string> loadedTagsList;
        List<string> gblSearchSubjectsAvailableTagsHrsNamesList;

        List<Subject_Tags> gblSubjectTagsListForUpdateFind;

        public ManageSubjectsDashboard()
        {
            InitializeComponent();
            tabControl1.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);
            this.WindowState = FormWindowState.Maximized;
        }

        private void ManageSubjectsDashboard_Load(object sender, EventArgs e)
        {
            //Add
            loadedTagsList = loadTags();

            tagsList = loadedTagsList.ToArray();

            fillCloseButtonClickStatus_Add();

            drawTagsInSubject_Add(loadedTagsList);
            chkParallelSubject_Add.Checked = true;
            chkParallelSubject_Add.Checked = false;
            loadYearAndSemester_Add();
            fillProgramsInAddSubject();

            //Edit
            gblSubjectTagsListForUpdateFind = new List<Subject_Tags>();
            radSubjectCode_Edit.Checked = true;

            editTagsStatusList = new List<SubjectEditStatus>();

            gblTagNames_label_Edit = new List<string>();
            gblTagNames_text_Edit = new List<string>();
            gblTagNames_button_Edit = new List<string>();

            //Search
            setInitialsInFilterSections_Search();
            radSubjectCode_Search.Checked = true;
            lblTagsL_Search.Visible = false;
            lblTagsHoursL_Search.Visible = false;
            gblTagNames_Search = new List<string>();

            gblSearchSubjectsAvailableTagsHrsNamesList = new List<string>();
            hideSearchDataSummaryAtLoading();

            //Remove
            radSubjectCode_Remove.Checked = true;
            hideDataInRemove();
            gblTagNames_Remove = new List<string>();
        }

        private void fillCloseButtonClickStatus_Add()
        {
            closeButtonClickStatus_Add.Clear();
            for (int i = 0; i < tagsList.Length; i++)
            {
                closeButtonClickStatus_Add[tagsList[i]] = true;
            }
        }

        private void fillCloseButtonClickStatus_Edit()
        {
            closeButtonClickStatus_Edit.Clear();
            foreach (Subject_Tags item in gblSubjectTagsListForUpdateFind)
            {
                closeButtonClickStatus_Edit[item.tag] = true;
            }
        }

        private void loadYearAndSemester_Add()
        {
            cmbOfferedYear_Add.Items.Clear();
            cmbSemester_Add.Items.Clear();

            List<int> yearsList = new List<int>();
            List<int> semestersList = new List<int>();

            //Todo
            yearsList.Add(1);
            yearsList.Add(2);
            yearsList.Add(3);
            yearsList.Add(4);

            //Todo
            semestersList.Add(1);
            semestersList.Add(2);

            for (int i = 0; i < yearsList.Count; i++)
            {
                cmbOfferedYear_Add.Items.Add(yearsList[i]);
            }
            cmbOfferedYear_Add.SelectedIndex = 0;

            for (int i = 0; i < semestersList.Count; i++)
            {
                cmbSemester_Add.Items.Add(semestersList[i]);
            }
            cmbSemester_Add.SelectedIndex = 0;
        }

        private void drawTagsInSubject_Add(List<string> loadedTagsList)
        {
            int initialLocation = 330;
            for (int i = 0; i < tagsList.Length; i++)
            {
                Label label = new Label();
                label.Location = new System.Drawing.Point(410, initialLocation);
                label.Size = new System.Drawing.Size(80, 20);
                label.Name = "lbl" + tagsList[i] + "_Add";
                label.Text = tagsList[i] + "";
                AddSubject.Controls.Add(label);
                initialLocation = initialLocation + 30;
            }

            initialLocation = 330;

            for (int i = 0; i < tagsList.Length; i++)
            {
                TextBox textbox = new TextBox();
                textbox.Location = new System.Drawing.Point(500, initialLocation);
                textbox.Size = new System.Drawing.Size(80, 20);
                textbox.Name = "txt" + tagsList[i] + "Hrs_Add";
                textbox.Text = "";
                AddSubject.Controls.Add(textbox);
                initialLocation = initialLocation + 30;
            }

            initialLocation = 330;

            for (int i = 0; i < tagsList.Length; i++)
            {
                Label lbl = new Label();
                lbl.Location = new System.Drawing.Point(600, initialLocation);
                lbl.Size = new System.Drawing.Size(50, 20);
                lbl.Name = "lbl" + tagsList[i] + "Hours_Add";
                lbl.Text = "hours";
                AddSubject.Controls.Add(lbl);
                initialLocation = initialLocation + 30;
            }

            initialLocation = 330;

            for (int i = 0; i < tagsList.Length; i++)
            {
                Button closeBtn = new Button();
                closeBtn.Location = new System.Drawing.Point(720, initialLocation);
                closeBtn.Size = new System.Drawing.Size(20, 20);
                closeBtn.Name = "btn" + tagsList[i] + "Close_Add";
                //closeBtn.Click += new EventHandler(CloseButtonClick_Add(tagsList[i]));
                string temp = tagsList[i] + "";
                closeBtn.Click += (se, ev) => button_Click(se, ev, temp);
                closeBtn.Text = "X";
                AddSubject.Controls.Add(closeBtn);
                initialLocation = initialLocation + 30;
            }
        }

        // String[] closedTags_Add;
        HashSet<string> closedTags_Add = new HashSet<string>();


        private void button_Click(object se, EventArgs ev, string needCloseType)
        {
            //check  if clicked item is already available
            //then remove

            foreach (var item in closeButtonClickStatus_Add)
            {

                if (item.Key.Equals(needCloseType))
                {
                    closeButtonClickStatus_Add[needCloseType] = !(item.Value);
                    break;
                }
            }


            foreach (var item in closeButtonClickStatus_Add)
            {
                //item.Key
                if (item.Value == false)
                {
                    string btnName = "btn" + item.Key + "Close_Add";
                    string txtName = "txt" + item.Key + "Hrs_Add";

                    Button btn = null;
                    TextBox txt = null;

                    if (AddSubject.Controls.ContainsKey(btnName))
                    {
                        btn = AddSubject.Controls[btnName] as Button;
                        txt = AddSubject.Controls[txtName] as TextBox;
                        //     btn.Enabled = false;
                        txt.Enabled = false;
                    }
                }
                else if (item.Value == true)
                {
                    string btnName = "btn" + item.Key + "Close_Add";
                    string txtName = "txt" + item.Key + "Hrs_Add";

                    Button btn = null;
                    TextBox txt = null;

                    if (AddSubject.Controls.ContainsKey(btnName))
                    {
                        btn = AddSubject.Controls[btnName] as Button;
                        txt = AddSubject.Controls[txtName] as TextBox;
                        //     btn.Enabled = false;
                        txt.Enabled = true;
                    }
                }
            }
        }


        private List<string> loadTags()
        {
            //Load tags from tags table
            //To Do

            List<string> tempTagsList = new List<string>();
            tempTagsList.Add("Lecture");
            tempTagsList.Add("Tutorial");
            tempTagsList.Add("Lab");
            tempTagsList.Add("Evaluation");

            return tempTagsList;
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



        private void btnAddSubject_Add_Click(object sender, EventArgs e)
        {
            int temp = 1;
            if (this.cmbOfferedYear_Add.GetItemText(this.cmbOfferedYear_Add.SelectedItem) == "" ||
                !(Int32.TryParse(this.cmbOfferedYear_Add.GetItemText(this.cmbOfferedYear_Add.SelectedItem), out temp))
                )
            {
                MessageBox.Show("Please provide year or check format");
            }
            else if(this.cmbSemester_Add.GetItemText(this.cmbSemester_Add.SelectedItem) == "" ||
                !(Int32.TryParse(this.cmbSemester_Add.GetItemText(this.cmbSemester_Add.SelectedItem), out temp))
                )
            {
                MessageBox.Show("Please provide semester or check format");
            }else if (txtSubjectName_Add.Text == "")
            {
                MessageBox.Show("Please enter subject name");
            }
            else if (txtSubjectCode_Add.Text == "")
            {
                MessageBox.Show("Please enter subject code");
            }
            else if (this.cmbProgram_Add.GetItemText(this.cmbProgram_Add.SelectedItem) == "")
            {
                MessageBox.Show("Please select a program");
            }
            else
            {
                //Correct
                Subject subjectObj = new Subject();

                List<Subject_Tags> subjectTagsListObj = new List<Subject_Tags>();

                int selectedYear_Add = 1;
                bool yearSuccess_Add = Int32.TryParse(this.cmbOfferedYear_Add.GetItemText(this.cmbOfferedYear_Add.SelectedItem), out selectedYear_Add);

                int selectedSemester_Add = 1;
                bool semesterSuccess_Add = Int32.TryParse(this.cmbSemester_Add.GetItemText(this.cmbSemester_Add.SelectedItem), out selectedSemester_Add);

                subjectObj.offeredYear = selectedYear_Add;
                subjectObj.offeredSemester = selectedSemester_Add;
                subjectObj.subjectCode = txtSubjectCode_Add.Text;
                subjectObj.subjectName = txtSubjectName_Add.Text;
                subjectObj.program = this.cmbProgram_Add.GetItemText(this.cmbProgram_Add.SelectedItem);

                if (chkParallelSubject_Add.Checked == true)
                {
                    subjectObj.isParallel = true;
                    subjectObj.category = txtCategory_Add.Text;
                }
                else if (chkParallelSubject_Add.Checked == false)
                {
                    subjectObj.isParallel = false;
                    subjectObj.category = "N/A";
                }

                bool errorStatus = false;

                foreach (var item in closeButtonClickStatus_Add)
                {
                    if (item.Value == true)
                    {
                        Subject_Tags subject_TagsObj = new Subject_Tags();
                        string txtName = "txt" + item.Key + "Hrs_Add";
                        TextBox txt = null;
                        txt = AddSubject.Controls[txtName] as TextBox;
                        //txt.Text
                        //item.Key
                        try
                        {
                            subject_TagsObj.subjectCode = txtSubjectCode_Add.Text;
                            subject_TagsObj.tag = item.Key;
                            subject_TagsObj.hrs = double.Parse(txt.Text, System.Globalization.CultureInfo.InvariantCulture);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            MessageBox.Show("Please fill all data or check validity");
                            errorStatus = true;
                        }
                        if (errorStatus == false)
                        {
                            subjectTagsListObj.Add(subject_TagsObj);
                        }

                    }
                }
                if (errorStatus == false)
                {
                    AddsubjectToDB(subjectObj, subjectTagsListObj);
                }
            }


        }

        private void AddsubjectToDB(Subject subjectObj, List<Subject_Tags> subjectTagsListObj)
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            //Adding subject
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects (
                                    subjectCode STRING PRIMARY KEY,
	                                subjectName TEXT,
	                                offeredYear INTEGER,
	                                offeredSemester INTEGER,
                                    program TEXT,
	                                isParallel BOOLEAN,
	                                category TEXT
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO subjects VALUES" +
                "(@subjectCode, @subjectName, @offeredYear, @offeredSemester, @program, @isParallel, @category)";

            cmd.Parameters.AddWithValue("@subjectCode", subjectObj.subjectCode.Trim());
            cmd.Parameters.AddWithValue("@subjectName", subjectObj.subjectName.Trim());
            cmd.Parameters.AddWithValue("@offeredYear", subjectObj.offeredYear);
            cmd.Parameters.AddWithValue("@offeredSemester", subjectObj.offeredSemester);
            cmd.Parameters.AddWithValue("@program", subjectObj.program);
            cmd.Parameters.AddWithValue("@isParallel", subjectObj.isParallel);
            cmd.Parameters.AddWithValue("@category", subjectObj.category.Trim());

            cmd.Prepare();

            cmd.ExecuteNonQuery();

            //Adding subject_tag mapping
            foreach (Subject_Tags subject_TagsObj in subjectTagsListObj) // Loop through List with foreach
            {
                using var cmd1 = new SQLiteCommand(con);

                cmd1.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects_tags (
           
                                        subjectCode STRING ,
	                                    tag STRING,
	                                    hrs DOUBLE,

	                                    PRIMARY KEY (subjectCode ,tag )
                )";
                cmd1.ExecuteNonQuery();

                cmd1.CommandText = "INSERT INTO subjects_tags VALUES" +
                    "(@subjectCodeM, @tag, @hrs)";

                cmd1.Parameters.AddWithValue("@subjectCodeM", subject_TagsObj.subjectCode);
                cmd1.Parameters.AddWithValue("@tag", subject_TagsObj.tag);
                cmd1.Parameters.AddWithValue("@hrs", subject_TagsObj.hrs);

                cmd1.Prepare();

                cmd1.ExecuteNonQuery();
            }
            con.Close();

            MessageBox.Show("Subject Added", "Success");
            resetSubject_Add();
        }

        private void resetSubject_Add()
        {
            ManageSubjectsDashboard manageSubjectsDashboardObj = new ManageSubjectsDashboard();
            manageSubjectsDashboardObj.Show();
            this.Hide();
        }

        private void chkParallelSubject_Add_CheckedChanged(object sender, EventArgs e)
        {
            if (chkParallelSubject_Add.Checked == true)
            {
                categorySetVisible();
            }
            else if (chkParallelSubject_Add.Checked == false)
            {
                categorySetInvisible();
            }

        }

        private void categorySetInvisible()
        {
            lblCategory_Add.Visible = false;
            txtCategory_Add.Visible = false;
        }

        private void categorySetVisible()
        {
            lblCategory_Add.Visible = true;
            txtCategory_Add.Visible = true;

        }

        private void btnReset_Add_Click(object sender, EventArgs e)
        {
            ManageSubjectsDashboard manageSubjectsDashboardObj = new ManageSubjectsDashboard();
            manageSubjectsDashboardObj.Show();
            this.Hide();
        }

        private void btnFindSubject_Search_Click(object sender, EventArgs e)
        {
            string searchType = "";
            if (radSubjectCode_Search.Checked == true)
            {
                searchType = "byCode";
            }
            if (radSubjectName_Search.Checked == true)
            {
                searchType = "byName";
            }

            if (searchType.Equals("byCode") && txtSearchSubject_Search.Text.Equals(""))
            {
                MessageBox.Show("Please enter a subject code for search");
            }
            else if (searchType.Equals("byName") && txtSearchSubject_Search.Text.Equals(""))
            {
                MessageBox.Show("Please enter a subject name for search");
            }
            else
            {
                Subject subjectObj_Search = new Subject();
                Subject_Tags subjectTags_Search = new Subject_Tags();
                List<Subject_Tags> subjectTagslist = new List<Subject_Tags>();

                bool isfound = findSubject_Search(txtSearchSubject_Search.Text, searchType);

                if (isfound == true)
                {
                    for (int i = 0; i<2; i++)
                    {
                        setFoundSubjectData_Search();
                        setsubjectTags();
                    }
                }
                else
                {
                    MessageBox.Show("Subject not found");
                }

            }
        }

        private void setsubjectTags()
        {
            lblTagsL_Search.Visible = true;
            lblTagsHoursL_Search.Visible = true;

            gblTagNames_Search.Clear();

            gblSearchSubjectsAvailableTagsHrsNamesList.Clear();

            lblTagsL_Search.Visible = true;
            lblTagsHoursL_Search.Visible = true;

            gblSubjectTagslist_Search.ToString();

            int initialLocation = 450;

            foreach (Subject_Tags obj in gblSubjectTagslist_Search)
            {
                Label lbl = new Label();
                lbl.Location = new System.Drawing.Point(1100, initialLocation);
                lbl.Size = new System.Drawing.Size(80, 20);
                lbl.Name = "lbl" + obj.tag + "Found_Search";
                lbl.Text = obj.tag;

                gblTagNames_Search.Add(lbl.Name);

                Label lbl1 = new Label();
                lbl1.Location = new System.Drawing.Point(1200, initialLocation);
                lbl1.Size = new System.Drawing.Size(50, 20);
                lbl1.Name = "lbl" + obj.hrs + "Found_Search";
                lbl1.Text = obj.hrs.ToString();

                gblTagNames_Search.Add(lbl1.Name);

                groupBox2.SendToBack();

                ViewSearchSubjects.Controls.Add(lbl);
                ViewSearchSubjects.Controls.Add(lbl1);

                initialLocation = initialLocation + 30;

                gblSearchSubjectsAvailableTagsHrsNamesList.Add(lbl.Name);
                gblSearchSubjectsAvailableTagsHrsNamesList.Add(lbl1.Name);

            }
        }

        private void setFoundSubjectData_Search()
        {
            lblSubjectCodeAns_Search.Text = gblSearchFoundObj_Search.subjectCode;
            lblSubjectNameAns_Search.Text = gblSearchFoundObj_Search.subjectName;
            lblYearAns_Search.Text = gblSearchFoundObj_Search.offeredYear.ToString();
            lblSemesterAns_Search.Text = gblSearchFoundObj_Search.offeredSemester.ToString();
            if (gblSearchFoundObj_Search.isParallel == true)
            {
                lblIsparallel_Search.Text = "Yes";
                lblCategory_Search.Visible = true;
                lblCategoryL_Search.Visible = true;
                lblCategory_Search.Text = gblSearchFoundObj_Search.category;
            }
            else
            {
                lblIsparallel_Search.Text = "No";
                lblCategoryL_Search.Visible = false;
                lblCategory_Search.Visible = false;
            }

        }

        private bool findSubject_Search(string searchKey, string searchType)
        {

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";
            if (searchType.Equals("byCode"))
            {
                stm = "select " +
                    "subjects.subjectCode AS Subject_Code ," +
                    "subjects.subjectName AS Subject_Name ," +
                    "subjects.offeredYear AS Year ," +
                    "subjects.offeredSemester AS Semester ," +
                    "subjects.isParallel AS Parallel_Subject ," +
                    "subjects.category AS Category ," +
                    "subjects_tags.tag AS Tag ," +
                    "subjects_tags.hrs AS Hours " +

                    "from subjects, subjects_tags" +
                    " where subjects.subjectCode = subjects_tags.subjectCode AND subjects.subjectCode = '" + searchKey + "'";
            }
            else if (searchType.Equals("byName"))
            {
                stm = "select " +
                    "subjects.subjectCode AS Subject_Code ," +
                    "subjects.subjectName AS Subject_Name ," +
                    "subjects.offeredYear AS Year ," +
                    "subjects.offeredSemester AS Semester ," +
                    "subjects.isParallel AS Parallel_Subject ," +
                    "subjects.category AS Category ," +
                    "subjects_tags.tag AS Tag ," +
                    "subjects_tags.hrs AS Hours " +
                    "from subjects, subjects_tags" +
                    " where subjects.subjectCode = subjects_tags.subjectCode AND subjects.subjectName = '" + searchKey + "'";
            }

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            Subject subjectObj_Search = new Subject();

            List<Subject_Tags> subjectTagslist = new List<Subject_Tags>();

            while (rdr.Read())
            {
                Subject_Tags subjectTagsObj_Search = new Subject_Tags();

                subjectObj_Search.subjectCode = $@"{ rdr.GetString(0),-8}";
                subjectObj_Search.subjectName = $@"{ rdr.GetString(1),-8}";
                subjectObj_Search.offeredYear = Int32.Parse($@"{rdr.GetInt32(2),-3}");
                subjectObj_Search.offeredSemester = Int32.Parse($@"{rdr.GetInt32(3),-3}");

                subjectObj_Search.isParallel = rdr.GetBoolean(rdr.GetOrdinal("Parallel_Subject"));

                subjectObj_Search.category = $@"{ rdr.GetString(5),-8}";

                subjectTagsObj_Search.subjectCode = $@"{ rdr.GetString(0),-8}";
                subjectTagsObj_Search.tag = $@"{ rdr.GetString(6),-8}";
                subjectTagsObj_Search.hrs = rdr.GetDouble(rdr.GetOrdinal("Hours"));

                subjectTagslist.Add(subjectTagsObj_Search);

            }

            gblSearchFoundObj_Search = subjectObj_Search;
            gblSubjectTagslist_Search = subjectTagslist;

            if (subjectObj_Search.subjectCode == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnRefreshGridView_Search_Click(object sender, EventArgs e)
        {
            refreshSubjectGrid();
        }

        private void refreshSubjectGrid()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);

            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(
                "select " +
                "subjects.subjectCode AS Subject_Code ," +
                "subjects.subjectName AS Subject_Name ," +
                "subjects.offeredYear AS Year ," +
                "subjects.offeredSemester AS Semester ," +
                "subjects.program AS Program ," +
                "subjects.isParallel AS Parallel_Subject ," +
                "subjects.category AS Category ," +
                "subjects_tags.tag AS Tag ," +
                "subjects_tags.hrs AS Hours " +


                "from subjects, subjects_tags " +
                "where subjects.subjectCode = subjects_tags.subjectCode"
                );

            cmd.Connection = conn;

            conn.Open();

            using var cmdCreateSubject = new SQLiteCommand(conn);

            cmdCreateSubject.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects (
                                    subjectCode STRING PRIMARY KEY,
	                                subjectName TEXT,
	                                offeredYear INTEGER,
	                                offeredSemester INTEGER,
                                    program TEXT,
	                                isParallel BOOLEAN,
	                                category TEXT   
                )";
            cmdCreateSubject.ExecuteNonQuery();

            using var cmdCreateSubjectTag = new SQLiteCommand(conn);

            cmdCreateSubjectTag.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects_tags (
                                        subjectCode STRING ,
	                                    tag STRING,
	                                    hrs DOUBLE,

	                                    PRIMARY KEY (subjectCode ,tag ) 
                )";
            cmdCreateSubjectTag.ExecuteNonQuery();

            cmd.ExecuteScalar();


            System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSubject_Search.DataSource = dt;
            conn.Close();
        }

        private void setInitialsInFilterSections_Search()
        {
            txtSubjectCode_Search.Text = "Any";
            txtSubjectCode_Search.GotFocus += txtSubjectCode_Search_GotFocus;
            txtSubjectCode_Search.LostFocus += txtSubjectCode_Search_LostFocus;

            txtSubjectName_Search.Text = "Any";
            txtSubjectName_Search.GotFocus += txtSubjectName_Search_GotFocus;
            txtSubjectName_Search.LostFocus += txtSubjectName_Search_LostFocus;

            setYears_Search();
            setSemesters_Search();
            setTags_Search();

            txtTo_Search.Text = "∞";
            txtTo_Search.GotFocus += txtTo_Search_GotFocus;
            txtTo_Search.LostFocus += txtTo_Search_LostFocus;

            txtFrom_Search.Text = "0";
            txtFrom_Search.GotFocus += txtFrom_Search_GotFocus;
            txtFrom_Search.LostFocus += txtFrom_Search_LostFocus;

        }

        private void txtFrom_Search_LostFocus(object sender, EventArgs e)
        {
            if (txtFrom_Search.Text == "")
                txtFrom_Search.Text = "0";
        }

        private void txtFrom_Search_GotFocus(object sender, EventArgs e)
        {
            if (txtFrom_Search.Text.Equals("0"))
            {
                txtFrom_Search.Text = "";
            }
        }

        private void txtTo_Search_LostFocus(object sender, EventArgs e)
        {
            if (txtTo_Search.Text == "")
                txtTo_Search.Text = "∞";
        }

        private void txtTo_Search_GotFocus(object sender, EventArgs e)
        {
            if (txtTo_Search.Text.Equals("∞"))
            {
                txtTo_Search.Text = "";
            }
        }

        private void setSemesters_Search()
        {
            List<int> semestersList = new List<int>();

            //Todo
            semestersList.Add(1);
            semestersList.Add(2);

            cmbSemester_Search.Items.Clear();

            cmbSemester_Search.Items.Add("Any");
            for (int i = 0; i < semestersList.Count; i++)
            {
                cmbSemester_Search.Items.Add(semestersList[i]);
            }
            cmbSemester_Search.SelectedIndex = 0;
        }

        private void setYears_Search()
        {
            List<int> yearsList = new List<int>();

            //Todo
            yearsList.Add(1);
            yearsList.Add(2);
            yearsList.Add(3);
            yearsList.Add(4);

            cmbYear_Search.Items.Clear();

            cmbYear_Search.Items.Add("Any");
            for (int i = 0; i < yearsList.Count; i++)
            {
                cmbYear_Search.Items.Add(yearsList[i]);
            }
            cmbYear_Search.SelectedIndex = 0;
        }

        private void setTags_Search()
        {
            cmbTag_Search.Items.Clear();

            cmbTag_Search.Items.Add("Any");
            for (int i = 0; i < tagsList.Length; i++)
            {
                cmbTag_Search.Items.Add(tagsList[i]);
            }
            cmbTag_Search.SelectedIndex = 0;

        }

        private void txtSubjectName_Search_LostFocus(object sender, EventArgs e)
        {
            if (txtSubjectName_Search.Text == "")
                txtSubjectName_Search.Text = "Any";
        }

        private void txtSubjectName_Search_GotFocus(object sender, EventArgs e)
        {
            if (txtSubjectName_Search.Text.Equals("Any"))
            {
                txtSubjectName_Search.Text = "";
            }
        }

        private void txtSubjectCode_Search_LostFocus(object sender, EventArgs e)
        {
            if (txtSubjectCode_Search.Text == "")
                txtSubjectCode_Search.Text = "Any";
        }

        private void txtSubjectCode_Search_GotFocus(object sender, EventArgs e)
        {
            if (txtSubjectCode_Search.Text.Equals("Any"))
            {
                txtSubjectCode_Search.Text = "";
            }

        }

        private void btnFilter_Search_Click(object sender, EventArgs e)
        {
            string subjectCode = txtSubjectCode_Search.Text;
            string subjectName = txtSubjectName_Search.Text;
            string year = this.cmbYear_Search.GetItemText(this.cmbYear_Search.SelectedItem);
            string semester = this.cmbSemester_Search.GetItemText(this.cmbSemester_Search.SelectedItem);
            string tag = this.cmbTag_Search.GetItemText(this.cmbTag_Search.SelectedItem);
            bool parallel = chkParallel_Search.Checked;
            string from = txtFrom_Search.Text;
            string to = txtTo_Search.Text;

            string query = generateFilterString(subjectCode, subjectName, year, semester, tag, parallel, from, to);
            // MessageBox.Show(query);
            filterSubjects_Search(query);

        }

        private string generateFilterString(string subjectCode, string subjectName, string year, string semester, string tag, bool parallel, string from, string to)
        {

            string q = "select " +
              "subjects.subjectCode AS Subject_Code ," +
              "subjects.subjectName AS Subject_Name ," +
              "subjects.offeredYear AS Year ," +
              "subjects.offeredSemester AS Semester ," +
              "subjects.isParallel AS Parallel_Subject ," +
              "subjects.category AS Category ," +
              "subjects_tags.tag AS Tag ," +
              "subjects_tags.hrs AS Hours " +

              "from subjects, subjects_tags " +
              "where subjects.subjectCode = subjects_tags.subjectCode ";

            if (!subjectCode.Equals("Any"))
            {
                string temp = " AND subjects.subjectCode = '" + subjectCode + "'";
                q = q + temp;
            }

            if (!subjectName.Equals("Any"))
            {
                string temp = " AND subjects.subjectName = '" + subjectName + "'";
                q = q + temp;
            }

            if (!year.Equals("Any"))
            {
                string temp = " AND subjects.offeredYear = " + year;
                q = q + temp;
            }

            if (!semester.Equals("Any"))
            {
                string temp = " AND subjects.offeredSemester = " + semester;
                q = q + temp;
            }

            if (!tag.Equals("Any"))
            {
                string temp = " AND subjects_tags.tag = '" + tag + "' ";
                q = q + temp;
            }
            if (parallel == true)
            {
                string temp = " AND subjects.isParallel = " + true;
                q = q + temp;
            }
            else if (parallel == false)
            {
                string temp = " AND subjects.isParallel = " + false;
                q = q + temp;
            }

            int fromNumber_Search;
            int toNumber_Search;
            bool successFrom_Search = Int32.TryParse(from, out fromNumber_Search);
            bool successTo_Search = Int32.TryParse(to, out toNumber_Search);

            if (to.Equals("∞"))
            {

                string temp = " AND subjects_tags.hrs >= " + fromNumber_Search;
                q = q + temp;
            }
            else
            {
                string temp = " AND subjects_tags.hrs >= " + fromNumber_Search + " AND subjects_tags.hrs <= " + toNumber_Search;
                q = q + temp;
            }

            return q;
        }

        private void filterSubjects_Search(string query)
        {

            Console.WriteLine(query);


            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);

            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(query);

            cmd.Connection = conn;

            conn.Open();

            System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSubject_Search.DataSource = dt;
            conn.Close();

        }

        private void btnReset_Search_Click(object sender, EventArgs e)
        {
            resetSearchFound();
            this.Hide();
            ManageSubjectsDashboard obj = new ManageSubjectsDashboard();
            obj.Show();
            obj.tabControl1.SelectedIndex = 3;
        }

        private void resetSearchFound()
        {

            lblSubjectCodeAns_Search.Text = "";
            lblSubjectNameAns_Search.Text = "";
            lblYearAns_Search.Text = "";
            lblSemesterAns_Search.Text = "";
            lblIsparallel_Search.Text = "";
            lblCategory_Search.Text = "";
            txtSearchSubject_Search.Text = "";

            gblSearchFoundObj_Search = null;
            gblSubjectTagslist_Search = null;


            lblTagsL_Search.Visible = false;
            lblTagsHoursL_Search.Visible = false;
            //Reset list

            Console.WriteLine(gblTagNames_Search);

            foreach (var str in gblTagNames_Search)
            {
                Label subjectTagLabels = (Label)this.GetControlByName(this, str);
                subjectTagLabels.Text = "";
            }
            gblTagNames_Search.Clear();
        }

        private void hideDataInRemove()
        {
            lblAnsSubjectCode_Remove.Text = "";
            lblAnsSubjectName_Remove.Text = "";
            lblAnsYear_Remove.Text = "";
            lblAnsSemester_Remove.Text = "";
            lblAnsIsParallel_Remove.Text = "";
            lblAnsCategory_Remove.Text = "";

            lblAnsSubjectCode_Remove.Visible = false;
            lblAnsSubjectName_Remove.Visible = false;
            lblAnsYear_Remove.Visible = false;
            lblAnsSemester_Remove.Visible = false;
            lblAnsIsParallel_Remove.Visible = false;
            lblAnsCategory_Remove.Visible = false;

            lblTags_Remove.Visible = false;
            lblHours_Remove.Visible = false;
        }

        private void btnFindSubject_Remove_Click(object sender, EventArgs e)
        {
            string searchType = "";
            if (radSubjectCode_Remove.Checked == true)
            {
                searchType = "byCode";
            }
            if (radSubjectName_Remove.Checked == true)
            {
                searchType = "byName";
            }

            if (searchType.Equals("byCode") && txtKeyword_Remove.Text.Equals(""))
            {
                MessageBox.Show("Please enter a subject code for find");
            }
            else if (searchType.Equals("byName") && txtKeyword_Remove.Text.Equals(""))
            {
                MessageBox.Show("Please enter a subject name for find");
            }
            else
            {
                Subject subjectObj_Search = new Subject();
                Subject_Tags subjectTags_Search = new Subject_Tags();
                List<Subject_Tags> subjectTagslist = new List<Subject_Tags>();

                bool isfound = findSubject_Search(txtKeyword_Remove.Text, searchType);

                if (isfound == true)
                {
                    for(int i = 0; i < 2; i++)
                    {
                        setFoundSubjectData_Remove();
                        setsubjectTags_Remove();
                    }
                }
                else
                {
                    MessageBox.Show("Subject not found");
                }

            }
        }

        private void setsubjectTags_Remove()
        {
            //  gblSubjectTagslist_Search.ToString();
            gblTagNames_Remove.Clear();

            int tempCounter = 0;
            foreach (Subject_Tags obj in gblSubjectTagslist_Search)
            {
                tempCounter++;
            }

            if (tempCounter > 0)
            {
                //Has tags
                lblTags_Remove.Visible = true;
                lblHours_Remove.Visible = true;

                int initialLocation = 420;

                foreach (Subject_Tags obj in gblSubjectTagslist_Search)
                {
                    Label lbl = new Label();
                    lbl.Location = new System.Drawing.Point(500, initialLocation);
                    lbl.Size = new System.Drawing.Size(80, 20);
                    lbl.Name = "lbl" + obj.tag + "Found_Remove";
                    lbl.Name = lbl.Name.Trim();
                    lbl.Text = obj.tag;

                    gblTagNames_Remove.Add(lbl.Name);

                    Label lbl1 = new Label();
                    lbl1.Location = new System.Drawing.Point(600, initialLocation);
                    lbl1.Size = new System.Drawing.Size(40, 20);
                    lbl1.Name = "lbl" + obj.hrs + "Found_Remove";
                    lbl1.Name = lbl1.Name.Trim();
                    lbl1.Text = obj.hrs.ToString() + "";

                    gblTagNames_Remove.Add(lbl1.Name);

                    groupBox4.SendToBack();

                    RemoveSubject.Controls.Add(lbl);
                    RemoveSubject.Controls.Add(lbl1);

                    initialLocation = initialLocation + 30;

                }
                initialLocation = 420;
            }
            else
            {
                //No tags
                lblTags_Remove.Visible = false;
                lblHours_Remove.Visible = false;
            }

        }

        private void setFoundSubjectData_Remove()
        {
            lblAnsSubjectCode_Remove.Visible = true;
            lblAnsSubjectName_Remove.Visible = true;
            lblAnsYear_Remove.Visible = true;
            lblAnsSemester_Remove.Visible = true;

            lblAnsSubjectCode_Remove.Text = gblSearchFoundObj_Search.subjectCode;
            lblAnsSubjectName_Remove.Text = gblSearchFoundObj_Search.subjectName;
            lblAnsYear_Remove.Text = gblSearchFoundObj_Search.offeredYear.ToString();
            lblAnsSemester_Remove.Text = gblSearchFoundObj_Search.offeredSemester.ToString();

            if (gblSearchFoundObj_Search.isParallel == true)
            {
                lblIsCategoryL_Remove.Visible = true;
                lblAnsIsParallel_Remove.Visible = true;
                lblAnsIsParallel_Remove.Text = "Yes";
                lblAnsCategory_Remove.Visible = true;
                lblAnsCategory_Remove.Text = gblSearchFoundObj_Search.category;
            }
            else if (gblSearchFoundObj_Search.isParallel == false)
            {
                lblAnsIsParallel_Remove.Visible = true;
                lblAnsIsParallel_Remove.Text = "No";
                lblIsCategoryL_Remove.Visible = false;
                lblAnsCategory_Remove.Visible = false;
            }
        }

        private void btnReset_Remove_Click(object sender, EventArgs e)
        {
            RemoveReset();
            this.Hide();
            ManageSubjectsDashboard obj = new ManageSubjectsDashboard();
            obj.Show();
            obj.tabControl1.SelectedIndex = 2;
        }

        private void RemoveReset()
        {
            foreach (var str in gblTagNames_Remove)
            {
                Label subjectTagLabels = (Label)this.GetControlByName(this, str);
                subjectTagLabels.Text = "";
            }
            gblTagNames_Remove.Clear();

            lblTags_Remove.Visible = false;
            lblHours_Remove.Visible = false;

            lblAnsSubjectCode_Remove.Text = "";
            lblAnsSubjectName_Remove.Text = "";
            lblAnsYear_Remove.Text = "";
            lblAnsSemester_Remove.Text = "";
            lblAnsIsParallel_Remove.Text = "";
            lblAnsCategory_Remove.Text = "";
        }


        public Control GetControlByName(Control ParentCntl, string NameToSearch)
        {
            if (ParentCntl.Name == NameToSearch)
                return ParentCntl;

            foreach (Control ChildCntl in ParentCntl.Controls)
            {
                Control ResultCntl = GetControlByName(ChildCntl, NameToSearch);
                if (ResultCntl != null)
                    return ResultCntl;
            }
            return null;
        }

        private void btnRemoveSubject_Remove_Click(object sender, EventArgs e)
        {
            if (lblAnsSubjectCode_Remove.Text.Equals(""))
            {

                if (radSubjectName_Remove.Checked == true)
                {
                    MessageBox.Show("Plese enter subject name");
                }
                else if (radSubjectCode_Remove.Checked == true)
                {
                    MessageBox.Show("Plese enter a subject code");
                }

            }
            else
            {
                string type = "";
                if (radSubjectName_Remove.Checked == true)
                {
                    type = "byName";
                }
                else if (radSubjectCode_Remove.Checked == true)
                {
                    type = "byCode";

                }
                removeSubjectFunction(lblAnsSubjectCode_Remove.Text.ToString().Trim(), type);

            }
        }


        private void removeSubjectFunction(string SubjectCode, string type)
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            //Adding subject
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects (
                                    subjectCode STRING PRIMARY KEY,
	                                subjectName TEXT,
	                                offeredYear INTEGER,
	                                offeredSemester INTEGER,
                                    program TEXT,
	                                isParallel BOOLEAN,
	                                category TEXT
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS subjects_tags (
                                        subjectCode STRING ,
	                                    tag STRING,
	                                    hrs DOUBLE,

	                                    PRIMARY KEY (subjectCode ,tag )
                                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM subjects_tags WHERE subjectCode = @subjectCode_t";
            cmd.Parameters.AddWithValue("@subjectCode_t", SubjectCode);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM subjects WHERE subjectCode = @subjectCode_s";
            cmd.Parameters.AddWithValue("@subjectCode_s", SubjectCode);
            cmd.Prepare();
            cmd.ExecuteNonQuery();


            con.Close();

            MessageBox.Show("Subject Removed Successfully", "Success");
            RemoveReset();

            this.Hide();
            ManageSubjectsDashboard obj = new ManageSubjectsDashboard();
            obj.Show();
            obj.tabControl1.SelectedIndex = 2;
        }

        private void btnFindSubject_Edit_Click(object sender, EventArgs e)
        {
            if (txtFindSubject_Edit.Text.ToString().Trim().Equals(""))
            {
                if (radSubjectCode_Edit.Checked)
                {
                    MessageBox.Show("Please enter subject code");
                }
                else if (radSubjectName_Edit.Checked)
                {
                    MessageBox.Show("Please enter subject Name");
                }
            }
            else
            {
                string type = "";

                if (radSubjectCode_Edit.Checked == true)
                {
                    type = "byCode";
                }
                else if (radSubjectName_Edit.Checked == true)
                {
                    type = "byName";
                }
                Subject subObj = getSubjectDataForUpdate(txtFindSubject_Edit.Text.ToString().Trim(), type);
                if (subObj.subjectCode == null)
                {
                    MessageBox.Show("Subject Not found");
                }
                else
                {
                    // Console.WriteLine(gblSubjectTagsListForUpdateFind);
                    fillBoxes_Edit();
                    fillFoundData_Edit(subObj);
                    drawTagsData_Edit();
                    initialBlockInTag_Edit();
                }

            }
        }

        private void initialBlockInTag_Edit()
        {
            Console.WriteLine(editTagsStatusList);

            foreach (SubjectEditStatus element in editTagsStatusList)
            {
                if (element.closeClickStatus == false)
                {
                    Console.WriteLine(element);
                    TextBox txtBox_Edit = (TextBox)this.GetControlByName(this, element.txtName);
                    txtBox_Edit.Enabled = false;
                }

            }
        }

        private void drawTagsData_Edit()
        {
            editTagsStatusList.Clear();

            Console.WriteLine(gblSubjectTagsListForUpdateFind);

            gblTagNames_label_Edit.Clear();
            gblTagNames_text_Edit.Clear();
            gblTagNames_button_Edit.Clear();

            int initialLocation = 410;
            List<SubjectEditStatus> tempArr = new List<SubjectEditStatus>();

            for (int i = 0; i < tagsList.Length; i++)
            {
                SubjectEditStatus subjectEditStatusObj = new SubjectEditStatus();

                //Label
                Label label = new Label();
                label.Location = new System.Drawing.Point(430, initialLocation);
                label.Size = new System.Drawing.Size(80, 20);
                label.Name = "lbl" + tagsList[i] + "_Edit";
                label.Text = tagsList[i].Trim() + "";
                EditSubject.Controls.Add(label);
                subjectEditStatusObj.tag = tagsList[i];
                subjectEditStatusObj.lblName = label.Name;

                //Textbox
                TextBox textbox = new TextBox();
                textbox.Location = new System.Drawing.Point(520, initialLocation);
                textbox.Size = new System.Drawing.Size(80, 20);
                textbox.Name = "txt" + tagsList[i] + "Hrs_Edit";
                string temp = tagsList[i];
                textbox.TextChanged += (se, ev) => text_Change_Edit(se, ev, textbox.Text, temp);
                textbox.Leave += (se, ev) => text_Leave_Edit(se, ev, textbox.Text, textbox.Name);
                foreach (Subject_Tags element in gblSubjectTagsListForUpdateFind)
                {
                    if (element.tag.Equals(tagsList[i]))
                    {
                        textbox.Text = element.hrs.ToString();
                        subjectEditStatusObj.hrs = double.Parse(textbox.Text, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (textbox.Text.Equals(""))
                {
                    textbox.Text = "N/A";
                    subjectEditStatusObj.hrs = 0;
                }
                EditSubject.Controls.Add(textbox);
                subjectEditStatusObj.txtName = textbox.Name;


                //hrs Label
                Label lbl = new Label();
                lbl.Location = new System.Drawing.Point(620, initialLocation);
                lbl.Size = new System.Drawing.Size(50, 20);
                lbl.Name = "lbl" + tagsList[i] + "Hours_Edit";
                lbl.Text = "hours";
                EditSubject.Controls.Add(lbl);
                subjectEditStatusObj.lblhoursName = lbl.Name;

                //Close button
                Button closeBtn = new Button();
                closeBtn.Location = new System.Drawing.Point(740, initialLocation);
                closeBtn.Size = new System.Drawing.Size(20, 20);
                closeBtn.Name = "btn" + tagsList[i] + "Close_Edit";
                string temp1 = tagsList[i] + "";
                closeBtn.Click += (se, ev) => button_Click_Edit(se, ev, temp1);
                closeBtn.Text = "X";
                EditSubject.Controls.Add(closeBtn);
                subjectEditStatusObj.lblCloseBtnName = closeBtn.Name;

                int number;
                bool success = Int32.TryParse(textbox.Text.Trim(), out number); ;

                if (success == true)
                {
                    subjectEditStatusObj.closeClickStatus = true;
                }
                else if (textbox.Text.Equals("") || textbox.Text.Equals("N/A"))
                {
                    subjectEditStatusObj.closeClickStatus = false;
                }
                else
                {
                    subjectEditStatusObj.closeClickStatus = false;
                }
                initialLocation = initialLocation + 30;
                tempArr.Add(subjectEditStatusObj);
            }
            editTagsStatusList = tempArr;

        }

        private void text_Leave_Edit(object se, EventArgs ev, string text, string name)
        {
            int number;
            bool parseSuccess = Int32.TryParse(text, out number);
            if (parseSuccess == false)
            {
                MessageBox.Show("Number of hours must be desimal");
            }
            else
            {
                if (text.Equals(""))
                {
                    TextBox txtBox_Edit = (TextBox)this.GetControlByName(this, name);
                    txtBox_Edit.Enabled = false;
                }
            }

        }

        private void text_Change_Edit(object se, EventArgs ev, string inputText, string tag)
        {
            int number;
            bool parseSuccess = Int32.TryParse(inputText, out number);

            if (parseSuccess == true && (inputText.Equals("") || inputText.Equals("N") || inputText.Equals("N/") == false))
            {
                for (var i = 0; i < editTagsStatusList.Count; i++)
                {
                    if (editTagsStatusList[i].tag.Equals(tag))
                    {
                        editTagsStatusList[i].hrs = double.Parse(inputText, System.Globalization.CultureInfo.InvariantCulture);

                    }
                }
            }
            Console.WriteLine(editTagsStatusList);
        }

        private void button_Click_Edit(object se, EventArgs ev, string closeType)
        {
            Console.WriteLine(closeType);

            for (var i = 0; i < editTagsStatusList.Count; i++)
            {
                TextBox txtBox_Edit = (TextBox)this.GetControlByName(this, editTagsStatusList[i].txtName);

                if (editTagsStatusList[i].tag.Equals(closeType))
                {
                    editTagsStatusList[i].closeClickStatus = !editTagsStatusList[i].closeClickStatus;
                }
            }
            updateEditTagBlockingInView();

        }

        private void updateEditTagBlockingInView()
        {
            Console.WriteLine(editTagsStatusList);
            foreach (SubjectEditStatus element in editTagsStatusList)
            {
                TextBox txtBox_Edit = (TextBox)this.GetControlByName(this, element.txtName);

                if (element.closeClickStatus == true)
                {
                    txtBox_Edit.Enabled = true;
                }
                else if (element.closeClickStatus == false)
                {
                    txtBox_Edit.Enabled = false;
                }
            }
        }

        private void fillFoundData_Edit(Subject subObj)
        {
            txtSubjectName_Edit.Text = subObj.subjectName;
            txtSubejctCode_Edit.Text = subObj.subjectCode;

            cmbOfferedYear_Edit.Text = subObj.offeredYear.ToString();
            cmbOfferedSemester_Edit.Text = subObj.offeredSemester.ToString();
            fillcmbProgram_Edit();

            cmbProgram_Edit.SelectedIndex = cmbProgram_Edit.FindString(subObj.program.Trim());

            chkIsParallel_Edit.Checked = subObj.isParallel;

            if (subObj.isParallel == false)
            {
                txtCategory_Edit.Visible = false;
                lblCategory_Edit.Visible = false;
            }
            else
            {
                txtCategory_Edit.Visible = true;
                lblCategory_Edit.Visible = true;
            }

            txtCategory_Edit.Text = subObj.category;
        }

        private void fillcmbProgram_Edit()
        {
            cmbProgram_Edit.Items.Clear();

            cmbProgram_Edit.Items.Add("SE");
            cmbProgram_Edit.Items.Add("IT");
            cmbProgram_Edit.Items.Add("CS");
            cmbProgram_Edit.Items.Add("IM");
            cmbProgram_Edit.Items.Add("DS");
        }

        private void fillBoxes_Edit()
        {
            cmbOfferedYear_Edit.Items.Add("1");
            cmbOfferedYear_Edit.Items.Add("2");
            cmbOfferedYear_Edit.Items.Add("3");
            cmbOfferedYear_Edit.Items.Add("4");

            cmbOfferedSemester_Edit.Items.Add("1");
            cmbOfferedSemester_Edit.Items.Add("2");
        }

        private Subject getSubjectDataForUpdate(string subjectSearchKeyForUpdate, string type)
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";
            if (type.Equals("byCode"))
            {
                stm = "select " +
                    "subjects.subjectCode AS Subject_Code ," +
                    "subjects.subjectName AS Subject_Name ," +
                    "subjects.offeredYear AS Year ," +
                    "subjects.offeredSemester AS Semester ," +
                    "subjects.program AS Program ," +
                    "subjects.isParallel AS Parallel_Subject ," +
                    "subjects.category AS Category ," +
                    "subjects_tags.tag AS Tag ," +
                    "subjects_tags.hrs AS Hours " +
                    "from subjects, subjects_tags" +
                    " where subjects.subjectCode = subjects_tags.subjectCode AND subjects.subjectCode = '" + subjectSearchKeyForUpdate + "'";
            }
            else if (type.Equals("byName"))
            {
                stm = "select " +
                    "subjects.subjectCode AS Subject_Code ," +
                    "subjects.subjectName AS Subject_Name ," +
                    "subjects.offeredYear AS Year ," +
                    "subjects.offeredSemester AS Semester ," +
                    "subjects.program AS Program ," +
                    "subjects.isParallel AS Parallel_Subject ," +
                    "subjects.category AS Category ," +
                    "subjects_tags.tag AS Tag ," +
                    "subjects_tags.hrs AS Hours " +
                    "from subjects, subjects_tags" +
                    " where subjects.subjectCode = subjects_tags.subjectCode AND subjects.subjectName = '" + subjectSearchKeyForUpdate + "'";
            }

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            Subject subjectObj_Search = new Subject();

            List<Subject_Tags> subjectTagslist = new List<Subject_Tags>();
            gblSubjectTagsListForUpdateFind.Clear();
            while (rdr.Read())
            {
                Subject_Tags subjectTagsObj_Search = new Subject_Tags();

                subjectObj_Search.subjectCode = $@"{ rdr.GetString(0),-8}";
                subjectObj_Search.subjectName = $@"{ rdr.GetString(1),-8}";
                subjectObj_Search.offeredYear = Int32.Parse($@"{rdr.GetInt32(2),-3}");
                subjectObj_Search.offeredSemester = Int32.Parse($@"{rdr.GetInt32(3),-3}");
                subjectObj_Search.program = $@"{ rdr.GetString(4),-8}";


                subjectObj_Search.isParallel = rdr.GetBoolean(rdr.GetOrdinal("Parallel_Subject"));
                subjectObj_Search.category = $@"{ rdr.GetString(6),-8}";
              
                subjectTagsObj_Search.subjectCode = $@"{ rdr.GetString(0),-8}";
                subjectTagsObj_Search.tag = $@"{ rdr.GetString(7),-8}";
                subjectTagsObj_Search.hrs = rdr.GetDouble(rdr.GetOrdinal("Hours"));



                subjectObj_Search.subjectCode = subjectObj_Search.subjectCode.Trim();
                subjectObj_Search.subjectName = subjectObj_Search.subjectName.Trim();
                subjectObj_Search.category = subjectObj_Search.category.Trim();
                subjectTagsObj_Search.subjectCode = subjectTagsObj_Search.subjectCode.Trim();
                subjectTagsObj_Search.tag = subjectTagsObj_Search.tag.Trim();

                subjectTagslist.Add(subjectTagsObj_Search);

            }

            gblSubjectTagsListForUpdateFind = subjectTagslist;

            return subjectObj_Search;
        }

        private void chkIsParallel_Edit_CheckedChanged(object sender, EventArgs e)
        {
            lblCategory_Edit.Visible = !lblCategory_Edit.Visible;
            txtCategory_Edit.Visible = !txtCategory_Edit.Visible;
        }

        private void hideSearchDataSummaryAtLoading()
        {
            lblSubjectCodeAns_Search.Text = "";
            lblSubjectNameAns_Search.Text = "";
            lblYearAns_Search.Text = "";
            lblSemesterAns_Search.Text = "";
            lblIsparallel_Search.Text = "";
            lblCategory_Search.Text = "";
        }

        private void btnResetSubject_Edit_Click(object sender, EventArgs e)
        {
            Console.WriteLine(gblTagNames_label_Edit);
            Console.WriteLine(gblTagNames_text_Edit);
            Console.WriteLine(gblTagNames_button_Edit);

            foreach (var str in gblTagNames_label_Edit)
            {
                Label subjectTagLabels = (Label)this.GetControlByName(this, str);
                subjectTagLabels.Text = "";
            }
            gblTagNames_label_Edit.Clear();


            foreach (var str in gblTagNames_text_Edit)
            {
                TextBox subjectText = (TextBox)this.GetControlByName(this, str);
                subjectText.Visible = false;
            }
            gblTagNames_text_Edit.Clear();

            foreach (var str in gblTagNames_button_Edit)
            {
                Button subjectText = (Button)this.GetControlByName(this, str);
                subjectText.Visible = false;
            }
            gblTagNames_button_Edit.Clear();

            cmbOfferedYear_Edit.Items.Clear();
            cmbOfferedSemester_Edit.Items.Clear();
            txtSubjectName_Edit.Text = "";
            txtSubejctCode_Edit.Text = "";
            chkIsParallel_Edit.Checked = false;

            txtFindSubject_Edit.Text = "";

            this.Hide();
            ManageSubjectsDashboard tempobj = new ManageSubjectsDashboard();
            tempobj.Show();
            tempobj.tabControl1.SelectedIndex = 1;

        }

        private void btnEditSubject_Edit_Click(object sender, EventArgs e)
        {
            if (txtSubejctCode_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select a subject first");
            }
            else
            {
                Subject obj = new Subject();

                int offeredYearNumber;
                bool success1 = Int32.TryParse(this.cmbOfferedYear_Edit.GetItemText(this.cmbOfferedYear_Edit.SelectedItem), out offeredYearNumber);
                obj.offeredYear = offeredYearNumber;

                int offeredSemesterNumber;
                bool success2 = Int32.TryParse(this.cmbOfferedSemester_Edit.GetItemText(this.cmbOfferedSemester_Edit.SelectedItem), out offeredSemesterNumber);
                obj.offeredSemester = offeredSemesterNumber;

                obj.subjectName = txtSubjectName_Edit.Text;
                obj.subjectCode = txtSubejctCode_Edit.Text;
                obj.isParallel = chkIsParallel_Edit.Checked;
                obj.category = txtCategory_Edit.Text;
                obj.program = this.cmbProgram_Edit.GetItemText(this.cmbProgram_Edit.SelectedItem);

                Console.WriteLine(editTagsStatusList);

                Updatesubject(obj);
                obj = null;
            }

            this.Hide();
            ManageSubjectsDashboard tempobj = new ManageSubjectsDashboard();
            tempobj.Show();
            tempobj.tabControl1.SelectedIndex = 1;
        }

        private void Updatesubject(Subject subjectObj)
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"UPDATE subjects SET subjectName= '" + subjectObj.subjectName + "' , " +
                "offeredYear = " + subjectObj.offeredYear + " , " +
                "offeredSemester = " + subjectObj.offeredSemester + " , " +
                "program = '" + subjectObj.program + "' , " +
                "isParallel = " + subjectObj.isParallel + " , " +
                "category = '" + subjectObj.category + "' " +
                "WHERE subjectCode = '" + subjectObj.subjectCode + "'";
            cmd.ExecuteNonQuery();


            Console.WriteLine(editTagsStatusList);
            foreach (SubjectEditStatus element in editTagsStatusList)
            {
                if (element.closeClickStatus == false || element.hrs == 0)
                {
                    //delete

                    cmd.CommandText = @"DELETE FROM subjects_tags " +
                    "WHERE subjectCode = '" + subjectObj.subjectCode + "' AND tag = '" + element.tag + "'";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    //first check whether already exist

                    string stm = "";

                    stm = "select * from subjects_tags where  subjectCode = '" + subjectObj.subjectCode + "' AND tag = '" + element.tag + "' ";

                    using var cmd1 = new SQLiteCommand(stm, con);
                    using SQLiteDataReader rdr = cmd1.ExecuteReader();

                    int counter = 0;
                    while (rdr.Read())
                    {
                        counter++;
                    }

                    rdr.Close();
                    if (counter == 0)
                    {
                        //No record in DB
                        //insert query

                        cmd.CommandText = @"INSERT INTO subjects_tags Values('" + subjectObj.subjectCode + "', '" + element.tag + "', " + element.hrs + " )";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        //Record in DB
                        // exist if(closeclickStatus== false || hrs == 0)  =>delete
                        // otherwise update
                        cmd.CommandText = @"Update subjects_tags SET hrs =" + element.hrs + " " +
                            "WHERE subjectCode = '" + subjectObj.subjectCode + "' AND tag = '" + element.tag + "'";
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            con.Close();
            MessageBox.Show("Update success");
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab == ViewSearchSubjects)
            {
                refreshSubjectGrid();
                fillProgramsInAddSubject();
            }
        }

        private void fillProgramsInAddSubject()
        {
            cmbProgram_Add.Items.Clear();

            cmbProgram_Add.Items.Add("SE");
            cmbProgram_Add.Items.Add("IT");
            cmbProgram_Add.Items.Add("CS");
            cmbProgram_Add.Items.Add("IM");
            cmbProgram_Add.Items.Add("DS");


        }

        private void imgLoggedUser_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login obj = new Login();
            obj.Show();
        }

        private void AddSubject_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login obj = new Login();
            obj.Show();
        }
        
    }
}
