using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre del producto no debe tener mas de 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
        [MinLength(20, ErrorMessage = "La descripción del producto debe tener un mínimo de 20 caracteres")]
        [MaxLength(250, ErrorMessage = "La descripción del producto debe tener un máximo de 250 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio del producto es obligatorio.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La imagen del producto es obligatoria.")]
        [Url]
        public string Foto { get; set; }

        public int Stock { get; set; }
        public decimal Costo { get; set; }



        /*
         * Relaciones
         */

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public List<CarritoItem>? CarritoItems { get; set; }
        public List<Descuento>? Descuentos { get; set; }




    }
}
