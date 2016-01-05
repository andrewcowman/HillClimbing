using System;

namespace HillClimbing {

    /// <summary>
    /// Network of radial basis functions.
    /// </summary>
    public class Network {

        private int _numInputs, _numOutputs; // number of inputs, outputs
        private int _indexInputs, _indexOutputs; // index for the inputs, outputs
        private double[] _longTermMemory; // long term weights and RBF parameters

        private GaussianFunction[] _rbfs; // array of rbfs: GaussianFunctions in this case

        /// <summary>
        /// Returns _longTermMemory.
        /// </summary>
        public double[] LongTermMemory {
            get { return _longTermMemory; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numInputs">Number of inputs.</param>
        /// <param name="numRBFs">Number of radial basis functions.</param>
        /// <param name="numOutputs">Number of outputs.</param>
        public Network(int numInputs, int numRBFs, int numOutputs) {

            this._numInputs = numInputs; // number of inputs
            this._numOutputs = numOutputs; // number of outputs

            int inputWeight = _numInputs * numRBFs; // total number of inputs in network

            // total number of outputs in network
            // + 1 because of bias RBF
            int outputWeight = _numOutputs * (numRBFs + 1);

            int rbfParams = (_numInputs + 1) * numRBFs; // total number of RBF width scalars and center vectors

            // allocate space for _longTermMemory
            _longTermMemory = new double[inputWeight + outputWeight + rbfParams];

            _indexInputs = 0; // inputs start at 0

            _indexOutputs = inputWeight + rbfParams; // outputs start after inputs and rbf params

            _rbfs = new GaussianFunction[numRBFs]; // create array of RBFs

            for(int i = 0; i < numRBFs; i++) {

                int rbfIndex = inputWeight + ((numInputs + 1) * i); // index of rbf parameters in _longTermMemory

                // create new Gaussian Function
                _rbfs[i] = new GaussianFunction(_numInputs, _longTermMemory, rbfIndex);
            }
        }

        /// <summary>
        /// Sends input through series of RBFs in network.
        /// Returns number of outputs of the network (2).
        /// </summary>
        /// <param name="input">Vector holding specie parameters.</param>
        /// <returns>Array containing the output of the network.</returns>
        public double[] ComputeRegression(double[] input) {
            double[] rbfOutput = new double[_rbfs.Length + 1]; // array to hold the outputs

            rbfOutput[rbfOutput.Length - 1] = 1; // adding in the bias, always 1

            // go through each RBF
            for(int i = 0; i < _rbfs.Length; i++) {

                double[] weightedInput = new double[input.Length]; // array to hold weighted input values

                // go through each input value
                for(int j = 0; j < input.Length; j++) {

                    int memIdx = _indexInputs + (i * _numInputs) + j; // location of coefficient value in _longTermMemory

                    weightedInput[j] = input[j] * _longTermMemory[memIdx]; // weight the input value
                }

                // computes the output for the specific RBF, e.g. Gaussian: (e^(-r^2))
                rbfOutput[i] = _rbfs[i].Evaluate(weightedInput);
            }


            double[] temp = new double[_numOutputs]; // holds weighted outputs for network

            // sum the RBFs
            for(int i = 0; i < temp.Length; i++) {

                double sum = 0.0; // init sum

                // go through each RBF
                for (int j = 0; j < rbfOutput.Length; j++) {

                    int memIdx = _indexOutputs + (i * (_rbfs.Length + 1)) + j; // index of RBF params in _longTermMemory

                    sum += rbfOutput[j] * _longTermMemory[memIdx]; // add the weighted output to sum
                }

                temp[i] = sum; // set output value
            }
            
            return temp; // return
        }

        /// <summary>
        /// Randomize each dimension in _longTermMemory
        /// </summary>
        public void Randomize() {
            Random r = new Random();
            for(int i = 0; i < _longTermMemory.Length; i++) {

                _longTermMemory[i] = (r.NextDouble() * 2) + -1; // randomize between -1 and 1

            }
        }
    }
}
