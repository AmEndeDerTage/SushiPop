using System.ComponentModel.DataAnnotations;

namespace _20241CBE12B_G2.Models
{
    public abstract class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(5, ErrorMessage = "El nombre debe tener un mínimo de 5 caracteres.")]
        [MaxLength(30, ErrorMessage = "El nombre debe tener un máximo de 30 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MinLength(5, ErrorMessage = "El apellido debe tener un mínimo de 5 caracteres")]
        [MaxLength(30, ErrorMessage = "El apellido debe tener un máximo de 30 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [MaxLength(100, ErrorMessage = "La dirección no puede superar los 100 caracteres.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MinLength(10, ErrorMessage = "El teléfono debe tener 10 caracteres.")]
        [MaxLength(10, ErrorMessage = "El teléfono debe tener 10 caracteres.")]
        public string Telefono { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage ="La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }


        [DataType(DataType.DateTime)]
        public DateTime FechaAlta { get; set; }

        public bool Activo { get; set; }
        public string Email { get; set; }

    }
}
