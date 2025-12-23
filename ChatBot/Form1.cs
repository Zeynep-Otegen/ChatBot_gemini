using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO; // 'File' hatasını çözmek için gerekli
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatBot
{
    public partial class Form1 : Form
    {
        // MODEL_NAME hatasını çözmek için buraya ekliyoruz
        private const string MODEL_NAME = "gemini-2.5-flash";
        private readonly HttpClient _httpClient;

        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            lblStatus.Text = "Durum: Hazır";
        }

        // secrets.json dosyasından anahtarı okuyan yardımcı metot
        private string GetApiKeyFromSecrets()
        {
            try
            {
                string path = "secrets.json";
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    JObject config = JObject.Parse(json);
                    return config["GeminiApiKey"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Config okuma hatası: " + ex.Message);
            }
            return null;
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

        // İki ayrı SendMessageAsync vardı, tek bir metotta birleştirdik
        private async Task SendMessageAsync()
        {
            string userMessage = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            // 1. API Anahtarını al
            string apiKey = GetApiKeyFromSecrets();
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("API Anahtarı bulunamadı! Lütfen secrets.json dosyasını kontrol edin.");
                return;
            }

            // 2. Dinamik URL oluştur
            string url = "https://generativelanguage.googleapis.com/v1/models/" + MODEL_NAME + ":generateContent?key=" + apiKey;

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

                // URL değişkenini burada kullanıyoruz
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
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