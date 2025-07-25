namespace _20241CBE12B_G2.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Confirmada { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        

        /*
         * Relaciones        
         */

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }


    }
}
