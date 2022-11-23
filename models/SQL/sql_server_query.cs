using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace basicClasses.models.SQL
{
    [info("filler")]
    [appliable("Action exe all")]
    class sql_server_query : ModelBase
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

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(Connection) && !string.IsNullOrWhiteSpace(spec.V(Connection)))
                curr_conn = spec.V(Connection);
           
            opis trtrt = ConnectAndQuery(curr_conn, spec.V(Query_text), spec.isHere(Capasity) && spec[Capasity].isInitlze ? spec[Capasity].intVal : 10000);

            message.body = "";          
            message.CopyArr(trtrt);          
        }


        public static void ConnectAndUpdate(string connectionString, string queryString)
        {
            try
            {
                using (var connection = new SqlConnection(
                  connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
               // GlobalOnlineLog.AddAnalytic(210, "BulkInsertSql.ConnectAndUpdate  ERROR", e.Message, e.StackTrace + e.InnerException, "");
            }
        }

        public static string[] GetColumnNames(SqlDataReader dr)
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
                using (var connection = new SqlConnection(
                  connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = 600;
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

    }

}
