using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoDealerExam.Models
{
    public class Automobil
    {
        [Key]
        public int IdAutomobil { get; set; }
        public string Marca { get; set; } = "";
        public string Model { get; set; } = "";
        public int AnFabricatie { get; set; }
        public decimal Pret { get; set; }
        public string TipCombustibil { get; set; } = "";
        public string Transmisie { get; set; } = "";
        public int Stoc { get; set; }

        public List<Comanda> Comenzi { get; set; } = new();

        [NotMapped]
        public string DenumireCompleta => $"{Marca} {Model} ({AnFabricatie}) - {Pret} €";
    }
}
