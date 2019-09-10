using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace CSharp2SqlLibrary {
    public class FileToDb {

        public static Connection Connection { get; set; }

        string[] SqlLines { get; set; }

        
        public static string[] FileToStringArr() {
            string path = @"C:\repos\SQLscripts\PrsDb-create-database-comments-not-allowed.sql";
            string[] sqlLines = File.ReadAllLines(path);

            /*              NEW PLAN, TO ALLOW ME TO ADD COMMENTS TO MY .SQL FILES.
                    I WANT TO ELIMINATE LINES THAT HAVE COMMENTS (AS INDICATED BY PRESENCE OF --)
                      -FIRST SHOULD ELIMATE LEADING WHITE SPACE, THEN CHECK FOR "--" CHARACTERS, THEN NOT INCLUDE THOSE LINES
            string[] prelimSqlLines = File.ReadAllLines(path);
            List<string> finalSqlLines = new List<string>;      // must make function return list, not array, and add to Library here AND in program
            foreach(var line in prelimSqlLines) {
                if !line.substring(2).equals("--") {
                finlSqlLines.Add(line);
            }            */


            return sqlLines;
        }

        public static void StringArrToDb(string[] sqlLines) {
            var SqlLines = sqlLines;
            string sqlText = "";
            foreach(var line in SqlLines) {
                sqlText += line;
            }
            //Console.WriteLine(sqlText);  //check console:  Did I load sqlText properly?

            //sqlText = "DELETE from Users WHERE Id = 4;";

            //sqlText = "INSERT INTO Users(Username, Password, Firstname, Lastname, Phone, Email, IsReviewer, IsAdmin) VALUES('NOTttMcZingerNEW', 'IWillMcZingYou', 'Margaret', 'Sprigg-Dudley', '513-555-1212', 'margaret@gmail.com', 1, 1);" +
              //  "INSERT INTO Users(Username, Password, Firstname, Lastname, Phone, Email, IsReviewer, IsAdmin) VALUES('NOTtttairplaneWatcher', 'plane', 'Max', 'Sprigg-Dudley', '513-555-8888', 'max@gmail.com', 1, 0);";
            var sqlcmd = new SqlCommand(sqlText, Connection.sqlConnection);
            sqlcmd.ExecuteNonQuery();
        }

    }
}

