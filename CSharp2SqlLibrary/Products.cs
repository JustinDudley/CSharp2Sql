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
        private const string SqlGetByPrtNbr = SqlGetAll + " Where PartNbr = @PartNbr ";   // CONCAT 
        private const string SqlDelete = "DELETE from Products WHERE Id = @Id ";
        private const string SqlInsert = "INSERT Products " +
            " (PartNbr, Name, Price, Unit, PhotoPath, VendorId) " +
            " VALUES (@PartNbr, @Name, @Price, @Unit, @PhotoPath, @VendorId) ";
        private const string SqlUpdate = "UPDATE Products SET " +
            " PartNbr = @PartNbr, Name = @Name, Price = @Price, Unit = @Unit, " +
            " PhotoPath = @PhotoPath, VendorId = @VendorId " +
            " WHERE Id = @Id ";


        public static Products GetByPartNbr(string partNbr) {
            var sqlcmd = new SqlCommand(SqlGetByPrtNbr, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@PartNbr", partNbr);
            var reader = sqlcmd.ExecuteReader();
            // got nothing back?  return null
            if(!reader.HasRows) {
                reader.Close();
                return null;
            }
            reader.Read();  // moves the pointer
            var product = new Products();
            LoadProductFromSql(product, reader);
            reader.Close();
            return product;
        }


        // Two ways to do insert.  Best: pass in an instance of the product.

        public static bool Insert(Products product) {       // most load up the vendor Id before we pass it into the insert
            var sqlcmd = new SqlCommand(SqlInsert, Connection.sqlConnection);
            SetParameterValues(product, sqlcmd);
            int rowsAffected = sqlcmd.ExecuteNonQuery();
            return (rowsAffected == 1);
        }

        // ideally, would refactor this block and delete, combine stuff
        public static bool Update(Products product) {
            var sqlcmd = new SqlCommand(SqlUpdate, Connection.sqlConnection);
            SetParameterValues(product, sqlcmd);
            sqlcmd.Parameters.AddWithValue("@Id", product.Id);            //must pass in ID too
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return rowsAffected == 1;
        }

        private static void SetParameterValues(Products product, SqlCommand sqlcmd) {
            sqlcmd.Parameters.AddWithValue("@PartNbr", product.PartNbr);
            sqlcmd.Parameters.AddWithValue("@Name", product.Name);
            sqlcmd.Parameters.AddWithValue("@Price", product.Price);
            sqlcmd.Parameters.AddWithValue("@Unit", product.Unit);
            sqlcmd.Parameters.AddWithValue("@Photopath", (object)product.PhotoPath ?? DBNull.Value); // "coalescing operator"
            sqlcmd.Parameters.AddWithValue("@VendorId", product.VendorId);
        }

        public static bool Delete(int id) {
            var sqlcmd = new SqlCommand(SqlDelete, Connection.sqlConnection);
            sqlcmd.Parameters.AddWithValue("@Id", id);            //must pass in ID too
            var rowsAffected = sqlcmd.ExecuteNonQuery();
            return rowsAffected == 1;
        }


        public static bool Delete(Products product) {
            // pass in the one to delete.  Diff from vendor and user where we did public static boo Delete(innt id).  That won't work here AS WELL.  Both are valid.  Actually, you can use BOTH in the program.  Id is simpler for user, but Product one can be okay, because the product one can just call the id one
            //Ideally, we let the new one call the old one
            //instead of putting another SQL command, just do:      (Could have done it in the otehr diretion too.  HAVe the other delete method call THIS one.  But:  You don't want a big body in both delete methods.  Have one call the other

            //consistent with Update and ?Insert? this way, using THIS delete method.
            return Delete(product.Id);
        }


        public static void LoadProductFromSql(Products product, SqlDataReader myReader) {  //accessed from Vendors class, so: public
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
