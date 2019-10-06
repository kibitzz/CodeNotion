using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace basicClasses.models.SQL
{
    [info("filler")]
    [appliable("Action exe all")]
    class MySqlQuery: ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string Connection = "Connection";

        [info("")]
        [model("")]
        public static readonly string Query_text = "Query_text";

        [info("for optimization at big datasets. by default 10k records")]
        [model("")]
        public static readonly string Capasity = "Capasity";

        string curr_conn;
        //[info("")]
        //[model("")]
        //public static readonly string Connection = "Connection";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(Connection) && !string.IsNullOrWhiteSpace(spec.V(Connection)))
                curr_conn = spec.V(Connection);
           

            opis trtrt = ConnectAndQuery(curr_conn, spec.V(Query_text), spec.isHere(Capasity) && spec[Capasity].isInitlze ? spec[Capasity].intVal : 10000);


            message.body = "";          
            message.CopyArr(trtrt);
            //message.AddArr(trtrt);

        }


        public static void ConnectAndUpdate(string connectionString, string queryString)
        {
            try
            {
                using (var connection = new MySqlConnection(
                  connectionString))
                {
                    var command = new MySqlCommand(queryString, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
               // GlobalOnlineLog.AddAnalytic(210, "BulkInsertSql.ConnectAndUpdate  ERROR", e.Message, e.StackTrace + e.InnerException, "");
            }
        }

        public static string[] GetColumnNames(MySqlDataReader dr)
        {
            var rez = new string[dr.FieldCount];

            for (int i = 0; i < rez.Length; i++)
                rez[i] = dr.GetName(i);


            return rez;
        }


        public static opis ConnectAndQuery(string connectionString, string queryString, int cap = 10000)
        {
            opis rez = new opis(cap);

            try
            {
                using (var connection = new MySqlConnection(
                  connectionString))
                {
                    var command = new MySqlCommand(queryString, connection);
                    command.Connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return rez;

                         var cn = GetColumnNames(reader);

                        while (reader.Read())
                        {
                            var itm = new opis();
                            for (int i = 0; i < cn.Length; i++)
                            {
                                itm[cn[i]].body = reader[i].ToString();
                            }
                            rez.AddArr(itm);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rez["Exception"]["Message"].body = e.Message;
                rez["Exception"]["StackTrace"].body = e.StackTrace;
              //  rez["Exception"]["InnerException"].body = e.InnerException.ToString();
            }

            return rez;
        }

        //public static string ComposeBulkInsert(List<ApiUserEvent> l)
        //{
        //    var sb = new StringBuilder(l.Count + 11);
        //    sb.Append("INSERT INTO `ApiUserEvents` (EventDate,EventHour,EventType,UserId,GroupId,TeacherId,EntityId,UniversityId) VALUES ");


        //    foreach (var i in l.Take(l.Count - 1))
        //        sb.Append("('" + i.EventDate.Date.ToString("yyyy-MM-dd HH:mm:ss") + "',"
        //            + i.EventDate.Hour
        //            + "," + i.EventType   //  remove Bearer string at start
        //            + "," + (i.UserToken != null ? AccessTokenContext.DeserializeToken(i.UserToken.Remove(0, 7)).UserId : 0) //i.UserId
        //            + "," + i.GroupId
        //            + "," + i.TeacherId
        //            + "," + i.EntityId
        //             + "," + i.UniversityId + "),");

        //    var ii = l.TakeLast(1).First();
        //    sb.Append("('" + ii.EventDate.Date.ToString("yyyy-MM-dd HH:mm:ss") + "',"
        //           + ii.EventDate.Hour
        //           + "," + ii.EventType   //  remove Bearer string at start
        //           + "," + (ii.UserToken != null ? AccessTokenContext.DeserializeToken(ii.UserToken.Remove(0, 7)).UserId : 0) //i.UserId
        //           + "," + ii.GroupId
        //           + "," + ii.TeacherId
        //           + "," + ii.EntityId
        //            + "," + ii.UniversityId + ")");



        //    sb.Append(";");

        //    return sb.ToString();
        //}


    }

}
