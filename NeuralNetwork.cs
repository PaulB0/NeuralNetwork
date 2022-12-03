using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tryagain
{
    internal class NeuralNetwork
    {
        public Dictionary<int, Layer> L;
        double learnRate;

        int numInputN, numHiddenN, numOutputN;

        public NeuralNetwork(int _numInputN, int _numHiddenN, int _numOutputN, double _learnRate)
        {
            L = new Dictionary<int, Layer>();

            L.Add(0, new Layer(_numInputN, 0));
            L.Add(1, new Layer(_numHiddenN, _numInputN));
            //L.Add(2, new Layer(numHiddenN, numHiddenN));
            L.Add(2, new Layer(_numOutputN, _numHiddenN));

            numInputN = _numInputN;
            numHiddenN = _numHiddenN;
            numOutputN = _numOutputN;

            learnRate = _learnRate;
        }

        public void SetStartWeightsBias()
        {

            L[1].N[0].weight[0] = 0.15;
            L[1].N[0].weight[1] = 0.20;

            L[1].N[1].weight[0] = 0.25;
            L[1].N[1].weight[1] = 0.30;

            L[2].N[0].weight[0] = 0.4;
            L[2].N[0].weight[1] = 0.45;


            L[1].Bias = 0.35;
            L[2].Bias = 0.60;
        }


        public void SetInputs(List<double> inputs)
        {
            int i = 0;

            foreach (var input in inputs)
            {
                L[0].N[i].output = input;
                i++;
            }



            //for (int j = 0; j < numHiddenN; j++)
            //{
            //    for (int k = 0; k < numInputN; k++)
            //    {
            //        L[1].N[j].weight[k] = 0.2;
            //    }
            //}


            //for (int j = 0; j < numOutputN; j++)
            //{
            //    for (int k = 0; k < numHiddenN; k++)
            //    {
            //        L[2].N[j].weight[k] = 0.2;
            //    }
            //}




        }

        public void SetInputsOrig()
        {
            L[0].N[0].output = 0.05;
            L[0].N[1].output = 0.10;

            L[1].Bias = 0.35;
            L[2].Bias = 0.60;

            L[1].N[0].weight[0] = 0.15;
            L[1].N[0].weight[1] = 0.20;

            L[1].N[1].weight[0] = 0.25;
            L[1].N[1].weight[1] = 0.30;

            L[2].N[0].weight[0] = 0.4;
            L[2].N[0].weight[1] = 0.45;

            L[2].N[1].weight[0] = 0.5;
            L[2].N[1].weight[1] = 0.55;

            L[2].N[0].target = 0.01;
            L[2].N[1].target = 0.99;
        }

        public void SetTarget(double target)
        {
            L[2].N[0].target = target;
        }

        public void ProcessNetwork()
        {

            for (int i = 1; i < L.Count; i++)
            {
                for (int j = 0; j < L[i].N.Count; j++)
                {
                    Neuron N = L[i].N[j];
                    N.output = 0;

                    for (int k = 0; k < L[i - 1].N.Count; k++)
                    {
                        N.output += L[i - 1].N[k].output * N.weight[k];
                    }
                    N.output += L[i].Bias;
                    N.output = Sigmoid(N.output);
                }
            }
        }

        public double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public double TargetError()
        {
            double error = 0;
            foreach (Neuron N in L[2].N.Values)
            {
                error += 0.5 * Math.Pow(N.target - N.output, 2);
            }
            return error;

        }

        public double BackProp()
        {
            for (int i = L.Count - 1; i > 0; i--) // for each level working backward from end
            {
                for (int j = 0; j < L[i].N.Count; j++) // for each neuron in level
                {
                    Neuron N = L[i].N[j];

                    int numInputsToNeuron = L[i - 1].N.Count;

                    double[] wtCorrArray = new double[numInputsToNeuron];

                    for (int k = 0; k < numInputsToNeuron; k++)// for each input to neuron
                    {

                        if (i == L.Count - 1) // output layer
                            N.dE_dout = N.output - N.target;

                        else if (i > 0) // hidden layer
                        {
                            N.dE_dout = 0;
                            foreach (Neuron nextL_N in L[i + 1].N.Values) // summing dE_dout for each higher level neuron
                            {
                                N.dE_dout += nextL_N.dE_dout * nextL_N.dout_dnet * nextL_N.weight[j];
                            }
                        }

                        N.dout_dnet = N.output * (1 - N.output);
                        N.dnet_dw = L[i - 1].N[k].output; ;

                        double dEtot_dw = N.dE_dout * N.dout_dnet * N.dnet_dw;

                        // new weight                   
                        // N.weightNew[k] = N.weight[k] - learnRate * dEtot_dw;

                        wtCorrArray[k] = dEtot_dw;
                    }

                    N.wtCorr.Add(wtCorrArray);
                }

            }

            return 0;
        }

        //public void ResetOutputs()
        //{
        //    for (int i = 1; i < L.Count; i++)
        //    {
        //        foreach (Neuron N in L[i].N.Values)
        //        {
        //            //for (int n = 0; n < L[i - 1].N.Count; n++)
        //            //{
        //            N.output = 0;
        //            //}
        //        }
        //    }
        //}

        //public void ResetWeightArrays()
        //{
        //    for (int i = 1; i < L.Count; i++)
        //    {
        //        foreach (Neuron N in L[i].N.Values)
        //        {
        //            N.wtCorr.Clear();
        //        }
        //    }
        //}

        //public void UpdateWeights()
        //{
        //    for (int i = 1; i < L.Count; i++)
        //    {
        //        foreach (Neuron N in L[i].N.Values)
        //        {
        //            for (int n = 0; n < L[i - 1].N.Count; n++)
        //            {
        //                N.weight[n] = N.weightNew[n];
        //            }
        //        }
        //    }
        //}

        public void UpdateWeights2()
        {
            for (int i = 1; i < L.Count; i++)
            {
                foreach (Neuron N in L[i].N.Values)
                {
                    for (int n = 0; n < L[i - 1].N.Count; n++)
                    {
                        // average all weight corrections
                        double average = 0;
                        foreach (double[] d in N.wtCorr)
                            average += d[n];

                        average /= N.wtCorr.Count;

                        N.weight[n] = N.weight[n] - learnRate * average; ;
                        //N.weightNew[k] = N.weight[k] - learnRate * dEtot_dw;
                    }

                    N.wtCorr.Clear();
                }
            }





        }


    }
}
