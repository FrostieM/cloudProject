using System.IO;
using System.Threading;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Linq;
using Google.Apis.Http;
using System;

namespace GoogleSheetLib
{
    public enum AccessType { ApiKey, User, ServiceAccount };

    public class AccessProvider
    {
        private readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string applicationName = SpreadsheetConfigReader.ApplicationName;
        private readonly string spreadsheetId = SpreadsheetConfigReader.SpreadsheetId;
        private readonly string serviceAccount = SpreadsheetConfigReader.ServiceAccount;
        private readonly string apiKey = SpreadsheetConfigReader.ApiKey;
        private readonly AccessType accessType;
        private SheetsService service = null;

        private SheetsService Service
        {
            get
            {
                if (service == null)
                    service = GetSheetsService(accessType);
                return service;
            }
        }

        public AccessProvider(AccessType type)
        {
            accessType = type;
            if (HasInternet())
                service = GetSheetsService(accessType);
        }

        public bool WriteData(IList<IList<object>> data, string sheetName)
        {
            if (!HasInternet(true))
                return false;

            if (!HasSheet(sheetName))
                CreateNewSheet(sheetName);

            if(ClearSheet(sheetName))
                return AppendEntries(data, sheetName);

            return false;
        }

        public bool AppendEntries(IEnumerable<IEnumerable<object>> data, string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var valueRange = new ValueRange();

            valueRange.Values = data.Cast<IList<object>>().ToList();

            var appendRequest = Service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            AppendValuesResponse response;
            try { response = appendRequest.Execute(); }
            catch (Exception e)
            {
                Console.WriteLine("AppendEntries method exception:\n" + e.Message);
                response = null;
            }

            return response != null;
        }

        public bool ClearSheet(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var requestBody = new ClearValuesRequest();

            var deleteRequest = Service.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);

            ClearValuesResponse response;
            try { response = deleteRequest.Execute(); }
            catch (Exception e)
            {
                Console.WriteLine("ClearSheet method exception:\n" + e.Message);
                response = null;
            }

            return response != null;
        }
        
        public bool CreateNewSheet(string sheetName)
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
            var requests = new List<Request> { addSheetRequest };

            var batchUpdate = new BatchUpdateSpreadsheetRequest();
            batchUpdate.Requests = requests;

            BatchUpdateSpreadsheetResponse response;
            try { response = Service.Spreadsheets.BatchUpdate(batchUpdate, spreadsheetId).Execute(); }
            catch (Exception e)
            {
                Console.WriteLine("CreateNewSheet method exception:\n" + e.Message);
                response = null;
            }

            return response != null;
        }

        public IEnumerable<IEnumerable<string>> ReadEntries(string sheetName, string Range = "A:Z")
        {
            var range = $"{sheetName}!" + Range;
            var response = Service.Spreadsheets.Values.Get(spreadsheetId, range).Execute();
            var values = response.Values.Select(list => list.Select(listItem => listItem.ToString()));
            return values;
        }

        public IEnumerable<string> GetSheetNames()
        {
            var spreadsheet = Service.Spreadsheets.Get(spreadsheetId).Execute();
            return spreadsheet.Sheets.Select(sheet => sheet.Properties.Title).ToList();
        }

        public bool HasSheet(string sheetName)
        {
            return GetSheetId(sheetName) != -1;
        }

        public int GetSheetId(string sheetName)
        {
            var spreadsheet = Service.Spreadsheets.Get(spreadsheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);

            if (sheet == null)
                return -1;

            var sheetId = (int) sheet.Properties.SheetId;
            return sheetId;
        }
        
        private UserCredential GetUserCredential()
        {
            using (var stream = new FileStream("configs/client_secret.json", FileMode.Open, FileAccess.Read))
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
            using (Stream stream = new FileStream("configs/service_account_secret.json", FileMode.Open, FileAccess.Read, FileShare.Read))
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
                    HttpClientFactory = GetHttpClientFactory(),
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });
        }

        private SheetsService GetSheetsService()
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientFactory = GetHttpClientFactory(),
                ApplicationName = applicationName,
                ApiKey = apiKey
            });
        }

        private SheetsService GetSheetsService(AccessType type)
        {
            if (type == AccessType.ServiceAccount || type == AccessType.User)
                return type == AccessType.ServiceAccount ?
                    GetSheetsService(GetServiceAccountCredential()) :
                    GetSheetsService(GetUserCredential());            
            else            
                return GetSheetsService();            
        }

        private HttpClientFactory GetHttpClientFactory()
        {
            return SpreadsheetConfigReader.UseProxy ?
                    new ProxyHttpClientFactory() :
                    new HttpClientFactory();
        }

        private bool HasInternet(bool print = false)
        {
            var status = ConnectionChecker.HasInternet();
            if(print)
            {
                var msg = status ? "Internet available." : "No internet!";
                Console.WriteLine(msg);
            }
            return status;
        }
    }
}