using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Linq;

namespace HillClimbing {

    /// <summary>
    /// Object to hold list of data for a csv file.
    /// </summary>
    public class DataSet {

        private List<object[]> _data = new List<object[]>(); // holds all records

        private string[] _headers; // holds field headers

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="headers">Headers for the data.</param>
        public DataSet(string[] headers) {
            _headers = headers;
        }

        /// <summary>
        /// Creates new dataset based off of csv file.
        /// </summary>
        /// <param name="sr">Streamreader for csv file.</param>
        /// <returns>New dataset with data from sr.</returns>
        public static DataSet Load(StreamReader sr) {

            DataSet data = null; // dataset to return

            using (CsvReader reader = new CsvReader(sr)) {

                int fieldCount = 0; // number of fields in csv file

                while(reader.Read()) {

                    if(data == null) { // first line

                        fieldCount = reader.FieldHeaders.Count(); // get number of headers

                        // keep track of file headers
                        string[] headers = new string[fieldCount];
                        for(int i = 0; i < fieldCount; i++) {
                            headers[i] = reader.FieldHeaders[i];
                        }

                        data = new DataSet(headers); // create new dataset based on headers
                    }

                    // retrieve data for all fields
                    object[] obj = new object[fieldCount];
                    for(int i = 0; i < fieldCount; i++) {
                        obj[i] = reader.GetField<string>(i);
                    }

                    data.Add(obj); // add record to data list
                }
            }

            return data; // return
        }

        /// <summary>
        /// Normalizes all data for a column.
        /// </summary>
        /// <param name="col">Column to normalize.</param>
        public void Normalize(int col) {

            double dHigh = GetHigh(col); // get highest value in column
            double dLow = GetLow(col); // get lowest value in column

            double nHigh = 1, nLow = 0; // normalized extremes: 1, 0 || 1, -1

            foreach(object[] da in _data) { // for each record in data
                double d = Convert.ToDouble(da[col]); // convert value to double
                da[col] = ((d-dLow) / (dHigh - dLow)) * (nHigh - nLow) + nLow; // normalize value
            }
        }

        /// <summary>
        /// Encodes each header using one of n encoding.
        /// </summary>
        /// <param name="col">Column of heading.</param>
        /// <returns>Dictionary of specie name and index.</returns>
        public Dictionary<string, int> EncodeOneOfN(int col) {

            string name = _headers[col]; // column name
            Dictionary<string, int> species = EnumerateClasses(col); // specie name and index
            InsertColumns(col + 1, species.Count - 1); // insert columns into data

            // fill _data with encoded species
            foreach(object[] obj in _data) {

                int idx = species[obj[col].ToString()]; // get index of specie name
                int specieCount = species.Count; // number of species

                // fill _data with encoded values
                for(int i = 0; i < specieCount; i++) {
                    obj[col + i] = (i == idx) ? 1 : 0;
                }
            }

            // fill _headers with number of classes
            for(int i = 0; i < species.Count; i++) {
                _headers[col + i] = name + "-" + i;
            }

            return species; // return
        }

        /// <summary>
        /// Inserts needed columns into _data.
        /// </summary>
        /// <param name="col">Starting column.</param>
        /// <param name="colCount">Number of columns to add.</param>
        private void InsertColumns(int col, int colCount) {
            
            AppendColumns(colCount); // create space for new columns in _data

        }

        /// <summary>
        /// Create room for equilateral encoding in _data.
        /// </summary>
        /// <param name="numColumns">Number of encoded columns.</param>
        private void AppendColumns(int numColumns) {

            string[] newHeaders = new string[numColumns + _headers.Length]; // new array with room for equilateral encoding

            Array.Copy(_headers, newHeaders, _headers.Length); // copy existing headers to newHeaders

            _headers = newHeaders; // set headers equal to newHeaders... headers is now correct length

            // create space in _data
            for(int i = 0; i < _data.Count; i++) {

                object[] row = _data[i]; // current row
                object[] newRow = new object[_headers.Length]; // more room

                Array.Copy(row, newRow, row.Length); // copy existing data to newRow

                _data.RemoveAt(i); // remove old row
                _data.Insert(i, newRow); // insert new row
            }
        }

        /// <summary>
        /// Creates a record object for each row in _data.
        /// </summary>
        /// <param name="inputStart">Starting column for inputs.</param>
        /// <param name="numInputs">Number of inputs.</param>
        /// <param name="idealStart">Starting column for ideals.</param>
        /// <param name="numIdeals">Number of ideals.</param>
        /// <returns>List of records.</returns>
        public List<Record> ExtractSupervised(int inputStart, int numInputs, int idealStart, int numIdeals) {

            List<Record> temp = new List<Record>(); // temp list

            for(int i = 0; i < _data.Count; i++) {

                object[] row = _data[i]; // get row from _data

                Record rec = new Record(numInputs, numIdeals); // create new Record for row

                // fill Record object with inputs
                for(int j = 0; j < numInputs; j++) {
                    rec.Input[j] = Convert.ToDouble(row[inputStart + j]);
                }

                // fill Record object with ideals
                for(int j = 0; j < numIdeals; j++) {
                    rec.Ideals[j] = Convert.ToDouble(row[idealStart + j]);
                }

                temp.Add(rec); // add to temp
            }
            
            return temp; // return
        }

        /// <summary>
        /// Find all unique classes and index them.
        /// </summary>
        /// <param name="col">Column for the classes.</param>
        /// <returns>Dictionary with class name and an index.</returns>
        private Dictionary<string, int> EnumerateClasses(int col) {

            HashSet<string> classes = new HashSet<string>(); // holds unique classes

            // iterate through _data
            foreach(object[] obj in _data) {
                classes.Add(obj[col].ToString());
            }

            Dictionary<string, int> dic = new Dictionary<string, int>(); // holds names and indexes

            // indexing of classes
            int idx = 0;
            foreach(string name in classes) {
                dic[name] = idx++;
            }

            return dic; // return
        }

        /// <summary>
        /// Add object[] to _data
        /// </summary>
        /// <param name="record">Array to add.</param>
        private void Add(object[] record) {
            _data.Add(record);
        }

        /// <summary>
        /// Gets highest value in column.
        /// </summary>
        /// <param name="col">Column to get highest value from.</param>
        /// <returns>Highest value.</returns>
        private double GetHigh(int col) {
            return _data.Select(obj => Convert.ToDouble(obj[col])).Max();
        }

        /// <summary>
        /// Gets lowest value in column.
        /// </summary>
        /// <param name="col">Column to get lowest value from.</param>
        /// <returns>Lowest value</returns>
        private double GetLow(int col) {
            return _data.Select(obj => Convert.ToDouble(obj[col])).Min();
        }
    }
}
