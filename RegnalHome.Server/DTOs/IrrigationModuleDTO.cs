using System.ComponentModel.DataAnnotations.Schema;
using RegnalHome.Common.Models;

namespace RegnalHome.Server.DTOs;

public class IrrigationModuleDTO : IrrigationModule
{
    [NotMapped] public new DateTime LastCommunication { get; set; }
}