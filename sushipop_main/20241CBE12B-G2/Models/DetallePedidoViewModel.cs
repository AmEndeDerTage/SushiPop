using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace _20241CBE12B_G2.Models
{
    public class DetallePedidoViewModel
    {
        public int CarritoId { get; set; }
        public string? Cliente { get; set; }
        public List<CarritoItem> Productos { get; set; } = new List<CarritoItem>();
        public string? Direccion { get; set; }
        public decimal Subtotal {  get; set; }
        public decimal? GastoEnvio { get;set; }
        public decimal? Total { get; set; }
    }
}
