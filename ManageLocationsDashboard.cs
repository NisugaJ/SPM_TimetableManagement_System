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

        private DataSet suitableRoomSubjectTagsDataSet = new DataSet();
        private DataTable suitableRoomSubjectTagsTable = new DataTable();

        private DataSet suitableRoomsForLecturersDataSet = new DataSet();
        private DataTable suitableRoomsForLecturersTable = new DataTable();

        private DataSet suitableRoomsForSessionsDataSet = new DataSet();
        private DataTable suitableRoomsForSessionsTable = new DataTable();

        private DataSet suitableRoomTypesFoeConsecutiveSessionsDataSet = new DataSet();
        private DataTable suitableRoomTypesFoeConsecutiveSessionsTable = new DataTable();

        private DataSet suitableRoomUnavailbleSlotsDataSet = new DataSet();
        private DataTable suitableRoomUnavailbleSlotsTable = new DataTable();
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
            createSuitableRoomSubjectsTagsTableIfEmpty();
            createSuitableRoomsForLecturersTableIfEmpty();
            createSuitableRoomsForSessionsTableIfEmpty();
            createSuitableRoomsForConsecutiveSessionsTableIfEmpty();
            createUnavailableTimeSlotsOfRoomsTableIfEmpty();

            loadBuildingsData();
            loadRoomData();
            loadBuildingsToComboBox();
            loadRoomTypesToComboBox();
            loadRoomTypeTagsData();
            loadSuitableRoomSubjectsTagsData();
            loadSuitableRoomsForLecturersData();
            loadSuitableRoomsForSessiosnsData();
            loadRoomUnavailableSlotsData();

            // Suitable Rooms for tags
            loadTagsToCombo();

            //nav4
            loadSubjectsToComboBox();
            loadRoomsToComboBox();
            loadSubjectsToComboBox();

            //nav5
            loadLecturersToComboBox();

            //nav6
            loadSessionsToComboBox();

            //nav7
            loadRoomTypesForConsecutiveSessiosnsData();


            //nav8
            loadTimeSlotsToComboBox();
            loadDaysToComboBox();
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

        private void createSuitableRoomSubjectsTagsTableIfEmpty()
        {
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS suitable_rooms_for_subject_tags (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                subjectCode STRING,
                                roomId INTEGER,
                                tagId INTEGER,
                                FOREIGN KEY(subjectCode) REFERENCES subjects(subjectCode),
                                FOREIGN KEY(roomId) REFERENCES rooms(roomID),
                                FOREIGN KEY(tagId) REFERENCES tags(id)
                )";
                cmd.ExecuteNonQuery();
            }
        }

        private void createSuitableRoomsForLecturersTableIfEmpty()
        {
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS suitable_rooms_for_lecturers (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                lecturerId INTEGER,
                                roomId INTEGER,
                                FOREIGN KEY(lecturerId) REFERENCES lecturers(lecturerID),
                                FOREIGN KEY(roomId) REFERENCES rooms(roomID)
                )";
                cmd.ExecuteNonQuery();
            }
        }

        private void createSuitableRoomsForSessionsTableIfEmpty()
        {
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS suitable_rooms_for_sessions (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                sessionId INTEGER,
                                roomId INTEGER,
                                FOREIGN KEY(sessionId) REFERENCES session(sessionId),
                                FOREIGN KEY(roomId) REFERENCES rooms(roomID)
                )";
                cmd.ExecuteNonQuery();
            }
        }

        private void createSuitableRoomsForConsecutiveSessionsTableIfEmpty()
        {
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS room_types_for_consecutive_sessions (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                tag1Id INTEGER,
                                tag2Id INTEGER,
                                roomType TEXT,
                                FOREIGN KEY(tag1Id) REFERENCES tags(id),
                                FOREIGN KEY(tag2Id) REFERENCES tags(id)
                )";
                cmd.ExecuteNonQuery();
            }
        }

        private void createUnavailableTimeSlotsOfRoomsTableIfEmpty()
        {
            {
                string cs = @"URI=file:.\" + Utils.dbName + ".db";

                using var con = new SQLiteConnection(cs);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                cmd.CommandText = @"CREATE TABLE  IF NOT EXISTS rooms_unavailable_slots (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                roomId INTEGER,
                                dayId INTEGER,
                                slotId INTEGER,
                                FOREIGN KEY(roomId) REFERENCES rooms(roomID),
                                FOREIGN KEY(dayId) REFERENCES Days(dayID),
                                FOREIGN KEY(slotId) REFERENCES TimeSlots(slotID)
                )";
                cmd.ExecuteNonQuery();
            }
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

        private void loadSuitableRoomSubjectsTagsData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = 
                "select s.subjectName, r.name, t.tag " +
                "from suitable_rooms_for_subject_tags srt, tags t, rooms r, subjects s " +
                "WHERE srt.tagId = t.id AND r.roomID = srt.roomId AND s.subjectCode = srt.subjectCode";
             
            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            suitableRoomSubjectTagsDataSet.Reset();
            dbAdapter.Fill(suitableRoomSubjectTagsDataSet);
            suitableRoomSubjectTagsTable = suitableRoomSubjectTagsDataSet.Tables[0];
            SuitableRoomSubjectsTagsTableGrid.DataSource = suitableRoomSubjectTagsTable;
            sql_conn.Close();

            
        }

        private void loadSuitableRoomsForLecturersData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText =
                "select l.name, r.name AS RoomName " +
                "from suitable_rooms_for_lecturers srl, rooms r, lecturers l " +
                "WHERE srl.roomId = r.roomID AND l.lecturerID = srl.lecturerId ";

            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            suitableRoomsForLecturersDataSet.Reset();
            dbAdapter.Fill(suitableRoomsForLecturersDataSet);
            suitableRoomsForLecturersTable = suitableRoomsForLecturersDataSet.Tables[0];
            suitableRoomsForLecturerTableGrid.DataSource = suitableRoomsForLecturersTable;
            sql_conn.Close();


        }


        private void loadSuitableRoomsForSessiosnsData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText =
                "select s.sessionId AS SessionID, s.groupId AS GroupID, r.name " +
                "from suitable_rooms_for_sessions srs, rooms r, session s " +
                "WHERE srs.roomId = r.roomID AND srs.sessionId = s.sessionId";

            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            suitableRoomsForSessionsDataSet.Reset();
            dbAdapter.Fill(suitableRoomsForSessionsDataSet);
            suitableRoomsForSessionsTable = suitableRoomsForSessionsDataSet.Tables[0];
            preferredRoomsForSessionsGrid.DataSource = suitableRoomsForSessionsTable;
            sql_conn.Close();


        }

        private void loadRoomTypesForConsecutiveSessiosnsData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText =
                 "select  t1.tag AS Session_1_Tag, t2.tag AS Session_2_Tag, srt.roomType from room_types_for_consecutive_sessions srt, tags t1, tags t2 WHERE srt.tag1Id = t1.id AND srt.tag2Id = t2.id ";

            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            suitableRoomTypesFoeConsecutiveSessionsDataSet.Reset();
            dbAdapter.Fill(suitableRoomTypesFoeConsecutiveSessionsDataSet);
            suitableRoomTypesFoeConsecutiveSessionsTable = suitableRoomTypesFoeConsecutiveSessionsDataSet.Tables[0];
            consecutiveSessionsGrid.DataSource = suitableRoomTypesFoeConsecutiveSessionsTable;
            sql_conn.Close();


        }

        private void loadRoomUnavailableSlotsData()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText =
                 "select  srt.id AS ID, r.name AS Room_name, t.fullTime AS TimeSlot , d.day AS DAY from rooms_unavailable_slots srt, TimeSlots t, rooms r, Days d WHERE srt.slotId = t.slotID AND srt.roomId = r.roomID AND srt.dayId = d.dayID";

            dbAdapter = new SQLiteDataAdapter(commandText, sql_conn);
            suitableRoomUnavailbleSlotsDataSet.Reset();
            dbAdapter.Fill(suitableRoomUnavailbleSlotsDataSet);
            suitableRoomUnavailbleSlotsTable = suitableRoomUnavailbleSlotsDataSet.Tables[0];
            unavailableTimeSlotsOfRoomsGrid.DataSource = suitableRoomUnavailbleSlotsTable;
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

            nav7roomTypeComboBox.DisplayMember = "Name";
            nav7roomTypeComboBox.ValueMember = "Value";
            nav7roomTypeComboBox.DataSource = dataSource;
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


            nav7TagsComboBox.DisplayMember = "tag";
            nav7TagsComboBox.ValueMember = "id";
            nav7TagsComboBox.DataSource = dt.Tables["Fleet"];

            DataSet dt2 = new DataSet();
            dbAdapter.Fill(dt2, "Fleet2");

            nav7TagsComboBox2.DisplayMember = "tag";
            nav7TagsComboBox2.ValueMember = "id";
            nav7TagsComboBox2.DataSource = dt2.Tables["Fleet2"];

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

            nav5RoomsComboBox.DisplayMember = "name";
            nav5RoomsComboBox.ValueMember = "roomID";
            nav5RoomsComboBox.DataSource = dt.Tables["Fleet"];


            nav6RoomsComboBox.DisplayMember = "name";
            nav6RoomsComboBox.ValueMember = "roomID";
            nav6RoomsComboBox.DataSource = dt.Tables["Fleet"];

            nav8RoomsComboBox.DisplayMember = "name";
            nav8RoomsComboBox.ValueMember = "roomID";
            nav8RoomsComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadLecturersToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from lecturers";
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

            nav5LecturersComboBox.DisplayMember = "name";
            nav5LecturersComboBox.ValueMember = "lecturerID";
            nav5LecturersComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadTimeSlotsToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from TimeSlots";
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

            nav8TimeSlotsComboBox.DisplayMember = "fullTime";
            nav8TimeSlotsComboBox.ValueMember = "slotID";
            nav8TimeSlotsComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadDaysToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from Days";
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

            nav8DaysComboBox.DisplayMember = "day";
            nav8DaysComboBox.ValueMember = "dayID";
            nav8DaysComboBox.DataSource = dt.Tables["Fleet"];

            sql_conn.Close();
        }

        private void loadSessionsToComboBox()
        {
            setConnection();
            sql_conn.Open();
            sql_cmd = sql_conn.CreateCommand();
            string commandText = "select * from session";
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

            nav6SesssionsComboBox.DisplayMember = "sessionId";
            nav6SesssionsComboBox.ValueMember = "sessionId";
            nav6SesssionsComboBox.DataSource = dt.Tables["Fleet"];

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

        private void button2_Click_2(object sender, EventArgs e)
        {
            if (true)
            {
                // Insert Room
                string insertQ = "insert into suitable_rooms_for_subject_tags (subjectCode, roomId, tagId   ) VALUES('" + nav4SubjectsComboBox.SelectedValue + "','"+nav4RoomsComboBox.SelectedValue+"', '" + nav4TagsComboBox.SelectedValue + "')";
                executeQuery(insertQ);
                loadSuitableRoomSubjectsTagsData();
            }
            else
            {
                MessageBox.Show(" Form has some errors !");
            }
        }

        private void dataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (true)
            {
                // Insert Room
                string insertQ = "insert into suitable_rooms_for_lecturers (lecturerId, roomId   ) VALUES('" + nav5LecturersComboBox.SelectedValue + "','" + nav5RoomsComboBox.SelectedValue +  "')";
                executeQuery(insertQ);
                loadSuitableRoomsForLecturersData();
            }
            else
            {
                MessageBox.Show(" Form has some errors !");
            }
        }

        private void PrefferedRoomForALecturer_Click(object sender, EventArgs e)
        {

        }

        private void PrefferedRoomForASession_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (true)
            {
                // Insert Room
                string insertQ = "insert into suitable_rooms_for_sessions (sessionId, roomId   ) VALUES('" + nav6SesssionsComboBox.SelectedValue + "','" + nav6RoomsComboBox.SelectedValue + "')";
                executeQuery(insertQ);
                loadSuitableRoomsForSessiosnsData();
            }
            else
            {
                MessageBox.Show(" Form has some errors !");
            }
        }

        private void ConsecutiveSessions_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (true)
            {
                // Insert Room
                string insertQ = "insert into room_types_for_consecutive_sessions (tag1Id, tag2Id, roomType  ) VALUES('" + nav7TagsComboBox.SelectedValue + "','" + nav7TagsComboBox2.SelectedValue  +"','"+nav7roomTypeComboBox.Text + "')";
                executeQuery(insertQ);
                loadRoomTypesForConsecutiveSessiosnsData();
            }
            else
            {
                MessageBox.Show(" Form has some errors !");
            }
        }

        private void ManageRoomUnavailability_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (true)
            {
                // Insert Room
                string insertQ = "insert into rooms_unavailable_slots (roomId, slotId, dayId  ) VALUES('" + nav8RoomsComboBox.SelectedValue + "','" + nav8TimeSlotsComboBox.SelectedValue +"','" + nav8DaysComboBox.SelectedValue + "')";
                executeQuery(insertQ);
                loadRoomUnavailableSlotsData();
            }
            else
            {
                MessageBox.Show(" Form has some errors !");
            }
        }

        private void nav8RoomsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void nav8DaysComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void nav8TimeSlotsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }
    }
}
