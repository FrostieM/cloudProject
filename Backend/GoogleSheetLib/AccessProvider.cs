using System.IO;
using System.Threading;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace GoogleSheetAccessProviderLib
{
    public enum AccessType { ApiKey, User, ServiceAccount };

    public class AccessProvider
    {
        private static JObject config = JObject.Parse(File.ReadAllText("spreadsheet_config.json"));

        private readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string applicationName = config["ApplicationName"].ToString();
        private readonly string spreadsheetId = config["SpreadsheetId"].ToString();
        private readonly string serviceAccount = config["ServiceAccount"].ToString();
        private readonly string apiKey = config["ApiKey"].ToString();
        private SheetsService service;

        public AccessProvider(AccessType accessType)
        {

            switch (accessType)
            {
                case AccessType.ApiKey:
                    {
                        service = GetSheetsService();
                    }
                    break;
                case AccessType.User:
                    {
                        var credential = GetUserCredential();
                        service = GetSheetsService(credential);
                    }
                    break;
                case AccessType.ServiceAccount:
                    {
                        var credential = GetServiceAccountCredential();
                        service = GetSheetsService(credential);
                    }
                    break;
                default:
                    service = GetSheetsService();
                    break;
            }
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

            var requests = new List<Request> {addSheetRequest};

            var batchUpdate = new BatchUpdateSpreadsheetRequest();
            batchUpdate.Requests = requests;
            service.Spreadsheets.BatchUpdate(batchUpdate, spreadsheetId).Execute();
        }

        private int GetSheetId(string sheetName)
        {
            var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);

            if (sheet == null)
                return -1;

            var sheetId = (int) sheet.Properties.SheetId;
            return sheetId;
        }

        public IEnumerable<IEnumerable<string>> ReadEntries(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

            var response = request.Execute();
            var values = response.Values.Select(list => list.Select(listItem => listItem.ToString()));
            return values;
        }

        public IEnumerable<string> GetSheetNames()
        {
            var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();

            return spreadsheet.Sheets.Select(sheet => sheet.Properties.Title).ToList();
        }

        private UserCredential GetUserCredential()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetCreds.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "User",
                    CancellationToken.None,
                    new FileDataStore(credentialPath, true)).Result;
            }
        }

        private ServiceAccountCredential GetServiceAccountCredential()
        {
            using (Stream stream = new FileStream("service_account_secret.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var credential = (ServiceAccountCredential)
                    GoogleCredential.FromStream(stream).UnderlyingCredential;

                var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                {
                    User = serviceAccount,
                    Key = credential.Key,
                    Scopes = scopes
                };
                return new ServiceAccountCredential(initializer);
            }
        }

        private SheetsService GetSheetsService(ICredential credential)
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