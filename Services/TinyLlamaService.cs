using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Data.Entity;
using ConsignmentWebsite.Models.EF;
using ConsignmentWebsite.Models;

namespace ConsignmentWebsite.Services
{
    public class TinyLlamaService
    {
        private readonly string _apiUrl = "https://f1f7-34-142-174-191.ngrok-free.app/generate";

        public async Task<string> GetResponseAsync(string prompt)
        {
            if (TryExtractCode(prompt, out string code))
            {
                var status = await GetOrderOrConsignmentStatusAsync(code);
                return status;
            }

            // Nếu không phải mã đơn, gọi mô hình TinyLlama
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                var requestData = new
                {
                    messages = new[] { new { role = "user", content = prompt } }
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(_apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<ResponseData>(responseString);
                        return responseData.response;
                    }
                    else
                    {
                        return $"Error: Unable to reach AI server. ({response.StatusCode})";
                    }
                }
                catch (TaskCanceledException)
                {
                    return "Error: Request timed out.";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }

        private bool TryExtractCode(string prompt, out string code)
        {
            var match = Regex.Match(prompt, @"(?:order|consignment)?\s*code\s*(?:is\s*)?([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                code = match.Groups[1].Value.Trim();
                return true;
            }

            code = null;
            return false;
        }

        private async Task<string> GetOrderOrConsignmentStatusAsync(string code)
        {
            using (var db = new ApplicationDbContext())
            {
                // Find on Orders tb
                var order = await db.Orders.FirstOrDefaultAsync(o => o.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
                if (order != null)
                {
                    string status = order.Status == 1 ? "Waiting for payment" :
                                    order.Status == 2 ? "Paid" : "Pending";

                    string shipping = order.ShippingStatus == 1 ? "Not Picked Up" :
                                      order.ShippingStatus == 2 ? "Shipping" :
                                      "Shipped";

                    return $"Order Code: {order.Code}\nStatus: {status}\nShipping Status: {shipping}\nTotal: {order.TotalAmount:N0} VND.";
                }

                // Find on ConsignmentOrders tb
                var consignment = await db.ConsignmentOrders.FirstOrDefaultAsync(c => c.ConsignmentCode.Equals(code, StringComparison.OrdinalIgnoreCase));
                if (consignment != null)
                {
                    string status = consignment.Status == 1 ? "In processing" : "Sold";
                    return $"Consignment Code: {consignment.ConsignmentCode}\nStatus: {status}\nTotal: {consignment.TotalAmount:N0} VND.";
                }
            }

            return $"Code \"{code}\" was not found in the system.";
        }

        private class ResponseData
        {
            public string response { get; set; }
        }
    }
}
