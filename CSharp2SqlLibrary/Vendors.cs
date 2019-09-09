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


        private static void SetParameterValues(Vendors vendor, SqlCommand sqlcmd) {
            sqlcmd.Parameters.AddWithValue("@Code", vendor.Code);
            sqlcmd.Parameters.AddWithValue("@Name", vendor.Name);
            sqlcmd.Parameters.AddWithValue("@Address", vendor.Address);
            sqlcmd.Parameters.AddWithValue("@City", vendor.City);
            sqlcmd.Parameters.AddWithValue("@State", vendor.State);
            sqlcmd.Parameters.AddWithValue("@Zip", vendor.Zip);
            sqlcmd.Parameters.AddWithValue("@Phone", vendor.Phone);
            sqlcmd.Parameters.AddWithValue("@Email", vendor.Email);
        }

        /* JD attempt, Monday earlyk a.m.
        //GETBYCODE
        public static Vendors GetByCode(string code) {
            var sqlcmd = new Sql
        }       */

        //UPDATE
        public static bool Update(Vendors vendor) {
            var sqlcmd = new SqlCommand(SqlUpdate, Connection.sqlConnection);
            SetParameterValues(vendor, sqlcmd);
            sqlcmd.Parameters.AddWithValue("@Id", vendor.Id);
            int rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
        }

        //INSERT
        public static bool Insert(Vendors vendor) {
            var sqlcmd = new SqlCommand(SqlInsert, Connection.sqlConnection);
            SetParameterValues(vendor, sqlcmd);
            int rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
        }
        

        //DELETE
        public static bool Delete(int id) {
            var sqlcmd = new SqlCommand(SqlDelete, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            int rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
          




        }

        // try collapsing the block below.  The region stuff stays.  To create region:  Type #, look at dropdown menu
        #region Instance Properties
        private static void LoadVendorFromSql(Vendors vendor, SqlDataReader myReader) {
            vendor.Id = ((int)myReader["Id"]);
            vendor.Code = myReader["Code"].ToString(); //this is the best conversion, will continute with it now
            vendor.Name = myReader["Name"].ToString();
            vendor.Address = myReader["Address"].ToString();
            vendor.City = myReader["City"].ToString();
            vendor.State = myReader["State"].ToString();
            vendor.Zip = myReader["Zip"].ToString();
            vendor.Phone = myReader["Phone"]?.ToString(); // note the ?
            vendor.Email = myReader["Email"]?.ToString(); // note the ?
            #endregion
        }


        public static Vendors GetByPk(int id) {
            var sqlcmd = new SqlCommand(SqlGetByPk, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            var myReader = sqlcmd.ExecuteReader();
            if (!myReader.HasRows) {
                myReader.Close();
                return null;
            }
            myReader.Read();
            var vendor = new Vendors();
            LoadVendorFromSql(vendor, myReader);
            
            myReader.Close();
            return vendor;
        }


        // GET ALL 
        public static List<Vendors> GetAll() {
            // don't need sql statement this time, because we defined it up above
            var sqlcmd = new SqlCommand(SqlGetAll, Connection.sqlConnection);  // the underrscore is actually a SqlConnection.  So he would like to rename this:  _Connection  changed to sqlConnection
            var myReader = sqlcmd.ExecuteReader();
            var vendors = new List<Vendors>();
            while(myReader.Read()) {
                var vendor = new Vendors();
                vendors.Add(vendor); //"I do that, even though I haven't filled it with data, so I don't forget to do it"
                LoadVendorFromSql(vendor, myReader);
            }
            myReader.Close();
            return vendors;
        }


        public override string ToString() {
            return $"Id: {Id}, Code: {Code}, Name: {Name}, Address: {Address}, {City}, {State},  " +
                $"Phone = {Phone}, Email: {Email} ";
                
        }
    }
}


      //Whoops, I just changed _Connection to sqlConnection EVERYWHERE, including in my comments!!!


