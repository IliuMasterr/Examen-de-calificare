using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoDealerExam.Models
{
    public class Comanda
    {
        [Key]
        public int IdComanda { get; set; }

        public int IdClient { get; set; }
        public Client? Client { get; set; }

        public int IdAutomobil { get; set; }
        public Automobil? Automobil { get; set; }

        public DateTime DataComanda { get; set; }
        public string StatusComanda { get; set; } = "";
    }
}
