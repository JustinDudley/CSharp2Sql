using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CSharp2SqlLibrary {

    public class Users {

        //PROPERTY = INSTANCE VARIABLE
        public static Connection Connection { get; set; }  // static, because we need just one for the class

        //public int Id { get; set; } // we need to protect the id from being changed by the user
        public int Id { get; private set; } // we need to protect the id from being changed by the user
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }   //        public bool? IsAdmin { get; set; }   ? indicates "nullable"
        public bool IsReviewer { get; set; }


        // UPDATE
        public static bool Update (Users user) {
            var sql = "UPDATE Users Set " +

            " Username = @Username, " +
            " Password = @Password, " +
            " FirstName = @Firstname, " +
            " Lastname = @Lastname, " +
            " Phone = @Phone, " +
            " Email = @Email, " +
            " IsAdmin = @IsAdmin, " +
            " IsReviewer = @IsReviewer " +
            " Where Id = @Id";
            
            var sqlcmd = new SqlCommand(sql, Connection._Connection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Username", user.Username);
            sqlcmd.Parameters.AddWithValue("@Password", user.Password);
            sqlcmd.Parameters.AddWithValue("@Firstname", user.Firstname);
            sqlcmd.Parameters.AddWithValue("@Lastname", user.Lastname);
            sqlcmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value); //C# null and SQL null are not the same
            sqlcmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value); // C# null and SQL null are not the same
            sqlcmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
            sqlcmd.Parameters.AddWithValue("@IsReviewer", user.IsReviewer);
            sqlcmd.Parameters.AddWithValue("@Id", user.Id);
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);

        }


        // INSERT
        public static bool Insert(Users user) {     //this time, we're passing an ENTIRE INSTANCE into the method.  // note bool return
            var sql = "INSERT into Users " +
                "(Username, Password, FirstName, Lastname, Phone, Email, IsAdmin, IsReviewer) " +
                " VALUES " +
                "(@Username, @Password, @Firstname, @Lastname, @Phone, @Email, @IsAdmin, @IsReviewer)";
            var sqlcmd = new SqlCommand(sql, Connection._Connection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Username", user.Username);
            sqlcmd.Parameters.AddWithValue("@Password", user.Password);
            sqlcmd.Parameters.AddWithValue("@Firstname", user.Firstname);
            sqlcmd.Parameters.AddWithValue("@Lastname", user.Lastname);
            sqlcmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value); //C# null and SQL null are not the same
            sqlcmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value); // C# null and SQL null are not the same
            sqlcmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
            sqlcmd.Parameters.AddWithValue("@IsReviewer", user.IsReviewer);
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
        }

        // DELETE    --safest to delete by ID
        public static bool Delete(int id) {
            var sql = "DELETE from Users WHERE Id = @Id;";
            var sqlcmd = new SqlCommand(sql, Connection._Connection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Id", id);
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);     //this is either true of false.  We want it to be true, because we wanted to take out one row.
            // if rowsAffected > 1, something really went wrong!  (If == 0, means couldn't find that Id)
        }


        // LOGIN
        public static Users Login(string username, string password) {
            var sql = "select * from Users where username = @username and password = @password";
            var sqlcmd = new SqlCommand(sql, Connection._Connection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Username", username);
            sqlcmd.Parameters.AddWithValue("@Password", password);

            var reader = sqlcmd.ExecuteReader(); //expensive resource, so need to close it
            if(!reader.HasRows) {
                reader.Close();
                return null;
            }
            reader.Read();
            var user = new Users();
            LoadUserFromSql(user, reader);

            reader.Close();
            return user;

        }


        // RETRIEVE SINGLE USER 
        public static Users GetByPk(int id) {
            var sql = "SELECT * FROM  Users WHERE Id = @Id"; //parameter, not a variable.  Sql scripting, stored procedures // concatenating is very bad practice, very bad idea.  Sequel injection errors (malicious).  concatenating values into sql statements is a bad idea. Safer way: scripting, stored procedures       
            var sqlcmd = new SqlCommand(sql, Connection._Connection);  // same as other one
            sqlcmd.Parameters.AddWithValue("@Id", id); //1st para:  Name of ?? in your sql statement. 2nd parameter:  The value we want to inject   //trying to pass Id into ?parameter? two lines above  // Eexist a collection called "parameters".  A collection of sql parameters.  This is the fix, so we don't get malicious sql injection
            var reader = sqlcmd.ExecuteReader(); //expensive resource, so need to close it
            if(!reader.HasRows) { //NOTICE THE !   NEGATES/ NEGATIVE           //PROPERTY of reader  // I'll do it if it has no rows.  Double-negative
                Console.WriteLine("Null");
                reader.Close();
                return null;
            }
            reader.Read(); // points us to the first row
            var user = new Users();  // create a single instance.  An empty instance of our user.
               // ELIMATED ONE LINE OF CODE AS ORDERED ----HERE---  AFTER COPYING THIS BIG BLOCK FROM SOMEWHERE ELSE

            LoadUserFromSql(user, reader);

            reader.Close();  // it was opened automatically for us.  Greg still forgets to close, after all these years.
            return user;
        }


        // GET ALL ROWS.  Our first method 
        public static List<Users> GetAll() {   // a collection of users instances
            var sql = "SELECT * FROM Users;";  // good idea:  Try the sql stmt within SSMS, see if you've written it correctly
            var sqlcmd = new SqlCommand(sql, Connection._Connection); // (sqlstmt, connectionObject). New sql object, called a command  // the actual connection is a product of our connection class, a property of our connnection class
            var reader = sqlcmd.ExecuteReader(); //no parameters necess. in this case.  // defining a sql command is not the same as executing it
            var users = new List<Users>();  // creating generic collection of users instances
            while (reader.Read()) {  //returns bool indicating whether the pointer is pointing to a spot below all the rows, it's pointing at nothing, the table is over.
                var user = new Users();  // create a single instance.  An empty instance of our user.
                users.Add(user);  // could also do this in a different place.  Greg likes to do it right away.

                LoadUserFromSql(user, reader);
            }

            reader.Close();
            return users;
        }

        // helper, called by methods in THIS class
        private static void LoadUserFromSql(Users user, SqlDataReader reader) {
           //load up Users object with fields from Db:
            user.Id = (int)reader["Id"]; // casts this fundamental object type (base class) to int type 
            user.Username = reader["Username"].ToString();
            user.Password = (string)reader["Password"];  // Eexist two ways to convert strings, Cast vs ToString. This is the OTHER way.
            user.Firstname = (string)reader["Firstname"];
            user.Lastname = (string)reader["Lastname"];
            user.Phone = reader["Phone"]?.ToString();       //handling null phone numbers. Turns out we can't cast with (string) --see failed attempt two lines below
            user.Email = reader["Email"]?.ToString(); // if comes back okay, just set it to ??string. If null, set to null. ? is a new syntax:  "If it's not null, call this method"
                                                      //user.Phone = (string)reader["Phone"]; 
                                                      //user.Email = (string)reader["Email"];
            user.IsAdmin = (bool)reader["IsAdmin"];
            user.IsReviewer = (bool)reader["IsReviewer"];
        }


        //????
        public override string ToString() {
            return $"Id={Id}, Username={Username}, Password={Password}, " +
                $"Name={Firstname} {Lastname}, Admin?={IsAdmin}, Reviewer?={IsReviewer}";




        /*  (( this was my initial idea for validating username/password.  Aborted quickly.  Can return to it later...  )) 
        public static bool UserPwdMatch(string user, string password) {
            var sql = "SELECT password FROM users WHERE username = 'rv';";
            var sqlcmd = new SqlCommand(sql, Connection._Connection);
            var reader = sqlcmd.ExecuteReader(); //expensive resource, so need to close it
            if()
            return
        }*/


        }
    }
}
