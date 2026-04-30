using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaRicaNoche.Api.Models
{
    [Table("cierre_caja_envios")]
    public partial class CierreCajaEnvio
    {
        [Key]
        public DateOnly Fecha { get; set; }
        public int? IdEstadoSunat { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public int? IntentosEnvio { get; set; }
        public string? HashXml { get; set; }

        [ForeignKey("IdEstadoSunat")]
        public virtual CatEstadoSunat? IdEstadoSunatNavigation { get; set; }
    }
}