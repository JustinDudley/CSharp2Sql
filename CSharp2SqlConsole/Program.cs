using System;
using CSharp2SqlLibrary;
using System.Diagnostics;
using System.Collections.Generic; // this is here temporarily

namespace CSharp2SqlConsole {
    class Program {

        // INSTANCE METHODS, WITHIN DRIVER CLASS, CALLED ON INSTANCE OF PROGRAM CLASS (CALLED FROM MAIN)

        void RunProductsTest() {
            var conn = new Connection(@"localhost\sqlexpress", "PrsDb");    // instantiate/construct an object, and...
            conn.Open();    // ...and call a method on this new object
            Products.Connection = conn;

            var product = new Products() {      // OKAY, here is a new, popular, alternate syntax for constructor.  Note semicolon AFTER the curly braces.  Good esp for testing.  Can choose to initialize with any properties you want without constructing multiple constructors to do so.
                PartNbr = "XYZ001", Name = "XYKZ Part", Price = 10, Unit = "EACH", PhotoPath = null, VendorId = 3  //note LACK of semicolon on this line    
            };
            try {       // Here's a nice try-catch block.  At least it tells the caller what's wrong. 
                // INSERT
                var success = Products.Insert(product);
                // UPDATE,  product.id = 3
                var p = Products.GetByPk(3);
                p.Name = "GregPartXYZff";
                p.VendorId = 9;  // THIS ISN'T WORKING.  THIS ISN'T WORKING
                success = Products.Update(p);
                //DELETE
                success = Products.Delete(3);   //Is THIS one working?????

            } catch(Exception ex) {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }




            // GetByPk
            Products unicycle = Products.GetByPk(2);
            Console.WriteLine($"Product {unicycle.Name} from Vendor {unicycle.Vendor.Name} is priced at {unicycle.Price}"); // not sure this line is right       */
            // featuring fancier print stmt than the ToString override below in GetAll()


            // GETALL
            var products = Products.GetAll();  // List<T>
            foreach (var prod in products) {
                Console.WriteLine(prod);                     //  ToString override
            }

            conn.Close();
        }


        void RunVendorsTest() {
            Connection conn = new Connection(@"localhost\sqlexpress", "PrsDb");
            conn.Open();   
            Vendors.Connection = conn;

            /*
            //GETBYCODE, JD attempt, early a.m.
            Vendors vCo = Vendors.GetByCode("JGGL");
            Console.WriteLine(vCo);
            */
            //GETALL
            var vendors = Vendors.GetAll();
            foreach (var ven in vendors) {
                Console.WriteLine(ven);
            }

            //GetByPk
            Vendors vendor = Vendors.GetByPk(2);        //assignment AND method call
            Console.WriteLine($"Vendor line deleted? {Vendors.Delete(3)}");


            //UPDATE
            Vendors vendorV = Vendors.GetByPk(4);
            vendorV.Name = "Doggie Bone";
            vendorV.State = "SD";
            bool success = Vendors.Update(vendorV);
            Console.WriteLine($" {(success ? "Update successful" : "Update failed")}");

            
            //INSERT
            Vendors vendorA = new Vendors();   // accesses default constructor in Vendors
            vendorA.Code = "MAGC";
            vendorA.Name = "Magic R Us";
            vendorA.Address = "111 Elm";
            vendorA.City = "Columbus";
            vendorA.State = "OH";
            vendorA.Zip = "33321";
            vendorA.Phone = "555-444-3333";
            vendorA.Email = "RUs@RUs.com";
            Console.WriteLine($"Vendor insertion successful? {Vendors.Insert(vendorA)}");
            

            conn.Close();
        }


        void RunUsersTest() { 
            // if following doesn't work:  Check the connection string.  That's almost always the problem
            var conn = new Connection(@"localhost\sqlexpress", "PrsDb"); // why the  @  ?:   backslash is special in a string.  rsquig.  Want to use \ in normal string?  Use two, like this   \\   It evaluates to just one.  But most people don't like to do that.  Intead, they preface the \ with @.  @ means:  There are no special characters in this string.  Just treat everything normally
            conn.Open(); // program may blow up here. Greg sez, throw an exception with an appropriate message, but don't catch the excep.
            Users.Connection = conn; //puts the open connection into Users class Connection method??


            //LOGIN
            var userLogin = Users.Login("rv", "rv");
            Console.WriteLine(userLogin);   //access print method in Users class?
            var userFailedLogin = Users.Login("xx", "Xx");
            Console.WriteLine(userFailedLogin?.ToString() ?? "Not found");


            //GETALL
            List<Users> userList = Users.GetAll(); // fascinatingly, when "var" is used instead of "List<Users>", the system doesn't require a "using System.Collections.Generic" at top !  
            foreach (var u in userList) {
                Console.WriteLine(u);
            }

            //GetByPk
            var user = Users.GetByPk(2); //2 is a PK user ID
                                         // !!!!!!!!!!!!!!!!!!!
            System.Diagnostics.Debug.WriteLine(user);  //see Library/using statement at top of script.  "Diagnostics" is redundant in this line
                                                       // var usernf = Users.GetByPk(222); // should be not found (nf).  Too high.  Not in our Db. Should set var to NULL
                                                       //var success = Users.Delete(1); //delete where pk == 3 ?? Does the Db have that?
            var user1 = Users.GetByPk(1); //use a read to check the previous line
            Debug.WriteLine(user1);



            //INSERT
            var newuser = new Users();  // instantiate Users object, here in driver class.  
            newuser.Username = "ArBC";  // load up its instance variables (properties/wrench) by hand
            newuser.Password = "XYeZ";
            newuser.Firstname = "Normally";
            //newuser.Phone = "5135555355";
            //newuser.Email = "persn@gmail";
            newuser.Lastname = "Usr";
            newuser.IsAdmin = false;
            newuser.IsReviewer = true;
            var insertSuccess = Users.Insert(newuser);  // Users.Insert() method 1. **Does the sql insertion,  2. **Returns a bool
            Console.WriteLine($"User insert successful? {insertSuccess}");



            //UPDATE
            var userabc = Users.GetByPk(2);
            userabc.Username = "Junebug";
            userabc.Firstname = "JD";
            userabc.Lastname = "Vegetarian";
            var updateSuccess = Users.Update(userabc);
            Console.WriteLine($"{ (updateSuccess ? "Update successful!" : "Update failed") }"); //ternary


            conn.Close();
        }


        void RefreshDatabase() {
            var conn = new Connection(@"localhost\sqlexpress", "PrsDb");   // "PrsDb" should maybe be "master" instead.
            conn.Open();
            FileToDb.Connection = conn;
                // 2-step process, where perhaps I should just have 1:
            string[] sqlLines = FileToDb.FileToStringArr();
            FileToDb.StringArrToDb(sqlLines);
            conn.Close();
        } 


        // MAIN
        static void Main(string[] args) {
            var pgm = new Program();

           pgm.RefreshDatabase();
           //pgm.RunVendorsTest();
           // pgm.RunUsersTest();
            pgm.RunProductsTest();



        //finee

            
            // THREE WAYS TO INSTANTIATE A STRING OBJECT:
            string MyString = "Hi";
            var AnotherString = "Hello";
            string ThirdString = new string("Hey ho");
            string[] strings = new string[] { MyString, AnotherString, ThirdString };
            //foreach(var str in strings) {
            //    Console.WriteLine(str);
            //  }
            
        }
    }
}

        /* Reason for Greg's Run() methods. --Now he can break his previous Run method into 2 methods:
               1. RunVendorsTest()
               2. RunUsersTest()      */


            /*  TO CHECK FILE EXISTS
             *  using System.IO;
            if (File.Exists(path)) {
                Console.WriteLine("Yes, file exists!");
            } else { Console.WriteLine("Sorry Charlie");
            }            */


        //conn.Open(); // program may blow up here
        //            Users.Connection = conn; //puts the open connection into Users class Connection method??
        //            var userList = Users.GetAll();
        //var user = Users.GetByPk(2); //2 is a PK user ID??
        //var usernf = Users.GetByPk(222); // should be not found (nf).  Too high.  Not in our Db. Should set var to NULL
        //conn.Close();
