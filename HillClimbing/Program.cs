using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System;

namespace HillClimbing {
    class Program {

        static void Main(string[] args) {
            Program prg = new Program();
            prg.Run();
        }

        public void Run() {

             // get iris file from resource stream
            Assembly assembly = Assembly.GetExecutingAssembly();
            var f = assembly.GetManifestResourceStream("HillClimbing.Resources.iris.csv");
            StreamReader sr = new StreamReader(f);

            // load all records into new dataset
            DataSet ds = DataSet.Load(sr);
            sr.Close();

            Dictionary<string, int> species = ds.EncodeOneOfN(4); // encode specie names using one of n encoding

            // get a list of records with data and encoded class name
            List<Record> trainingData = ds.ExtractSupervised(0, 4, 4, 3);

            Network net = new Network(4, 4, 2); // new RBF network
            net.Randomize(); // initialize the networks state
            Scorer score = new Scorer(trainingData); // new scorer
            HillClimb hc = new HillClimb(net, score, 1.2, 1); // new hill climbing training algorithm
            Iterate(hc, 100, 0.01); // iterate through hill climbing algorithm
            QueryOneOfN(net, trainingData, species); // display ideal and actual specie names
        }

        /// <summary>
        /// Iterate through a hill climbing algorithm.
        /// Tries to minimize score.
        /// </summary>
        /// <param name="hc">Algorithm to iterate through.</param>
        /// <param name="numIterations">Maximum iterations.</param>
        /// <param name="minScore">Minimum score.</param>
        private void Iterate(HillClimb hc, int numIterations, double minScore) {
            int iterationNumber = 0; // number of iterations
            bool done = false; // is the algorithm done?

            // while not done...
            do {
                iterationNumber++; // increase number of iterations
                hc.Iteration(); // run an iteration

                // if iteration reaches limit or score reaches min score
                if(iterationNumber >= numIterations || hc.LastError < minScore) {
                    done = true; // set done = true
                }

                // write out iteration # and score
                Console.WriteLine("Iteration #" + iterationNumber + ", Score=" + hc.LastError + " (Minimize)");
            } while(!done);
        }

        /// <summary>
        /// Goes through each record in the training set and displays the ideal output and the actual output.
        /// </summary>
        /// <param name="net">RBF network.</param>
        /// <param name="trainingData">Training data.</param>
        /// <param name="specieNames">Specie names.</param>
        private void QueryOneOfN(Network net, List<Record> trainingData, Dictionary<string, int> species) {
            // invert the specie list
            Dictionary<int, string> invSpecies = new Dictionary<int, string>();
            foreach(string key in species.Keys) {
                int value = species[key];
                invSpecies[value] = key;
            }

            // for each training record
            foreach(Record rec in trainingData) {

                double[] output = net.ComputeRegression(rec.Input); // run the input through the RBF network

                int idealIdx = GetMax(rec.Ideals); // find the index of the ideal name
                int actualIdx = GetMax(output); // find the index of the actual name

                Console.WriteLine("Guess: " + invSpecies[actualIdx] + " Actual: " + invSpecies[idealIdx]);
            }
        }

        /// <summary>
        /// Returns the index with the max value.
        /// </summary>
        /// <param name="da">Array to go through.</param>
        /// <returns>Index of the max value.</returns>
        private int GetMax(double[] da) {
            int result = -1;
            double max = double.MinValue;

            for(int i = 0; i < da.Length; i++) {
                if(da[i] > max) {
                    max = da[i];
                    result = i;
                }
            }

            return result;
        }
    }
}
