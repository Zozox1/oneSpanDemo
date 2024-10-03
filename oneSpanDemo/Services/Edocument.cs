namespace oneSpanDemo.Services
{
    using Models;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class Edocument : Iedocument
    {
        private IConfiguration configuration;
        private IoneSpanAuthorize iOneSpanAuthorize;
        private readonly string sandBoxUrl;
        private const int minDocumentsBytes = 1000;

        public Edocument(IConfiguration _iconfig,IoneSpanAuthorize _iOneSpanAuthorize)
        {
            configuration = _iconfig;
            iOneSpanAuthorize = _iOneSpanAuthorize;
            sandBoxUrl = configuration.GetValue<string>("OneSpanSandboxUrl");
        }
        public async Task<DocumentsResult> GetOneSpanDocument(string transaction)
        {
            DocumentsResult documentsResult = new DocumentsResult();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string token = iOneSpanAuthorize.GetAccessToken();

                    if (string.IsNullOrEmpty(token))
                    {
                        documentsResult.status = DocumentsResultStatus.UnAuthorized;
                        return documentsResult;
                    }

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                    Stream filesToReturn = await httpClient.GetStreamAsync(sandBoxUrl + "api/packages/" + transaction + "/documents/zip");

                    if (filesToReturn == null || filesToReturn.Length < minDocumentsBytes)
                        documentsResult.status = DocumentsResultStatus.NoFile;
                    else
                    {
                        documentsResult.status = DocumentsResultStatus.OK;
                        documentsResult.stream = filesToReturn;
                    }

                }
            }
            catch (Exception ex) 
            {
                documentsResult.status = DocumentsResultStatus.UnAbleToLoad;
                //TODO Logging 
            }
            return documentsResult;
        }
    }
}
