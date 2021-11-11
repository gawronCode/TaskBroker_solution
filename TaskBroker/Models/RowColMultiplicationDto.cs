using System.Collections.Generic;

namespace TaskBroker.Models
{
    public class RowColMultiplicationDto
    {
        /// <summary>
        /// Wektor będący wierszem z macierzy A
        /// </summary>
        public IEnumerable<double> VectorRowA { get; set; }
        /// <summary>
        /// Wektor będący kolumną z macierzy B
        /// </summary>
        public IEnumerable<double> VectorColB { get; set; }
        /// <summary>
        /// Index wiersza
        /// </summary>
        public int RowIndex { get; set; }
        /// <summary>
        /// Index kolumny
        /// </summary>
        public int ColIndex { get; set; }
        /// <summary>
        /// Skalar wynikowy mnożenia
        /// </summary>
        public double ResultScalar { get; set; }
    }
}
