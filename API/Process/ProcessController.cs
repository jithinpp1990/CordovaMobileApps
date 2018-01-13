using System;
using CCBankWebAPI.Dtos;
using CCBankWebAPI.Helpers;
using UrbanBank.Helpers;
using System.Configuration;
using System.Collections.Generic;

namespace CCBankWebAPI.Process
{

    public interface IProcessController
    {
        ValidateMemberResposeDto MemberValidation(MemberLoginRequestDto request);
    }
    internal class ProcessController : IProcessController
    {
        private DataBaseHelper _dbHelper;
        internal ProcessController()
        {
            _dbHelper = new DataBaseHelper();
        }

        #region Easy Trans Region 
        public ValidateMemberResposeDto MemberValidation(MemberLoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.UserrName) || string.IsNullOrEmpty(request.Password))
                return new ValidateMemberResposeDto() { AuthenticationSuccess = false, ErrorMessage = ErrorCodes.InvalidCredentials };
            var response = _dbHelper.ValidateMemberLogin(request.UserrName, request.Password, request.DeviceId);
            return response;
        }

        public GetBranchListResponseDto GetBranchList()
        {
            return _dbHelper.GetBranchList();
        }

        public GetManagementDetailsResponseDto GetManagementDetails()
        {
            return _dbHelper.GetManagementDetails();
        }

        public ChequeBookResponseDto GetInitialDataForChequeBook(RequestBase request)
        {
            int StmtFromDateDiff;
            var response = _dbHelper.GetInitialeckDataForChequeRequest(Convert.ToInt64(request.MemberId),request.Service.ToString());
            response.StmtFromDateDiff = int.TryParse(ConfigurationManager.AppSettings["StmtFromDate"], out StmtFromDateDiff) ? StmtFromDateDiff : 0;
            return response;
        }

        public NewChequeResponseDto RequestNewChequeBook(CheckBookRequestDto request)
        {
            var response = _dbHelper.RequestNewChequeBook(Convert.ToInt64(request.DepositId));
            return new NewChequeResponseDto() { IsChequeRequestSuccesful = response };
        }

        public AppyStatementModel RequestStatement(GetInitialDataForAccountStatementRequestDto request, long memberId)
        {
            decimal openingbalance;
            AppyStatementModel response = new AppyStatementModel();
            response.ResponseStatement = new StatementResponseDto();
            if(request.Service== "accountstatement")
            {
                response.ResponseStatement.StatementDetails = _dbHelper.GetStatementTransactionDetails(memberId, request.AccountHeadModel.DepositId, request.AccountHeadModel.AccountTypeId, request.dateFrom, request.dateTo, out openingbalance);
                response.ResponseStatement.OpeningBalance = openingbalance;
            }
            else if (request.Service == "accountstatementtrade")
            {
                response.ResponseStatement.StatementDetails = _dbHelper.GetStatementTransactionDetailsTrade(memberId, request.AccountHeadModel.DepositId, request.AccountHeadModel.AccountTypeId, request.dateFrom, request.dateTo, out openingbalance);
                response.ResponseStatement.OpeningBalance = openingbalance;
            }

            return response;

        }

        public AppyStatementModel RequestBalance(GetInitialDataForAccountStatementRequestDto request, long memberId)
        {
            AppyStatementModel response = new AppyStatementModel();
            response.ResponseStatement = new StatementResponseDto();
            //response.ResponseStatement.MemberDetails = _dbHelper.GetStatmentMemberDetails(memberId, request.AccountHeadModel.DepositId, request.dateTo);
            response.ResponseStatement.BalanceAmt = _dbHelper.GetAccountBalance(request.AccountHeadModel.AccountTypeId,request.AccountHeadModel.DepositId);
            return response;

        }

        public AccountSummayResponseDto GetAccountSummary(AccountSummaryRequestDto request)
        {
            AccountSummayResponseDto response = new AccountSummayResponseDto();
            response.AccountSummaryList = _dbHelper.GetAccountSummary(Convert.ToInt64(request.MemberId));
            // response.MemberDetails = _dbHelper.GetMemberDetailsById(Convert.ToInt64(request.MemberId));
            return response;
        }

        public MemberModel GetAccountDetails(RequestBase request)
        {
            return _dbHelper.GetMemberDetailsById(Convert.ToInt64(request.MemberId));
        }

        public NotificationsDto GetNotifications(RequestBase request)
        {
            return _dbHelper.GetNotifications(Convert.ToInt64(request.MemberId));
        }

        internal bool ValidateRequest(RequestBase request)
        {
            return true;
            //return _dbHelper.ValidateRequest(request);
        }

        internal VerifyCustomerResponseDto VerifyCustomer(string customerId)
        {
            var response = _dbHelper.VerifyCustomer(customerId);
            return response;
        }

        internal VerifyCustomerResponseDto VerifyOTP(VerifyCustomerRequestDto model)
        {
            var response = _dbHelper.VerifyOTP(model);
            return response;
        }

        internal VerifyCustomerResponseDto UpdatePassword(string newpassword, string customerId, string deviceId,string oldpassword,string type)
        {
            var response = _dbHelper.UpdatePassword(newpassword, customerId, deviceId,oldpassword,type);
            return response;
        }
        //internal VerifyCustomerResponseDto UpdatePIN(string oldpassword, string password, string customerId, string deviceId)
        //{
        //    var response = _dbHelper.UpdatePassword(password, customerId, deviceId,oldpassword);
        //    return response;
        //}
        internal IList<BeneficiaryModel> GetBeneficiaries(BeneficiaryRequestModel request)
        {
            var response = _dbHelper.GetBeneficiaryList(Convert.ToInt64(request.MemberId), request.TransferType);
            return response;
        }

        internal GetInitialDataForFundTransferResponseDto GetInitialDataForFundTransfer(BeneficiaryRequestModel request)
        {
            var response = _dbHelper.GetInitialDataForFundTransfer(request);
            return response;
        }

        internal bool SendOTP(RequestBase request)
        {
            var response = _dbHelper.SendOTP(request.UserId);
            return response;
        }
        internal bool SendOTPCustomer(RequestBase request)
        {
            var response = _dbHelper.SendOTP(request.UserId,request.MemberId);
            return response;
        }

        internal string SubmitTransfer(TransferRequestDto model)
        {
            return _dbHelper.SubmitTransfer(model);
        }
        internal SMSContentModel SubmitTransfer_new(TransferRequestDto model)
        {
            var Messagemodel = _dbHelper.SubmitTransfer_new(model);
            if (Messagemodel.status == "S")
            {
                var response = new TradeTranResponseModel();
                var status_cust = _dbHelper.SendMessage(Messagemodel.CustomerMessage, Messagemodel.CustomerMobile, Messagemodel.CustomerEmail);
                var status_trad = _dbHelper.SendMessage(Messagemodel.TraderMessage, Messagemodel.TraderMobile, Messagemodel.TraderEmail);
                return Messagemodel;
            }
            else
            {
                return Messagemodel;
            }
        }
        //internal string SubmitTradeTran(TradeTransRequestDto model)
        //{
        //    string FullString = null;
        //    string message = null;
        //    string mobile = null;
        //    string email = null;
        //    FullString = _dbHelper.SubmitTradeTran(model);
        //    message = FullString.Substring(0, FullString.IndexOf('*'));
        //    mobile = FullString.Substring(FullString.IndexOf('*')+1,(FullString.IndexOf('*', FullString.IndexOf('*') + 1)- FullString.IndexOf('*'))-1);
        //    email= FullString.Substring(FullString.LastIndexOf('*')+1,(FullString.Length- FullString.LastIndexOf('*'))-1);
        //   var status= _dbHelper.SendMessage(message, mobile, email);
        //    return message;
        //}
        internal SMSContentModel SubmitTradeTran(TradeTransRequestDto model)
        {
            var Messagemodel = _dbHelper.SubmitTradeTran(model);
            if(Messagemodel.status=="S")
            {
                var response = new TradeTranResponseModel();
                var status_cust = _dbHelper.SendMessage(Messagemodel.CustomerMessage,Messagemodel.CustomerMobile, Messagemodel.CustomerEmail);
                var status_trad = _dbHelper.SendMessage(Messagemodel.TraderMessage, Messagemodel.TraderMobile, Messagemodel.TraderEmail);
                return Messagemodel;
            }
            else
            {
                return Messagemodel;
            }          
        }
        internal IFSCModel ValidateIFSC(IFSCModel model)
        {
            return _dbHelper.ValidateIFSC(model);
        }
        internal TraderRequestModel ValidateTrader(TraderRequestModel model)
        {
            return _dbHelper.ValidateTrader(model);
        }
        internal string AddBeneficiary(BeneficiaryModel model)
        {           
            return _dbHelper.AddBeneficiary(model);
        }

        internal string DeleteBeneficary(BeneficiaryModel model)
        {
            return _dbHelper.AddBeneficiary(model);
        }

        #endregion

        #region Business Correpondent Region
        public ValidateMemberResposeDto AgentValidation(MemberLoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.UserrName) || string.IsNullOrEmpty(request.Password))
                return new ValidateMemberResposeDto() { AuthenticationSuccess = false, ErrorMessage = ErrorCodes.InvalidCredentials };
            var response = _dbHelper.ValidateAgentLogin(request.UserrName, request.Password, request.DeviceId);
            return response;
        }
        internal VerifyCustomerResponseDto VerifyAgent(string agentCode)
        {
            var response = _dbHelper.VerifyAgent(agentCode);
            return response;
        }

        internal VerifyCustomerResponseDto VerifyAgentOTP(VerifyCustomerRequestDto model)
        {
            var response = _dbHelper.VerifyAgentOTP(model);
            return response;
        }

        internal VerifyCustomerResponseDto UpdateAgentPassword(VerifyCustomerRequestDto model)
        {
            var response = _dbHelper.UpdateAgentPassword(model);
            return response;
        }

        public AcHeadListDto GetAgentAccountHeads(RequestBase request)
        {
            var response = _dbHelper.GetAgentAccountHeads(request);            
            return response;
        }


        internal CustomerDetDto ValidateAcNo(AccountNoValidateRequestDto request)
        {
            return _dbHelper.ValidateAcNo(request);
        }

        internal SMSContentModel SubmitCollection(CollectionSubmitRequestDto request)
        {
            var Messagemodel = _dbHelper.SubmitCollection(request);
            if (Messagemodel.status == "S")
            {
                var response = new TradeTranResponseModel();
                var status_cust = _dbHelper.SendMessage(Messagemodel.CustomerMessage, Messagemodel.CustomerMobile, Messagemodel.CustomerEmail);
                var status_trad = _dbHelper.SendMessage(Messagemodel.TraderMessage, Messagemodel.TraderMobile, Messagemodel.TraderEmail);
                return Messagemodel;
            }
            else
            {
                return Messagemodel;
            }
        }

        internal List<CustomerDetDto> GetCollectionAcList(RequestBase request)
        {
            var collectionAcList = _dbHelper.GetCollectionAcList(request);
            return collectionAcList;
        }

        internal List<CollectionStmtResponseDto> GetCollectionStatement(CollectionStmtRequestDto request)
        {
            return _dbHelper.GetCollectionStatement(request);
        }
        internal AgentProfile GetAgentAccountDetails(RequestBase request)
        {
            return _dbHelper.GetAgentAccountDetails(request);
        }


        #endregion

    }
}