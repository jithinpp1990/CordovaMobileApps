using CCBankWebAPI.Dtos;
using CCBankWebAPI.Process;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CCBankWebAPI.Controllers
{
    public class ContentController : ApiController
    {
        private ProcessController _processController;
        private static string DocumentPath = $"{ConfigurationManager.AppSettings["DocumentPath"]}";
        //[HttpPost]
        public HttpResponseMessage GetDocument()
        {

            //if (!_processController.ValidateRequest(request))
            //    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            return FileAsAttachment("TestDocument.pdf");
        }
        public HttpResponseMessage test()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "test"); ;
        }

        [HttpGet]
        public HttpResponseMessage GetDocumentNames(RequestBase request)
        {
            //if (!_processController.ValidateRequest(request))
               // return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var files = new DirectoryInfo(DocumentPath).EnumerateFiles()
                            .Select(x => Path.GetFileNameWithoutExtension(x.Name))
                            .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, files);
        }
        private static HttpResponseMessage FileAsAttachment(string filename)
        {
            if (File.Exists(DocumentPath))
            {
                //if (Directory.GetFiles(DocumentPath, "*.*").Length > 0)
                //{
                HttpResponseMessage result = new HttpResponseMessage();
                var files = new DirectoryInfo(DocumentPath).GetFiles($"*{filename}*.*");
                if (!files.Any())
                {
                    result.StatusCode = HttpStatusCode.NoContent;
                    return result;
                }
                using (var stream = files.First().OpenRead())
                {
                    result.Content = new StreamContent(stream);
                }
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = filename;
                return result;
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
