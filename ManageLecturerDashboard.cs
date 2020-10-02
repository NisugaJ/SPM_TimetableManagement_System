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
    public partial class ManageLecturerDashboard : Form
    {
        int levelNumber = 1;
        string imageLink = "";
        string gblSafeFileName = "";
        string gblSafeFileName_Edit = "";
        string completeImagePath = "";
        bool setImage = false;
        string imagePathForRemove = "";
        string imagePathForUpdate = "";
        string newImagePathForUpdate = "";
        string gblUpdatedFullPath_Edit = "";
        bool chooseImageButtonTouched = false;

        public ManageLecturerDashboard()
        {
            InitializeComponent();
            tabControl1.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);
            this.WindowState = FormWindowState.Maximized;
        }

        private void ManageLecturerDashboard_Load(object sender, EventArgs e)
        {
            //Add tab
            loading1.Visible = false;

            fillEmployeeLevelCmb();
            fillTitleCmb();

            fillCenterCmb();

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath = ".\\lecturerDefaultImage.png";

            pictureBoxLecturer.Image = Image.FromFile(@""+ completePath);          
            pictureBoxLecturer.SizeMode = PictureBoxSizeMode.StretchImage;

            cleanlecturerSummary();

            //Search tab
            radFindByIdSearch.Checked = true;
            refreshLecturersSearch();
            fillComboBoxesInSearch();
            cleanLecturerSummaryAndSetInitialImage_Search();

            refreshLecturersSearch();

            //Remove tab
            radFindById_Remove.Checked = true;
            cleanLecturerSummaryAndSetInitialImage_Remove();

            //Edit tab

            string workingDirectory1 = Environment.CurrentDirectory;
            string projectDirectory1 = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath1 = ".\\lecturerDefaultImage.png";

            pictureBoxLecturer_Edit.Image = Image.FromFile(@"" + completePath1);
            pictureBoxLecturer_Edit.SizeMode = PictureBoxSizeMode.StretchImage;


            cleanLecturerEditTab();
        }

        private void cleanLecturerEditTab()
        {

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath = ".\\lecturerDefaultImage.png";

            pictureBoxLecturer_Edit.Image = Image.FromFile(@"" + completePath);
            pictureBoxLecturer_Edit.SizeMode = PictureBoxSizeMode.StretchImage;

            lblSelectedImg_Edit.Text = "";
            txtEmpRank_Edit.Text = "";

            lblLecTitle_Edit.Text = "";
            lblLecName_Edit.Text = "";
            lblLecID_Edit.Text = "";
            lblLecDepartment_Edit.Text = "";
            lblLecFaculty_Edit.Text = "";
            lblLecCenter_Edit.Text = "";

            radFindById_Edit.Checked = true;
            txtFind_Edit.Text = "";

            txtName_Edit.Text = "";
            txtEmpID_Edit.Text = "";

            imagePathForUpdate = "";
            
            cmbTitle_Edit.Items.Clear();
            cmbTitle_Edit.Text = "";
            cmbFaculty_Edit.Items.Clear();
            cmbFaculty_Edit.Text = "";
            cmbDepartment_Edit.Items.Clear();
            cmbDepartment_Edit.Text = "";
            cmbCenter_Edit.Items.Clear();
            cmbCenter_Edit.Text = "";
            cmbBuilding_Edit.Items.Clear();
            cmbBuilding_Edit.Text = "";
            cmbEmpLevel_Edit.Items.Clear();
            cmbEmpLevel_Edit.Text = "";

            chooseImageButtonTouched = false;

        }

        private void cleanLecturerSummaryAndSetInitialImage_Remove()
        {
            lblLecTitle_Remove.Text = "";
            lblLecName_Remove.Text = "";
            lblLecID_Remove.Text = "";
            lblLecDepartment_Remove.Text = "";
            lblLecFaculty_Remove.Text = "";
            lblLecturerCenter.Text = "";

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath = ".\\lecturerDefaultImage.png";

            pictureBoxLecturer_Remove.Image = Image.FromFile(@"" + completePath);
            pictureBoxLecturer_Remove.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void cleanLecturerSummaryAndSetInitialImage_Search()
        {
            lblLecTitleSearch.Text = "";
            lblLecNameSearch.Text = "";
            lblLecIDSearch.Text = "";
            lblLecDepartmentSearch.Text = "";
            lblLecFacultySearch.Text = "";
            lblLecCenterSearch.Text = "";
            lblLecRankSearch.Text = "";

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath = ".\\lecturerDefaultImage.png";

            pictureBoxLecturerSearch.Image = Image.FromFile(@"" + completePath);
            pictureBoxLecturerSearch.SizeMode = PictureBoxSizeMode.StretchImage;

        }


        private void fillCenterCmb()
        {
            cmbCenter.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";
          
           stm = "SELECT centerName FROM center";
            
            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbCenter.Items.Add(""+ $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbCenter.SelectedIndex = 0;

            string selectedCenter = this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem);
            fillBuildingCmb_New(selectedCenter);
            fillFacultyCmb_New(selectedCenter);
 
        }



        private void fillBuildingCmb_New(string selectedCenter)
        {
            cmbBuilding.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";
            
            stm = "SELECT building FROM center_building WHERE centerName ='"+ selectedCenter + "'";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbBuilding.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbBuilding.SelectedIndex = 0;

        }


        private void fillFacultyCmb_New(string selectedCenter)
        {
            cmbFaculty.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT faculty FROM center_faculty WHERE centerName ='" + selectedCenter + "'";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbFaculty.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbFaculty.SelectedIndex = 0;

        }

        private void fillTitleCmb()
        {
            cmbTitle.Items.Clear();

            cmbTitle.Items.Add("Mr.");
            cmbTitle.Items.Add("Mrs.");
            cmbTitle.Items.Add("Miss.");
            cmbTitle.Items.Add("Ms.");

            cmbTitle.SelectedIndex = 0;
        }

        private void fillEmployeeLevelCmb()
        {
            cmbEmpLevel.Items.Clear();

            cmbEmpLevel.Items.Add("1 - Professor");
            cmbEmpLevel.Items.Add("2 - Assistant Professor");
            cmbEmpLevel.Items.Add("3 - Senior Lecturer(HG)");
            cmbEmpLevel.Items.Add("4 - Senior Lecturer");
            cmbEmpLevel.Items.Add("5 - Lecturer");
            cmbEmpLevel.Items.Add("6 - Assistnt Lecturer");
            cmbEmpLevel.Items.Add("7 - Instructors");
        }

        private void setEmployeeRank()
        {
            
            
            if (cmbEmpLevel.Text.Equals("1 - Professor")){
                levelNumber = 1;
            }else if (cmbEmpLevel.Text.Equals("2 - Assistant Professor")){
                levelNumber = 2;
            }else if (cmbEmpLevel.Text.Equals("3 - Senior Lecturer(HG)")){
                levelNumber = 3;
            }else if (cmbEmpLevel.Text.Equals("4 - Senior Lecturer")){
                levelNumber = 4;
            }else if (cmbEmpLevel.Text.Equals("5 - Lecturer")){
                levelNumber = 5;
            } else if (cmbEmpLevel.Text.Equals("6 - Assistnt Lecturer")){
                levelNumber = 6;
            }else if (cmbEmpLevel.Text.Equals("7 - Instructors")) {
                levelNumber = 7;
            }
            String rank = levelNumber + "."+ txtEmpID.Text;
            txtEmpRank.Text = rank;
        
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

        private void cmbEmpLevel_Changed(object sender, EventArgs e)
        {
            setEmployeeRank();
        }

        private void btnAddLecturer_Click(object sender, EventArgs e)
        {
            loading1.Visible = true;

            int number;

            bool parseIntSuccess = Int32.TryParse(txtEmpID.Text, out number);

            if (cmbTitle.Text.Equals(""))
            {
                MessageBox.Show("Please select Mr/Mrs/Miss/Ms", "Error");
            }
            else if (txtName.Text.Equals("")){
                MessageBox.Show("Please enter name", "Error");
            }
            else if (txtEmpID.Text.Equals("")){
                MessageBox.Show("Please enter employee ID", "Error");
            }
            else if (!parseIntSuccess || !(txtEmpID.Text.Length==6))
            {
                MessageBox.Show("Employee ID must be a 6 digits number", "Error");
            }
            else if (cmbFaculty.Text.Equals("")){
                MessageBox.Show("Please select Faculty", "Error");
            }
            else if (cmbDepartment.Text.Equals("")){
                MessageBox.Show("Please select Department", "Error");
            }
            else if (cmbCenter.Text.Equals("")){
                MessageBox.Show("Please select center", "Error");
            }
            else if (cmbBuilding.Text.Equals("")){
                MessageBox.Show("Please select building", "Error");
            }
            else if (cmbEmpLevel.Text.Equals("")){
                MessageBox.Show("Please select employee level", "Error");
            }
            else if (setImage==false)
            {
                MessageBox.Show("Please select image", "Error");
            }
            else
            {
                saveImage();
                addEmployee();  
            }
            loading1.Visible = false;
        }

        private void saveImage()
        {
            if (!imageLink.Equals("")){
                string projectPath = Application.StartupPath.Substring(0, (Application.StartupPath.Length - 10));
                string imageFolderPath = ".\\";
                string imageFolderIsExists = ".\\";

                string subPath = "images"; // your code goes here

                System.IO.Directory.CreateDirectory(subPath);

                completeImagePath = imageFolderPath + "\\"+gblSafeFileName;
                File.Copy(imageLink, Path.Combine(@""+imageFolderPath, Path.GetFileName(imageLink)), true);
            }
            else
            {
                MessageBox.Show("No Image!!", "Error");
            }
        }

        private void addEmployee()
        {
            string cs = @"URI=file:.\"+Utils.dbName+".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS lecturers (
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
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO lecturers VALUES" +
                "(@lecturerID, @title, @name, @faculty, @department, @center, @building, @employeeLevel, @photoPath)";

            cmd.Parameters.AddWithValue("@lecturerID", txtEmpID.Text);
            cmd.Parameters.AddWithValue("@title", this.cmbTitle.GetItemText(this.cmbTitle.SelectedItem));
            cmd.Parameters.AddWithValue("@name", txtName.Text);
            cmd.Parameters.AddWithValue("@faculty", this.cmbFaculty.GetItemText(this.cmbFaculty.SelectedItem));
            cmd.Parameters.AddWithValue("@department", this.cmbDepartment.GetItemText(this.cmbDepartment.SelectedItem));
            cmd.Parameters.AddWithValue("@center", this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem));
            cmd.Parameters.AddWithValue("@building", this.cmbBuilding.GetItemText(this.cmbBuilding.SelectedItem));
            cmd.Parameters.AddWithValue("@employeeLevel", levelNumber);
            cmd.Parameters.AddWithValue("@photoPath", completeImagePath);
            
            cmd.Prepare();
            try
            {
                cmd.ExecuteNonQuery();

                MessageBox.Show("Employee Added", "Success");
                resetInputData();
                cleanlecturerSummary();
            }
            catch(SQLiteException e)
            {
                if (e.ErrorCode == 19)
                {
                    MessageBox.Show("Same lecturer Id already in the system!");

                    try
                    {
                        // Check if file exists with its full path    
                        if (File.Exists(@"" + completeImagePath))
                        {
                            // If file found, delete it
                            File.Delete(@"" + completeImagePath);

                        }
                        else
                        {
                            MessageBox.Show("File not found");
                        }
                    }
                    catch (IOException ioExp)
                    {
                        MessageBox.Show(ioExp.Message);
                    }

                }
            }
            finally
            {
                con.Close();
            }
     
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if(open.ShowDialog() == DialogResult.OK)
            {
                lblSelectedImg.Text = open.SafeFileName;
                gblSafeFileName = open.SafeFileName;
                pictureBoxLecturer.Image =new Bitmap(open.FileName);
                pictureBoxLecturer.SizeMode = PictureBoxSizeMode.StretchImage;
                imageLink = open.FileName;
                setImage = true;
            }
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            resetInputData();
            cleanlecturerSummary();
        }

        private void resetInputData()
        {

            levelNumber = 1;
            imageLink = "";
            gblSafeFileName = "";
            completeImagePath = "";

            txtName.Text = "";
            txtEmpID.Text = "";
            lblSelectedImg.Text = "Image Not Selected";

            fillEmployeeLevelCmb();
            fillTitleCmb();

            fillCenterCmb();


            cmbTitle.Text = "";
            cmbFaculty.Text = "";
            cmbDepartment.Text = "";
            cmbCenter.Text = "";
            cmbBuilding.Text = "";
            cmbEmpLevel.Text = "";
            txtEmpRank.Text = "";


            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath =".\\lecturerDefaultImage.png";

            pictureBoxLecturer.Image = Image.FromFile(@"" + completePath);
            pictureBoxLecturer.SizeMode = PictureBoxSizeMode.StretchImage;

            setImage = false;
        }

        private void cleanlecturerSummary()
        {
            lblLecTitle.Text = "";
            lblLecName.Text = "";
            lblLecID.Text = "";
            lblLecDepartment.Text = "";
            lblLecFaculty.Text = "";
            lblLecCenter.Text = "";
               
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            lblLecTitle.Text = this.cmbTitle.GetItemText(this.cmbTitle.SelectedItem);
            lblLecName.Text = txtName.Text;
        }

        private void txtEmpID_Leave(object sender, EventArgs e)
        {
            setEmployeeRank();
            lblLecID.Text = txtEmpID.Text;
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Faculty indexchanged
            lblLecFaculty.Text = this.cmbFaculty.GetItemText(this.cmbFaculty.SelectedItem);
            fillDepartmentCmb_New(this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem) , this.cmbFaculty.GetItemText(this.cmbFaculty.SelectedItem));
        }

        private void cmbTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblLecTitle.Text = this.cmbTitle.GetItemText(this.cmbTitle.SelectedItem);
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblLecDepartment.Text = this.cmbDepartment.GetItemText(this.cmbDepartment.SelectedItem);
        }

        private void cmbCenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            //center index changed
            lblLecCenter.Text = this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem);

            fillBuildingCmb_New(this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem));
            fillFacultyCmb_New(this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem));
            fillDepartmentCmb_New(this.cmbCenter.GetItemText(this.cmbCenter.SelectedItem), this.cmbFaculty.GetItemText(this.cmbFaculty.SelectedItem));
        }

        private void fillDepartmentCmb_New(string selectedCenter, string selectedFaculty)
        {
            cmbDepartment.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT department FROM center_faculty_department WHERE centerName = '" + selectedCenter + "' AND faculty='" + selectedFaculty + "'";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbDepartment.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbDepartment.SelectedIndex = 0;

        }

        private void btnFindSearch_Click(object sender, EventArgs e)
        {
            int LecIdInt;
           
            if (radFindByIdSearch.Checked == true )
            {
                if (Int32.TryParse(txtSearch.Text, out LecIdInt) && (000000 <= LecIdInt && LecIdInt<= 999999))
                {
                    searchLecturer();
                }
                else
                {
                    MessageBox.Show("Lecturer ID must be a 6 digits number", "Error");
                }
            }
            else if(radFindByNameSearch.Checked == true)
            {
                searchLecturer();
            }
        }

        private void searchLecturer()
        {

            String keywordForSearch = txtSearch.Text;
            bool findEmployeeByIdSearch = radFindByIdSearch.Checked;
            bool findEmployeeByNameSearch = radFindByNameSearch.Checked;

            String searchType = "";
            if (findEmployeeByIdSearch)
            {
                searchType = "byId";
            }
            else if (findEmployeeByNameSearch)
            {
                searchType = "byName";
            }
            Lecturer lecObj = new Lecturer();
            lecObj = findEmployeeData(keywordForSearch, searchType);
          
            if (lecObj.name != null)
            {
                pictureBoxLecturerSearch.ImageLocation = @"" + lecObj.photoPath;
                pictureBoxLecturerSearch.SizeMode = PictureBoxSizeMode.StretchImage;

                lblLecTitleSearch.Text = lecObj.title;
                lblLecNameSearch.Text = lecObj.name;
                lblLecIDSearch.Text = "" + lecObj.lecturerID;
                lblLecDepartmentSearch.Text = lecObj.department;
                lblLecFacultySearch.Text = lecObj.faculty;
                lblLecCenterSearch.Text = lecObj.center;
                lblLecRankSearch.Text = lecObj.employeeLevel + "." + lecObj.lecturerID;
            }
            else
            {
                MessageBox.Show("Lecturer not found");
            }
        }

        private Lecturer findEmployeeData(string keywordForSearch, string searchType)
        {
           
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm= "";
            if (searchType.Equals("byId")){
                stm = "SELECT * FROM lecturers WHERE lecturerID="+ keywordForSearch;
            }
            else if (searchType.Equals("byName")){
                stm = "SELECT * FROM lecturers WHERE name='" + keywordForSearch+"'";
            }

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            Lecturer lecObj = new Lecturer();
            while (rdr.Read())
            {
               // Console.WriteLine($@"{rdr.GetInt32(0),-3} {rdr.GetString(1),-8} {rdr.GetInt32(2),8}");
                lecObj.lecturerID = Int32.Parse($@"{rdr.GetInt32(0),-3}");
                lecObj.title = $@"{ rdr.GetString(1),-8}";
                lecObj.name = $@"{ rdr.GetString(2),-8}";
                lecObj.faculty = $@"{ rdr.GetString(3),-8}";
                lecObj.department = $@"{ rdr.GetString(4),-8}";
                lecObj.center = $@"{ rdr.GetString(5),-8}";
                lecObj.building = $@"{ rdr.GetString(6),-8}";
                lecObj.employeeLevel = Int32.Parse($@"{rdr.GetInt32(7),-3}");
                lecObj.photoPath = $@"{ rdr.GetString(8),-8}";
                    
            }

            return lecObj;

        }

        private void btnResetSearch_Click(object sender, EventArgs e)
        {
            refreshLecturersSearch();
            resetLecturerSigleView();
            txtSearch.Text = "";
            radFindByIdSearch.Checked = true;
        }

        private void resetLecturerSigleView()
        {

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath = ".\\lecturerDefaultImage.png";

            pictureBoxLecturerSearch.Image = Image.FromFile(@"" + completePath);
            pictureBoxLecturerSearch.SizeMode = PictureBoxSizeMode.StretchImage;

            lblLecTitleSearch.Text = "";
            lblLecNameSearch.Text = "";
            lblLecIDSearch.Text = "";
            lblLecDepartmentSearch.Text = "";
            lblLecFacultySearch.Text = "";
            lblLecCenterSearch.Text = "";
            lblLecRankSearch.Text = "";
        }

        private void btnRefreshSearch_Click(object sender, EventArgs e)
        {
            fillComboBoxesInSearch();

            refreshLecturersSearch();

            //Reset code
            refreshLecturersSearch();
            resetLecturerSigleView();
            txtSearch.Text = "";
            radFindByIdSearch.Checked = true;

        }

        private void refreshLecturersSearch()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);
            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("select * from lecturers");
            cmd.Connection = conn;

            conn.Open();

            using var cmdCreate = new SQLiteCommand(conn);

            cmdCreate.CommandText = @"CREATE TABLE  IF NOT EXISTS lecturers (
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
            cmdCreate.ExecuteNonQuery();

            cmd.ExecuteScalar();
            System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSearchLecturer.DataSource = dt;
            conn.Close();
        }

        private void dataGridViewSearchLecturer_DoubleClick(object sender, EventArgs e)
        {
            lblLecIDSearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[0].Value.ToString();
            lblLecTitleSearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[1].Value.ToString();
            lblLecNameSearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[2].Value.ToString();
            lblLecFacultySearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[3].Value.ToString();
            lblLecDepartmentSearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[4].Value.ToString();
            lblLecCenterSearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[5].Value.ToString();
            String imagePath = dataGridViewSearchLecturer.CurrentRow.Cells[8].Value.ToString();
            lblLecRankSearch.Text = dataGridViewSearchLecturer.CurrentRow.Cells[7].Value.ToString() + "." + lblLecIDSearch.Text;

            pictureBoxLecturerSearch.ImageLocation = @""+ imagePath;
            pictureBoxLecturerSearch.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void fillComboBoxesInSearch()
        {
            fillCenterCmb_SearchNew();

            fillEmployeeLevelSearch();
        }

        private void fillCenterCmb_SearchNew()
        {
            cmbCenterSearch.Items.Clear();
            cmbCenterSearch.Items.Add("Any");

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT centerName FROM center";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbCenterSearch.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbCenterSearch.SelectedIndex = 0;

            string selectedCenter = this.cmbCenterSearch.GetItemText(this.cmbCenterSearch.SelectedItem);
            fillBuildingCmb_SearchNew(selectedCenter);
            fillFacultyCmb_SearchNew(selectedCenter);
            fillDepartmentCmbAny_New();

        }

        private void fillBuildingCmb_SearchNew(string selectedCenter)
        {
            cmbBuildingSearch.Items.Clear();
            cmbBuildingSearch.Items.Add("Any");

            if (selectedCenter.Equals("Any"))
            {
                cmbBuildingSearch.SelectedIndex = 0;
            }
            else
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();
                string stm = "";

                stm = "SELECT building FROM center_building WHERE centerName ='" + selectedCenter + "'";

                using var cmd = new SQLiteCommand(stm, con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cmbBuildingSearch.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
                }
                cmbBuildingSearch.SelectedIndex = 0;
            }

        }


        private void fillFacultyCmb_SearchNew(string selectedCenter)
        {
            cmbFacultySearch.Items.Clear();
            cmbFacultySearch.Items.Add("Any");

            if (selectedCenter.Equals("Any"))
            {
                cmbFacultySearch.SelectedIndex = 0;
            }
            else
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();
                string stm = "";

                stm = "SELECT faculty FROM center_faculty WHERE centerName ='" + selectedCenter + "'";

                using var cmd = new SQLiteCommand(stm, con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cmbFacultySearch.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
                }
                cmbFacultySearch.SelectedIndex = 0;
            }
        }


        private void fillDepartmentCmbAny_New()
        {
            cmbDepartmentSearch.Items.Clear();
            cmbDepartmentSearch.Items.Add("Any");
            cmbDepartmentSearch.SelectedIndex = 0;
                
        }

        private void fillEmployeeLevelSearch()
        {
            cmbEmpLevelSearch.Items.Clear();

            cmbEmpLevelSearch.Items.Add("Any");

            cmbEmpLevelSearch.Items.Add("1 - Professor");
            cmbEmpLevelSearch.Items.Add("2 - Assistant Professor");
            cmbEmpLevelSearch.Items.Add("3 - Senior Lecturer(HG)");
            cmbEmpLevelSearch.Items.Add("4 - Senior Lecturer");
            cmbEmpLevelSearch.Items.Add("5 - Lecturer");
            cmbEmpLevelSearch.Items.Add("6 - Assistnt Lecturer");
            cmbEmpLevelSearch.Items.Add("7 - Instructors");

            cmbEmpLevelSearch.SelectedIndex = 0;
        }

        private void cmbFacultySearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillDepartmentCmb_SearchNew(this.cmbCenterSearch.GetItemText(this.cmbCenterSearch.SelectedItem), this.cmbFacultySearch.GetItemText(this.cmbFacultySearch.SelectedItem));

            filterLecturerByDropdown();
        }

     
        private void cmbDepartmentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterLecturerByDropdown();
        }

        private void cmbCenterSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillBuildingCmb_SearchNew(this.cmbCenterSearch.GetItemText(this.cmbCenterSearch.SelectedItem));
            fillFacultyCmb_SearchNew(this.cmbCenterSearch.GetItemText(this.cmbCenterSearch.SelectedItem));
            fillDepartmentCmb_SearchNew(this.cmbCenterSearch.GetItemText(this.cmbCenterSearch.SelectedItem), this.cmbFacultySearch.GetItemText(this.cmbFacultySearch.SelectedItem));


            filterLecturerByDropdown();
        }

        private void fillDepartmentCmb_SearchNew(string selectedCenter, string selectedFaculty)
        {
            cmbDepartmentSearch.Items.Clear();
            cmbDepartmentSearch.Items.Add("Any");

            if ( !selectedCenter.Equals("Any") && !selectedFaculty.Equals("Any"))
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();
                string stm = "";


                stm = "SELECT department FROM center_faculty_department WHERE centerName = '" + selectedCenter + "' AND faculty='" + selectedFaculty + "'";

                using var cmd = new SQLiteCommand(stm, con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cmbDepartmentSearch.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
                }
                cmbDepartmentSearch.SelectedIndex = 0;
            }
            else
            {
                cmbDepartmentSearch.SelectedIndex = 0;
            }

        }

        private void cmbBuildingSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterLecturerByDropdown();
        }

        private void cmbEmpLevelSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterLecturerByDropdown();
        }

        private void filterLecturerByDropdown()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(cs);

            string query = genarateQueryForSearchInComboBoxSelection();

            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(query);
            cmd.Connection = conn;

            conn.Open();

            using var cmdCreate = new SQLiteCommand(conn);

            cmdCreate.CommandText = @"CREATE TABLE  IF NOT EXISTS lecturers (
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
            cmdCreate.ExecuteNonQuery();

            cmd.ExecuteScalar();
            System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSearchLecturer.DataSource = dt;
            conn.Close();
            
        }

        private String genarateQueryForSearchInComboBoxSelection()
        {

            string selectedFaculty = this.cmbFacultySearch.GetItemText(this.cmbFacultySearch.SelectedItem);
            string selectedDepartment = this.cmbDepartmentSearch.GetItemText(this.cmbDepartmentSearch.SelectedItem);
            string selectedCenter = this.cmbCenterSearch.GetItemText(this.cmbCenterSearch.SelectedItem);
            string selectedBuilding = this.cmbBuildingSearch.GetItemText(this.cmbBuildingSearch.SelectedItem);
            string selectedLevel = this.cmbEmpLevelSearch.GetItemText(this.cmbEmpLevelSearch.SelectedItem);
            int selectedLevelNumber=1;
            int appendCounter = 0;

            if (selectedLevel.Equals("1 - Professor")){
                selectedLevelNumber = 1;
            }else if (selectedLevel.Equals("2 - Assistant Professor")){
                selectedLevelNumber = 2;
            }else if (selectedLevel.Equals("3 - Senior Lecturer(HG)")){
                selectedLevelNumber = 3;
            }else if (selectedLevel.Equals("4 - Senior Lecturer")){
                selectedLevelNumber = 4;
            }else if (selectedLevel.Equals("5 - Lecturer")){
                selectedLevelNumber = 5;
            }else if (selectedLevel.Equals("6 - Assistnt Lecturer")){
                selectedLevelNumber = 6;
            }else if (selectedLevel.Equals("7 - Instructors")){
                selectedLevelNumber = 7;
            }

            string query = "SELECT * FROM lecturers ";

            //Append selected Faculty
            if (selectedFaculty.Equals("Any")) {
                // Do nothing
            }
            else{
                if (appendCounter == 0)
                {
                    query = query + "WHERE faculty='"+ selectedFaculty+"'";
                    appendCounter++;
                }
                else
                {
                    query = query + "AND faculty='" + selectedFaculty + "'";
                }
            }

            //Append selected Department
            if (selectedDepartment.Equals("Any")) {
                // Do nothing
            }
            else {
                if (appendCounter == 0)
                {
                    query = query + "WHERE department='" + selectedDepartment + "'";
                    appendCounter++;
                }
                else
                {
                    query = query + "AND department='" + selectedDepartment + "'";
                }
            }

            //Append selected Center
            if (selectedCenter.Equals("Any")){
                // Do nothing
            }
            else {
                if (appendCounter == 0)
                {
                    query = query + "WHERE center='" + selectedCenter + "'";
                    appendCounter++;
                }
                else
                {
                    query = query + "AND center='" + selectedCenter + "'";
                }
            }

            //Append selected Building
            if (selectedBuilding.Equals("Any"))
            {
                // Do nothing
            }
            else
            {
                if (appendCounter == 0)
                {
                    query = query + "WHERE building='" + selectedBuilding + "'";
                    appendCounter++;
                }
                else
                {
                    query = query + "AND building='" + selectedBuilding + "'";
                }
            }

            //Append selected Level
            if (selectedLevel.Equals("Any"))
            {
                // Do nothing
            }
            else
            {
                if (appendCounter == 0)
                {
                    query = query + "WHERE employeeLevel='" + selectedLevelNumber + "'";
                    appendCounter++;
                }
                else
                {
                    query = query + "AND employeeLevel='" + selectedLevelNumber + "'";
                }
            }


            return query;
        }

        private void btnRemove_Remove_Click(object sender, EventArgs e)
        {
            if (lblLecID_Remove.Text.Equals(""))
            {
                MessageBox.Show("Please find a lecturer for delete");
            }
            else
            {
                deleteLecturerImage();
                
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS lecturers (
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
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM lecturers WHERE lecturerID = @lecturerID"; 

                cmd.Parameters.AddWithValue("@lecturerID", lblLecID_Remove.Text);
                
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                con.Close();

                MessageBox.Show("Employee Deleted successfully.", "Success");
                cleanLecturerData_Remove();
                
            }
        }

        private void deleteLecturerImage()
        {

                try
                {
                    // Check if file exists with its full path    
                    if (File.Exists(@""+ imagePathForRemove))
                    {
                        // If file found, delete it
                        File.Delete(@"" + imagePathForRemove);
                  
                    }
                    else{
                        MessageBox.Show("File not found");
                    }
                }
                catch (IOException ioExp)
                {
                    MessageBox.Show(ioExp.Message);
                }
        }

        private void btnReset_Remove_Click(object sender, EventArgs e)
        {
            radFindById_Remove.Checked = true;
            cleanLecturerData_Remove();
        }

        private void cleanLecturerData_Remove()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string completePath = ".\\lecturerDefaultImage.png";

            pictureBoxLecturer_Remove.Image = Image.FromFile(@"" + completePath);
            pictureBoxLecturer_Remove.SizeMode = PictureBoxSizeMode.StretchImage;

            txtFind_Remove.Text = "";

            lblLecTitle_Remove.Text = "";
            lblLecName_Remove.Text = "";
            lblLecID_Remove.Text = "";
            lblLecDepartment_Remove.Text = "";
            lblLecFaculty_Remove.Text = "";
            lblLecturerCenter.Text = "";

            imagePathForRemove = "";
        }

        private void btnFind_Remove_Click(object sender, EventArgs e)
        {

            int LecIdInt;

            if (radFindById_Remove.Checked == true)
            {
                if (Int32.TryParse(txtFind_Remove.Text, out LecIdInt) && (000000 <= LecIdInt && LecIdInt <= 999999))
                {
                    searchLecturerForRemove();
                }
                else
                {
                    MessageBox.Show("Lecturer ID must be a 6 digits number", "Error");
                }
            }
            else if (radFindByName_Remove.Checked == true)
            {
                searchLecturerForRemove();
            }

        }


        private void searchLecturerForRemove()
        {

            String keywordForSearch = txtFind_Remove.Text;
            bool findEmployeeById_Remove = radFindById_Remove.Checked;
            bool findEmployeeByName_Remove = radFindByName_Remove.Checked;

            String searchType = "";
            if (findEmployeeById_Remove)
            {
                searchType = "byId";
            }
            else if (findEmployeeByName_Remove)
            {
                searchType = "byName";
            }
            Lecturer lecObj = new Lecturer();
            lecObj = findEmployeeData(keywordForSearch, searchType);
         
            if (lecObj.name != null)
            {
                pictureBoxLecturer_Remove.ImageLocation = @"" + lecObj.photoPath;
                pictureBoxLecturer_Remove.SizeMode = PictureBoxSizeMode.StretchImage;

                lblLecTitle_Remove.Text = lecObj.title;
                lblLecName_Remove.Text = lecObj.name;
                lblLecID_Remove.Text = "" + lecObj.lecturerID;
                lblLecDepartment_Remove.Text = lecObj.department;
                lblLecFaculty_Remove.Text = lecObj.faculty;
                lblLecturerCenter.Text = lecObj.center;

                imagePathForRemove = lecObj.photoPath;

            }
            else
            {
                MessageBox.Show("Lecturer not found");
            }
        }

        private void btnReset_Edit_Click(object sender, EventArgs e)
        {
            cleanLecturerEditTab();
        }

        private void btnFind_Edit_Click(object sender, EventArgs e)
        {

            int LecIdInt;

            if (radFindById_Edit.Checked == true)
            {
                if (Int32.TryParse(txtFind_Edit.Text, out LecIdInt) && (000000 <= LecIdInt && LecIdInt <= 999999))
                {
                    searchLecturerForEdit();
                }
                else
                {
                    MessageBox.Show("Lecturer ID must be a 6 digits number", "Error");
                }
            }
            else if (radFindByName_Edit.Checked == true)
            {
                searchLecturerForEdit();
            }

        }

        private void searchLecturerForEdit()
        {

            String keywordForSearch = txtFind_Edit.Text;
            bool findEmployeeById_Edit= radFindById_Edit.Checked;
            bool findEmployeeByName_Edit = radFindByName_Edit.Checked;

            String searchType = "";
            if (findEmployeeById_Edit)
            {
                searchType = "byId";
            }
            else if (findEmployeeByName_Edit)
            {
                searchType = "byName";
            }
            Lecturer lecObj = new Lecturer();
            lecObj = findEmployeeData(keywordForSearch, searchType);

            if (lecObj.name != null)
            {
                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
                string completePath = ".\\lecturerDefaultImage.png";

                pictureBoxLecturer_Edit.Image = Image.FromFile(@"" + completePath);
                pictureBoxLecturer_Edit.SizeMode = PictureBoxSizeMode.StretchImage;

                lblLecTitle_Edit.Text = lecObj.title;
                lblLecName_Edit.Text = lecObj.name;
                lblLecID_Edit.Text = "" + lecObj.lecturerID;
                lblLecDepartment_Edit.Text = lecObj.department;
                lblLecFaculty_Edit.Text = lecObj.faculty;
                lblLecCenter_Edit.Text = lecObj.center;

                imagePathForUpdate = lecObj.photoPath;

                loadLecturerDataCmbs_Edit();
                setDataToCmbAndTxt_Edit(lecObj);
                cmbBoxSetSpecificValue(lecObj);
                fillLecImageFoundForSearch(lecObj);

            }
            else
            {
                MessageBox.Show("Lecturer not found");
            }

        }

        private void fillLecImageFoundForSearch(Lecturer lecObj)
        {

            Image imgRemoveLecturer = null;

            using (FileStream stream = new FileStream(lecObj.photoPath, FileMode.Open))
            {
                imgRemoveLecturer = Image.FromStream(stream);
            }

            pictureBoxLecturer_Edit.Image = imgRemoveLecturer;

        }

        private void cmbBoxSetSpecificValue(Lecturer lecObj)
        {
            cmbTitle_Edit.SelectedIndex = cmbTitle_Edit.FindStringExact(lecObj.title.Trim());
            cmbFaculty_Edit.SelectedIndex = cmbFaculty_Edit.FindStringExact(lecObj.faculty.Trim());
            cmbDepartment_Edit.SelectedIndex = cmbDepartment_Edit.FindStringExact(lecObj.department.Trim());
            cmbCenter_Edit.SelectedIndex = cmbCenter_Edit.FindStringExact(lecObj.center.Trim());
            cmbBuilding_Edit.SelectedIndex = cmbBuilding_Edit.FindStringExact(lecObj.building.Trim());

            int locFound = 0;
            for (int i = 0; i < cmbEmpLevel_Edit.Items.Count; i++)
            {
                string value = cmbEmpLevel_Edit.GetItemText(cmbEmpLevel_Edit.Items[i]);
                if (value.Contains(lecObj.employeeLevel + ""))
                {
                    locFound = i;
                }
            }
            cmbEmpLevel_Edit.SelectedIndex = locFound;

     
        }

        private void setDataToCmbAndTxt_Edit(Lecturer lecObj)
        {
            txtName_Edit.Text = lecObj.name.Trim();
            txtEmpID_Edit.Text = lecObj.lecturerID+"";
            txtEmpRank_Edit.Text = lecObj.employeeLevel + "."+ lecObj.lecturerID;
            
        }

        private void loadLecturerDataCmbs_Edit()
        {
            fillEmployeeLevelCmb_Edit();
            fillTitleCmb_Edit();
            
            fillFacultyCmb_Edit();
            fillDepartmentCmb_Edit();
            fillCenterCmb_Edit_New();
            fillBuilding_Edit();
        }

        private void fillDepartmentCmb_Edit()
        {
            cmbDepartment_Edit.Items.Clear();

            cmbDepartment_Edit.Items.Add("SE");
            cmbDepartment_Edit.Items.Add("IT");
            cmbDepartment_Edit.Items.Add("CSNE");
        }

        private void fillCenterCmb_Edit_New()
        {
            cmbCenter_Edit.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT centerName FROM center";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbCenter_Edit.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbCenter_Edit.SelectedIndex = 0;

            string selectedCenter = this.cmbCenter_Edit.GetItemText(this.cmbCenter_Edit.SelectedItem);
            fillBuildingCmb_Edit_New(selectedCenter);
            fillFacultyCmb_Edit_New(selectedCenter);

        }

        private void fillBuildingCmb_Edit_New(string selectedCenter)
        {
            cmbBuilding_Edit.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT building FROM center_building WHERE centerName ='" + selectedCenter + "'";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbBuilding_Edit.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbBuilding_Edit.SelectedIndex = 0;

        }


        private void fillFacultyCmb_Edit_New(string selectedCenter)
        {
            cmbFaculty_Edit.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT faculty FROM center_faculty WHERE centerName ='" + selectedCenter + "'";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbFaculty_Edit.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbFaculty_Edit.SelectedIndex = 0;

        }

        private void fillBuilding_Edit()
        {
            cmbBuilding_Edit.Items.Clear();

            cmbBuilding_Edit.Items.Add("Main building");
            cmbBuilding_Edit.Items.Add("New building");
        }


        private void fillFacultyCmb_Edit()
        {
            cmbFaculty_Edit.Items.Clear();

            cmbFaculty_Edit.Items.Add("Faculty of Computing");
            cmbFaculty_Edit.Items.Add("Faculty of Engineering");
            cmbFaculty_Edit.Items.Add("Faculty of Business");
        }

        private void fillTitleCmb_Edit()
        {
            cmbTitle_Edit.Items.Clear();

            cmbTitle_Edit.Items.Add("Mr.");
            cmbTitle_Edit.Items.Add("Mrs.");
            cmbTitle_Edit.Items.Add("Miss.");
            cmbTitle_Edit.Items.Add("Ms.");
        }

        private void fillEmployeeLevelCmb_Edit()
        {
            cmbEmpLevel_Edit.Items.Clear();

            cmbEmpLevel_Edit.Items.Add("1 - Professor");
            cmbEmpLevel_Edit.Items.Add("2 - Assistant Professor");
            cmbEmpLevel_Edit.Items.Add("3 - Senior Lecturer(HG)");
            cmbEmpLevel_Edit.Items.Add("4 - Senior Lecturer");
            cmbEmpLevel_Edit.Items.Add("5 - Lecturer");
            cmbEmpLevel_Edit.Items.Add("6 - Assistnt Lecturer");
            cmbEmpLevel_Edit.Items.Add("7 - Instructors");
        }

        private void btnEditLecturer_Click(object sender, EventArgs e)
        {
            int tempEmpId = 0;

            Int32.TryParse(txtEmpID_Edit.Text, out tempEmpId);

            int tempEmpLevel = 0;

            if (cmbEmpLevel_Edit.Text.Equals("1 - Professor")){
                tempEmpLevel = 1;
            }else if (cmbEmpLevel_Edit.Text.Equals("2 - Assistant Professor")){
                tempEmpLevel = 2;
            }else if (cmbEmpLevel_Edit.Text.Equals("3 - Senior Lecturer(HG)")){
                tempEmpLevel = 3;
            }else if (cmbEmpLevel_Edit.Text.Equals("4 - Senior Lecturer")){
                tempEmpLevel = 4;
            }else if (cmbEmpLevel_Edit.Text.Equals("5 - Lecturer")){
                tempEmpLevel = 5;
            }else if (cmbEmpLevel_Edit.Text.Equals("6 - Assistnt Lecturer")) {
                tempEmpLevel = 6;
            } else if (cmbEmpLevel_Edit.Text.Equals("7 - Instructors")) {
                tempEmpLevel = 7;
            }

                
            string title = this.cmbTitle_Edit.GetItemText(this.cmbTitle_Edit.SelectedItem);
            string name = txtName_Edit.Text;
            int empID = tempEmpId;
            string faculty = this.cmbFaculty_Edit.GetItemText(this.cmbFaculty_Edit.SelectedItem);
            string department = this.cmbDepartment_Edit.GetItemText(this.cmbDepartment_Edit.SelectedItem);
            string center = this.cmbCenter_Edit.GetItemText(this.cmbCenter_Edit.SelectedItem);
            string building = this.cmbBuilding_Edit.GetItemText(this.cmbBuilding_Edit.SelectedItem);
            int empLevel = tempEmpLevel;


            int number;

            bool parseIntSuccess = Int32.TryParse(txtEmpID_Edit.Text, out number);

            if (cmbTitle_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select Mr/Mrs/Miss/Ms", "Error");
            }
            else if (txtName_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please enter name", "Error");
            }
            else if (txtEmpID_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please enter employee ID", "Error");
            }
            else if (!parseIntSuccess || !(txtEmpID_Edit.Text.Length == 6))
            {
                MessageBox.Show("Employee ID must be a 6 digits number", "Error");
            }
            else if (cmbFaculty_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select Faculty", "Error");
            }
            else if (cmbDepartment_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select Department", "Error");
            }
            else if (cmbCenter_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select center", "Error");
            }
            else if (cmbBuilding_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select building", "Error");
            }
            else if (cmbEmpLevel_Edit.Text.Equals(""))
            {
                MessageBox.Show("Please select employee level", "Error");
            }
            else
            {
                if (chooseImageButtonTouched == true)
                {
                    //delete existing image and submit new image
 
                    //Delete Old image
                    try
                    {
                        pictureBoxLecturer_Edit.Image = null;
                        // Check if file exists with its full path    
                        if (File.Exists(@"" + imagePathForUpdate))
                        {
                            // If file found, delete it    
                            File.Delete(@"" + imagePathForUpdate);
                        }
                        else
                        {
                            MessageBox.Show("File not found");
                        }
                    }
                    catch (IOException ioExp)
                    {
                        MessageBox.Show(ioExp.Message);
                    }
 
                    //Add new Image

                    if (!newImagePathForUpdate.Equals(""))
                    {
                        string projectPath = Application.StartupPath.Substring(0, (Application.StartupPath.Length - 10));
                        string imageFolderPath = ".\\";
                        completeImagePath = imageFolderPath + "\\" + gblSafeFileName_Edit;
                        File.Copy(newImagePathForUpdate, Path.Combine(@"" + imageFolderPath, Path.GetFileName(newImagePathForUpdate)), true);
                        
                        gblUpdatedFullPath_Edit= imageFolderPath+"\\"+gblSafeFileName_Edit;

                    }
                   
                }

                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS lecturers (
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
                cmd.ExecuteNonQuery();

                if (chooseImageButtonTouched)
                {
                    cmd.CommandText = @"UPDATE lecturers SET title= '" + cmbTitle_Edit.Text + "' , " +
                    "name = '" + txtName_Edit.Text + "' , " +
                    "faculty = '" + cmbFaculty_Edit.Text + "' , " +
                    "department = '" + cmbDepartment_Edit.Text + "' , " +
                    "center = '" + cmbCenter_Edit.Text + "' , " +
                    "building = '" + cmbBuilding_Edit.Text + "' , " +
                    "employeeLevel = '" + tempEmpLevel + "' , " +
                    "photoPath = '" + gblUpdatedFullPath_Edit + "' " +
                    "WHERE lecturerID = '" + empID + "'";
                    cmd.ExecuteNonQuery();

                }
                else
                {
                    cmd.CommandText = @"UPDATE lecturers SET title= '" + cmbTitle_Edit.Text + "' , " +
                    "name = '" + txtName_Edit.Text + "' , " +
                    "faculty = '" + cmbFaculty_Edit.Text + "' , " +
                    "department = '" + cmbDepartment_Edit.Text + "' , " +
                    "center = '" + cmbCenter_Edit.Text + "' , " +
                    "building = '" + cmbBuilding_Edit.Text + "' , " +
                    "employeeLevel = '" + tempEmpLevel + "'  " +
                    "WHERE lecturerID = '" + empID + "' ";
                    cmd.ExecuteNonQuery();

                }

                con.Close();
                MessageBox.Show("Update success");
                cleanLecturerEditTab();
            }

        }

        private void btnBrowse_Edit_Click(object sender, EventArgs e)
        {
            chooseImageButtonTouched =true;


            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                lblSelectedImg_Edit.Text = open.SafeFileName;
                gblSafeFileName_Edit = open.SafeFileName;
                pictureBoxLecturer_Edit.Image = new Bitmap(open.FileName);
                pictureBoxLecturer_Edit.SizeMode = PictureBoxSizeMode.StretchImage;
                newImagePathForUpdate = open.FileName;
                setImage = true;
            }
        }

        private void cmbEmpLevel_Edit_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmbtxt = this.cmbEmpLevel_Edit.GetItemText(this.cmbEmpLevel_Edit.SelectedItem);

            int temp=0;
            if (cmbtxt.Equals("1 - Professor")){
                temp = 1;
            }else if (cmbtxt.Equals("2 - Assistant Professor")){
                temp = 2;
            }else if (cmbtxt.Equals("3 - Senior Lecturer(HG)")){
                temp = 3;
            }else if (cmbtxt.Equals("4 - Senior Lecturer")){
                temp = 4;
            } else if (cmbtxt.Equals("5 - Lecturer")) {
                temp = 5;
            } else if (cmbtxt.Equals("6 - Assistnt Lecturer")){
                temp = 6;
            } else if (cmbtxt.Equals("7 - Instructors")) {
                temp = 7;
            }

            txtEmpRank_Edit.Text = temp + "." + txtEmpID_Edit.Text;
            
             
        }

        private void cmbCenter_Edit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //center index changed
            fillBuildingCmb_Edit_New(this.cmbCenter_Edit.GetItemText(this.cmbCenter_Edit.SelectedItem));
            fillFacultyCmb_Edit_New(this.cmbCenter_Edit.GetItemText(this.cmbCenter_Edit.SelectedItem));
            fillDepartmentCmb_Edit_New(this.cmbCenter_Edit.GetItemText(this.cmbCenter_Edit.SelectedItem), this.cmbFaculty_Edit.GetItemText(this.cmbFaculty_Edit.SelectedItem));

        }

        private void cmbFaculty_Edit_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillDepartmentCmb_Edit_New(this.cmbCenter_Edit.GetItemText(this.cmbCenter_Edit.SelectedItem), this.cmbFaculty_Edit.GetItemText(this.cmbFaculty_Edit.SelectedItem));
        }

        private void fillDepartmentCmb_Edit_New(string selectedCenter, string selectedFaculty)
        {
            cmbDepartment_Edit.Items.Clear();

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();
            string stm = "";

            stm = "SELECT department FROM center_faculty_department WHERE centerName = '" + selectedCenter + "' AND faculty='" + selectedFaculty + "'";

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                cmbDepartment_Edit.Items.Add("" + $@"{ rdr.GetString(0),-8}".Trim());
            }
            cmbDepartment_Edit.SelectedIndex = 0;

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab == ViewSearchLecturers)
            {
                refreshLecturersSearch();

                //Reset code
                refreshLecturersSearch();
                resetLecturerSigleView();
                txtSearch.Text = "";
                radFindByIdSearch.Checked = true;
            }
        }

        private void imgLoggedUser_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login obj = new Login();
            obj.Show();
        }
    }
}
