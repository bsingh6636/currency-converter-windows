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
            this.Width = 500;
            this.Height = 400;
            this.BackColor = System.Drawing.Color.LightBlue;

            // Title Label
            var lblTitle = new Label
            {
                Text = "Currency Converter",
                Left = 150,
                Top = 20,
                Width = 200,
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Amount Input
            var lblAmount = new Label { Text = "Amount:", Left = 50, Top = 80, Width = 100 };
            txtAmount = new TextBox { Left = 150, Top = 80, Width = 250 };

            // Base Currency Selector
            var lblBaseCurrency = new Label { Text = "Base Currency:", Left = 50, Top = 120, Width = 100 };
            cmbBaseCurrency = new ComboBox { Left = 150, Top = 120, Width = 250 };

            // Target Currency Selector
            var lblTargetCurrency = new Label { Text = "Target Currency:", Left = 50, Top = 160, Width = 100 };
            cmbTargetCurrency = new ComboBox { Left = 150, Top = 160, Width = 250 };

            // Convert Button
            btnConvert = new Button
            {
                Text = "Convert",
                Left = 200,
                Top = 200,
                Width = 100,
                BackColor = System.Drawing.Color.MediumSeaGreen,
                ForeColor = System.Drawing.Color.White
            };
            btnConvert.Click += async (sender, e) => await ConvertCurrency();

            // Loading Label (Spinner)
            lblLoading = new Label
            {
                Text = "Loading...",
                Left = 200,
                Top = 250,
                Width = 100,
                Visible = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Result Label
            lblResult = new Label
            {
                Text = "Result: ",
                Left = 50,
                Top = 300,
                Width = 400,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.DarkBlue
            };

            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblAmount);
            this.Controls.Add(txtAmount);
            this.Controls.Add(lblBaseCurrency);
            this.Controls.Add(cmbBaseCurrency);
            this.Controls.Add(lblTargetCurrency);
            this.Controls.Add(cmbTargetCurrency);
            this.Controls.Add(btnConvert);
            this.Controls.Add(lblLoading);
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

        // Add NPR to dropdowns first
        cmbBaseCurrency.Items.Add("NPR");
        cmbTargetCurrency.Items.Add("NPR");

        if (response?.Conversion_Rates != null)
        {
            foreach (var symbol in response.Conversion_Rates)
            {
                string display = $"{symbol.Key}"; // Display only currency code
                cmbBaseCurrency.Items.Add(display);
                cmbTargetCurrency.Items.Add(display);
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
            Application.Run(new ConverterForm());
        }
    }
}
