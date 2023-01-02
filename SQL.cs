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
            RunSQLCommand("delete from [dbo].[weights]");
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

        public dynamic DistinctDates(DateOnly from, DateOnly to, bool intkey)
        {
            Dictionary<DateOnly, int> dict_by_dates = new Dictionary<DateOnly, int>();
            Dictionary<int, DateOnly> dict_by_count = new Dictionary<int, DateOnly>();

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("distinct_dates", cnn))
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

                    int i = 0;
                    while (dr.Read())
                    {
                        DateOnly dataDate = DateOnly.FromDateTime(DateTime.Parse(dr["_date"].ToString()));
                        dict_by_dates.Add(dataDate, i);
                        dict_by_count.Add(i, dataDate);
                        i++;
                    }
                    cnn.Close();
                }
            }

            if (!intkey)
                return dict_by_dates;
            else
                return dict_by_count;
        }

        //public Dictionary<string, int> AllNames()
        //{
        //    Dictionary<string, int> dict_input_name = new Dictionary<string, int>();

        //    using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("all_names", cnn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cnn.Open();

        //            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //            int count = 0;

        //            while (dr.Read())
        //            {
        //                dict_input_name.Add(dr["_stock"].ToString(), count);
        //                count++;
        //            }
        //            cnn.Close();
        //        }
        //    }
        //    return dict_input_name;
        //}


        public Dictionary<string, int> InputNames()
        {
            Dictionary<string, int> dict_input_name = new Dictionary<string, int>();

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("input_names", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    int count = 0;

                    while (dr.Read())
                    {
                        dict_input_name.Add(dr["_stock"].ToString(), count);
                        count++;
                    }
                    cnn.Close();
                }
            }
            return dict_input_name;
        }

        public Dictionary<string, int> TargetNames()
        {
            Dictionary<string, int> dict_target_name = new Dictionary<string, int>();

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("target_names", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    int count = 0;

                    while (dr.Read())
                    {
                        dict_target_name.Add(dr["_stock"].ToString(), count);
                        count++;
                    }
                    cnn.Close();
                }
            }
            return dict_target_name;
        }


        public void FillDataArrays(DateOnly approxSetStart, DateOnly approxSetEnd, Dictionary<DateOnly, int> testDatesDateKey,
            Dictionary<string, int> inputNamesDict, Dictionary<string, int> targetNamesDict, decimal[,] dataInputs, decimal[,] dataTargets)
        {

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("get_all_stock_vals", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlParameter start_date = cmd.Parameters.Add("@start_date", SqlDbType.Date);
                    start_date.Direction = ParameterDirection.Input;
                    start_date.Value = approxSetStart.ToString("yyyy-MM-dd");

                    SqlParameter end_date = cmd.Parameters.Add("@end_date", SqlDbType.Date);
                    end_date.Direction = ParameterDirection.Input;
                    end_date.Value = approxSetEnd.ToString("yyyy-MM-dd");

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        DateOnly dataDate = DateOnly.FromDateTime(DateTime.Parse(dr["_date"].ToString()));
                        int dateIndex = testDatesDateKey[dataDate];

                        decimal val = (decimal)dr["_close"];
                        string stockName = dr["_stock"].ToString();

                        if (inputNamesDict.ContainsKey(stockName))
                        {
                            int stockIndex = inputNamesDict[stockName];
                            dataInputs[stockIndex, dateIndex] = val;
                        }
                        if (targetNamesDict.ContainsKey(stockName))
                        {
                            int stockIndex = targetNamesDict[stockName];
                            dataTargets[stockIndex, dateIndex] = val;
                        }
                    }
                    cnn.Close();

                }
            }

        }


        public (decimal[,], decimal[,]) GetInputArray(DateOnly from, DateOnly to)
        {
            Dictionary<string, int> dict_Inputs = InputNames();
            int numStocks = dict_Inputs.Count;

            Dictionary<string, int> dict_Targets = InputNames();
            int numTargets = dict_Targets.Count;

            Dictionary<DateOnly, int> distinctDates = DistinctDates(from, to, false);
            int numDates = distinctDates.Count;

            decimal[,] dataInputs = new decimal[numStocks, numDates];
            decimal[,] dataTargets = new decimal[numTargets, numDates];

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("get_input_target_vals", cnn))
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
                        DateOnly dataDate = DateOnly.FromDateTime(DateTime.Parse(dr["_date"].ToString()));
                        int dateIndex = distinctDates[dataDate];

                        int targ = (int)dr["_target"];

                        if (targ == 0)
                        {
                            string input = dr["_stock"].ToString();
                            int inputIndex = dict_Inputs[input];
                            decimal val = (decimal)dr["_close"];
                            dataInputs[inputIndex, dateIndex] = val;
                        }
                        else
                        {
                            string target = dr["_stock"].ToString();
                            int targetIndex = dict_Targets[target];
                            decimal val = (decimal)dr["_close"];
                            dataTargets[targetIndex, dateIndex] = val;
                        }

                    }

                    cnn.Close();
                }
            }

            return (dataInputs, dataTargets);
        }

        //public SqlDataReader GetInputArray22(DateOnly from, DateOnly to)
        //{
        //    //Dictionary<string, int> dict_Inputs = InputTargetNames(0);
        //    //int numStocks = dict_Inputs.Count;

        //    //Dictionary<DateOnly, int> distinctDates = DistinctDates(from, to, false);
        //    //int numDates = distinctDates.Count;

        //    //decimal[,] dataInputs = new decimal[numStocks, numDates];
        //    //decimal[,] dataTargets = new decimal[numTargets, numDates];

        //    using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("get_input_target_vals", cnn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cnn.Open();

        //            SqlParameter start_date = cmd.Parameters.Add("@start_date", SqlDbType.Date);
        //            start_date.Direction = ParameterDirection.Input;
        //            start_date.Value = from.ToString("yyyy-MM-dd"); // "2017-02-01";

        //            SqlParameter end_date = cmd.Parameters.Add("@end_date", SqlDbType.Date);
        //            end_date.Direction = ParameterDirection.Input;
        //            end_date.Value = to.ToString("yyyy-MM-dd"); // "2017-02-01";

        //            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //            //while (dr.Read())
        //            //{
        //            //    DateOnly dataDate = DateOnly.FromDateTime(DateTime.Parse(dr["_date"].ToString()));
        //            //    int dateIndex = distinctDates[dataDate];

        //            //    int targ = (int)dr["_target"];

        //            //    if (targ == 0)
        //            //    {
        //            //        string input = dr["_stock"].ToString();
        //            //        int inputIndex = dict_Inputs[input];
        //            //        decimal val = (decimal)dr["_close"];
        //            //        dataInputs[inputIndex, dateIndex] = val;

        //            //    }
        //            //}

        //            cnn.Close();
        //        }
        //    }

        //    return (dr);
        //}


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

        public void WriteLog(double error, double target, double output)
        {

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("write_log", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    //SqlParameter sql_day = cmd.Parameters.Add("@day", SqlDbType.Date);
                    //sql_day.Direction = ParameterDirection.Input;
                    //sql_day.Value = D.ToString("yyyy-MM-dd"); // "2017-02-01";

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

        public void LogWeights(NeuralNetwork NN)
        {

            using (SqlConnection cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2"))
            {
                using (SqlCommand cmd = new SqlCommand("log_weights", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cnn.Open();

                    SqlParameter sql_level = cmd.Parameters.Add("@level", SqlDbType.Int);
                    sql_level.Direction = ParameterDirection.Input;

                    SqlParameter sql_node = cmd.Parameters.Add("@node", SqlDbType.Int);
                    sql_node.Direction = ParameterDirection.Input;

                    SqlParameter sql_node_1 = cmd.Parameters.Add("@node_1", SqlDbType.Int);
                    sql_node_1.Direction = ParameterDirection.Input;

                    SqlParameter sql_weight = cmd.Parameters.Add("@weight", SqlDbType.Float);
                    sql_weight.Direction = ParameterDirection.Input;


                    for (int nn = 0; nn < NN.L[1].N.Count; nn++)
                    {
                        for (int nn_1 = 0; nn_1 < NN.L[0].N.Count; nn_1++)
                        {
                            sql_level.Value = 1;
                            sql_node.Value = nn;
                            sql_node_1.Value = nn_1;
                            sql_weight.Value = NN.L[1].N[nn].weight[nn_1];
                            cmd.ExecuteNonQuery();
                        }
                    }

                    for (int nn = 0; nn < NN.L[2].N.Count; nn++)
                    {
                        for (int nn_1 = 0; nn_1 < NN.L[1].N.Count; nn_1++)
                        {
                            sql_level.Value = 2;
                            sql_node.Value = nn;
                            sql_node_1.Value = nn_1;
                            sql_weight.Value = NN.L[2].N[nn].weight[nn_1];
                            cmd.ExecuteNonQuery();
                        }
                    }




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

            string sql = "insert into history (_stock, _date, [_open], _max, _min, [_close], _volume) values (@stock, @date, @open, @max, @min, @close, @volume)";
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

        public void LoadFTSE100()
        {
            SqlConnection cnn;
            cnn = new SqlConnection(@"Data Source=127.0.0.1,14333;Initial Catalog=Stocks;User ID=sa;Password=Sysadmin2");
            cnn.Open();

            string sql = "insert into [group] (_stock, _target) values (@stock,0)";
            SqlCommand cmd = new SqlCommand(sql, cnn);
            cmd.Parameters.Add("@stock", SqlDbType.Char);

            string[] lines = System.IO.File.ReadAllLines(@"D:\Home\ftse100.txt");
            foreach (string line in lines)
            {
                cmd.Parameters["@stock"].Value = line;
                cmd.ExecuteNonQuery();
            }

            cnn.Close();

        }

    }
}
