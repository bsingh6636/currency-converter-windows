Currency Converter Application
This project is a Windows Forms-based Currency Converter application built using C# and .NET Framework. It allows users to convert an amount from one currency to another using live exchange rates fetched from an API. The application includes a custom implementation to handle conversions involving the Nepalese Rupee (NPR), which is not available in the API response.

Features
Live Currency Exchange Rates: Fetches real-time rates from the ExchangeRate-API or ExchangeRate.host.
Simple and Intuitive UI: User-friendly interface for selecting base and target currencies and entering the amount to convert.
Custom NPR Support: Adds support for NPR (Nepalese Rupee) with predefined exchange rates.
Error Handling: Handles invalid inputs and network errors gracefully.
Dynamic Dropdowns: Fetches and populates currency options dynamically.
Screenshots
Main Interface:
(https://res.cloudinary.com/bsingh6636/image/upload/v1732097790/projects/currencyCOnverterWindows.png)

Installation and Setup
Clone the Repository:

bash
Copy code
git clone https://github.com/bsingh6636/currency-converter-windows.git
cd currency-converter
Requirements:

Windows OS
Visual Studio 2022 or later
.NET Framework 4.7.2 or later
Open the Project:

Open the .sln file in Visual Studio.
Run the Application:

Press F5 or click Start to run the application.
Usage
Launch the application.
Enter the amount you wish to convert.
Select the base currency and target currency from the dropdown menus.
Click Convert to see the converted amount.
If converting to or from NPR, the application uses predefined rates:
1 NPR to USD: 0.0075
1 USD to NPR: 133.33
API Integration
The application uses the following API to fetch live exchange rates:

ExchangeRate-API: Fetches live exchange rates for most major currencies.
API Endpoints
Fetch Currency List and Rates:


bash
https://v6.exchangerate-api.com/v6/<api_key>/latest/USD
Convert Between Currencies:

vbnet
Copy code
https://api.exchangerate.host/convert?from=<BASE>&to=<TARGET>
Project Structure
graphql
Copy code
CurrencyConverterApp/
├── ConverterForm.cs          # Main application logic
├── Program.cs                # Entry point
├── CurrencyResponse.cs       # Model for API responses
├── README.md                 # Project documentation
├── app.config                # Configuration settings
└── CurrencyConverterApp.sln  # Visual Studio solution file
Future Improvements
Support for More APIs: Integrate additional APIs to ensure redundancy and accuracy.
Currency Flags: Add flags for each currency to enhance the dropdown UI.
Offline Mode: Cache exchange rates for offline use.
Advanced Options: Allow users to set custom exchange rates.
License
This project is licensed under the MIT License. See the LICENSE file for details.

Contributing
Contributions are welcome! If you have suggestions or encounter issues:

Fork the repository.
Create a feature branch (git checkout -b feature-name).
Commit your changes (git commit -m "Add feature").
Push to the branch (git push origin feature-name).
Open a pull request.
Author
Your Name
Your GitHub Profile
Your Portfolio

Feel free to customize this README to better suit your needs! Let me know if you need further assistance. 🚀