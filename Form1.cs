using System.Data;
using System.Data.SqlClient;

namespace tryagain
{
    public partial class Form1 : Form
    {

        NeuralNetwork NN;
        SQL Sql = new SQL();


        public Form1()
        {
            InitializeComponent();

            bool useTest1 = false;
            bool useTest2 = false;
            bool useTest3 = false;
            bool useTest5 = true;



            //Sql.LoadFTSE100();
            //Sql.LoadStocks();


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
                NN = new NeuralNetwork(10, 10, 1, 0.5);
                TestRef3(NN, Sql);
            }

            if (useTest5)
            {
                NN = new NeuralNetwork(100, 100, 1, 0.5);
                TestRef5(NN, Sql);
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
                    Sql.WriteLog(Etot, NN.L[2].N[0].target, NN.L[2].N[0].output);

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
                    Sql.WriteLog(Etot, target, NN.L[2].N[0].output);
            }
        }

        private void TestRef3(NeuralNetwork NN, SQL Sql)
        {
            double Etot = 0;


            DateOnly targetDay = new DateOnly(2017, 4, 15); // last training target day + 3
            int trainingDuration = 15; // days

            DateOnly from = targetDay.AddDays(-trainingDuration);
            DateOnly to = targetDay.AddDays(-1);


            NN.SetStartWeightsBias();

            int finalTrainingTargetDay = 0;


            for (int k = 0; k < 15; k++)
            {

                from = from.AddDays(1);
                to = to.AddDays(1);

                var dataArrays = Sql.GetInputArray(from, to);
                decimal[,] stockVals = dataArrays.Item1;
                decimal[,] targetVals = dataArrays.Item2;


                //int numStocks = stockVals.GetLength(0);
                int numDates = stockVals.GetLength(1);

                for (int j = 0; j < 2000; j++)
                {
                    // percent calc sets start at day 2, target sets end at last day-1 
                    for (int dateIndex = 1; dateIndex < numDates - 1; dateIndex++)
                    //for (int dateIndex = 1; dateIndex < numDates - 4; dateIndex++)
                    {
                        NN.SetInputs2(stockVals, dateIndex, 1);
                        NN.SetTarget2(targetVals, dateIndex + 1);
                        //NN.SetTarget2(targetVals, dateIndex);

                        NN.ProcessNetwork();
                        NN.BackProp();

                        if (j % 1000 == 0)
                        {
                            //Etot = NN.TargetError();
                            //Sql.WriteLog(from, Etot, NN.L[2].N[0].target, NN.L[2].N[0].output);
                        }
                        finalTrainingTargetDay = dateIndex + 1;
                    }
                    NN.UpdateWeights2();

                    //NN.ProcessNetwork();
                    //Etot = NN.TargetError();
                    ////if (j % 10 == 0)
                    ////    Sql.WriteLog(from, Etot, 0, NN.L[2].N[0].output);

                }

                // test model on un-trained days

                //for (int i = 0; i < 1; i++)
                //{
                //    NN.SetInputs2(stockVals, finalTrainingTargetDay + i);
                //    NN.SetTarget2(targetVals, finalTrainingTargetDay + 1 + i);
                //    NN.ProcessNetwork();
                //    Sql.WriteLog(from, Etot, NN.L[2].N[0].target, NN.L[2].N[0].output);
                //}


            }

        }

                private void TestRef4(NeuralNetwork NN, SQL Sql)
        {
            // encompassing range of dates and data
            DateOnly approxSetStart = new DateOnly(2017, 2, 1);
            DateOnly approxSetEnd = new DateOnly(2017, 7, 1);
            int trainDuration = 30; // 15;// 30; // days
            int pcDeltaDays = 1;

            Dictionary<int, DateOnly> testDatesIntKey = Sql.DistinctDates(approxSetStart, approxSetEnd, true);
            Dictionary<DateOnly, int> testDatesDateKey = Sql.DistinctDates(approxSetStart, approxSetEnd, false);

            Dictionary<string, int> inputNamesDict = Sql.InputNames();
            int numInputs = inputNamesDict.Count;

            Dictionary<string, int> targetNamesDict = Sql.TargetNames();
            int numTargets = targetNamesDict.Count;

            int approxSetRange = approxSetEnd.DayNumber - approxSetStart.DayNumber;

            decimal[,] dataInputs = new decimal[numInputs, approxSetRange];
            decimal[,] dataTargets = new decimal[numTargets, approxSetRange];

            Sql.FillDataArrays(approxSetStart, approxSetEnd, testDatesDateKey, inputNamesDict, targetNamesDict, dataInputs, dataTargets);


            // given one prediction day index
            int pdIndex = testDatesIntKey.Count - 9;

            //for (int pdIndex = trainDuration+14; pdIndex <= testDatesIntKey.Count - 4; pdIndex++)
            //{

                OnePDsine(pdIndex);

            //}

            //Sql.LogWeights(NN);

        }

        private void TestRef5(NeuralNetwork NN, SQL Sql)
        {
            // encompassing range of dates and data
            DateOnly approxSetStart = new DateOnly(2017, 2, 1);
            DateOnly approxSetEnd = new DateOnly(2017, 5, 1);
            int trainDuration = 30; // 15;// 30; // days
            int pcDeltaDays = 4;

            Dictionary<int, DateOnly> testDatesIntKey = Sql.DistinctDates(approxSetStart, approxSetEnd, true);
            Dictionary<DateOnly, int> testDatesDateKey = Sql.DistinctDates(approxSetStart, approxSetEnd, false);

            Dictionary<string, int> inputNamesDict = Sql.InputNames();
            int numInputs = inputNamesDict.Count;

            Dictionary<string, int> targetNamesDict = Sql.TargetNames();
            int numTargets = targetNamesDict.Count;

            int approxSetRange = approxSetEnd.DayNumber - approxSetStart.DayNumber;

            decimal[,] dataInputs = new decimal[numInputs, approxSetRange];
            decimal[,] dataTargets = new decimal[numTargets, approxSetRange];

            Sql.FillDataArrays(approxSetStart, approxSetEnd, testDatesDateKey, inputNamesDict, targetNamesDict, dataInputs, dataTargets);


            // given one prediction day index
            int pdIndex = 1;

 NN.SetStartWeightsBias2();

            for (int aaa = 1; aaa <= 400; aaa++)
            {
                OnePDsine(aaa);
            }

            OnePDsine(400);

        }


        private void OnePDsine(int pdIndex)
        {
           

            double nextval = NN.SetInputsSine(pdIndex);
            NN.SetTargetSine(nextval);


            for (int j = 0; j < 1; j++)
            {
                NN.ProcessNetwork();
                NN.BackProp();
                NN.UpdateWeights2();              
            }



            double Etot = NN.TargetError();
            Sql.WriteLog(Etot, NN.L[2].N[0].target, NN.L[2].N[0].output);


            nextval = NN.SetInputsSine(pdIndex + 1);
            //NN.SetTargetSine(nextval);

            NN.ProcessNetwork();
 
            Etot = NN.TargetError();
            Sql.WriteLog(Etot, nextval, NN.L[2].N[0].output);




        }


    }

}