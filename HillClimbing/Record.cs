namespace HillClimbing {
    public class Record {

        private double[] _input;
        private double[] _ideals;

        public double[] Input {
            get { return _input; }
        }

        public double[] Ideals {
            get { return _ideals; }
        }

        public Record(int numInputs, int numIdeals) {
            _input = new double[numInputs];
            _ideals = new double[numIdeals];
        }
    }
}
