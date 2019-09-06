using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CSharp2SqlLibrary {
    public class Products {

        // STATIC VARIABLES
        public static Connection Connection { get; set; }  // it's statis, because the methods using it are static
        // DEFINE CONSTANTS
        private const string SqlGetAll = "SELECT * FROM Products ";
        private const string SqlGetByPk = "SELECT * FROM Products WHERE Id = @Id ";
        private const string SqlDelete = "DELETE from Products WHERE Id = @Id ";
        private const string SqlInsert = "INSERT Products " +
            " (PartNbr, Name, Price, Unit, PhotoPath, VendorId) " +
            " VALUES (@PartNbr, @Name, @Price, @Unit, @PhotoPath, @VendorId) ";
        private const string SqlUpdate = "UPDATE Products SET " +
            " PartNbr = @PartNbr, Name = @Name, Price = @Price, Unit = @Unit, " +
            " PhotoPath = @PhotoPath, VendorId = @VendorId " +
            " WHERE Id = @Id ";

        // Okay, so we keep most of these public, because we access them a lot from the driver class.  
        #region  Instance Propertites
        public int Id { get; private set; }
        public string PartNbr { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string PhotoPath { get; set; }
        public int VendorId { get; set; } // It's questionable whether we need the VendorId at ALL.   Because it's already included withing the ?reg. Id? //we won't change the VendorId to private, becasue we ahve to have a way to change that if we make a mistake or something

        public Vendors Vendor { get; private set; } // this one is different.
        #endregion


        // is below all ok?  Went by fast...
        public static Products GetByPk(int id) {
            var sqlcmd = new SqlCommand(SqlGetByPk, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            var reader = sqlcmd.ExecuteReader();
            if (!reader.HasRows) {
                reader.Close();
                return null;
            }
            reader.Read();
            var product = new Products();
            LoadProductFromSql(product, reader);

            reader.Close();

            Vendors.Connection = Connection;
            var vendor = Vendors.GetByPk(product.VendorId);
            product.Vendor = vendor; 
            
            //then return the individual product:
            return product;
        }



        public static List<Products> GetAll() {
            var sqlcmd = new SqlCommand(SqlGetAll, Connection.sqlConnection);
            var reader = sqlcmd.ExecuteReader();  // execute the reader
            var products = new List<Products>();
            while (reader.Read()) {
                var product = new Products();
                products.Add(product);  // add empty product instance to the hcollection.  You can Do it now, so youdont' forget later. 
                LoadProductFromSql(product, reader);
            }
            reader.Close();

            //because of data reading/  opening-a-connection issue ???   We have to do the tricky thing below.  

            Vendors.Connection = Connection; // so vendor can use Db
            foreach(var prod in products) {
                var vendor = Vendors.GetByPk(prod.VendorId);   // !!!  <--  !!!!  We're USING fucntions of the vendor class to read fjflfdsljfl)                
                                                             // we'll put whole vender instance into the vendro property of the product" (?)
                                                           //point to a vendor id that doesn't exist?  Bad!  
                                                           //prod.VendorId is going to close inside its own method, so we
                                                           // don't have to worry about it
                                                           
                //If we had TWO data readers open (impossible) we wouldn't have to do all this stuff/nonsense
                prod.Vendor = vendor;
            }
            return products;

            // something above is "EXACTLY LIKE A JOIN"    "select p.*, v.* from products p join vendors v on v.id  p.vendorId"
            // JOIN VIEW  JOIN VIEW    JOIN VIEW
        }


        private static void LoadProductFromSql(Products product, SqlDataReader reader) {
            product.Id = (int)reader["Id"];
            product.PartNbr = reader["PartNbr"].ToString();
            product.Name = reader["Name"].ToString();
            product.Price = (decimal)reader["Price"];
            product.Unit = reader["Unit"].ToString();
            product.PhotoPath = reader["PhotoPath"]?.ToString();
            product.VendorId = (int)reader["VendorId"];
        }


        // ToString Override
        public override string ToString() {
            return $"Id={Id}, PartNbr={PartNbr}, Name={Name}, Price={Price}, Unit={Unit}, VendorId={VendorId}, Vendor={Vendor}";
        }

    }
}

/*OPTIONAL SYNTAX, FOR INSIDE LoadProductFromSql method:
            // product.Id = reader.GetInt32(reader.GetOrdinal("Id"));  // more verbose way to do conversion.  GetOrdinal returns index of..., and you put that into GetInt32.  GetInt32 automatically casts whatever it gets into an integer
            // There are Getdate, Getstring, GetThisAndThat, just like w/ line above.  But Greg prefers the syntax below.
            // The problem:  The reader gives you an object.  You need to turn that object into the type you need.  
 */
