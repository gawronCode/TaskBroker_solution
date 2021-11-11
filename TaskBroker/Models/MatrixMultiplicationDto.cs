namespace TaskBroker.Models
{
    public class MatrixMultiplicationDto
    {
        /// <summary>
        /// Macierz A
        /// </summary>
        public MatrixDto MatrixA { get; set; }
        /// <summary>
        /// Macierz B
        /// </summary>
        public MatrixDto MatrixB { get; set; }
        /// <summary>
        /// Macierz poowstała w wyniku pomnożenia macierzy A przez macież B => AxB
        /// </summary>
        public MatrixDto MatrixAxB { get; set; }
    }
}
