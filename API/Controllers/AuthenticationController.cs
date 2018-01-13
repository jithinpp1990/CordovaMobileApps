using CCBankWebAPI.Dtos;
using CCBankWebAPI.Process;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CCBankWebAPI.Controllers
{
    public class AuthenticationController : ApiController
    {
        [Dependency]
        public IProcessController _processController { get; set; }

        #region Easy Trans Region
        [HttpPost]
        public HttpResponseMessage AuthenticateUser(MemberLoginRequestDto model)
        {
            //object result = null;
            var result = new ProcessController().MemberValidation(model);
            result = new ProcessController().MemberValidation(model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpGet]
        public HttpResponseMessage test()
        {
            var response = "test success";
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpGet]
        public HttpResponseMessage GetBranchList()
        {
            var response = new ProcessController().GetBranchList();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage GetImages()
        {
            var response = getImageAsByteArray();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        private IList<ImageModel> getImageAsByteArray()
        {
            var response = new List<ImageModel>();
            var path = ConfigurationManager.AppSettings["ImagePath"].ToString();
            foreach (var image in Directory.EnumerateFiles(path, "*.png", SearchOption.TopDirectoryOnly))
            {
                var imageModel = new ImageModel();
                using (var ms = new MemoryStream())
                {
                    var imageFile = Image.FromFile(image);
                    imageFile.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    imageModel.ImageByte = ms.ToArray();
                    imageModel.ImageName = Path.GetFileName(image);
                }
                response.Add(imageModel);
            }
            return response;
        }

        public static BranchDetailsDto GetDummy()
        {
            var BranchDetails = new BranchDetailsDto();
            BranchDetails.BranchName = "Maradu";
            BranchDetails.Address = new List<string>() { "Jayanthy Road", "Maradu P.O", "Tripunithura", "Ernakulam" };
            BranchDetails.PhoneNumber = "9995511616";
            BranchDetails.Email = "ccban@gmail.com";
            BranchDetails.Manager = "Sugunan";
            BranchDetails.ManagerContact = "956111445";
            BranchDetails.Latitude = 9.948294M;
            BranchDetails.Longitude = 76.347777M;
            return BranchDetails;
        }

        public static ManagementDetailsDto GetMgmtDummy()
        {
            var mgmt = new ManagementDetailsDto()
            {
                Designation = "Vice President",
                Name = "Shaji",
                PhoneNumber = "9995511616"
            };
            return mgmt;
        }

        [HttpGet]
        public HttpResponseMessage GetManagementDetails()
        {
            var response = new ProcessController().GetManagementDetails();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion

        #region Business Correpondent Region

        [HttpPost]
        public HttpResponseMessage AuthenticateAgent(MemberLoginRequestDto model)
        {
            var result = new ProcessController().AgentValidation(model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        #endregion
    }
}
