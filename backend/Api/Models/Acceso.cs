namespace Api.Models;

public class Acceso
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; } // nulo = el socio está dentro ahora
}
