using System.Collections.Generic;

namespace HillClimbing {
    public class Scorer {

        private List<Record> _trainingData; // holds data to be used to train with

        private ErrorCalculator _errCalc = new ErrorCalculator(); // calculates error

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trainingData">Data to be used to train with.</param>
        public Scorer(List<Record> trainingData) {
            _trainingData = trainingData;
        }

        /// <summary>
        /// Determines a score for a RBF network.
        /// </summary>
        /// <param name="net">RBF network to determine a score for.</param>
        /// <returns>Double that represents the score.</returns>
        public double Score(Network net) {

            _errCalc.Clear(); // clear the error calculator

            foreach(var data in _trainingData) {

                double[] output = net.ComputeRegression(data.Input); // send input through RBFs in network

                _errCalc.UpdateError(output, data.Ideals); // updates error based on new outputs

            }

            return _errCalc.Calculate(); // calculates MSE error
        }
    }
}
