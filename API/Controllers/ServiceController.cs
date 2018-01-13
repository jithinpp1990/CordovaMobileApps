using CCBankWebAPI.Dtos;
using CCBankWebAPI.Helpers;
using CCBankWebAPI.Process;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace CCBankWebAPI.Controllers
{

    public class ServiceController : ApiController
    {
        private ProcessController _processController;
        public ServiceController()
        {
            _processController = new ProcessController();
        }

        #region Easy Trans Region

        #region ChequeBook

        [HttpPost]
        public HttpResponseMessage GetAccountHeads(RequestBase model)
        {
                
            if (!_processController.ValidateRequest(model))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetInitialDataForChequeBook(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage test()
        {
            var response = "test success";
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        //[HttpPost]
        ////public HttpResponseMessage GetServices(RequestBase model)
        ////{

        ////    //if (!_processController.ValidateRequest(model))
        ////       // return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
        ////    var response = _processController.GetServices();
        ////    return Request.CreateResponse(HttpStatusCode.OK, response);
        ////}

        //[HttpPost]
        public HttpResponseMessage RequestCheckBook(CheckBookRequestDto model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.RequestNewChequeBook(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion

        #region Statement
        [HttpPost]
        public HttpResponseMessage RequestStatement(StatementRequestDto model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var request = new GetInitialDataForAccountStatementRequestDto();
            request.AccountHeadModel.DepositId = model.DepositId;
            request.AccountHeadModel.AccountTypeId = model.AccountType;
            request.dateFrom = model.DateFrom.ToString("yyyy-MM-dd");
            request.dateTo = model.DateTo.ToString("yyyy-MM-dd");
            request.AppId = model.AppId;
            request.Service = model.Service;
            request.MemberId = model.MemberId;
            var response = _processController.RequestStatement(request, Convert.ToInt64(model.MemberId));
            response.DateFrom = request.dateFrom;
            response.DateTo = request.dateTo;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion


        #region Statement
        [HttpPost]
        public HttpResponseMessage RequestBalance(StatementRequestDto model)
        {
           // var response = "1000000";

            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var request = new GetInitialDataForAccountStatementRequestDto();
            request.AccountHeadModel.DepositId = model.DepositId;
            request.AccountHeadModel.AccountTypeId = model.AccountType;           
            var response = _processController.RequestBalance(request, Convert.ToInt64(model.MemberId));
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion

        #region Account Summary
        [HttpPost]
        public HttpResponseMessage GetAccountSummary(AccountSummaryRequestDto model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetAccountSummary(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion

        #region Password Reset

        [HttpPost]
        public HttpResponseMessage VerifyCustomer(VerifyCustomerRequestDto model)
        {

            if (string.IsNullOrEmpty(model.CustomerId))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid Customer id");
            var response = _processController.VerifyCustomer(model.CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage VerifyOTP(VerifyCustomerRequestDto model)
        {
            if (string.IsNullOrEmpty(model.OTP))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid OTP");
            var response = _processController.VerifyOTP(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage UpdatePassword(VerifyCustomerRequestDto model)
        {
            var expression = new Regex("^(?=.{8,20}$)(?=.*[0-9])(?=.*[a-zA-Z]).*");
            if (!expression.IsMatch(model.Password))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid password format");
            var response = _processController.UpdatePassword(model.Password, model.CustomerId, model.DeviceId,model.OldPassword,model.Type);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage UpdatePIN(VerifyCustomerRequestDto model)
        {
            if (string.IsNullOrEmpty(model.OldPassword))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid Old PIN");
            var response = _processController.UpdatePassword(model.Password, model.CustomerId, model.DeviceId, model.OldPassword, model.Type);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion

        #region Account details
        [HttpPost]
        public HttpResponseMessage GetAccountDetails(RequestBase model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetAccountDetails(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion

        #region Notifications
        [HttpPost]
        public HttpResponseMessage GetNotifications(RequestBase model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetNotifications(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion

        #region Fund transfer

        [HttpPost]
        public HttpResponseMessage GetTransferDetails(BeneficiaryRequestModel model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetInitialDataForFundTransfer(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SendOTP(RequestBase model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.SendOTP(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        public HttpResponseMessage SendOTPCustomer(CustomerModel model)
        {
            var response = false; ;
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            if (model.Service == "TI" || model.Service == "collection")
            {
                model.MemberId = model.CustomerMemberId;
                response = _processController.SendOTPCustomer(model);
            }
            else
            {
                response = _processController.SendOTP(model);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SubmitTransfer_old(TransferRequestDto model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.SubmitTransfer(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpPost]
        public HttpResponseMessage SubmitTransfer(TransferRequestDto model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.SubmitTransfer_new(model);
            return Request.CreateResponse(HttpStatusCode.OK, response.CustomerMessage);
        }


        [HttpPost]
        public HttpResponseMessage SubmitTradeTran(TradeTransRequestDto model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.SubmitTradeTran(model);
            if (response.status=="S")
            {
                response.ReceiptHtml = TemplateManager.GetReceipt(DateTime.Now.Date, model.RefNo, "", "Rs. " + model.Amount);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage GetBeneficiaryList(BeneficiaryRequestModel model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetBeneficiaries(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage GetBeneficiary(BeneficiaryRequestModel model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetBeneficiaries(model).ToList().FirstOrDefault(x => x.Id == model.BeneficiaryId);
            response.BankBranch = "Tripunithura";
            response.BankName = "ICICI";
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage ValidateTrader(TraderRequestModel model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.ValidateTrader(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion

        #region Beneficiary
        [HttpPost]
        public HttpResponseMessage ValidateIFSC(IFSCModel model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.ValidateIFSC(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        public HttpResponseMessage AddBeneficiary(BeneficiaryModel model)
        {
            try
            {
                if (!_processController.ValidateRequest(model as RequestBase))
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
                var response = _processController.AddBeneficiary(model);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.ToString());
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteBeneficiary(BeneficiaryModel model)
        {
            if (!_processController.ValidateRequest(model as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.DeleteBeneficary(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion
        #endregion

        #region Business Correpondent Region
        [HttpPost]
        public HttpResponseMessage VerifyAgent(VerifyCustomerRequestDto request)
        {

            if (string.IsNullOrEmpty(request.AgentCode))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid Customer id");
            var response = _processController.VerifyAgent(request.AgentCode);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage VerifyAgentOTP(VerifyCustomerRequestDto request)
        {
            if (string.IsNullOrEmpty(request.OTP))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid OTP");
            var response = _processController.VerifyAgentOTP(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage UpdateAgentPassword(VerifyCustomerRequestDto model)
        {
            var expression = new Regex("^(?=.{8,20}$)(?=.*[0-9])(?=.*[a-zA-Z]).*");
            if (!expression.IsMatch(model.Password))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid password format");
            var response = _processController.UpdateAgentPassword(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage UpdateAgentPIN(VerifyCustomerRequestDto request)
        {
            if (string.IsNullOrEmpty(request.OldPassword))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Invalid Old PIN");
            var response = _processController.UpdatePassword(request.Password, request.CustomerId, request.DeviceId, request.OldPassword, request.Type);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage GetAgentAccountHeads(RequestBase request)
        {

            if (!_processController.ValidateRequest(request))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetAgentAccountHeads(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage ValidateAcNo(AccountNoValidateRequestDto request)
        {
            if (!_processController.ValidateRequest(request as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.ValidateAcNo(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SubmitCollection(CollectionSubmitRequestDto request)
        {
            if (!_processController.ValidateRequest(request as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.SubmitCollection(request);
            if (response.status == "S")
            {
                response.ReceiptHtml = TemplateManager.GetReceipt(DateTime.Now.Date, request.RefNo, "", "Rs. " + request.Amount);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage GetCollectionAcList(RequestBase request)
        {
            if (!_processController.ValidateRequest(request as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetCollectionAcList(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }


        [HttpPost]
        public HttpResponseMessage GetCollectionStatement(CollectionStmtRequestDto request)
        {
            var response = _processController.GetCollectionStatement(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage GetAgentAccountDetails(RequestBase request)
        {
            if (!_processController.ValidateRequest(request as RequestBase))
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new UnauthorizedAccessException("Unauthorized access. Please try to log in again."));
            var response = _processController.GetAgentAccountDetails(request);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        #endregion
    }
}