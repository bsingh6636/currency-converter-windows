using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrencyConverterApp
{
    public class ConverterForm : Form
    {
        private TextBox txtAmount;
        private ComboBox cmbBaseCurrency;
        private ComboBox cmbTargetCurrency;
        private Button btnConvert;
        private Label lblResult;
        private Label lblLoading;

        public ConverterForm()
        {
            // Form Configuration
            this.Text = "Currency Converter";
            this.Width = 600;
            this.Height = 500;
            this.BackColor = System.Drawing.Color.FromArgb(240, 248, 255); // AliceBlue

            // Header Bar
            var headerPanel = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(30, 144, 255), // DodgerBlue
                Dock = DockStyle.Top,
                Height = 60
            };

            var lblHeader = new Label
            {
                Text = "Currency Converter",
                Font = new System.Drawing.Font("Arial", 18, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(lblHeader);

            // Input Panel
            var inputPanel = new Panel
            {
                Top = 100,
                Left = 50,
                Width = 500,
                Height = 250,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245), // WhiteSmoke
                BorderStyle = BorderStyle.FixedSingle
            };

            // Amount Input
            var lblAmount = new Label { Text = "Amount:", Left = 30, Top = 20, Width = 100 };
            txtAmount = new TextBox { Left = 150, Top = 20, Width = 300 };

            // Base Currency Selector
            var lblBaseCurrency = new Label { Text = "Base Currency:", Left = 30, Top = 70, Width = 100 };
            cmbBaseCurrency = new ComboBox { Left = 150, Top = 70, Width = 300 };

            // Target Currency Selector
            var lblTargetCurrency = new Label { Text = "Target Currency:", Left = 30, Top = 120, Width = 100 };
            cmbTargetCurrency = new ComboBox { Left = 150, Top = 120, Width = 300 };

            // Add to Input Panel
            inputPanel.Controls.Add(lblAmount);
            inputPanel.Controls.Add(txtAmount);
            inputPanel.Controls.Add(lblBaseCurrency);
            inputPanel.Controls.Add(cmbBaseCurrency);
            inputPanel.Controls.Add(lblTargetCurrency);
            inputPanel.Controls.Add(cmbTargetCurrency);

            // Convert Button
            btnConvert = new Button
            {
                Text = "Convert",
                Left = 250,
                Top = 180,
                Width = 100,
                BackColor = System.Drawing.Color.FromArgb(34, 139, 34), // ForestGreen
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnConvert.FlatAppearance.BorderSize = 0;
            btnConvert.MouseEnter += (s, e) => btnConvert.BackColor = System.Drawing.Color.FromArgb(0, 128, 0); // Darker Green
            btnConvert.MouseLeave += (s, e) => btnConvert.BackColor = System.Drawing.Color.FromArgb(34, 139, 34); // ForestGreen
            btnConvert.Click += async (sender, e) => await ConvertCurrency();
            inputPanel.Controls.Add(btnConvert);

            // Loading Label
            lblLoading = new Label
            {
                Text = "Loading...",
                Left = 250,
                Top = 210,
                Width = 100,
                ForeColor = System.Drawing.Color.Gray,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Visible = false
            };
            inputPanel.Controls.Add(lblLoading);

            // Result Label
            lblResult = new Label
            {
                Text = "Result: ",
                Left = 50,
                Top = 380,
                Width = 500,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.DarkBlue
            };

            // Add Controls
            this.Controls.Add(headerPanel);
            this.Controls.Add(inputPanel);
            this.Controls.Add(lblResult);

            // Fetch Currency List
            _ = FetchCurrencyList();
        }

        public class CurrencyResponse
        {
            public string Result { get; set; }
            public string Base_Code { get; set; }
            public Dictionary<string, decimal> Conversion_Rates { get; set; }
        }

        private async Task FetchCurrencyList()
        {
            try
            {
                lblLoading.Visible = true;

                using HttpClient client = new HttpClient();
                var response = await client.GetFromJsonAsync<CurrencyResponse>("https://v6.exchangerate-api.com/v6/36a6c66876719e286f1a6b8f/latest/USD");

                cmbBaseCurrency.Items.Add("NPR");
                cmbTargetCurrency.Items.Add("NPR");

                if (response?.Conversion_Rates != null)
                {
                    foreach (var symbol in response.Conversion_Rates)
                    {
                        cmbBaseCurrency.Items.Add(symbol.Key);
                        cmbTargetCurrency.Items.Add(symbol.Key);
                    }
                }
                else
                {
                    MessageBox.Show("Error fetching currency list.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                lblLoading.Visible = false;
            }
        }

        private async Task ConvertCurrency()
        {
            if (decimal.TryParse(txtAmount.Text, out decimal amount) &&
                cmbBaseCurrency.SelectedItem != null &&
                cmbTargetCurrency.SelectedItem != null)
            {
                string baseCurrency = cmbBaseCurrency.SelectedItem.ToString();
                string targetCurrency = cmbTargetCurrency.SelectedItem.ToString();

                try
                {
                    lblLoading.Visible = true;
                    decimal conversionRate = 0;

                    // Handle NPR separately
                    if (baseCurrency == "NPR" || targetCurrency == "NPR")
                    {
                        conversionRate = (baseCurrency == "NPR") ? 0.0075m : 133.33m;
                    }
                    else
                    {
                        using HttpClient client = new HttpClient();
                        string url = $"https://api.exchangerate.host/convert?from={baseCurrency}&to={targetCurrency}";
                        var response = await client.GetFromJsonAsync<ExchangeRateConvertResponse>(url);

                        if (response != null && response.Info != null)
                        {
                            conversionRate = response.Info.Rate;
                        }
                    }

                    if (conversionRate > 0)
                    {
                        decimal convertedAmount = amount * conversionRate;
                        lblResult.Text = $"Result: {amount} {baseCurrency} = {convertedAmount:F2} {targetCurrency}";
                    }
                    else
                    {
                        lblResult.Text = "Error: Unable to fetch exchange rate.";
                    }
                }
                catch (Exception ex)
                {
                    lblResult.Text = $"Error: {ex.Message}";
                }
                finally
                {
                    lblLoading.Visible = false;
                }
            }
            else
            {
                lblResult.Text = "Error: Invalid input.";
            }
        }

        public class CurrencySymbolsResponse
        {
            public Dictionary<string, Currency> Symbols { get; set; }
        }

        public class Currency
        {
            public string Description { get; set; }
        }

        public class ExchangeRateConvertResponse
        {
            public ExchangeRateInfo Info { get; set; }
        }

        public class ExchangeRateInfo
        {
            public decimal Rate { get; set; }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Run the application with the ConverterForm
            Application.Run(new ConverterForm());
        }
    }
}



