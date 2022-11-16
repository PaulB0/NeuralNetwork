namespace tryagain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            int numInputN = 6;
            int numHiddenN = 1000; // 100;// 6;
            int numOutputN = 1;
            double learnRate = 0.5;
            double Etot = 0;


            NeuralNetwork NN = new NeuralNetwork(numInputN, numHiddenN, numOutputN, learnRate);

            SQL Sql = new SQL(NN);

            Sql.RunSQLCommand("delete from [dbo].[log]");


            DateOnly D = new DateOnly(2017, 2, 1); // start of training

            List<DateOnly> dates = Sql.GetDays(D, D.AddDays(10));

            // NEED TO AVERAGE NEW WEIGHTS ACROSS ALL TRAINING
            // PLUS TRY SPECCING ALL WEIGHTS - NOT JUST LEAVE AS ZERO

            //for (int j = 0; j < 10; j++)
            //{

            for (int i = 0; i < dates.Count - 1; i++)
            {
                //Etot = 10;
                //while (Etot > 0.002)
                //{
                List<double> inputs = Sql.GetInputs(dates[i]);
                NN.SetInputs(inputs);

                double target = Sql.GetTarget(dates[i + 1]);
                NN.SetTarget(target);

                NN.ProcessNetwork();
                Etot = NN.TargetError();

                NN.BackProp();
                //NN.UpdateWeights();

                Sql.WriteLog(dates[i], Etot, target, NN.L[2].N[0].output);
            }

            NN.UpdateWeights2();


            for (int i = 0; i < dates.Count - 1; i++)
            {
                //Etot = 10;
                //while (Etot > 0.002)
                //{
                List<double> inputs = Sql.GetInputs(dates[i]);
                NN.SetInputs(inputs);

                double target = Sql.GetTarget(dates[i + 1]);
                NN.SetTarget(target);

                NN.ProcessNetwork();
                Etot = NN.TargetError();

                NN.BackProp();
                //NN.UpdateWeights();

                Sql.WriteLog(dates[i], Etot, target, NN.L[2].N[0].output);
            }








            //}

            //}


            int r = 2;


        }
    }
}