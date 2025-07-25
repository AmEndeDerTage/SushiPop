namespace _20241CBE12B_G2.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }

        public decimal PrecioUnitarioConDescuento { get; set; }
        public int Cantidad { get; set; }

        /*
         * Relaciones
         */

        public int CarritoId { get; set; }
        public Carrito? Carrito { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }



    }
}
