using System;
using System.Data.SqlClient;
using SQLDatabaseHelper;

namespace ConnectToDatabase
{
    class Program
    {
        static void Main()
        {
            using ( SQLDatabase db = new SQLDatabase(@"server = DESKTOP-TEPR7CC; database = Registration; integrated security = true"))
            {
                try
                {

                    db.BeginTransaction();

                    db.ExecuteNonQuery("insert into Registration (FirstName, LastName ,Email, Sex, [Birth Date], [User Name], Password) values(@FirstN, @LastN, @Email, @Sex , @BD, @UserN, @Pass)",
                        new SqlParameter("@FirstN", "ბრძოლვერდი"),
                        new SqlParameter("@LastN", "გაწერელია"),
                        new SqlParameter("@Email", "Brdzolo@gmail.com"),
                        new SqlParameter("@Sex", "M"),
                        new SqlParameter("@BD", new DateTime(1958, 10, 5)),
                        new SqlParameter("@UserN", "BrdzolikoGawo"),
                        new SqlParameter("@Pass", "BrdzoloNatelasgiji"));

                    db.ExecuteNonQuery("insert into  Registration (FirstName, LastName ,Email, Sex, [Birth Date], [User Name], Password) values(@FirstN, @LastN, @Email, @Sex , @BD, @UserN, @Pass)",
                            new SqlParameter("@FirstN", "ჯიმშერი"),
                            new SqlParameter("@LastN", "ბერდიმუხამედოვი"),
                            new SqlParameter("@Email", "Jimsher@gmail.com"),
                            new SqlParameter("@Sex", "M"),
                            new SqlParameter("@BD", new DateTime(1944, 07, 3)),
                            new SqlParameter("@UserN", "Jimshera"),
                            new SqlParameter("@Pass", "Jimsho123"));

                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollBackTransaction();
                }
            }
        }
    }
}
