using System;
using CSharp2SqlLibrary;
using System.Diagnostics;

namespace CSharp2SqlConsole {
    class Program {

        // Reason for Greg's run method.:
        /*  --now he can break his previous Run method into 2 methods:
         *      1. RunVendorsTest()
         *      2. RunUsersTest()
            */

        void RunProductsTest() {
            var conn = new Connection(@"localhost\sqlexpress", "PrsDb");
            conn.Open();
            Products.Connection = conn;
            var products = Products.GetAll();


            /*  I may have mucked up these two lines...
            var stilts = Products.GetByPk(3);  //trying to read my echo //Greg tested his own data.  Not sure what we're testing for.  I can add in my own data and try to see what we're doing
            Console.WriteLine($"Product {p.Name} from Vendor {p.Vendor.Name} is priced at {p.Price}"); // not sure this line is right
            */


            foreach(var p in products) {
                //Console.WriteLine(p);   //weird display, so we change it
                Console.WriteLine($"Product {p.Name} from Vendor {p.Vendor.Name} is priced at {p.Price}"); // not sure this line is right 
                // JOIN VIEW.  GIVES JOIN VIEW.  GIVES JOIN VIEW
            }
            conn.Close();
        }


        void RunVendorsTest() {
            //"We'll add some tests"  good verbage for JD
            var conn = new Connection(@"localhost\sqlexpress", "PrsDb");
            conn.Open();
            Vendors.Connection = conn;

            var vendors = Vendors.GetAll();
            foreach(var v in vendors) {
                Console.WriteLine(v.Name); //later we'll just print the vendor instance . Not ready to do this yet ????
            }

            Vendors vendor = Vendors.GetByPk(2);
            Console.WriteLine($"Vendor line deleted? {Vendors.Delete(3)}");


            /* MY WEEKEND WORK
            //INSERT
            Vendors vendor11 = new Vendors();   // accesses default constructor in Vendors
            vendor11.Code = "MAGC";
            vendor11.Name = "Magic R Us";
            vendor11.Address = "111 Elm";
            vendor11.City = "Columbus";
            vendor11.State = "OH";
            vendor11.Zip = "33321";
            vendor11.Phone = "555-444-3333";
            vendor11.Email = "RUs@RUs.com";
            Console.WriteLine($"Vendor insertion successful? {Vendors.Insert(vendor11)}");
            */
            

            conn.Close();
        }


        //void Run() {      // old name for method
        void RunUsersTest() { 
            // if following doesn't work:  Check the connection string.  That's almost always the problem
            var conn = new Connection(@"localhost\sqlexpress", "PrsDb"); //  backslash is special in a string.  rsquig.  Want to use \ in normal string?  Use two, like this   \\   It evaluates to just one.  But most people don't like to do that.  Intead, they preface the \ with @.  @ means:  There are no special characters in this string.  Just treat everything normally
            conn.Open(); // program may blow up here
            Users.Connection = conn; //puts the open connection into Users class Connection method??
            var userLogin = Users.Login("rv", "rv");
            Console.WriteLine(userLogin);   //access print method in Users class?
            var userFailedLogin = Users.Login("xx", "Xx");
            Console.WriteLine(userFailedLogin?.ToString() ?? "Not found");

            var users = Users.GetAll();
            foreach(var u in users) {
                Console.WriteLine(u);
            }



            var user = Users.GetByPk(2); //2 is a PK user ID?
            // !!!!!!!!!!!!!!!!!!!
            System.Diagnostics.Debug.WriteLine(user);  //see Library/using statement at top of script
           // var usernf = Users.GetByPk(222); // should be not found (nf).  Too high.  Not in our Db. Should set var to NULL
            //var success = Users.Delete(1); //delete where pk == 3 ?? Does the Db have that?
            var user1 = Users.GetByPk(1); //use a read to check the previous line
            Debug.WriteLine(user1);



            
            //INSERT
            var newuser = new Users();
            newuser.Username = "ArBC";
            newuser.Password = "XYeZ";
            newuser.Firstname = "Normally";
            //newuser.Phone = "5135555355";
            //newuser.Email = "persn@gmail";
            newuser.Lastname = "Usr";
            newuser.IsAdmin = false;
            newuser.IsReviewer = true;
            var insertSuccess = Users.Insert(newuser);
            Console.WriteLine($"User insert successful? {insertSuccess}");
            


            //UPDATE
            var userabc = Users.GetByPk(2);
            userabc.Username = "Junebug";
            userabc.Firstname = "JD";
            userabc.Lastname = "Vegetarian";
            var updateSuccess = Users.Update(userabc);


            conn.Close();
        }


        static void Main(string[] args) {
            var pgm = new Program();
            //pgm.RunVendorsTest();
           // pgm.RunUsersTest();
            pgm.RunProductsTest();




            // CONSIDER WRITING A METHOD, MAYBE HERE IN THE DRIVER CLASS, THAT RE-SETS/RE-POPULATES MY DATABASE.  CALL IT FROM MAIN
            // ON MOST RUNS, SO I DON'T GET CRAZY CHANGES  BUILDING UPON CRAZY CHANGES.   I CAN COMMENT IT OUT IF I WANT TO MAKE 
            // STEPWISE CHANGES.
            //OKAY, HERE IT IS:

            void ResetDb() {
               // string sqlstmt = /* drop all the tables, and put in the whole text from my SSMS file that creates the PrsDb database*/
                //need connection?
               // sqlcmd  kjfkdjfkdjfldjfljlkjfsj 
               // kfjkds    dkfj  dsjfdksjfdj   kjfjdk

            }

        }
    }
}

//conn.Open(); // program may blow up here
//            Users.Connection = conn; //puts the open connection into Users class Connection method??
//            var users = Users.GetAll();
//var user = Users.GetByPk(2); //2 is a PK user ID??
//var usernf = Users.GetByPk(222); // should be not found (nf).  Too high.  Not in our Db. Should set var to NULL
//conn.Close();