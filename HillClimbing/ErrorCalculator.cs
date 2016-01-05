namespace HillClimbing {

    /// <summary>
    /// Responsible for calculating the error between the actual and ideal vectors.
    /// </summary>
    public class ErrorCalculator {

        private double _error; // error
        private int _setSize; // dimensions of input/ideal

        /// <summary>
        /// Reset calculator fields.
        /// </summary>
        public void Clear() {
            _error = 0;_setSize = 0; // reset
        }

        /// <summary>
        /// Calculate error based on new actual/ideal pair.
        /// </summary>
        /// <param name="actual">The actual classification.</param>
        /// <param name="ideal">The ideal classification.</param>
        public void UpdateError(double[] actual, double[] ideal) {
            for(int i = 0; i < actual.Length; i++) {

                double delta = (ideal[i] - actual[i]); // distance between outputs

                _error += delta * delta; // error += delta^2
            }

            _setSize += ideal.Length; // dimensions
        }

        /// <summary>
        /// Calculate error with MSE.
        /// </summary>
        /// <returns>MSE error.</returns>
        public double Calculate() {
            return _error / _setSize;
        }
    }
}
