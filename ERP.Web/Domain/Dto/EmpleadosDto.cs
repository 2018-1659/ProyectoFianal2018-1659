namespace ERP.Web.Domain.Dto
{
    public class EmpleadosDto
    {
        public int Id { get; set; }//No requerir (Generado por la DB)
        public int PersonaId { get; set; }//No requerir (Generado por la DB)
        public decimal LimiteDeCredito { get; set; }//Si
        public PersonaDto DatosPersonales { get; set; } = new PersonaDto();
        
    }
}
