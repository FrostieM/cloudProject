using System.Configuration;
using System.Collections.Specialized;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Linq;


namespace GoogleSheetAccessProviderLib
{
	public class AccessProvider
    {
        private readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string applicationName = "GoogleSheetAccessProvider";
        private readonly string spreadsheetId = "1LrsHlVFmjkWU3vV6HOH1cj25jsxAUyUYYCZ-wNPoeD8";
        private readonly string apiKey = "AIzaSyAIdlTNEWiVbkIyPsvOene6ZkpbmD5hmck";
        private SheetsService service;

        public AccessProvider()
        {
            service = GetSheetsService();
        }

        public void WriteData(IList<IList<object>> data, string sheetName)
        {
            if (GetSheetId(sheetName) == -1)
                CreateNewSheet(sheetName);

            ClearSheet(sheetName);
            AppendEntries(data, sheetName);
        }

        public void AppendEntries(IList<IList<object>> data, string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var valueRange = new ValueRange();

            valueRange.Values = data;

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            appendRequest.Execute();
        }
              
        public void ClearSheet(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
            deleteRequest.Execute();
        }

        public void CreateNewSheet(string sheetName)
        {
            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                }
            };

            List<Request> requests = new List<Request> { addSheetRequest };

            BatchUpdateSpreadsheetRequest batchUpdate = new BatchUpdateSpreadsheetRequest();
            batchUpdate.Requests = requests;
            service.Spreadsheets.BatchUpdate(batchUpdate, spreadsheetId).Execute();
        }

        private int GetSheetId(string sheetName)
        {
            var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);

            if (sheet == null)
                return -1;

            int sheetId = (int)sheet.Properties.SheetId;
            return sheetId;
        }

        public IEnumerable<IEnumerable<string>> ReadEntries(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
            
            var response = request.Execute();
            var values = response.Values.Select(list => list.Select(listItem => listItem.ToString()));
            return values;
        }

        public IEnumerable<string> GetSheetNames()
        {
            var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
            
            var sheetList = new List<string>();
            foreach (var sheet in spreadsheet.Sheets)
            {
                sheetList.Add(sheet.Properties.Title);
            }

            return sheetList;
        }

        private UserCredential GetUserCredential()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetCreds.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "User",
                    CancellationToken.None,
                    new FileDataStore(credentialPath, true)).Result;
            }
        }

        private SheetsService GetSheetsService(UserCredential credential)
        {
            return new SheetsService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });
        }

        private SheetsService GetSheetsService()
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = applicationName,
                ApiKey = apiKey
            });
        }
    }
}
