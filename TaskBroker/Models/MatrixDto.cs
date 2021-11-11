using System.Collections.Generic;

namespace TaskBroker.Models
{
    /// <summary>
    /// Macierz składająca się z kolumn i wierszy
    /// </summary>
    public class MatrixDto
    {
        public IEnumerable<IEnumerable<double>> ColumnsRows { get; set; }
    }
}
