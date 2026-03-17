using AutoDealerExam.Models;
using AutoDealerExam.ViewModel;
using iText.Kernel.Pdf;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using iText.Layout;
using iText.Layout.Element;

namespace AutoDealerExam.Windows
{
    public partial class ReportWindow : Window
    {
        private readonly List<RaportClientView> _raportView;
        private readonly int _totalAutomobile;
        private readonly string _celMaiComandat;

        public ReportWindow(List<RaportClient> raport, int totalAutomobile, string celMaiComandat)
        {
            InitializeComponent();

            _totalAutomobile = totalAutomobile;
            _celMaiComandat = celMaiComandat;

            _raportView = raport
                .Select(r => new RaportClientView
                {
                    NumeClient = r.NumeClient,
                    NumarComenzi = r.NumarComenzi,
                    ValoareTotala = r.ValoareTotala,
                    ValoareTotalaFormatata = r.ValoareTotala.ToString("N0", CultureInfo.InvariantCulture)
                })
                .ToList();

            dgRaport.ItemsSource = _raportView;

            IncarcaStatistici();
        }

        private void IncarcaStatistici()
        {
            txtTotalAutomobile.Text = _totalAutomobile.ToString();
            txtTotalComenzi.Text = _raportView.Sum(x => x.NumarComenzi).ToString();
            txtValoareTotala.Text = _raportView.Sum(x => x.ValoareTotala).ToString("N0", CultureInfo.InvariantCulture);
            txtCelMaiComandat.Text = string.IsNullOrWhiteSpace(_celMaiComandat) ? "—" : _celMaiComandat;
        }

        private void BtnInchide_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnExportaTxt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Salvare raport TXT",
                    Filter = "Fișiere text (*.txt)|*.txt",
                    FileName = "RaportComenzi.txt"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("RAPORT SUMAR COMENZI");
                sb.AppendLine(new string('=', 60));
                sb.AppendLine($"Total automobile: {_totalAutomobile}");
                sb.AppendLine($"Total comenzi: {_raportView.Sum(x => x.NumarComenzi)}");
                sb.AppendLine($"Valoare totală: {_raportView.Sum(x => x.ValoareTotala):N0} EUR");
                sb.AppendLine($"Cel mai comandat: {_celMaiComandat}");
                sb.AppendLine(new string('-', 60));
                sb.AppendLine();

                sb.AppendLine("Raport pe clienți:");
                sb.AppendLine();

                foreach (var item in _raportView)
                {
                    sb.AppendLine($"Client: {item.NumeClient}");
                    sb.AppendLine($"Număr comenzi: {item.NumarComenzi}");
                    sb.AppendLine($"Valoare totală: {item.ValoareTotala:N0} EUR");
                    sb.AppendLine(new string('-', 40));
                }

                File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("Raportul a fost exportat cu succes.", "Succes",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la export: " + ex.Message, "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportaPDF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";

            if (saveFileDialog.ShowDialog() == true)
            {
                PdfWriter writer = new PdfWriter(saveFileDialog.FileName);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph("Raport comenzi"));
                document.Add(new Paragraph("Export PDF funcționează!"));

                document.Close();

                MessageBox.Show("PDF creat cu succes!");
            }
        }
    }

    public class RaportClientView
    {
        public string NumeClient { get; set; } = "";
        public int NumarComenzi { get; set; }
        public decimal ValoareTotala { get; set; }
        public string ValoareTotalaFormatata { get; set; } = "";
    }
}