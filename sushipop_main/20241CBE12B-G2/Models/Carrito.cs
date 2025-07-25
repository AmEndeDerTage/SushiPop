using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public class Carrito
    {
        public int Id { get; set; }

        [Display(Name = "Carrito Procesado")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public bool Procesado { get; set; }

        [Display(Name = "Carrito Cancelado")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public bool Cancelado { get; set; }

        /*
         Relaciones
         */

        public virtual Pedido? Pedido { get; set; }

        public List<CarritoItem>? CarritoItems { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }




    }
}
