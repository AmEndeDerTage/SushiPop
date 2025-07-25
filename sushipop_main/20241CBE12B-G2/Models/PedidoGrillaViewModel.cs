namespace _20241CBE12B_G2.Models
{
    public class PedidoGrillaViewModel
    {
        public int Id { get; set; }
        public int? NroPedido { get; set; }

        public DateTime? FechaCompra { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? GastoEnvio { get; set; }

        public decimal? Total { get; set; }

        public string? Estado { get; set; }

    }
}
