using Data.TheDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class MySqlHelper
    {
        public static void GetList()
        {
            using (var db = new UserContext())
            {
                db.Add(new User { NAME = "eee", AGE = "22" });
                db.SaveChanges();

                User s = db.Find<User>(2);
                db.SaveChanges();

            }
        }

    }
}
