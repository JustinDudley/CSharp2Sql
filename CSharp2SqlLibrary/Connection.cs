using System;
using System.Collections.Generic;
using System.Data.SqlClient; // this was generated when we hovered on SqlConnection, and asked the system to generate something
using System.Text;

namespace CSharp2SqlLibrary {
    public class Connection {

                    // INSTANCE VARIABLE, USES LIBRARY
        public SqlConnection sqlConnection { get; set; } = null;   // Property. An actual connection object.  //underscore indicates a PROPERTY. (Dev shortcut)  [editted out] 

                    // CONSTRUCTOR 
        public Connection(string server, string database) {
            string connStr /*greg's shortcut */ = $"server={server};database={database};trusted_connection=true;"; //this connection string works for pretty much all SQL Db's  //final semicolon optional.  Good practice. // key-value pair, separated by semicolon
            this.sqlConnection = new SqlConnection(connStr);    // 3. now create an instance of the SQL connection. We have created a connection object now   
        }

                    // INSTANCE METHODS (TWO)
        public void Open() {
            this.sqlConnection.Open();         // Almost always, the connection string is the problem.     
            if (this.sqlConnection.State != System.Data.ConnectionState.Open) {      // to check whether you've actually opened a connection.  //"Open is one of the states.  hover over connectionstate
            throw new Exception("Connection did not open!");  //Greg sez:  "Let it blow.  Don't catch the exception. (Typically).  Just give a message, as shown."
            }
        }   

        public void Close() {        
            if(this.sqlConnection.State != System.Data.ConnectionState.Open) {
                return;          // maybe it NEVER got opened...
            }
            this.sqlConnection.Close();
        }



            // [connStr]:   connection string format:  " 1. Server & instance;  2. the database;  3. authentication  "
        // trusted_connection = true    --will use same user that you logged into SQL with
      

        // instantiate.  THEN open the connection
        //COULD create an instance, then open it, close it, open it, close it...

    }
}
