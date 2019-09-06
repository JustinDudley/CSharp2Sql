using System;
using System.Collections.Generic;
using System.Data.SqlClient; // this was generated when we hovered on SqlConnection, and asked the system to generate something
using System.Text;

namespace CSharp2SqlLibrary {
    public class Connection {

        public SqlConnection _Connection { get; set; } = null;   //2. now we create an actual connection object. It's a property.// class name is also "Connection".  // this is the actual connection?//underscore in line above:  a dev shortcut to indicate a PROPERTY. 

        public void Open() {
            this._Connection.Open();  //Greg typically doesn't handle the possible exceptions.  Let it blow.  Almost always, the connection string is the problem. 
            //now check whether you've actually opened a connection:
            if (this._Connection.State != System.Data.ConnectionState.Open) { //"Open is one of the states.  hover over connectionstate
            throw new Exception("Connection did not open!");
            }
        }   

        public void Close() {        // we're expecting the user will close connection
            if(this._Connection.State != System.Data.ConnectionState.Open) {
                return;      // maybe it NEVER got opened...
            }
            this._Connection.Close();
        }

        //1. CONSTRUCTOR --the only constr. in this class, 3rd parameter not needed, for some reason
        public Connection(string server, string database) {
            var connStr /*greg's shortcut */ = $"server={server};database={database};trusted_connection=true;"; //this c.s. works for pretty much all SQL Db's  //final semicolon optional.  Good practice, for future devs.   // key-value pair, separated by semicolon //note: no quotes allowed inside a string
            this._Connection = new SqlConnection(connStr);    // 3. now create an instance of the SQL connection. We have created a connection object now   
        }
        


        // before he put stuff into a method:

        ////2. now we create an actual connection object
        //public SqlConnection _Connection { get; set; } = null; // class name is also "Connection"
        ////underscore in line above:  a dev shortcut to indicate a PROPERTY. 
        //// greg uses conn, that's his thing


        ////1. CONSTRUCTOR --the only constr. in this class, 3rd parameter not needed, for some reason
        //public Connection(string server, string database) {
        //    var connStr /*greg's shortcut */ = $"server={server};database={database};trusted_connection=true;"; //this c.s. works for pretty much all SQL Db's  //final semicolon optional.  Good practice, for future devs.   // key-value pair, separated by semicolon //note: no quotes allowed inside a string
        //    // 3. now create an instance of the SQL connection
        //    this._Connection = new SqlConnection(connStr); //we have created a connection object now
        //    //the diff:  TRUST.  Don't really understand yet.  That's okay
        //    //instantiate.  THEN open the connection
        //    this._Connection.Open();  //Greg typically doesn't handle the possible exceptions.  Let it blow.  Almost always, the connection string is the problem. 
        //    //now check whether you've actually opened a connection:
        //    if (this._Connection.State != System.Data.ConnectionState.Open) { //"Open is one of the states.  hover over connectionstate
        //        throw new Exception("Connection did not open!");
        //    }

        //}
        //COULD create an instance, then open it, close it, open it, close it...

            // "Do your build... Hopefully it build successfully..."  -what is meant?


    }
}
