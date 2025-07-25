using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public class Cliente : Usuario
    {
        [Display(Name = "Número de cliente")]
        public int? NumeroCliente { get; set; }

        public List<Carrito>? Carritos { get; set; }
        public List<Reserva>? Reservas { get; set; }
    }
}
