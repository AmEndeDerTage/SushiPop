using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Display(Name = "Numero de pedido")]
        [Required(ErrorMessage = "El número de pedido es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El número de pedido no puede ser mayor a 100 caracteres")]
        public int NroPedido { get; set; }
     
        [Display(Name = "Fecha de compra")]
        [Required(ErrorMessage = "La fecha de compra es obligatoria")]
        public DateTime FechaCompra { get; set; }

        [Display(Name = "Subtotal")]
        [Required(ErrorMessage = "El subtotal es obligatorio")]
        public decimal Subtotal { get; set; }

        [Display(Name = "Gasto de envío")]
        [Required(ErrorMessage = "El gasto de envío es obligatorio")]
        public decimal GastoEnvio { get; set; }

        [Display(Name = "Total")]
        [Required(ErrorMessage = "El total es obligatorio")]
        public decimal Total { get; set; }

        [Display(Name = "Estado del pedido")]
        [Required(ErrorMessage = "El estado del pedido es obligatorio")]
        public int Estado { get; set; }


        /*
         * Relaciones
         */

        public virtual Reclamo? Reclamo { get; set; }

        public int CarritoId { get; set; }
        public Carrito? Carrito { get; set; }



    }
}
