using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoDealerExam.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }
        public string Nume { get; set; } = "";
        public string Prenume { get; set; } = "";
        public string Telefon { get; set; } = "";
        public string Email { get; set; } = "";

        public List<Comanda> Comenzi { get; set; } = new();

        [NotMapped]
        public string NumeComplet => $"{Nume} {Prenume}";
    }
}
