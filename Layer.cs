using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tryagain
{
    internal class Layer
    {
        public Dictionary<int, Neuron> N = new Dictionary<int, Neuron>();
        public double Bias = 0;

        public Layer(int numNThis, int numNLast)
        {
            for (int i = 0; i < numNThis; i++)
            {
                N.Add(i, new Neuron(numNLast));
            }
        }
    }
}
