using AutoDealerExam.Data;
using AutoDealerExam.Models;
using AutoDealerExam.ViewModel;
using AutoDealerExam.Windows;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace AutoDealerExam
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context = new();

        private int? _selectedAutomobilId = null;
        private int? _selectedClientId = null;

        public MainWindow()
        {
            InitializeComponent();

            _context.Database.EnsureCreated();

            InitializeStaticCombos();
            SeedData();
            LoadAllData();

            dpDataComanda.SelectedDate = DateTime.Today;
            cmbStatusComanda.SelectedIndex = 0;
        }

        private void InitializeStaticCombos()
        {
            cmbTipCombustibil.ItemsSource = new List<string> { "Benzină", "Diesel", "Electric", "Hibrid" };
            cmbTransmisie.ItemsSource = new List<string> { "Manuală", "Automată" };

            cmbFiltruCombustibil.ItemsSource = new List<string> { "Toate", "Benzină", "Diesel", "Electric", "Hibrid" };
            cmbFiltruCombustibil.SelectedIndex = 0;

            cmbStatusComanda.ItemsSource = new List<string> { "În așteptare", "Confirmată", "Livrată" };
        }

        private void LoadAllData()
        {
            LoadAutomobile();
            LoadClienti();
            LoadComenzi();
            LoadComboData();
        }

        private void LoadComboData()
        {
            var clienti = _context.Clienti
                .OrderBy(c => c.Nume)
                .ThenBy(c => c.Prenume)
                .ToList();

            var automobile = _context.Automobile
                .Where(a => a.Stoc > 0)
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Model)
                .ToList();

            cmbClientComanda.ItemsSource = clienti;
            cmbFiltruClientComenzi.ItemsSource = clienti;
            cmbAutomobilComanda.ItemsSource = automobile;
        }

        private void LoadAutomobile()
        {
            dgAutomobile.ItemsSource = _context.Automobile
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Model)
                .ToList();
        }

        private void LoadClienti()
        {
            dgClienti.ItemsSource = _context.Clienti
                .OrderBy(c => c.Nume)
                .ThenBy(c => c.Prenume)
                .ToList();
        }

        private void LoadComenzi(int? idClient = null)
        {
            var query = _context.Comenzi
                .Include(c => c.Client)
                .Include(c => c.Automobil)
                .AsQueryable();

            if (idClient.HasValue)
            {
                query = query.Where(c => c.IdClient == idClient.Value);
            }

            var rezultat = query
                .OrderByDescending(c => c.DataComanda)
                .Select(c => new ComandaAfisare
                {
                    IdComanda = c.IdComanda,
                    Client = c.Client!.Nume + " " + c.Client.Prenume,
                    Automobil = c.Automobil!.Marca + " " + c.Automobil.Model,
                    DataComanda = c.DataComanda,
                    StatusComanda = c.StatusComanda,
                    Valoare = c.Automobil!.Pret
                })
                .ToList();

            dgComenzi.ItemsSource = rezultat;
        }

        private void SeedData()
        {
            if (_context.Automobile.Any() || _context.Clienti.Any() || _context.Comenzi.Any())
                return;

            var automobile = new List<Automobil>
            {
                new Automobil { Marca = "Toyota", Model = "Corolla", AnFabricatie = 2021, Pret = 18500, TipCombustibil = "Benzină", Transmisie = "Automată", Stoc = 3 },
                new Automobil { Marca = "BMW", Model = "X5", AnFabricatie = 2020, Pret = 42000, TipCombustibil = "Diesel", Transmisie = "Automată", Stoc = 2 },
                new Automobil { Marca = "Audi", Model = "A4", AnFabricatie = 2019, Pret = 25500, TipCombustibil = "Diesel", Transmisie = "Manuală", Stoc = 4 },
                new Automobil { Marca = "Tesla", Model = "Model 3", AnFabricatie = 2022, Pret = 38900, TipCombustibil = "Electric", Transmisie = "Automată", Stoc = 3 },
                new Automobil { Marca = "Dacia", Model = "Duster", AnFabricatie = 2023, Pret = 21000, TipCombustibil = "Benzină", Transmisie = "Manuală", Stoc = 5 },
                new Automobil { Marca = "Mercedes", Model = "C220", AnFabricatie = 2018, Pret = 28000, TipCombustibil = "Diesel", Transmisie = "Automată", Stoc = 2 },
                new Automobil { Marca = "Volkswagen", Model = "Passat", AnFabricatie = 2020, Pret = 23000, TipCombustibil = "Diesel", Transmisie = "Manuală", Stoc = 3 },
                new Automobil { Marca = "Hyundai", Model = "Tucson", AnFabricatie = 2021, Pret = 27000, TipCombustibil = "Hibrid", Transmisie = "Automată", Stoc = 2 }
            };

            var clienti = new List<Client>
            {
                new Client { Nume = "Popa", Prenume = "Ion", Telefon = "069000111", Email = "ion.popa@mail.com" },
                new Client { Nume = "Rusu", Prenume = "Maria", Telefon = "069000222", Email = "maria.rusu@mail.com" },
                new Client { Nume = "Ceban", Prenume = "Victor", Telefon = "069000333", Email = "victor.ceban@mail.com" },
                new Client { Nume = "Munteanu", Prenume = "Elena", Telefon = "069000444", Email = "elena.munteanu@mail.com" },
                new Client { Nume = "Grigore", Prenume = "Ana", Telefon = "069000555", Email = "ana.grigore@mail.com" },
                new Client { Nume = "Balan", Prenume = "Sergiu", Telefon = "069000666", Email = "sergiu.balan@mail.com" }
            };

            _context.Automobile.AddRange(automobile);
            _context.Clienti.AddRange(clienti);
            _context.SaveChanges();

            AddSeedOrder(1, 2, new DateTime(2026, 3, 1), "Confirmată");
            AddSeedOrder(2, 1, new DateTime(2026, 3, 2), "În așteptare");
            AddSeedOrder(3, 4, new DateTime(2026, 3, 3), "Confirmată");
            AddSeedOrder(4, 5, new DateTime(2026, 3, 4), "Livrată");
            AddSeedOrder(5, 3, new DateTime(2026, 3, 5), "Confirmată");
            AddSeedOrder(6, 8, new DateTime(2026, 3, 5), "În așteptare");
            AddSeedOrder(1, 6, new DateTime(2026, 3, 6), "Confirmată");
            AddSeedOrder(2, 7, new DateTime(2026, 3, 7), "Livrată");
        }

        private void AddSeedOrder(int idClient, int idAutomobil, DateTime data, string status)
        {
            var auto = _context.Automobile.First(a => a.IdAutomobil == idAutomobil);
            if (auto.Stoc <= 0) return;

            auto.Stoc--;

            _context.Comenzi.Add(new Comanda
            {
                IdClient = idClient,
                IdAutomobil = idAutomobil,
                DataComanda = data,
                StatusComanda = status
            });

            _context.SaveChanges();
        }

        // =========================
        // AUTOMOBILE
        // =========================
        private void btnSalveazaAutomobil_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidareAutomobil(out int an, out decimal pret, out int stoc))
                return;

            if (_selectedAutomobilId == null)
            {
                var automobilNou = new Automobil
                {
                    Marca = txtMarca.Text.Trim(),
                    Model = txtModel.Text.Trim(),
                    AnFabricatie = an,
                    Pret = pret,
                    TipCombustibil = cmbTipCombustibil.Text,
                    Transmisie = cmbTransmisie.Text,
                    Stoc = stoc
                };

                _context.Automobile.Add(automobilNou);
            }
            else
            {
                var automobil = _context.Automobile.Find(_selectedAutomobilId.Value);
                if (automobil == null) return;

                automobil.Marca = txtMarca.Text.Trim();
                automobil.Model = txtModel.Text.Trim();
                automobil.AnFabricatie = an;
                automobil.Pret = pret;
                automobil.TipCombustibil = cmbTipCombustibil.Text;
                automobil.Transmisie = cmbTransmisie.Text;
                automobil.Stoc = stoc;
            }

            _context.SaveChanges();
            LoadAllData();
            ClearAutomobilForm();
            MessageBox.Show("Datele automobilului au fost salvate.");
        }

        private bool ValidareAutomobil(out int an, out decimal pret, out int stoc)
        {
            an = 0;
            pret = 0;
            stoc = 0;

            if (string.IsNullOrWhiteSpace(txtMarca.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtAnFabricatie.Text) ||
                string.IsNullOrWhiteSpace(txtPret.Text) ||
                string.IsNullOrWhiteSpace(cmbTipCombustibil.Text) ||
                string.IsNullOrWhiteSpace(cmbTransmisie.Text) ||
                string.IsNullOrWhiteSpace(txtStoc.Text))
            {
                MessageBox.Show("Completați toate câmpurile pentru automobil.");
                return false;
            }

            if (!int.TryParse(txtAnFabricatie.Text, out an))
            {
                MessageBox.Show("Anul de fabricație trebuie să fie număr întreg.");
                return false;
            }

            if (an < 1900 || an > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Anul de fabricație nu este valid.");
                return false;
            }

            if (!decimal.TryParse(txtPret.Text, out pret) || pret <= 0)
            {
                MessageBox.Show("Prețul trebuie să fie mai mare decât 0.");
                return false;
            }

            if (!int.TryParse(txtStoc.Text, out stoc) || stoc < 0)
            {
                MessageBox.Show("Stocul nu poate fi negativ.");
                return false;
            }

            return true;
        }

        private void btnStergeAutomobil_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAutomobilId == null)
            {
                MessageBox.Show("Selectați un automobil.");
                return;
            }

            bool areComenzi = _context.Comenzi.Any(c => c.IdAutomobil == _selectedAutomobilId.Value);
            if (areComenzi)
            {
                MessageBox.Show("Automobilul nu poate fi șters deoarece are comenzi asociate.");
                return;
            }

            var automobil = _context.Automobile.Find(_selectedAutomobilId.Value);
            if (automobil == null) return;

            _context.Automobile.Remove(automobil);
            _context.SaveChanges();

            LoadAllData();
            ClearAutomobilForm();
            MessageBox.Show("Automobil șters.");
        }

        private void btnGolesteAutomobil_Click(object sender, RoutedEventArgs e)
        {
            ClearAutomobilForm();
        }

        private void ClearAutomobilForm()
        {
            _selectedAutomobilId = null;
            txtMarca.Clear();
            txtModel.Clear();
            txtAnFabricatie.Clear();
            txtPret.Clear();
            txtStoc.Clear();
            cmbTipCombustibil.SelectedIndex = -1;
            cmbTransmisie.SelectedIndex = -1;
            dgAutomobile.SelectedItem = null;
        }

        private void dgAutomobile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAutomobile.SelectedItem is not Automobil automobil)
                return;

            _selectedAutomobilId = automobil.IdAutomobil;
            txtMarca.Text = automobil.Marca;
            txtModel.Text = automobil.Model;
            txtAnFabricatie.Text = automobil.AnFabricatie.ToString();
            txtPret.Text = automobil.Pret.ToString();
            txtStoc.Text = automobil.Stoc.ToString();
            cmbTipCombustibil.Text = automobil.TipCombustibil;
            cmbTransmisie.Text = automobil.Transmisie;
        }

        private void btnFiltreazaAutomobile_Click(object sender, RoutedEventArgs e)
        {
            var query = _context.Automobile.AsQueryable();

            string cautare = txtCautaAuto.Text.Trim();
            if (!string.IsNullOrWhiteSpace(cautare))
            {
                query = query.Where(a => a.Marca.Contains(cautare) || a.Model.Contains(cautare));
            }

            string combustibil = cmbFiltruCombustibil.Text;
            if (!string.IsNullOrWhiteSpace(combustibil) && combustibil != "Toate")
            {
                query = query.Where(a => a.TipCombustibil == combustibil);
            }

            if (int.TryParse(txtAnMinim.Text, out int anMinim))
            {
                query = query.Where(a => a.AnFabricatie >= anMinim);
            }

            if (decimal.TryParse(txtPretMaxim.Text, out decimal pretMaxim))
            {
                query = query.Where(a => a.Pret <= pretMaxim);
            }

            dgAutomobile.ItemsSource = query
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Model)
                .ToList();
        }

        private void btnReseteazaAutomobile_Click(object sender, RoutedEventArgs e)
        {
            txtCautaAuto.Clear();
            txtAnMinim.Clear();
            txtPretMaxim.Clear();
            cmbFiltruCombustibil.SelectedIndex = 0;
            LoadAutomobile();
        }

        // =========================
        // CLIENȚI
        // =========================
        private void btnSalveazaClient_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidareClient())
                return;

            try
            {
                if (_selectedClientId == null)
                {
                    var clientNou = new Client
                    {
                        Nume = txtNume.Text.Trim(),
                        Prenume = txtPrenume.Text.Trim(),
                        Telefon = txtTelefon.Text.Trim(),
                        Email = txtEmail.Text.Trim()
                    };

                    _context.Clienti.Add(clientNou);
                }
                else
                {
                    var client = _context.Clienti.Find(_selectedClientId.Value);
                    if (client == null) return;

                    client.Nume = txtNume.Text.Trim();
                    client.Prenume = txtPrenume.Text.Trim();
                    client.Telefon = txtTelefon.Text.Trim();
                    client.Email = txtEmail.Text.Trim();
                }

                _context.SaveChanges();
                LoadAllData();
                ClearClientForm();
                MessageBox.Show("Datele clientului au fost salvate.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la salvarea clientului: " + ex.Message);
            }
        }

        private bool ValidareClient()
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text) ||
                string.IsNullOrWhiteSpace(txtPrenume.Text) ||
                string.IsNullOrWhiteSpace(txtTelefon.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Completați toate câmpurile pentru client.");
                return false;
            }

            if (!Regex.IsMatch(txtTelefon.Text.Trim(), @"^\d{8,15}$"))
            {
                MessageBox.Show("Telefonul trebuie să conțină doar cifre și să aibă între 8 și 15 caractere.");
                return false;
            }

            if (!txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Emailul nu este valid.");
                return false;
            }

            return true;
        }

        private void btnStergeClient_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClientId == null)
            {
                MessageBox.Show("Selectați un client.");
                return;
            }

            bool areComenzi = _context.Comenzi.Any(c => c.IdClient == _selectedClientId.Value);
            if (areComenzi)
            {
                MessageBox.Show("Clientul nu poate fi șters deoarece are comenzi asociate.");
                return;
            }

            var client = _context.Clienti.Find(_selectedClientId.Value);
            if (client == null) return;

            _context.Clienti.Remove(client);
            _context.SaveChanges();

            LoadAllData();
            ClearClientForm();
            MessageBox.Show("Client șters.");
        }

        private void btnGolesteClient_Click(object sender, RoutedEventArgs e)
        {
            ClearClientForm();
        }

        private void ClearClientForm()
        {
            _selectedClientId = null;
            txtNume.Clear();
            txtPrenume.Clear();
            txtTelefon.Clear();
            txtEmail.Clear();
            dgClienti.SelectedItem = null;
        }

        private void dgClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgClienti.SelectedItem is not Client client)
                return;

            _selectedClientId = client.IdClient;
            txtNume.Text = client.Nume;
            txtPrenume.Text = client.Prenume;
            txtTelefon.Text = client.Telefon;
            txtEmail.Text = client.Email;
        }

        private void btnCautaClient_Click(object sender, RoutedEventArgs e)
        {
            string cautare = txtCautaClient.Text.Trim();

            var rezultat = _context.Clienti
                .Where(c => c.Nume.Contains(cautare) ||
                            c.Prenume.Contains(cautare) ||
                            c.Email.Contains(cautare))
                .OrderBy(c => c.Nume)
                .ThenBy(c => c.Prenume)
                .ToList();

            dgClienti.ItemsSource = rezultat;
        }

        private void btnReseteazaClienti_Click(object sender, RoutedEventArgs e)
        {
            txtCautaClient.Clear();
            LoadClienti();
        }

        // =========================
        // COMENZI
        // =========================
        private void btnPlaseazaComanda_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClientComanda.SelectedValue == null ||
                cmbAutomobilComanda.SelectedValue == null ||
                dpDataComanda.SelectedDate == null ||
                string.IsNullOrWhiteSpace(cmbStatusComanda.Text))
            {
                MessageBox.Show("Completați toate datele comenzii.");
                return;
            }

            int idClient = (int)cmbClientComanda.SelectedValue;
            int idAutomobil = (int)cmbAutomobilComanda.SelectedValue;

            var automobil = _context.Automobile.Find(idAutomobil);
            if (automobil == null) return;

            if (automobil.Stoc <= 0)
            {
                MessageBox.Show("Automobilul selectat nu mai este în stoc.");
                return;
            }

            var comanda = new Comanda
            {
                IdClient = idClient,
                IdAutomobil = idAutomobil,
                DataComanda = dpDataComanda.SelectedDate.Value,
                StatusComanda = cmbStatusComanda.Text
            };

            automobil.Stoc--;
            _context.Comenzi.Add(comanda);
            _context.SaveChanges();

            LoadAllData();
            MessageBox.Show("Comanda a fost înregistrată.");
        }

        private void btnAnuleazaComanda_Click(object sender, RoutedEventArgs e)
        {
            if (dgComenzi.SelectedItem is not ComandaAfisare comandaSelectata)
            {
                MessageBox.Show("Selectați o comandă.");
                return;
            }

            var comanda = _context.Comenzi
                .Include(c => c.Automobil)
                .FirstOrDefault(c => c.IdComanda == comandaSelectata.IdComanda);

            if (comanda == null) return;

            if (comanda.StatusComanda == "Anulată")
            {
                MessageBox.Show("Comanda este deja anulată.");
                return;
            }

            comanda.StatusComanda = "Anulată";

            if (comanda.Automobil != null)
                comanda.Automobil.Stoc++;

            _context.SaveChanges();

            LoadAllData();
            MessageBox.Show("Comanda a fost anulată.");
        }

        private void btnComenziClient_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFiltruClientComenzi.SelectedValue == null)
            {
                MessageBox.Show("Selectați un client.");
                return;
            }

            int idClient = (int)cmbFiltruClientComenzi.SelectedValue;
            LoadComenzi(idClient);
        }

        private void btnToateComenzile_Click(object sender, RoutedEventArgs e)
        {
            LoadComenzi();
        }

        private void btnRaport_Click(object sender, RoutedEventArgs e)
        {
            var raport = _context.Comenzi
    .Include(c => c.Client)
    .Include(c => c.Automobil)
    .AsEnumerable()
    .GroupBy(c => c.Client.Nume + " " + c.Client.Prenume)
    .Select(g => new RaportClient
    {
        NumeClient = g.Key,
        NumarComenzi = g.Count(),
        ValoareTotala = g.Sum(x => x.Automobil.Pret)
    })
    .OrderByDescending(r => r.ValoareTotala)
    .ToList();

            int totalAutomobile = _context.Automobile.Count();

            string celMaiComandat = _context.Comenzi
                .Include(c => c.Automobil)
                .AsEnumerable()
                .GroupBy(c => c.Automobil.Marca + " " + c.Automobil.Model)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "—";

            var reportWindow = new ReportWindow(raport, totalAutomobile, celMaiComandat)
            {
                Owner = this
            };

            reportWindow.ShowDialog();
        }
    }
 }