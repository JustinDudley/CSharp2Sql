using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CSharp2SqlLibrary {
    public class Vendors {

        // STATIC VARIABLE
        public static Connection Connection { get; set; } 

        //PROPERTIES   == INSTANCE VARIABLES
        public int Id { get; private set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //is that all done right?



        // this time, we embed sql statements differently from how we did with Users:
        private const string SqlGetAll = "SELECT * FROM Vendors";  //const means constant.  C# keyword?
        private const string SqlGetByPk = SqlGetAll + " WHERE Id = @Id";  // or, could use interpolated string  $
        private const string SqlDelete = "DELETE FROM Vendors WHERE Id = @Id";  // semicolin inside quotes is okay, yes?
        private const string SqlUpdate = "UPDATE Vendors SET " +
            " Code = @Code, Name = @Name, Address = @Address, City = @City, State = @State, Zip = @Zip, " +
            " Phone = @Phone, Email = @Email " +
            " WHERE Id = @Id ";
        private const string SqlInsert = "INSERT into Vendors " +
            " (Code, Name, Address, City, State, Zip, Phone, Email )" +
            " VALUES (@Code, @Name, @Address, @City, @State, @Zip, @Phone, @Email) ";

        /* MY WEEKEND WORK
        //INSERT
        public static bool Insert(Vendors vendor) {

        }
        */

        //DELETE
        public static bool Delete(int id) {
            var sqlcmd = new SqlCommand(SqlDelete, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            int rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
          




        }

        // try collapsing the block below.  The region stuff stays.  To create region:  Type #, look at dropdown menu
        #region Instance Properties
        private static void LoadVendorFromSql(Vendors vendor, SqlDataReader reader) {
            vendor.Id = ((int)reader["Id"]);
            vendor.Code = reader["Code"].ToString(); //this is the best conversion, will continute with it now
            vendor.Name = reader["Name"].ToString();
            vendor.Address = reader["Address"].ToString();
            vendor.City = reader["City"].ToString();
            vendor.State = reader["State"].ToString();
            vendor.Zip = reader["Zip"].ToString();
            vendor.Phone = reader["Phone"]?.ToString(); // note the ?
            vendor.Email = reader["Email"]?.ToString(); // note the ?
            #endregion
        }


        public static Vendors GetByPk(int id) {
            var sqlcmd = new SqlCommand(SqlGetByPk, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            var reader = sqlcmd.ExecuteReader();
            if (!reader.HasRows) {
                reader.Close();
                return null;
            }
            reader.Read();
            var vendor = new Vendors();
            LoadVendorFromSql(vendor, reader);

            reader.Close();
            return vendor;

                //WILL NEED A STRING OVERRIDE IN THIS CLASS, JUST LIKE USERS
        }


        //JD Attempt:
        //GET BY PK
        //public static Vendors GetByPk(int id) {
        //    string sql = "SELECT FROM Vendors WHERE Id = @Id";
        //    SqlCommand sqlcmd = new SqlCommand(sql, Connection.sqlConnection);
        //    sqlcmd.Parameters.AddWithValue(@Id = id);
        //    var reader = sqlcmd.ExecuteReader();
        //    reader.Read();
        //    if(!reader.HasRows) {

        //    }

        //    if(reader[id] == id) {
        //        LoadVendorFromSql()
        //    }
        //}


        // GET ALL 
        public static List<Vendors> GetAll() {
            // don't need sql statement this time, because we defined it up above
            var sqlcmd = new SqlCommand(SqlGetAll, Connection.sqlConnection);  // the underrscore is actually a SqlConnection.  So he would like to rename this:  _Connection  changed to sqlConnection
            var reader = sqlcmd.ExecuteReader();
            var vendors = new List<Vendors>();
            while(reader.Read()) {
                var vendor = new Vendors();
                vendors.Add(vendor); //"I do that, even though I haven't filled it with data, so I don't forget to do it"
                LoadVendorFromSql(vendor, reader);
            }
            reader.Close();
            return vendors;
        }


        //Whoops, I just changed _Connection to sqlConnection EVERYWHERE, including in my comments!!!

    }
}
