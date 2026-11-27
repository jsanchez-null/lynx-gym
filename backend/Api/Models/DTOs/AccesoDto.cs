namespace Api.Models.DTOs;

public class AccesoDto
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
}

