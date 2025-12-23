using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatBot
{
    public partial class Form1 : Form
    {
        // YENİ OLUŞTURDUĞUN projenin anahtarını buraya yapıştır
        private const string API_KEY = "AIzaSyCNVCwvFN3RjWj1Hv1RJtC9_jx4Gy27uic";

        // 2025 sonu itibariyle en geniş kota 'gemini-1.5-flash' modelindedir.
      
        private const string MODEL_NAME = "gemini-2.5-flash";

        // API anahtarınızı buraya tekrar yapıştırın


        // URL yapısını v1beta olarak güncelleyelim (2.5 modelleri beta aşamasındadır)
        private const string GEMINI_URL =
            "https://generativelanguage.googleapis.com/v1/models/" + MODEL_NAME + ":generateContent?key=" + API_KEY;
      
        private readonly HttpClient _httpClient;

        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            lblStatus.Text = "Durum: Hazır";
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendMessageAsync();
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await SendMessageAsync();
            }
        }

        private async Task SendMessageAsync()
        {
            string userMessage = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AppendToHistory("Sen", userMessage);
            txtMessage.Clear();
            SetLoading(true);

            try
            {
                var payload = new
                {
                    contents = new[] {
                        new { parts = new[] { new { text = userMessage } } }
                    }
                };

                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(GEMINI_URL, content);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Hata mesajını daha okunaklı hale getirdik
                    JObject errorObj = JObject.Parse(responseJson);
                    string errMsg = errorObj["error"]?["message"]?.ToString() ?? "Bilinmeyen Hata";
                    AppendToHistory("Sistem", "Hata (" + (int)response.StatusCode + "): " + errMsg);
                    return;
                }

                JObject parsed = JObject.Parse(responseJson);
                string botMessage = parsed["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                AppendToHistory("Gemini", botMessage ?? "Cevap boş döndü.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sistem hatası: " + ex.Message);
            }
            finally
            {
                SetLoading(false);
                txtMessage.Focus();
            }
        }

        private void AppendToHistory(string sender, string message)
        {
            rtbHistory.AppendText(sender + ": " + message + "\n\n");
            rtbHistory.ScrollToCaret();
        }

        private void SetLoading(bool loading)
        {
            btnSend.Enabled = !loading;
            txtMessage.Enabled = !loading;
            lblStatus.Text = loading ? "Durum: Gemini yanıtlıyor..." : "Durum: Hazır";
        }
    }
}