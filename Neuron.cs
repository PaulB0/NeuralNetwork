using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tryagain
{

    internal class Neuron
    {
        public double[] weight;
        public double[] weightNew;
        public List<double[]> wtCorr;
        public double output = 0;
        public double target = 0;


        public double dE_dout = 0;
        public double dout_dnet = 0;
        public double dnet_dw = 0;


        public Neuron(int numNLastLayer)
        {
            weight = new double[numNLastLayer];
            weightNew = new double[numNLastLayer];
            wtCorr = new List<double[]>();
        }


    }
}
