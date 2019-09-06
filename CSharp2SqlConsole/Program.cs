using System;
using CSharp2SqlLibrary;
using System.Diagnostics;

namespace CSharp2SqlConsole {
    class Program {

        void Run() {
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



            /*
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
            success = Users.Insert(newuser);
            */



            //UPDATE
            var userabc = Users.GetByPk(2);
            userabc.Username = "Junebug";
            userabc.Firstname = "JD";
            userabc.Lastname = "Vegetarian";
            var success = Users.Update(userabc);



            conn.Close();



        }


        static void Main(string[] args) {
            var pgm = new Program();
            pgm.Run();


        }
    }
}

//conn.Open(); // program may blow up here
//            Users.Connection = conn; //puts the open connection into Users class Connection method??
//            var users = Users.GetAll();
//var user = Users.GetByPk(2); //2 is a PK user ID??
//var usernf = Users.GetByPk(222); // should be not found (nf).  Too high.  Not in our Db. Should set var to NULL
//conn.Close();