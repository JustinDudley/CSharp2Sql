using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CSharp2SqlLibrary {
    public class Products {

        // STATIC VARIABLES
        public static Connection Connection { get; set; }  // it's static, because the methods using it are static

        // Okay, so we keep most of these public, because we access them a lot from the driver class.  
        #region  Instance Propertites
        public int Id { get; private set; }
        public string PartNbr { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string PhotoPath { get; set; }
        public int VendorId { get; set; } // It's questionable whether we need the VendorId at ALL.   Because it's already included withing the ?reg. Id? //we won't change the VendorId to private, becasue we ahve to have a way to change that if we make a mistake or something

        // and ONE MORE instance property: A Vendors object. We attach one more variable to a product instance:  The Vendors object that the product's VendorId points to
        public Vendors Vendor { get; private set; }
        #endregion


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



        private static void LoadProductFromSql(Products product, SqlDataReader myReader) {
            product.Id = (int)myReader["Id"];
            product.PartNbr = myReader["PartNbr"].ToString();
            product.Name = myReader["Name"].ToString();
            product.Price = (decimal)myReader["Price"];
            product.Unit = myReader["Unit"].ToString();
            product.PhotoPath = myReader["PhotoPath"]?.ToString();
            product.VendorId = (int)myReader["VendorId"];
        }


        public static Products GetByPk(int id) {
            var sqlcmd = new SqlCommand(SqlGetByPk, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            var myReader = sqlcmd.ExecuteReader();
            if (!myReader.HasRows) {
                myReader.Close();
                return null;
            }
            myReader.Read();
            var product = new Products();   // note: here we are instantiating an instance inside its own class, not Program class
            LoadProductFromSql(product, myReader);
            myReader.Close();

                    // ...and we attach one more variable to product:  The vendor pointed to by the product's VendorId.
                    // - essentially the same as JOIN VIEW    JOIN VIEW    JOIN VIEW
            Vendors.Connection = Connection;
            Vendors vendor = Vendors.GetByPk(product.VendorId);  // VendorId, as found in product table, IS the PK field for vendor table
            product.Vendor = vendor; 
            
            return product;   
        }



        public static List<Products> GetAll() {
            var sqlcmd = new SqlCommand(SqlGetAll, Connection.sqlConnection);
            var myReader = sqlcmd.ExecuteReader();  // execute the myReader
            var products = new List<Products>();
            while (myReader.Read()) {
                var product = new Products();
                products.Add(product);  // add empty product instance to collection. Can do now, so don't forget later.
                LoadProductFromSql(product, myReader);
            }
            myReader.Close();

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
        }


        public override string ToString() {
            return $"Id={Id}, PartNbr={PartNbr}, Name={Name}, Price={Price}, " +
                $"Unit={Unit}, VendorId={VendorId}, Vendor={Vendor}";
        }
    }
}

            /* OPTIONAL SYNTAX, FOR INSIDE LoadProductFromSql method:
            // product.Id = myReader.GetInt32(myReader.GetOrdinal("Id"));  // more verbose way to do conversion.  GetOrdinal returns index of..., and you put that into GetInt32.  GetInt32 automatically casts whatever it gets into an integer
            // There are Getdate, Getstring, GetThisAndThat, just like w/ line above.  But Greg prefers the syntax below.
            // The problem:  The myReader gives you an object.  You need to turn that object into the type you need.   */
