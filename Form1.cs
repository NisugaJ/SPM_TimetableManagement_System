using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timetable_Management_System.src;
using System.Data.SQLite;
using System.IO;

namespace Timetable_Management_System
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            logoPictureBox.BackColor = Color.Transparent;
            lblPassword.BackColor = Color.Transparent;
            txtPassword.PasswordChar = '*';


        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
			if(txtUsername.Text.Equals("admin") && txtPassword.Text.Equals("admin"))
           {
				this.Hide();
				ManageStudentsDashboard obj = new ManageStudentsDashboard();
				obj.Show();
			}
            else
            {
				MessageBox.Show("Incorrent Username or password");
            }

        }

        private void Login_Load(object sender, EventArgs e)
        {
            createDBIfNotExist();
        }

        private void createDBIfNotExist()
        {

            string cs = @"URI=file:.\" + Utils.dbName + ".db";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = @"
CREATE TABLE  IF NOT EXISTS center (

	centerName STRING,

	PRIMARY KEY (centerName)
);

CREATE TABLE  IF NOT EXISTS center_building (

	centerName STRING,
	building STRING,

	PRIMARY KEY (centerName, building),
	FOREIGN KEY(centerName) REFERENCES center(centerName)
);

CREATE TABLE  IF NOT EXISTS center_faculty (

	centerName STRING,
	faculty STRING,

	PRIMARY KEY (centerName, faculty),
	FOREIGN KEY(centerName) REFERENCES center(centerName)
);

CREATE TABLE  IF NOT EXISTS center_faculty_department (

	centerName STRING,
	faculty STRING,
	department STRING,

	PRIMARY KEY (centerName, faculty,department),
	FOREIGN KEY(centerName) REFERENCES center_faculty(centerName),
	FOREIGN KEY(centerName) REFERENCES center_faculty(faculty)
);

";
            cmd.ExecuteNonQuery();


            using var cmd1 = new SQLiteCommand(con);

            cmd1.CommandText = @"

INSERT OR IGNORE INTO center
VALUES('Malabe'),('Metro'),('Matara'),('Kandy'),('Kurunegala');

INSERT OR IGNORE INTO center_building
VALUES	('Malabe','Main Building'),('Malabe','New Building'),('Malabe','D building'),
		('Metro','Boc Tower'),('Metro','Old building'),('Metro','A building'),
		('Matara','A building'),('Matara','B building'),('Matara','C building'),
		('Kandy','P building'),('Kandy','Q building'),('Kandy','R building'),
		('Kurunegala','1 building'),('Kurunegala','2 building'),('Kurunegala','3 building');


INSERT OR IGNORE INTO center_faculty
VALUES	('Malabe','Faculty of Computing'),('Malabe','Faculty of Engineering'),('Malabe','Faculty of Business'),('Malabe','Faculty of Humanities & Sciences'),
		('Metro','Faculty of Computing'),('Metro','Faculty of Engineering'),
		('Matara','Faculty of Computing'),('Matara','Faculty of Business'),
		('Kandy','Faculty of Computing'),('Kandy','Faculty of Business'),
		('Kurunegala','Faculty of Computing'),('Kurunegala','Faculty of Engineering');
		
		
INSERT OR IGNORE INTO center_faculty_department
VALUES	('Malabe','Faculty of Computing','SE Department'),
		('Malabe','Faculty of Computing','IT Department'),
		('Malabe','Faculty of Computing','CSSE Department'),

		('Malabe','Faculty of Engineering','Electrical Department'),
		('Malabe','Faculty of Engineering','Civil Department'),
		('Malabe','Faculty of Engineering','Mechanical Department'),
						
		('Malabe','Faculty of Business','Production Department'),
		('Malabe','Faculty of Business','Accounting Department'),
		('Malabe','Faculty of Business','Finance Department'),
						
		('Malabe','Faculty of Humanities & Sciences','Philosophy Department'),
		('Malabe','Faculty of Humanities & Sciences','Theology Department'),
		('Malabe','Faculty of Humanities & Sciences','Anthropology Department'),


		
		
		('Metro','Faculty of Computing','SE Department'),
		('Metro','Faculty of Computing','IT Department'),
		('Metro','Faculty of Computing','Networking Department'),
		
		('Metro','Faculty of Engineering','Electrical Department'),
		('Metro','Faculty of Engineering','Civil Department'),
		('Metro','Faculty of Engineering','Chemical Department'),
		
		
		
		
		('Matara','Faculty of Computing','SE Department'),
		('Matara','Faculty of Computing','IT Department'),
		('Matara','Faculty of Computing','CCNE Department'),
		
		('Matara','Faculty of Business','Philosophy Department'),
		('Matara','Faculty of Business','Theology Department'),
		('Matara','Faculty of Business','Anthropology Department'),
		
		
		
		
		('Kandy','Faculty of Computing','SE Department'),
		('Kandy','Faculty of Computing','IT Department'),
		('Kandy','Faculty of Computing','Cyber Security Department'),
		
		('Kandy','Faculty of Business','Production Department'),
		('Kandy','Faculty of Business','Marketing Department'),
		('Kandy','Faculty of Business','Finance Department'),
		
		
		
		
		('Kurunegala','Faculty of Computing','SE Department'),
		('Kurunegala','Faculty of Computing','IT Department'),
		('Kurunegala','Faculty of Computing','Data Science Department'),
		
		('Kurunegala','Faculty of Engineering','Production Department'),
		('Kurunegala','Faculty of Engineering','Marketing Department'),
		('Kurunegala','Faculty of Engineering','Management Department');
";
            cmd1.ExecuteNonQuery();







			using var cmd2 = new SQLiteCommand(con);

			cmd2.CommandText = @"
	CREATE TABLE  IF NOT EXISTS Time_table1(
								tableID INTEGER PRIMARY KEY AUTOINCREMENT,
								tableType TEXT,
								noDays INTEGER,
								days TEXT,
								workingTime TEXT,
								startTime TEXT,
								EndTime TEXT
				)
";
			cmd2.ExecuteNonQuery();


			//ravindu insert
			using var cmd3 = new SQLiteCommand(con);

			cmd3.CommandText = @"
	CREATE TABLE  IF NOT EXISTS Days(
								dayID INTEGER PRIMARY KEY,
								day TEXT
				)
";

			cmd3.ExecuteNonQuery();

			using var cmd4 = new SQLiteCommand(con);

			cmd4.CommandText = @"

INSERT OR IGNORE INTO Days
VALUES ('1','Monday'),('2','Tuesday'),('3','Wednesday'),('4','Thursday'),
		('5','Froday'),('6','Saturday'),('7','Sunday');
";

			cmd4.ExecuteNonQuery();



			cmd3.CommandText = @"
				CREATE TABLE  IF NOT EXISTS testSessions(
								testID INTEGER PRIMARY KEY,
								sessionId INTEGER,
								lecID INTEGER,
								groupName TEXT,
								groupId INTEGER,
								roomName TEXT
				)";

			cmd3.ExecuteNonQuery();

			cmd3.CommandText = @"
				CREATE TABLE  IF NOT EXISTS testLec(
								lecId INTEGER PRIMARY KEY ,
								lecName TEXT,
								lecNo INTEGER
				)";

			cmd3.ExecuteNonQuery();


			cmd4.CommandText = @"

			INSERT OR IGNORE INTO testSessions(testID,sessionId,lecID,groupName,groupId,roomName) VALUES (1,5,6,'Group1',1,'A502'),(2,10,6,'Group3',3,'A506'),
			(3,7,6,'Group3',3,'A502'),(4,9,3,'Group2',2,'S111')
			,(5,15, 3, 'Group1', 1, 'S111'),(6,16, 13, 'Group4', 4, 'S112'),(7,32, 22, 'Group4', 4, 'A506'),(8,25, 22, 'Group2', 2, 'S112');
";

			cmd4.ExecuteNonQuery();

			cmd4.CommandText = @"

			INSERT OR IGNORE INTO testLec(lecId,lecName,lecNo) VALUES(1,'rav',6),(2,'amanda',3),(3,'mithabani',13),(4,'chamod',22);
";

			cmd4.ExecuteNonQuery();

			con.Close();

		}
    }
}
