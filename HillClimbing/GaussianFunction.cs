using System;

namespace HillClimbing {

    /// <summary>
    /// Used to manipulate data using Gaussian distribution.
    /// </summary>
    public class GaussianFunction {

        private int _dimensions; // number of inputs
        private int _indexWidth; // index in _parameters of the width of the function
        private int _indexCenters; // index in _parameters of the center of function
        private double[] _parameters; // long term memory

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dimensions">Number of inputs.</param>
        /// <param name="parameters">Long term memory.</param>
        /// <param name="index">Index in parameters of the RBF parameters.</param>
        public GaussianFunction(int dimensions, double[] parameters, int index) {
            _dimensions = dimensions;
            _parameters = parameters;
            _indexWidth = index;
            _indexCenters = index + 1;
        }

        /// <summary>
        /// Scales da to within the Gaussian distribution.
        /// </summary>
        /// <param name="da">Array to scale.</param>
        /// <returns>Value on the Gaussian distribution.</returns>
        public double Evaluate(double[] da) {

            double value = 0; 
            double width = _parameters[_indexWidth]; // width (radius) of Gaussian function

            for(int i = 0; i < _dimensions; i++) {

                double center = _parameters[_indexCenters + i]; // center for each input

                // perform Gaussian function... previously (2.0 * width * width)
                value += Math.Pow(da[i] - center, 2) / (width * width);
            }

            return Math.Exp(-value);
        }
    }
}