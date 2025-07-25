using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public class Reclamo
    {
        public int Id { get; set; }

        [Display(Name = "Nombre completo")]
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [MaxLength(255, ErrorMessage = "Nombre completo no debe superar los 255 caracteres.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa una dirección de Email válida")]
        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El Telefono es obligatorio.")]
        [MinLength(10, ErrorMessage = "El teléfono debe tener 10 caracteres.")]
        [MaxLength(10, ErrorMessage = "El teléfono debe tener 10 caracteres.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe ingresar un mensaje.")]
        [MaxLength(4000, ErrorMessage = "El mensaje no debe tener mas de 4000 caracteres.")]
        public string DetalleReclamo { get; set; }

        /* 
         Relaciones 
         */

        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

    }
}
