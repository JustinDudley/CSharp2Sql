using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CSharp2SqlLibrary {

    public class Users {

        // STATIC VARIABLE
        public static Connection Connection { get; set; }  // static, because we need just one for the class
        
        // PROPERTIES
        public int Id { get; private set; } // we need to protect the id from being changed by the user
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }   //        public bool? IsAdmin { get; set; }   ? indicates "nullable"
        public bool IsReviewer { get; set; }


        private static void SetParameterValues(Users myUser, SqlCommand sqlcmd) {
            sqlcmd.Parameters.AddWithValue("@Username", myUser.Username);
            sqlcmd.Parameters.AddWithValue("@Password", myUser.Password);
            sqlcmd.Parameters.AddWithValue("@Firstname", myUser.Firstname);
            sqlcmd.Parameters.AddWithValue("@Lastname", myUser.Lastname);
            sqlcmd.Parameters.AddWithValue("@Phone", (object)myUser.Phone ?? DBNull.Value); //C# null and SQL null are not the same
            sqlcmd.Parameters.AddWithValue("@Email", (object)myUser.Email ?? DBNull.Value); // C# null and SQL null are not the same
            sqlcmd.Parameters.AddWithValue("@IsAdmin", myUser.IsAdmin);
            sqlcmd.Parameters.AddWithValue("@IsReviewer", myUser.IsReviewer);
        }


        // UPDATE
        public static bool Update (Users myUser) {
            var sqlText = "UPDATE Users Set " +
            " Username = @Username, " +
            " Password = @Password, " +
            " FirstName = @Firstname, " +
            " Lastname = @Lastname, " +
            " Phone = @Phone, " +
            " Email = @Email, " +
            " IsAdmin = @IsAdmin, " +
            " IsReviewer = @IsReviewer " +
            " Where Id = @Id";
            var sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);  
            SetParameterValues(myUser, sqlcmd);
            sqlcmd.Parameters.AddWithValue("@Id", myUser.Id);
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
        }


        // INSERT
        public static bool Insert(Users myUser) {     //this time, we're passing an ENTIRE INSTANCE into the method.
            string sqlText = "INSERT into Users " +
                "(Username, Password, FirstName, Lastname, Phone, Email, IsAdmin, IsReviewer) " +
                " VALUES " +
                "(@Username, @Password, @Firstname, @Lastname, @Phone, @Email, @IsAdmin, @IsReviewer)";
            var sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);  // same as other one
            SetParameterValues(myUser, sqlcmd);
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
        }


        // DELETE    --safest to delete by ID
        public static bool Delete(int id) {
            var sqlText = "DELETE from Users WHERE Id = @Id;";
            var sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Id", id);
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);     //this is either true of false.  We want it to be true, because we wanted to take out one row.
            // if rowsAffected > 1, something really went wrong!  (If == 0, means couldn't find that Id)
        }


        // LOGIN
        public static Users Login(string username, string password) {
            var sqlText = "select * from Users where username = @username and password = @password";
            var sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Username", username);
            sqlcmd.Parameters.AddWithValue("@Password", password);

            var myReader = sqlcmd.ExecuteReader(); //expensive resource, so need to close it
            if(!myReader.HasRows) {
                myReader.Close();
                return null;
            }
            myReader.Read();
            var myUser = new Users();
            LoadUserFromSql(myUser, myReader);
            myReader.Close();
            return myUser;
        }


                // helper method , called by methods in THIS class
        private static void LoadUserFromSql(Users myUser, SqlDataReader myReader) {     //load up Users object with fields from Db:
            myUser.Id = (int)myReader["Id"]; // casts this fundamental object type (base class) to int type 
            myUser.Username = myReader["Username"].ToString();
            myUser.Password = (string)myReader["Password"];  // Eexist two ways to convert strings, Cast vs ToString. This is the OTHER way.
            myUser.Firstname = (string)myReader["Firstname"]; // In the other classes, we abandon this casting technique in favor of .ToString()
            myUser.Lastname = (string)myReader["Lastname"];
            myUser.Phone = myReader["Phone"]?.ToString();       //handling null phone numbers. Turns out we can't cast with (string) --see failed attempt two lines below
            myUser.Email = myReader["Email"]?.ToString(); // if comes back okay, just set it to ??string. If null, set to null. ? is a new syntax:  "If it's not null, call this method"
                                                      //myUser.Phone = (string)myReader["Phone"]; 
                                                      //myUser.Email = (string)myReader["Email"];
            myUser.IsAdmin = (bool)myReader["IsAdmin"];
            myUser.IsReviewer = (bool)myReader["IsReviewer"];
        }

                 // RETRIEVE SINGLE USER 
        public static Users GetByPk(int id) {
            var sqlText = "SELECT * FROM  Users WHERE Id = @Id"; //parameter, not a variable.  Sql scripting, stored procedures // concatenating is very bad practice, very bad idea.  Sequel injection errors (malicious).  concatenating values into sql statements is a bad idea. Safer way: scripting, stored procedures       
            var sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Id", id); //1st para:  Name of ?? in your sql statement. 2nd parameter:  The value we want to inject   //trying to pass Id into ?parameter? two lines above  // Eexist a collection called "parameters".  A collection of sql parameters.  This is the fix, so we don't get malicious sql injection
            SqlDataReader myReader = sqlcmd.ExecuteReader(); //expensive resource, so need to close it
            if(!myReader.HasRows) { //NOTICE THE !   NEGATES/ NEGATIVE           //PROPERTY of myReader  // I'll do it if it has no rows.  Double-negative
                Console.WriteLine("Null");
                myReader.Close();
                return null;
            }
            myReader.Read(); // points us to the first row
            var myUser = new Users();  // create a single instance.  An empty instance of our myUser.
            LoadUserFromSql(myUser, myReader);

            myReader.Close();  // it was opened automatically for us.  Greg still forgets to close, after all these years.
            return myUser;
        }


                    // GET ALL ROWS
        public static List<Users> GetAll() {                                            // a collection of users instances
            string sqlText = "SELECT * FROM Users;";                                    // good idea:  Try the sql stmt within SSMS, see if you've written it correctly
            SqlCommand sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);      // this command object holds sqlConnection, which in turn holds the **connection string** with the name of my server and my Db. (sqlstmt, connectionObject). New sql object, called a command  // the actual connection is a product of our connection class, a property of our connnection class
            SqlDataReader myReader = sqlcmd.ExecuteReader();                            // HERE'S THE BEEF. defining a sql command is not the same as executing it   //no parameters necess. in this case.
            var userList = new List<Users>();           // creating generic collection of users instances
            while (myReader.Read()) {                   // a -METHOD-, that also has a -RETURN VALUE-: Advances pointer to the next record. Returns bool indicating whether the pointer is pointing to a spot below all the roww. When it's pointing at nothing, the table is over.
                Users myUser = new Users();             // create a single instance.  An empty instance of our user.
                userList.Add(myUser);                   // could also do this in a different place.  Greg likes to do it right away.
                LoadUserFromSql(myUser, myReader);      // call method to load properties one by one
            }

            myReader.Close();
            return userList;
        }


        public override string ToString() {     // used in Program class to print, eg. Console.WriteLine(user)
            return $"Id={Id}, Username={Username}, Password={Password}, " +
                $"Name={Firstname} {Lastname}, Admin?={IsAdmin}, Reviewer?={IsReviewer}";
        }
    }
}
