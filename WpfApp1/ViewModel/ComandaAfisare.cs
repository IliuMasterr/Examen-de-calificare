using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDealerExam.ViewModel
{
    public class ComandaAfisare
    {
        public int IdComanda { get; set; }
        public string Client { get; set; } = "";
        public string Automobil { get; set; } = "";
        public DateTime DataComanda { get; set; }
        public string StatusComanda { get; set; } = "";
        public decimal Valoare { get; set; }
    }
}
