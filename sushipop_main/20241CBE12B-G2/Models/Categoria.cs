using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Categoría es obligatorio.")]
        [MaxLength(100, ErrorMessage = "Categoría no debe tener mas de 100 caracteres.")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(4000, ErrorMessage = "La descripción no debe tener mas de 400 caracteres.")]
        public string? Descripcion { get; set; }


        /*
         * Relaciones
         */

        public List<Producto>? Productos { get; set; }
    }
}
