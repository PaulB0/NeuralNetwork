using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using System.Globalization;



namespace tryagain
{
    internal class SQL
    {

        public SQL()
        {
            RunSQLCommand("delete from [dbo].[log]");
        }

        public List<DateOnly> GetDays(DateOnly from, DateOnly to)
        {
            List<DateOnly> list_dates = new List<DateOnly>();

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("get_days", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlParameter start_date = cmd.Parameters.Add("@start_date", SqlDbType.Date);
                    start_date.Direction = ParameterDirection.Input;
                    start_date.Value = from.ToString("yyyy-MM-dd"); // "2017-02-01";

                    SqlParameter end_date = cmd.Parameters.Add("@end_date", SqlDbType.Date);
                    end_date.Direction = ParameterDirection.Input;
                    end_date.Value = to.ToString("yyyy-MM-dd"); // "2017-02-01";

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        //string gg = dr["_date"].ToString();
                        //DateTime ss = DateTime.Parse(dr["_date"].ToString());
                        DateOnly a_date = DateOnly.FromDateTime(DateTime.Parse(dr["_date"].ToString()));
                        //DateOnly percent = DateOnly.Parse(ss);
                        list_dates.Add(a_date);
                    }


                    cnn.Close();
                }
            }

            return list_dates;
        }

        public List<double> GetInputs(DateOnly D)
        {
            List<double> group = new List<double>();

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("get_inputs", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlParameter day = cmd.Parameters.Add("@day", SqlDbType.Date);
                    day.Direction = ParameterDirection.Input;
                    day.Value = D.ToString("yyyy-MM-dd"); // "2017-02-01";

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        //string gg = dr["_stock"].ToString();
                        double percent = Convert.ToDouble(dr["pc"]);
                        group.Add(percent);
                    }


                    cnn.Close();
                }
            }

            return group;
        }

        public double GetTarget(DateOnly D)
        {
            double target = 0;

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("get_target", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlParameter day = cmd.Parameters.Add("@day", SqlDbType.Date);
                    day.Direction = ParameterDirection.Input;
                    day.Value = D.ToString("yyyy-MM-dd"); // "2017-02-01";

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        //string gg = dr["_stock"].ToString();
                        double percent = Convert.ToDouble(dr["pc"]);
                        target = percent;
                    }


                    cnn.Close();
                }
            }

            return target;
        }

        public void WriteLog(DateOnly D, double error, double target, double output)
        {

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("write_log", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlParameter sql_day = cmd.Parameters.Add("@day", SqlDbType.Date);
                    sql_day.Direction = ParameterDirection.Input;
                    sql_day.Value = D.ToString("yyyy-MM-dd"); // "2017-02-01";

                    SqlParameter sql_error = cmd.Parameters.Add("@error", SqlDbType.Float);
                    sql_error.Direction = ParameterDirection.Input;
                    sql_error.Value = error;

                    SqlParameter sql_target = cmd.Parameters.Add("@target", SqlDbType.Float);
                    sql_target.Direction = ParameterDirection.Input;
                    sql_target.Value = target;

                    SqlParameter sql_output = cmd.Parameters.Add("@output", SqlDbType.Float);
                    sql_output.Direction = ParameterDirection.Input;
                    sql_output.Value = output;



                    cmd.ExecuteNonQuery();


                    cnn.Close();
                }
            }
        }

        public void RunSQLCommand(string query)
        {
            SqlConnection cnn;
            cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2");
            cnn.Open();

            //string sql = "insert into price_history (stock, date, [open], max, min, [close], volume) values (@stock, @date, @open, @max, @min, @close, @volume)";
            SqlCommand cmd = new SqlCommand(query, cnn);

            cmd.ExecuteNonQuery();
            cnn.Close();
        }


            public void LoadStocks()
        {
            SqlConnection cnn;
            cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2");
            cnn.Open();

            string sql = "insert into price_history (stock, date, [open], max, min, [close], volume) values (@stock, @date, @open, @max, @min, @close, @volume)";
            SqlCommand cmd = new SqlCommand(sql, cnn);

            cmd.Parameters.Add("@stock", SqlDbType.Char);
            cmd.Parameters.Add("@date", SqlDbType.Date);
            cmd.Parameters.Add("@open", SqlDbType.Money);
            cmd.Parameters.Add("@max", SqlDbType.Money);
            cmd.Parameters.Add("@min", SqlDbType.Money);
            cmd.Parameters.Add("@close", SqlDbType.Money);
            cmd.Parameters.Add("@volume", SqlDbType.Int);

            string[] subDirs = System.IO.Directory.GetDirectories(@"D:\Home\EODDATA_Short");

            foreach (string sd in subDirs)
            {
                string[] files = System.IO.Directory.GetFiles(sd, "*.txt");
                foreach (string fi in files)
                {
                    string[] lines = System.IO.File.ReadAllLines(fi);
                    foreach (string line in lines)
                    {

                        string[] fields = line.Split(',');
                        string _stock = fields[0];
                        DateTime _date = DateTime.ParseExact(fields[1], "yyyyMMdd", CultureInfo.InvariantCulture);

                        decimal _open = Decimal.Parse(fields[2]);
                        decimal _max = Decimal.Parse(fields[3]);
                        decimal _min = Decimal.Parse(fields[4]);
                        decimal _close = Decimal.Parse(fields[5]);
                        int _volume = int.Parse(fields[6]);

                        cmd.Parameters["@stock"].Value = _stock;
                        cmd.Parameters["@date"].Value = _date;
                        cmd.Parameters["@open"].Value = _open;
                        cmd.Parameters["@max"].Value = _max;
                        cmd.Parameters["@min"].Value = _min;
                        cmd.Parameters["@close"].Value = _close;
                        cmd.Parameters["@volume"].Value = _volume;

                        cmd.ExecuteNonQuery();

                    }
                }
            }

            cnn.Close();

        }

    }

}
