using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSheetController
{
    private GoogleCredential credential;
    private SheetsService service;
    private string spreadSheetID = "1bwz3bRaBck3JGaxiz7xtP5yqQE04KWFcODRKqQFuNEM";
    private string dataPath = Path.Combine(Application.dataPath, "Resources", "Data");

    public DataSheetController()
    {
        InitializeCredential();
    }

    private void InitializeCredential()
    {
        try
        {
            // OAuth2 인증 설정
            string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string ApplicationName = "DungeonRestaurant";

            TextAsset credentialsFile = Resources.Load<TextAsset>("credentials");

            using (var stream = new MemoryStream(credentialsFile.bytes))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            // SheetService 초기화
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("!! API Auth Exception !! : " + ex.Message);
        }
    }

    private string GetJSON(string range)
    {
        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadSheetID, range);

        ValueRange response = request.Execute();
        IList<IList<System.Object>> values = response.Values;

        if (values != null && values.Count > 0)
        {
            var headers = values[0];
            var jsonArray = new List<Dictionary<string, object>>();

            for (int i = 1; i < values.Count; i++)
            {
                var row = values[i];

                if (string.IsNullOrEmpty(row[0]?.ToString()))
                {
                    break;
                }

                var jsonObject = new Dictionary<string, object>();

                for (int j = 0; j < row.Count; j++)
                {
                    string header = headers[j].ToString();
                    if (!header.StartsWith("#"))
                    {
                        jsonObject[headers[j].ToString()] = row[j];
                    }
                    
                }

                jsonArray.Add(jsonObject);
            }

            var json = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);
            return json;
        }
        else
        {
            Debug.Log($"Can't load data in sheet : {range}");
            return null;
        }
    }


    public List<T> DeserializeData<T>()
    {
        var filePath = Path.Combine(dataPath, $"{typeof(T).Name}.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<List<T>>(json);
            return result;
        }
        else
        {
            TextAsset jsonFile = Resources.Load<TextAsset>($"Data/{typeof(T).Name}");
            if (jsonFile != null)
            {
                string json = jsonFile.text;
                var result = JsonConvert.DeserializeObject<List<T>>(json);
                return result;
            }
            else
            {
                Debug.LogError("File not found : " + filePath);
                return null;
            }
        }
    }

    public List<T> DeserializeDataFromSheet<T>(bool isJSONFileSave)
    {
        try
        {
            string sheetName = typeof(T).Name;
            string range = $"{sheetName}!A1:Z";

            var json = GetJSON(range);
            var result = JsonConvert.DeserializeObject<List<T>>(json);
            if (isJSONFileSave)
            {
                var filePath = Path.Combine(dataPath, $"{sheetName}.json");
                File.WriteAllText(filePath, json);
            }
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"DeserializeDataFromSheet() Fail : {ex.Message}");
            throw ex;
        }
    }

    public List<T> Deserialize<T>()
    {
        List<T> resultList;
        try
        {
            resultList = DeserializeDataFromSheet<T>(true);
        }
        catch
        {
            resultList = DeserializeData<T>();
        }

        return resultList;
    }

    private void DownloadEntireDataSheet()
    {
        try
        {
            // 스프레드 시트 데이터 요청
            var spreadSheet = service.Spreadsheets.Get(spreadSheetID).Execute();
            var sheets = spreadSheet.Sheets;

            int sheetIndex = 0;
            int nonDataSheetCount = 2;

            foreach (var sheet in sheets)
            {
                if (sheetIndex < nonDataSheetCount)
                {
                    sheetIndex++;
                    continue;
                }

                string range = $"{sheet.Properties.Title}!A1:Z";

                var json = GetJSON(range);
                var filePath = Path.Combine(dataPath, $"{sheet.Properties.Title}.json");
                File.WriteAllText(filePath, json);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"!! Datasheet 로드 중 예외 발생 !! : {ex.Message}");
        }
    }
}
