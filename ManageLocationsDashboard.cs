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
using Timetable_Management_System.src;

namespace Timetable_Management_System
{
    public partial class ManageLocationsDashboard : Form
    {
        private SQLiteConnection sql_conn;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dbAdapter;

        private DataSet buildingDataSet = new DataSet();
        private DataTable buildingsTable = new DataTable();

        private DataSet roomDataSet = new DataSet();
        private DataTable roomsTable = new DataTable();

        private DataSet suitableRoomTypeTagsDataSet = new DataSet();
        private DataTable suitableRoomTypeTagsTable = new DataTable();

        public class RoomType
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public ManageLocationsDashboard()
        {
            InitializeComponent();
            tabControl1.DrawItem += new DrawItemEventHandler(tabControl1_DrawItem);
            this.WindowState = FormWindowState.Maximized;
            
        }

        private void ManageLocationsDashboard_Load(object sender, EventArgs e)
        {

            createBuildingTableIfEmpty();
            createRoomsTableIfEmpty();
            createSuitableRoomTypesForTagsTableIfEmpty();

            loadBuildingsData();
            loadRoomData();
            loadBuildingsToComboBox();
            loadRoomTypesToComboBox();
            loadRoomTypeTagsData();

            // Suitable Rooms for tags
            loadTagsToCombo();

            //nav4
            loadSubjectsToComboBox();
            loadRoomsToComboBox();
            loadSubjectsToComboBox();
            
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Buildings_Click(object sender, EventArgs e)
        {

        }

        // Create "buildings" table if not exists
        private void createBuildingTableIfEmpty()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            SQLiteCommand cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS buildings (
                                buildingID INTEGER PRIMARY KEY AUTOINCREMENT,
                                name TEXT
                )";
            cmd.ExecuteNonQuery();
        }

        // Create "rooms" table if not exists
        private void createRoomsTableIfEmpty()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS rooms (
                                roomID INTEGER PRIMARY KEY,
                                name TEXT,
                                buildingID INTEGER,
                                capacity INTEGER,
                                type TEXT,

                                FOREIGN KEY(buildingID) REFERENCES buildings(buildingID)
                )";
            cmd.ExecuteNonQuery();
        }

        private void createSuitableRoomTypesForTagsTableIfEmpty()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS suitable_roomtype_tags (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                roomType TEXT,
                                tagId INTEGER,
                                FOREIGN KEY(tagId) REFERENCES tags(id)
                )";
            cmd.ExecuteNonQuery();
        }
        // Create SQLite DB Conn
        private void setConnection()
        {
            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            sql_conn = new SQLiteConnection(cs);
            //sql_conn.Open();
        }

        private void executeQuery(string txtQuery)
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_conn.Close();
        }

        private void loadRoomData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from rooms";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            roomDataSet.Reset();
            dbAdapter.Fill(roomDataSet);
            roomsTable = roomDataSet.Tables[0];
            roomsGrid.DataSource = roomsTable;
            sql_conn.Close();
        }


        private void loadRoomTypeTagsData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select srt.roomType, t.tag from suitable_roomtype_tags srt, tags t WHERE srt.tagId = t.id";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            suitableRoomTypeTagsDataSet.Reset();
            dbAdapter.Fill(suitableRoomTypeTagsDataSet);
            suitableRoomTypeTagsTable = suitableRoomTypeTagsDataSet.Tables[0];
            suitableRoomTypeTagsGrid.DataSource = suitableRoomTypeTagsTable;
            sql_conn.Close();
        }
        private void loadBuildingsData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from buildings";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            buildingDataSet.Reset();
            dbAdapter.Fill(buildingDataSet);
            buildingsTable = buildingDataSet.Tables[0];
            buildingsGrid.DataSource = buildingsTable;
            sql_conn.Close();
        }

        private void addBuildingBtn_Click(object sender, EventArgs e)
        {
            if(buildingNameVal.TextLength > 0)
            {
                // Insert Building
                string insertQ = "insert into buildings (name) VALUES('"+buildingNameVal.Text+"')";
                executeQuery(insertQ);
                loadBuildingsData();
            }
            else
            {
                MessageBox.Show("Building Name cannot be empty !");
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lblBuildings_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
               if (editBuildingIdVal.TextLength > 0)
            {
                // Delete Building
                
                string delQ = "delete from buildings where buildingID = '" + editBuildingIdVal.Text + "'";
                executeQuery(delQ);
                loadBuildingsData();
            }
        }

        private void buildingsGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            editBuildingNameVal.Text = buildingsGrid.SelectedRows[0].Cells[1].Value.ToString();
            editBuildingIdVal.Text = buildingsGrid.SelectedRows[0].Cells[0].Value.ToString() ;
        }

        private void buildingEditBtn_Click(object sender, EventArgs e)
        {
            if(editBuildingNameVal.TextLength > 0)
            {
                // Update Building
                string updateQ = "update buildings set name = '" + editBuildingNameVal.Text + "' where buildingID = '" + editBuildingIdVal.Text + "'";
                executeQuery(updateQ);
                loadBuildingsData();
            }
         
        }

        private void label2_Click(object sender, EventArgs e)
        {
         
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void addRoomBtn_Click(object sender, EventArgs e)
        {
            if (roomNameVal.TextLength > 0
                &&
                roomCapacityVal.Value > 0
                )
            {
                // Insert Room
                string insertQ = "insert into rooms (name, buildingID, capacity, type ) VALUES('" + roomNameVal.Text + "', "+comboBuildings.SelectedValue+", "+ roomCapacityVal.Value+ ", '"+ comboRoomTypes.Text  +"')";
                executeQuery(insertQ);
                loadRoomData();
            }
            else
            {
                MessageBox.Show("Room Form has some errors !");
            }
        }

        private void Rooms_Click(object sender, EventArgs e)
        {

        }


        private void loadBuildingsToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from buildings";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);

            //Fill the DataTable with records from Table.
            DataSet dt = new DataSet();
            dbAdapter.Fill(dt,"Fleet");

            //Insert the Default Item to DataTable.
            /*DataRow row = dt.NewRow();
            row[0] = 0;
            row[1] = "Please select";
            dt.Rows.InsertAt(row, 0);
            */

            comboBuildings.DisplayMember = "name";
            comboBuildings.ValueMember = "buildingID";
            comboBuildings.DataSource = dt.Tables["Fleet"];

            editComboBuildings.DisplayMember = "name";
            editComboBuildings.ValueMember = "buildingID";
            editComboBuildings.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadRoomTypesToComboBox()
        {

            //Build a RoomTypes list
            var dataSource = new List<RoomType>();
            dataSource.Add(new RoomType() { Name = "Lecture Hall", Value = "1" });
            dataSource.Add(new RoomType() { Name = "Laboratory", Value = "2" });
            //dataSource.Add(new RoomType() { Name = "blah", Value = "blah" });

            //Setup data binding
            comboRoomTypes.DisplayMember = "Name";
            comboRoomTypes.ValueMember = "Value";
            comboRoomTypes.DataSource = dataSource;

            //Setup data binding
            editComboRoomType.DisplayMember = "Name";
            editComboRoomType.ValueMember = "Value";
            editComboRoomType.DataSource = dataSource;


            nav3comboRoomTypes.DisplayMember = "Name";
            nav3comboRoomTypes.ValueMember = "Value";
            nav3comboRoomTypes.DataSource = dataSource;
        }



        private void loadTagsToCombo()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from tags";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);

            //Fill the DataTable with records from Table.
            DataSet dt = new DataSet();
            dbAdapter.Fill(dt, "Fleet");

            //Insert the Default Item to DataTable.
            /*DataRow row = dt.NewRow();
            row[0] = 0;
            row[1] = "Please select";
            dt.Rows.InsertAt(row, 0);
            */

            comboBoxTags.DisplayMember = "tag";
            comboBoxTags.ValueMember = "id";
            comboBoxTags.DataSource = dt.Tables["Fleet"];

            nav4TagsComboBox.DisplayMember = "tag";
            nav4TagsComboBox.ValueMember = "id";
            nav4TagsComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadRoomsToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select roomID, name from rooms";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);

            //Fill the DataTable with records from Table.
            DataSet dt = new DataSet();
            dbAdapter.Fill(dt, "Fleet");

            //Insert the Default Item to DataTable.
            /*DataRow row = dt.NewRow();
            row[0] = 0;
            row[1] = "Please select";
            dt.Rows.InsertAt(row, 0);
            */

            nav4RoomsComboBox.DisplayMember = "name";
            nav4RoomsComboBox.ValueMember = "roomID";
            nav4RoomsComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadSubjectsToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select subjectCode, subjectName from subjects";
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);

            //Fill the DataTable with records from Table.
            DataSet dt = new DataSet();
            dbAdapter.Fill(dt, "Fleet");

            //Insert the Default Item to DataTable.
            /*DataRow row = dt.NewRow();
            row[0] = 0;
            row[1] = "Please select";
            dt.Rows.InsertAt(row, 0);
            */

            nav4SubjectsComboBox.DisplayMember = "subjectName";
            nav4SubjectsComboBox.ValueMember = "subjectCode";
            nav4SubjectsComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void roomsGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            editRoomId.Text = roomsGrid.SelectedRows[0].Cells[0].Value.ToString();
            editRoomNameVal.Text = roomsGrid.SelectedRows[0].Cells[1].Value.ToString();
            editComboBuildings.SelectedValue = roomsGrid.SelectedRows[0].Cells[2].Value.ToString();

            editRoomCapacityVal.Value = Int32.Parse( roomsGrid.SelectedRows[0].Cells[3].Value.ToString() );
            editComboRoomType.Text = roomsGrid.SelectedRows[0].Cells[4].Value.ToString();
        }

        private void deleteRoomBtn_Click(object sender, EventArgs e)
        {
            if (editRoomId.TextLength > 0)
            {
                // Delete Building

                string delQ = "delete from rooms where roomID = '" + editRoomId.Text + "'";
                executeQuery(delQ);
                loadRoomData();
            }
        }

        private void updateRoomBtn_Click(object sender, EventArgs e)
        {
            if (editRoomId.TextLength > 0)
            {
                // Update Building
                string updateQ = "update rooms set name = '" + editRoomNameVal.Text + "', buildingID = "+editComboBuildings.SelectedValue+", capacity = "+editRoomCapacityVal.Value+" , type = '"+ editComboRoomType.Text +"' where roomID = " + editRoomId.Text  ;

                executeQuery(updateQ);
                loadRoomData();
            }
        }

        private void imgLoggedUser_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login obj = new Login();
            obj.Show();
        }

        private void lblRoomsForTags_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void RoomsForTags_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(true)
            {
                // Insert Room
                string insertQ = "insert into suitable_roomtype_tags (roomType, tagId ) VALUES('" + nav3comboRoomTypes.Text + "', '" + comboBoxTags.SelectedValue + "')";
                executeQuery(insertQ);
                loadRoomTypeTagsData();
            }
            else
            {
                MessageBox.Show(" Form has some errors !");
            }
        }

        private void lblPrefferedRoomForASubject_Click(object sender, EventArgs e)
        {

        }

        private void PrefferedRoomForASubject_Click(object sender, EventArgs e)
        {

        }
    }
}
