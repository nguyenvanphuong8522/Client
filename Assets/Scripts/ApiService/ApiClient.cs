using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ApiClient : MonoBehaviour
{
    private HttpClient _httpClient;

    private string userName;

    private string password;

    private string url;

    private void Awake()
    {
        url = "https://localhost:7245/api/Account/login";
        _httpClient = new HttpClient();
    }

    public void SetNamePassword(string username, string password)
    {
        this.userName = username;
        this.password = password;
    }

    public async Task<bool> Login()
    {
        Account newCategory = new Account
        {
            UserName = userName,
            Password = password
        };


        string json = JsonConvert.SerializeObject(newCategory);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log($"Success: {responseBody}");
            return true;
        }
        Debug.LogError($"Failed: {response.StatusCode}");
        return false;
    }
    public async Task<bool> SignUp()
    {
        Account newCategory = new Account
        {
            UserName = userName,
            Password = password
        };

        string json = JsonConvert.SerializeObject(newCategory);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log($"Success: {responseBody}");
            return true;

        }
        Debug.LogError($"Failed: {response.StatusCode}");
        return false;
    }

}
