namespace HillClimbing {
    /// <summary>
    /// Hill climbing training algorithm.
    /// </summary>
    public class HillClimb {

        private Network _net; // network to perform hill climbing on
        private Scorer _scorer; // scoring object
        private double[] _stepSize; // size of the step for each value in _data
        private double[] _candidate = new double[5]; // potential moves for the algorithm to take

        private double _lastError; // last error

        public double LastError {
            get { return _lastError; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="net">Network to perform hill climbing algorithm on.</param>
        /// <param name="score">Scoring object.</param>
        /// <param name="accel">Acceleration of step sizes.</param>
        /// <param name="stepSize">Initial step sizes.</param>
        public HillClimb(Network net, Scorer score, double accel, double stepSize) {
            _net = net;
            _scorer = score;
            _stepSize = new double[net.LongTermMemory.Length];

            for (int i = 0; i < _stepSize.Length; i++) {
                _stepSize[i] = stepSize;
            }

            _candidate[0] = -accel;
            _candidate[1] = -(1/accel);
            _candidate[2] = 0;
            _candidate[3] = 1/accel;
            _candidate[4] = accel;

            _lastError = double.MaxValue;
        }

        /// <summary>
        /// Run through an iteration of hill climbing.
        /// </summary>
        public void Iteration() {
            
            // loop over all dimensions and try to improve each
            for(int i = 0; i < _net.LongTermMemory.Length; i++) {

                int best = -1; // index of best candidate
                double bestScore = double.MaxValue; // score to minimize

                // try each candidate move for this dimension
                for(int j = 0; j < _candidate.Length; j++) {
                    
                    // try the candidate and score it, then back out of the candidate
                    _net.LongTermMemory[i] += _stepSize[i] * _candidate[j];
                    double temp = _scorer.Score(_net);
                    _net.LongTermMemory[i] -= _stepSize[i] * _candidate[j]; // back out

                    // keep track of candidate with most improvement
                    if(temp < bestScore) {
                        bestScore = temp;
                        best = j;
                        _lastError = bestScore;
                    }
                }

                // if there was a best candidate
                // move in that direction
                if(best != -1) {
                    _net.LongTermMemory[i] += _stepSize[i] * _candidate[best];
                    _stepSize[i] = _stepSize[i] * _candidate[best];
                }
            }
        }
    }
}
