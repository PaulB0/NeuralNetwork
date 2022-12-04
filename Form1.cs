namespace tryagain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            bool useTest1 = false;
            bool useTest2 = false;
            bool useTest3 = true;

            NeuralNetwork NN;
            SQL Sql = new SQL();

            if (useTest1)
            {
                NN = new NeuralNetwork(2, 2, 2, 0.5);
                TestRef1(NN, Sql);
            }

            if (useTest2)
            {
                NN = new NeuralNetwork(2, 2, 1, 0.5);
                TestRef2(NN, Sql);
            }

            if (useTest3)
            {
                NN = new NeuralNetwork(2, 2, 1, 0.5);
                TestRef3(NN, Sql);
            }

        }


        private void TestRef1(NeuralNetwork NN, SQL Sql)
        {
            double Etot = 0;

            NN.SetInputsOrig();

            // repeat everything enough to reduce error
            for (int i = 0; i < 10000; i++)
            {
                // run through all training data and average the weight corrections
                for (int j = 0; j < 2; j++)
                {
                    NN.ProcessNetwork();
                    NN.BackProp();
                }

                NN.UpdateWeights2();

                NN.ProcessNetwork();
                Etot = NN.TargetError();

                if (i % 1000 == 0)
                    Sql.WriteLog(new DateOnly(2017, 2, 5), Etot, 0.01, NN.L[2].N[0].output);

            }

        }

        private void TestRef2(NeuralNetwork NN, SQL Sql)
        {
            double Etot = 0;
            DateOnly D = new DateOnly(2017, 2, 5); // start of training
            List<DateOnly> dates = Sql.GetDays(D, D.AddDays(10));

            NN.SetStartWeightsBias();

            List<double> inputs = Sql.GetInputs(dates[0]);
            NN.SetInputs(inputs);

            //double target = Sql.GetTarget(dates[i + 1]);
            double target = Sql.GetTarget(dates[0]); // test with ADM
            NN.SetTarget(target);

            for (int j = 0; j < 20000; j++)
            {
                NN.ProcessNetwork();
                NN.BackProp();
                NN.UpdateWeights2();

                Etot = NN.TargetError();
                if (j % 1000 == 0)
                    Sql.WriteLog(dates[0], Etot, target, NN.L[2].N[0].output);
            }
        }

        private void TestRef3(NeuralNetwork NN, SQL Sql)
        {
            double Etot = 0;

            DateOnly from = new DateOnly(2017, 2, 5); // start of training
            DateOnly to = new DateOnly(2017, 2, 20); // start of training

            decimal[,] stockVals = Sql.GetInputArray(from, to);

            int numStocks = stockVals.GetLength(0);
            int numDates = stockVals.GetLength(1);

            NN.SetStartWeightsBias();

            for (int j = 0; j < 2000; j++)
            {
                // percent calc requires start at day 2
                for (int dateIndex = 1; dateIndex < numDates - 1; dateIndex++)
                {
                    NN.SetInputs2(stockVals, dateIndex);

                    NN.ProcessNetwork();
                    NN.BackProp();
                }
                NN.UpdateWeights2();

                NN.ProcessNetwork();
                Etot = NN.TargetError();

                if (j % 10 == 0)
                    Sql.WriteLog(from, Etot, 0, NN.L[2].N[0].output);

            }
        }


    }
}