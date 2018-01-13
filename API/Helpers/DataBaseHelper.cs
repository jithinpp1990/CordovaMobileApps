using System.Data;
using System;
using System.Data.Odbc;
using System.Linq;
using CCBankWebAPI.Helpers;
using CCBankWebAPI.Dtos;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using CCBankWebAPI.Infrastructure;
using System.Globalization;

namespace UrbanBank.Helpers
{
    public class DataBaseHelper
    {
        private OdbcCommand _command;
       
        private OdbcDataAdapter _adapter;
        private DataSet _dataset;
        private DataTable _datatable;
        private OdbcConnection _conn;
        private const string _salt = "!CCBan@kMobile#App$";
        private object dateTimeStyles;

        public DataBaseHelper()
        {
            _conn = ConnectionHelper.Open(_conn);
            _dataset = new DataSet();
        }
        #region  Easy Trans Region 
        public ValidateMemberResposeDto ValidateMemberLogin(string username, string password, string deviceid)
        {

            int LoginAttempts = 0;
            var response = new ValidateMemberResposeDto();
            response.AuthenticationSuccess = true;
            _command = new OdbcCommand(QueryBuilder.GetMemberForAuthenitcation(), _conn);
            _command.Parameters.Add(new OdbcParameter("username", username));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Members);
            if (_dataset.Tables[EntityName.Members].Rows.Count == 0)
            {
                response.AuthenticationSuccess = false;
                response.ErrorMessage = ErrorCodes.InvalidCredentials;
                return response;
            }
            //implement BCrypt here
            var MemberTable = _dataset.Tables[EntityName.Members].Rows[0];
            LoginAttempts = int.TryParse(MemberTable["login_attempts"].ToString(), out LoginAttempts) ? LoginAttempts : 0;
            var DeviceId = MemberTable["device_id"].ToString();
            var Password = MemberTable["member_password"].ToString();
            var Name = MemberTable["name"].ToString();
            var MemberId = MemberTable["member_id"].ToString();
            var AppId = MemberTable["appid"].ToString();
            if (!BCrypt.Net.BCrypt.CheckPassword(password, Password))
            {
                response.AuthenticationSuccess = false;
                response.ErrorMessage = ErrorCodes.InvalidCredentials;
                UpdateLoginAttempt(username, LoginAttempts);
            }
            if (!string.Equals(deviceid, DeviceId))
            {
                response.ErrorMessage = ErrorCodes.InvalidMobileNumber;
                response.AuthenticationSuccess = false;
            }
            if (LoginAttempts > 3)
            {
                response.AuthenticationSuccess = false;
                response.ErrorMessage = ErrorCodes.LoginAttemptsExceeded;
                UpdateLoginAttempt(username, LoginAttempts);
            }
            if (!response.AuthenticationSuccess)
                return response;
            CreateSessionToken(username, response);
            response.SessionToken.Username = new string(Name.Take(12).ToArray());
            response.SessionToken.MemberId = MemberId;
            response.SessionToken.AppId = AppId;
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal VerifyCustomerResponseDto VerifyOTP(VerifyCustomerRequestDto model)
        {
            var response = new VerifyCustomerResponseDto();
            response.ErrorMessage = "Invalid OTP";
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetMemberForOTPValidation(), _conn);
            _command.Parameters.Add(new OdbcParameter("id", model.CustomerId));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Members);
            if (_dataset.Tables[EntityName.Members].Rows.Count == 0)
                return response;

            var Table = _dataset.Tables[EntityName.Members].Rows[0];
            if (Table["OTP"] != null || !string.IsNullOrEmpty(Table["OTP"].ToString()))
            {
                var OTPTime = Convert.ToDateTime(Table["OTP_time"].ToString());
                var OTP = Table["OTP"].ToString();
                if (OTP != model.OTP)
                    return response;
                var ValidityTime = Convert.ToInt16(ConfigurationManager.AppSettings["OTPValiditySeconds"].ToString());
                if (DateTime.Now.Subtract(OTPTime).Seconds > ValidityTime)
                {
                    response.ErrorMessage = "OTP is timed out. Please try resending.";
                    return response;
                }
                response.ValidCustomer = true;
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal GetInitialDataForFundTransferResponseDto GetInitialDataForFundTransfer(BeneficiaryRequestModel request)
        {
            var response = new GetInitialDataForFundTransferResponseDto();
            var memberId = Convert.ToInt64(request.MemberId);
            response.FromAccounts = GetMemberAccountHeads(memberId, request.Service, request.AppId);
            response.ToAccounts = GetBeneficiaryList(memberId, request.TransferType);
            return response;
        }
        internal string SubmitTransfer(TransferRequestDto model)
        {
            //need to create a transaction and email/sms
            string response = string.Empty;
            if (!ValidateTransferModel(response, model))
                return response;
            _command = new OdbcCommand("select sf_net_banking_amount_transfer('" + Convert.ToInt64(model.MemberId) + "','" + model.FromAccount.accountType + "','" + model.FromAccount.Id + "','" + model.ToAccount.Id + "','" + model.Amount + "','" + model.OTP + "','" + model.Service + "','" + model.AppId + "')ls_message", _conn);

            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read())
                {
                    response = Convert.ToString(reader["ls_message"]);
                }
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal SMSContentModel SubmitTransfer_new(TransferRequestDto model)
        {
            var response = new SMSContentModel();
            _command = new OdbcCommand("call sp_net_banking_amount_transfer(?,?,?,?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Add(new OdbcParameter("ai_customer_id", model.MemberId));
            _command.Parameters.Add(new OdbcParameter("ai_account_type_id", model.FromAccount.accountType));
            _command.Parameters.Add(new OdbcParameter("ai_account_id", model.FromAccount.Id));
            _command.Parameters.Add(new OdbcParameter("ai_beneficiary_id", model.ToAccount.Id));
            _command.Parameters.Add(new OdbcParameter("ad_amount", model.Amount));
            _command.Parameters.Add(new OdbcParameter("as_otp", model.OTP));
            _command.Parameters.Add(new OdbcParameter("as_service_type", model.Service));
            _command.Parameters.Add(new OdbcParameter("as_app_id", model.AppId));
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.status = reader["transfer_status"]?.ToString();
                response.CustomerMessage = reader["message_customer"]?.ToString();
                response.TraderMessage = reader["message_beneficiary"]?.ToString();
                response.CustomerMobile = reader["mobile_no_customer"]?.ToString();
                response.CustomerEmail = reader["email_customer"]?.ToString();
                response.TraderMobile = reader["mobile_no_beneficiary"]?.ToString();
                response.TraderEmail = reader["email_beneficiary"]?.ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal SMSContentModel SubmitTradeTran(TradeTransRequestDto model)
        {
            var response = new SMSContentModel();
            _command = new OdbcCommand("call sp_net_banking_amount_transfer_trade(?,?,?,?,?,?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Add(new OdbcParameter("ai_customer_id", model.MemberId));
            _command.Parameters.Add(new OdbcParameter("ai_account_type_id", model.FromAccount.accountType));
            _command.Parameters.Add(new OdbcParameter("ai_account_id", model.FromAccount.Id));
            _command.Parameters.Add(new OdbcParameter("ai_trader_id", model.TraderId));
            _command.Parameters.Add(new OdbcParameter("as_trade_code", model.TradeCode));
            _command.Parameters.Add(new OdbcParameter("ad_amount", model.Amount));
            _command.Parameters.Add(new OdbcParameter("as_otp", model.OTP));
            _command.Parameters.Add(new OdbcParameter("as_refno", model.RefNo));
            _command.Parameters.Add(new OdbcParameter("as_service_type", model.Service));
            _command.Parameters.Add(new OdbcParameter("as_app_id", model.AppId));
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.status = reader["transfer_status"]?.ToString();
                response.CustomerMessage = reader["message_customer"]?.ToString();
                response.TraderMessage = reader["message_trader"]?.ToString();
                response.CustomerMobile = reader["mobile_no_customer"]?.ToString();
                response.CustomerEmail = reader["email_customer"]?.ToString();
                response.TraderMobile = reader["mobile_no_trader"]?.ToString();
                response.TraderEmail = reader["email_trader"]?.ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal string DeleteBeneficiary(BeneficiaryModel model)
        {
            //throw new NotImplementedException();
            var response = new VerifyCustomerResponseDto();
            ConnectionHelper.Reset(_conn);
            var query = "update customer_beneficiary_status  set status = 'D' where beneficiary_id = '" + model.Id + "'";
            _command = new OdbcCommand(query, _conn);
            var result = _command.ExecuteNonQuery();
            ConnectionHelper.close_conn(_conn);
            return "Beneficiary Deletion Successfull";
        }
        internal string AddBeneficiary(BeneficiaryModel model)
        {
            try
            {
                var response = string.Empty;
                if (!ValidateBeneficiaryModel(response, model))
                    return response;
                _command = new OdbcCommand("select sf_add_customer_beneficiery('" + Convert.ToInt64(model.MemberId) + "','" + model.BeneficiaryName + "','" + model.AccountNumber + "','" + model.IFSC + "','" + model.MaxLimit + "','" + model.RequestMode + "','" + model.AddressLine1 + "','" + model.AddressLine2 + "','" + model.AddressLine3 + "','" + model.BeneficiaryType + "','" + model.Id + "','" + model.Service + "','" + model.OTP + "')ls_message", _conn);

                using (var reader = _command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        response = Convert.ToString(reader["ls_message"]);
                    }
                }
                ConnectionHelper.close_conn(_conn);
                return response;
            }
            catch (Exception ex)
            {
                ConnectionHelper.close_conn(_conn);
                return ex.ToString();
            }
        }
        private bool ValidateBeneficiaryModel(string response, BeneficiaryModel model)
        {
            if (string.IsNullOrEmpty(model.AccountNumber) || string.IsNullOrEmpty(model.BeneficiaryName) || model.MaxLimit == 0 || string.IsNullOrEmpty(model.OTP))
            {

                response = "Invalid details.";
                return false;
            }
            return true;
        }
        internal IFSCModel ValidateIFSC(IFSCModel model)
        {

            var response = new IFSCModel();
            _command = new OdbcCommand("call sp_validate_ifsc(?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Add(new OdbcParameter("as_ifsc", model.IFSC));
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.IFSC = reader["ifsc"]?.ToString();
                response.BankName = reader["bank_name"]?.ToString();
                response.BankBranch = reader["branch_name"]?.ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal TraderRequestModel ValidateTrader(TraderRequestModel model)
        {
            var response = new TraderRequestModel();
            _command = new OdbcCommand("call sp_get_trader_det(?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Add(new OdbcParameter("as_trade_code", model.TradeCode));
            _command.Parameters.Add(new OdbcParameter("as_service_type", model.Service));
            _command.Parameters.Add(new OdbcParameter("as_app_id", model.AppId));
            _command.Parameters.Add(new OdbcParameter("ai_member_id", model.MemberId));
            _command.Parameters.Add(new OdbcParameter("as_barcode_data", model.QRCodeData));
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.TraderId = reader["trader_id"]?.ToString();
                response.TradeCode = reader["trade_code"]?.ToString();
                response.TraderName = reader["trader_name"]?.ToString();
                response.TraderAddress = reader["trader_address"]?.ToString();
                response.TraderMobile = reader["trader_mobile_no"]?.ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        private bool ValidateTransferModel(string response, TransferRequestDto model)
        {
            if (model.Amount == 0 || model.FromAccount == null || model.ToAccount == null || model.OTP == null)
            {
                response = "Invalid Request. Please verify and try again.";
                return false;
            }
            return true;
        }
        internal bool SendOTP(string customerId, string customermemberid = null)
        {
            string EmailId;
            string MobileNr;
            if (customermemberid == null)
                GetMemberById(customerId);
            else
                GetMemberByMemberId(customermemberid);
            var abc = "abc";
            EmailId = _dataset.Tables[EntityName.Members].Rows[0]["email_no"] == null ? string.Empty : _dataset.Tables[EntityName.Members].Rows[0]["email_no"].ToString();
            MobileNr = _dataset.Tables[EntityName.Members].Rows[0]["mobile_no"] == null ? string.Empty : _dataset.Tables[EntityName.Members].Rows[0]["mobile_no"].ToString();

            if (_dataset.Tables[EntityName.Members].Rows.Count == 0
                || string.IsNullOrEmpty(EmailId)
                || string.IsNullOrEmpty(MobileNr))
            {
                return false;
            }
            var OTP = new OTPHelper().GenerateOTP(6);
            var success = new SmsHelper().SendSms(MobileNr, "Hi, Your OTP for " + ConfigurationManager.AppSettings["BankName"].ToString() + " is " + OTP);
            success &= SendEmail(EmailId, OTP);
            UpdateOTP(OTP, success.ToString(), customerId);
            return success;
        }
        internal bool SendMessage(string message, string mobile, string email)
        {

            var OTP = new OTPHelper().GenerateOTP(4);
            if (mobile != null && mobile != "")
            {
                var success = new SmsHelper().SendSms(mobile, message);
                if (email != null && email != "")
                {
                    success &= SendEmail(email, message);
                }
                return success;
            }
            else
                return false;
        }
        private void UpdateOTP(string otp, string status, string customerId)
        {
            var response = new VerifyCustomerResponseDto();
            ConnectionHelper.Reset(_conn);
            var query = "update members set OTP = '" + otp + "', OTP_time = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where member_user_id = '" + customerId + "'";
            _command = new OdbcCommand(query, _conn);
            var result = _command.ExecuteNonQuery();
            ConnectionHelper.close_conn(_conn);
        }
        //result(branch_id integer, branch_name varchar(50),address varchar(200),phone varchar(15),email varchar(50),manager varchar(50),mang_contact varchar(15))
        internal GetBranchListResponseDto GetBranchList()
        {
            var response = new GetBranchListResponseDto();
            if (response.BranchDetailsList.Any())
                return response;
            _command = new OdbcCommand("call sp_get_branch_contacts()", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                var BranchDetails = new BranchDetailsDto();
                BranchDetails.BranchName = reader["branch_name"]?.ToString();
                BranchDetails.Address = reader["address"]?.ToString().Split(',').ToList();
                BranchDetails.PhoneNumber = reader["phone"].ToString();
                BranchDetails.Email = reader["email"].ToString();
                BranchDetails.Manager = reader["manager"].ToString();
                BranchDetails.ManagerContact = reader["mang_contact"].ToString();
                BranchDetails.Latitude = (decimal)reader["Latitude_"];
                BranchDetails.Longitude = (decimal)reader["Longitude_"];
                response.BranchDetailsList.Add(BranchDetails);
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        //result(name varchar(50),designation varchar(200),phone varchar(15))
        internal GetManagementDetailsResponseDto GetManagementDetails()
        {
            var response = new GetManagementDetailsResponseDto();
            _command = new OdbcCommand("call sp_get_management_contacts()", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                var BranchDetails = new ManagementDetailsDto();
                BranchDetails.Name = reader["name"]?.ToString();
                BranchDetails.Designation = reader["designation"]?.ToString();
                BranchDetails.PhoneNumber = reader["phone"].ToString();
                response.ManagementDetailsList.Add(BranchDetails);
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal NotificationsDto GetNotifications(long memberid)
        {
            var response = new NotificationsDto();
            // ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_member_notifications(?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_member_id", memberid);
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.Notifications.Add(reader["messages"].ToString());
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal VerifyCustomerResponseDto UpdatePassword(string newpassword, string customerId, string deviceId, string oldpass, string type)
        {
            var response = new VerifyCustomerResponseDto();
            int result = 0;
            ConnectionHelper.Reset(_conn);
            if (type == "PIN")
            {
                _command = new OdbcCommand("select sf_update_password_pin('" + customerId + "','" + deviceId + "','" + newpassword + "','" + oldpass + "','" + type + "') result", _conn);
            }
            else if (type == "PAS")
            {
                var Hpassword = BCrypt.Net.BCrypt.HashPassword(newpassword, BCrypt.Net.BCrypt.GenerateSalt());
                _command = new OdbcCommand("select sf_update_password_pin('" + customerId + "','" + deviceId + "','" + Hpassword + "','" + oldpass + "','" + type + "') result", _conn);
            }
            //var query = "update members set member_password = '" + Hpassword + "', device_id = '" + deviceId + "' where member_user_id = '" + customerId + "'";
            //_command = new OdbcCommand(query, _conn);
            //var result = _command.ExecuteNonQuery();
            //response.ValidCustomer = result == 1;           

            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = int.Parse(Convert.ToString(reader["result"]));
                }
            }
            response.ValidCustomer = result == 1;
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        internal VerifyCustomerResponseDto VerifyCustomer(string customerId)
        {
            var response = new VerifyCustomerResponseDto();
            string EmailId, MobileNr;
            GetMemberById(customerId);
            if (_dataset.Tables[EntityName.Members].Rows.Count == 0)
            {
                response.ErrorMessage = ErrorCodes.InvalidCustomerId;
                return response;
            }
            EmailId = _dataset.Tables[EntityName.Members].Rows[0]["email_no"] == null ? string.Empty : _dataset.Tables[EntityName.Members].Rows[0]["email_no"].ToString();
            if (string.IsNullOrEmpty(EmailId))
            {
                response.ErrorMessage = ErrorCodes.InvalidEmailId;
                return response;
            }
            MobileNr = _dataset.Tables[EntityName.Members].Rows[0]["mobile_no"] == null ? string.Empty : _dataset.Tables[EntityName.Members].Rows[0]["mobile_no"].ToString();
            if (string.IsNullOrEmpty(MobileNr))
            {
                response.ErrorMessage = ErrorCodes.InvalidMobileNr;
                return response;
            }
            var OTP = new OTPHelper().GenerateOTP(4);
            response.ValidCustomer = SendEmail(EmailId, OTP);
            response.ValidCustomer |= new SmsHelper().SendSms(MobileNr, "Hi, Your OTP for " + ConfigurationManager.AppSettings["BankName"].ToString() + " is " + OTP);
            if (response.ValidCustomer)
            {
                _command = new OdbcCommand(QueryBuilder.UpdateOTP(), _conn);
                _command.Parameters.Add(new OdbcParameter("otp", OTP));
                _command.Parameters.Add(new OdbcParameter("otptime", DateTime.Now));
                _command.Parameters.Add(new OdbcParameter("customerId", customerId));
                _command.ExecuteNonQuery();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        private void GetMemberById(string customerId)
        {
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetMemberById(), _conn);
            _command.Parameters.Add(new OdbcParameter("customerId", customerId));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Members);
        }
        private void GetMemberByMemberId(string customermemberid)
        {
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetMemberByMemberId(), _conn);
            _command.Parameters.Add(new OdbcParameter("customermemberid", customermemberid));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Members);
        }
        private static bool SendEmail(string EmailId, string OTP)
        {
            var Subject = "Verification email";
            var Body = new StringBuilder();
            Body.Append("<div>Hi,</div>");
            Body.AppendFormat("<div>This mail is to notify that {0}</div>", OTP);
            Body.AppendLine("<div><b>If you are unaware of this change, please contact the bank immediately</b></div>");
            Body.AppendLine("<div>Thanks</div>");
            Body.AppendLine("<div>Test Co-Op Bank</div>");
            return new EmailHepler().Send(EmailId, Subject, Body.ToString());
        }
        internal bool ValidateRequest(RequestBase request)
        {
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.ValidateSessionToken(), _conn);
            _command.Parameters.Add(new OdbcParameter("username", request.UserId));
            _command.Parameters.Add(new OdbcParameter("token", request.SessionToken));
            _command.Parameters.Add(new OdbcParameter("deviceid", request.DeviceId));
            int count = Convert.ToInt32(_command.ExecuteScalar());
            ConnectionHelper.close_conn(_conn);
            return count > 0;
        }
        public ChequeBookResponseDto GetInitialeckDataForChequeRequest(long memberid, string service)
        {
            var response = new ChequeBookResponseDto();
            //List<AccountHeadModel> ModeOfAccount = GetAccountHeads(memberid);
            List<AccountHeadModel> ModeOfAccount = GetMemberAccountHeads(memberid, service);
            response.ModeOfAccount = ModeOfAccount;
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        private void CreateSessionToken(string username, ValidateMemberResposeDto response)
        {
            response.SessionToken = new RequestBase();
            response.SessionToken.SessionToken = new Guid().ToString();
            response.SessionToken.UserId = username;
            _command = new OdbcCommand(QueryBuilder.UpdateMembersWithAttemptsAndToken(), _conn);
            _command.Parameters.Add(new OdbcParameter("token", response.SessionToken.SessionToken));
            _command.Parameters.Add(new OdbcParameter("username", username));
            _command.ExecuteNonQuery();
            ConnectionHelper.close_conn(_conn);
        }
        public bool RequestNewChequeBook(long depositId)
        {
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.InsertNewChequeRequest(), _conn);
            _command.Parameters.Add(new OdbcParameter("depositid", depositId));
            var result = _command.ExecuteNonQuery();
            ConnectionHelper.close_conn(_conn);
            return result == 1;


        }
        private int UpdateLoginAttempt(string username, int LoginAttempts)
        {
            _command = new OdbcCommand(QueryBuilder.LockCustomer(), _conn);
            _command.Parameters.Add(new OdbcParameter("attemptCount", LoginAttempts++));
            _command.Parameters.Add(new OdbcParameter("username", username));
            _command.ExecuteNonQuery();
            ConnectionHelper.close_conn(_conn);
            return LoginAttempts;
        }
        public StatementMemberModel GetStatmentMemberDetails(long membeid, long accountNr, string dateTo)
        {
            StatementMemberModel response = new StatementMemberModel();
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetMemberDetailsForStatment(), _conn);
            _command.Parameters.Add(new OdbcParameter("memberid", membeid));
            _command.Parameters.Add(new OdbcParameter("accountNr", accountNr));
            _command.Parameters.Add(new OdbcParameter("dateTo", dateTo));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Members);
            if (_dataset.Tables[EntityName.Members].Rows.Count > 0)
            {
                var resultantRow = _dataset.Tables[EntityName.Members].Rows[0];
                response.AccountNr = resultantRow["deposit_no"].ToString();
                response.AccountHead = resultantRow["account_id"].ToString();
                response.Name = resultantRow["name"].ToString();
                response.Address = resultantRow["address"].ToString();
                response.PresentAddress = resultantRow["present_address"].ToString();
                response.Operator = resultantRow["operated_by"].ToString();
                response.Category = resultantRow["account_name"].ToString();
                response.AccountMode = resultantRow["operation_mode"].ToString();
                response.AccountDate = resultantRow["deposit_date"].ToString().Substring(0, 10);
                response.BranchName = resultantRow["branch_name"].ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        public string GetAccountBalance(long accounttype, long accountid)
        {
            string Balance = null;
            _command = new OdbcCommand("select sf_get_account_balance_web('" + accounttype + "','" + accountid + "') balance", _conn);
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Balance = Convert.ToString(reader["balance"]);
                }
            }
            ConnectionHelper.close_conn(_conn);
            return Balance;
        }
        public List<StatementModel> GetStatementTransactionDetails(long memberid, long depositid, long accounttypeid, string datefrom, string dateto, out decimal openingbalance)
        {
            openingbalance = 0;
            decimal previousBalance = 0;
            var response = new List<StatementModel>();
            string errorMessage = "Error while generating statement. Please contact your home branch";
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_get_accountstatement_web(?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_personalledger_id", depositid);
            _command.Parameters.AddWithValue("@ai_account_id", accounttypeid);
            _command.Parameters.AddWithValue("@ad_from_date", datefrom);
            _command.Parameters.AddWithValue("@ad_to_date", dateto);
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                StatementModel transactionModel = new StatementModel();
                DateTime dt = (DateTime)reader["scroll_date"]; //DateTime.ParseExact(reader["scroll_date"].ToString(), "dd/MM/yyyy HH:mm:ss", null).ToString().Substring(0, 10);
                transactionModel.ScrollDate = dt.ToString("dd/MM/yyyy");
                transactionModel.ScrollNr = reader["scroll_id"].ToString();
                transactionModel.Particulars = reader["particulars"].ToString();
                transactionModel.Deposit = (decimal)reader["amount_in"];
                transactionModel.Withdrawal = (decimal)reader["amount_out"];
                transactionModel.Balance = (decimal)reader["balance"];
                var cashortransfer = reader["account_id"].ToString();
                transactionModel.CashOrTransfer = cashortransfer.Equals("100") ? "C" : "T";
                response.Add(transactionModel);
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        public List<StatementModel> GetStatementTransactionDetailsTrade(long memberid, long depositid, long accounttypeid, string datefrom, string dateto, out decimal openingbalance)
        {
            openingbalance = 0;
            decimal previousBalance = 0;
            var response = new List<StatementModel>();
            string errorMessage = "Error while generating statement. Please contact your home branch";
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_get_accountstatement_web_trade(?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_member_id", memberid);
            _command.Parameters.AddWithValue("@ai_personalledger_id", depositid);
            _command.Parameters.AddWithValue("@ai_account_id", accounttypeid);
            _command.Parameters.AddWithValue("@ad_from_date", datefrom);
            _command.Parameters.AddWithValue("@ad_to_date", dateto);
            var reader = _command.ExecuteReader();

            while (reader.Read())
            {
                StatementModel transactionModel = new StatementModel();

                DateTime dt = (DateTime)reader["scroll_date"]; //DateTime.ParseExact(reader["scroll_date"].ToString(), "dd/MM/yyyy HH:mm:ss", null).ToString().Substring(0, 10);
                transactionModel.ScrollDate = dt.ToString("dd/MM/yyyy");
                transactionModel.ScrollNr = reader["scroll_no"].ToString();
                transactionModel.Particulars = reader["particulars"].ToString();
                transactionModel.Deposit = (decimal)reader["amount"];
                transactionModel.DrCr = reader["dr_cr"].ToString();
                transactionModel.AccountNo = reader["account_no"].ToString();
                response.Add(transactionModel);
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }
        public List<AccountSummaryModel> GetAccountSummary(long memeberid)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            AccountSummaryModel response = new AccountSummaryModel();
            // ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_ac_summary_members_web(?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_member_id", memeberid);
            _command.Parameters.AddWithValue("@ai_member_id", date);
            var reader = _command.ExecuteReader();
            List<AccountSummaryModel> AcountSummaryList = new List<AccountSummaryModel>();
            while (reader.Read())
            {
                long depId = 0;
                AccountSummaryModel summary = new AccountSummaryModel();
                summary.AcccountName = reader["ac_code"].ToString();
                summary.AccountNr = reader["account_no"].ToString();
                summary.DepositId = long.TryParse(reader["ledger_id"].ToString(), out depId) ? depId : 0;
                summary.AccountStartedOn = reader["started_on"].ToString().Substring(0, 10);
                summary.CurrentBalance = reader["current_balance"].ToString();
                summary.BranchCode = reader["br_code"].ToString();
                AcountSummaryList.Add(summary);

            }
            ConnectionHelper.close_conn(_conn);
            return AcountSummaryList;
        }
        public MemberModel GetMemberDetailsById(long id)
        {
            MemberModel Member = new MemberModel();
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetMemberDetailsById(), _conn);
            _command.Parameters.Add(new OdbcParameter("id", id));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Members);
            if (_dataset.Tables[EntityName.Members].Rows.Count > 0)
            {
                Member.Name = _dataset.Tables[EntityName.Members].Rows[0]["name"].ToString();
                Member.Address = _dataset.Tables[EntityName.Members].Rows[0]["present_address"].ToString();
                Member.PhoneNumber = _dataset.Tables[EntityName.Members].Rows[0]["phone"].ToString();
                Member.Email = _dataset.Tables[EntityName.Members].Rows[0]["email_no"] == null ? "No email found!" : _dataset.Tables[EntityName.Members].Rows[0]["email_no"].ToString();
                Member.Address = string.IsNullOrEmpty(Member.Address) ? _dataset.Tables[EntityName.Members].Rows[0]["address"].ToString() : Member.Address;
                Member.AddressLine = Member.Address.Split(',');
            }
            ConnectionHelper.close_conn(_conn);
            return Member;
        }
        private List<AccountHeadModel> GetAccountHeads(long memberid)
        {
            var ModeOfAccount = new List<AccountHeadModel>();
            ConnectionHelper.Reset(_conn);
            _dataset.Clear();
            _command = new OdbcCommand(QueryBuilder.AllDepositsOfaMember(), _conn);
            _command.Parameters.Add(new OdbcParameter("id", memberid));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Deposits);
            if (_dataset.Tables[EntityName.Deposits].Rows.Count > 0)
            {
                string depositType;
                long depositid;
                foreach (DataRow row in _dataset.Tables[EntityName.Deposits].Rows)
                {
                    depositType = row["account_name"].ToString() + " - " + row["deposit_no"].ToString();
                    depositid = (Convert.ToInt64(row["deposit_id"].ToString()));
                    ModeOfAccount.Add(new AccountHeadModel() { Caption = depositType, Id = depositid });
                }
            }
            ConnectionHelper.close_conn(_conn);
            return ModeOfAccount;
        }
        public List<AccountHeadModel> GetMemberAccountHeads(long memberid, string service, string appid = null)
        {
            //long AcId = 0;
            var ModeOfAccount = new List<AccountHeadModel>();
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_get_member_accountheads(?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_member_id", memberid);
            _command.Parameters.AddWithValue("@as_service_type", service);
            _command.Parameters.AddWithValue("@as_app_id", appid);
            var reader = _command.ExecuteReader();
            List<AccountHeadModel> AccountHeadList = new List<AccountHeadModel>();
            while (reader.Read())
            {
                AccountHeadModel acheads = new AccountHeadModel();
                acheads.Caption = reader["accountname"].ToString() + " - " + reader["accountno"].ToString();
                acheads.Id = int.Parse(reader["accountid"].ToString());
                acheads.accountType = int.Parse(reader["accounttypeid"].ToString());
                AccountHeadList.Add(acheads);
            }
            ConnectionHelper.close_conn(_conn);
            return AccountHeadList;
        }
        public IList<BeneficiaryModel> GetBeneficiaryList(long memberid, string type)
        {
            var BeneficiaryList = new List<BeneficiaryModel>();
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_customer_beneficiary_lists(?,?)", _conn);
            _command.Parameters.Add(new OdbcParameter("ai_member_id", memberid));
            _command.Parameters.Add(new OdbcParameter("as_beneficiary_type", type));
            _command.CommandType = CommandType.StoredProcedure;
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                var Beneficiary = new BeneficiaryModel();
                Beneficiary.Id = Convert.ToInt16(reader["beneficiary_id"]);
                Beneficiary.BeneficiaryName = reader["beneficiary_name"]?.ToString();
                Beneficiary.AccountNumber = reader["account_no"].ToString();
                Beneficiary.BankName = reader["bank_name"].ToString();
                Beneficiary.BankBranch = reader["branch"].ToString();
                Beneficiary.IFSC = reader["ifsc"].ToString();
                Beneficiary.BeneficiaryType = reader["beneficiary_type"].ToString();
                Beneficiary.IsEditable = bool.Parse(reader["IsEditable"].ToString());
                Beneficiary.AddressLine1 = reader["address1"].ToString();
                Beneficiary.AddressLine2 = reader["address2"].ToString();
                Beneficiary.AddressLine3 = reader["address3"].ToString();
                Beneficiary.MaxLimit = int.Parse(reader["transactionlimit"].ToString());
                Beneficiary.AccountType = reader["account_type"].ToString();
                //max limit is needed
                BeneficiaryList.Add(Beneficiary);
            }
            ConnectionHelper.close_conn(_conn);
            return BeneficiaryList;
        }

        #endregion


        #region Business Correpondent Region
        public ValidateMemberResposeDto ValidateAgentLogin(string username, string password, string deviceid)
        {

            int LoginAttempts = 0;
            var response = new ValidateMemberResposeDto();
            response.AuthenticationSuccess = true;
            _command = new OdbcCommand(QueryBuilder.GetAgentForAuthenitcation(), _conn);
            _command.Parameters.Add(new OdbcParameter("username", username));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Agents);
            if (_dataset.Tables[EntityName.Agents].Rows.Count == 0)
            {
                response.AuthenticationSuccess = false;
                response.ErrorMessage = ErrorCodes.InvalidCredentials;
                return response;
            }
            //implement BCrypt here
            var AgentTable = _dataset.Tables[EntityName.Agents].Rows[0];
            LoginAttempts = int.TryParse(AgentTable["login_attempts"].ToString(), out LoginAttempts) ? LoginAttempts : 0;
            var DeviceId = AgentTable["device_id"].ToString();
            var Password = AgentTable["agent_password"].ToString();
            var Name = AgentTable["agent_name"].ToString();
            var AgentId = AgentTable["agent_id"].ToString();
            var AppId = AgentTable["appid"].ToString();
            if (!BCrypt.Net.BCrypt.CheckPassword(password, Password))
            {
                response.AuthenticationSuccess = false;
                response.ErrorMessage = ErrorCodes.InvalidCredentials;
                UpdateLoginAttempt(username, LoginAttempts);
            }
            if (!string.Equals(deviceid, DeviceId))
            {
                response.ErrorMessage = ErrorCodes.InvalidMobileNumber;
                response.AuthenticationSuccess = false;
            }
            if (LoginAttempts > 3)
            {
                response.AuthenticationSuccess = false;
                response.ErrorMessage = ErrorCodes.LoginAttemptsExceeded;
                UpdateLoginAttempt(username, LoginAttempts);
            }
            if (!response.AuthenticationSuccess)
                return response;
            CreateSessionToken(username, response);
            response.SessionToken.Username = new string(Name.Take(12).ToArray());
            response.SessionToken.AgentId = AgentId;
            response.SessionToken.AppId = AppId;
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        internal VerifyCustomerResponseDto VerifyAgent(string agentCode)
        {
            var response = new VerifyCustomerResponseDto();
            string EmailId, MobileNr;
            GetAgentByCode(agentCode);
            if (_dataset.Tables[EntityName.Agents].Rows.Count == 0)
            {
                response.ErrorMessage = ErrorCodes.InvalidAgentCode;
                return response;
            }
            EmailId = _dataset.Tables[EntityName.Agents].Rows[0]["email_no"] == null ? string.Empty : _dataset.Tables[EntityName.Agents].Rows[0]["email_no"].ToString();
            if (string.IsNullOrEmpty(EmailId))
            {
                response.ErrorMessage = ErrorCodes.InvalidEmailId;
                return response;
            }
            MobileNr = _dataset.Tables[EntityName.Agents].Rows[0]["mobile_no"] == null ? string.Empty : _dataset.Tables[EntityName.Agents].Rows[0]["mobile_no"].ToString();
            if (string.IsNullOrEmpty(MobileNr))
            {
                response.ErrorMessage = ErrorCodes.InvalidMobileNr;
                return response;
            }
            var OTP = new OTPHelper().GenerateOTP(4);
            response.ValidCustomer = SendEmail(EmailId, OTP);
            response.ValidCustomer |= new SmsHelper().SendSms(MobileNr, "Hi, Your OTP for " + ConfigurationManager.AppSettings["BankName"].ToString() + " is " + OTP);
            if (response.ValidCustomer)
            {
                _command = new OdbcCommand(QueryBuilder.UpdateAgentOTP(), _conn);
                _command.Parameters.Add(new OdbcParameter("otp", OTP));
                _command.Parameters.Add(new OdbcParameter("otptime", DateTime.Now));
                _command.Parameters.Add(new OdbcParameter("agentCode", agentCode));
                _command.ExecuteNonQuery();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        private void GetAgentByCode(string agentCode)
        {
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetAgentByCode(), _conn);
            _command.Parameters.Add(new OdbcParameter("agentCode", agentCode));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Agents);
        }

        internal VerifyCustomerResponseDto VerifyAgentOTP(VerifyCustomerRequestDto model)
        {
            var response = new VerifyCustomerResponseDto();
            response.ErrorMessage = "Invalid OTP";
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand(QueryBuilder.GetAgentForOTPValidation(), _conn);
            _command.Parameters.Add(new OdbcParameter("id", model.AgentCode));
            _adapter = new OdbcDataAdapter(_command);
            _adapter.Fill(_dataset, EntityName.Agents);
            if (_dataset.Tables[EntityName.Agents].Rows.Count == 0)
                return response;

            var Table = _dataset.Tables[EntityName.Agents].Rows[0];
            if (Table["OTP"] != null || !string.IsNullOrEmpty(Table["OTP"].ToString()))
            {
                var OTPTime = Convert.ToDateTime(Table["OTP_time"].ToString());
                var OTP = Table["OTP"].ToString();
                if (OTP != model.OTP)
                    return response;
                var ValidityTime = Convert.ToInt16(ConfigurationManager.AppSettings["OTPValiditySeconds"].ToString());
                if (DateTime.Now.Subtract(OTPTime).Seconds > ValidityTime)
                {
                    response.ErrorMessage = "OTP is timed out. Please try resending.";
                    return response;
                }
                response.ValidCustomer = true;
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        internal VerifyCustomerResponseDto UpdateAgentPassword(VerifyCustomerRequestDto model)
        {
            var response = new VerifyCustomerResponseDto();
            int result = 0;
            ConnectionHelper.Reset(_conn);
            if (model.Type == "PIN")
            {
                _command = new OdbcCommand("select sf_update_agent_password('" + model.AgentCode + "','" + model.DeviceId + "','" + model.Password + "','" + model.OldPassword + "','" + model.Type + "') result", _conn);
            }
            else if (model.Type == "PAS")
            {
                var Hpassword = BCrypt.Net.BCrypt.HashPassword(model.Password, BCrypt.Net.BCrypt.GenerateSalt());
                _command = new OdbcCommand("select sf_update_agent_password('" + model.AgentCode + "','" + model.DeviceId + "','" + Hpassword + "','" + model.OldPassword + "','" + model.Type + "') result", _conn);
            }
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = int.Parse(Convert.ToString(reader["result"]));
                }
            }
            response.ValidCustomer = result == 1;
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        public AcHeadListDto GetAgentAccountHeads(RequestBase request)
        {
            AcHeadListDto acheadList = new AcHeadListDto();
            acheadList.HeadOfAccounts = new List<AcHeadDto>();
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_get_agent_accountheads(?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_agent_id", request.AgentId);
            _command.Parameters.AddWithValue("@as_service_type", request.Service);
            _command.Parameters.AddWithValue("@as_app_id", request.AppId);
            var reader = _command.ExecuteReader();
            List<AcHeadDto> AcHeadList = new List<AcHeadDto>();
            while (reader.Read())
            {
                //test
                AcHeadDto acheads = new AcHeadDto();
                acheads.AcHeadName = reader["account_name"].ToString();
                acheads.AcHeadId = int.Parse(reader["account_id"].ToString());
                acheads.AcHeadCode = reader["account_short_code"].ToString();
                acheadList.HeadOfAccounts.Add(acheads);
            }
            ConnectionHelper.close_conn(_conn);

            return acheadList;
        }

        internal CustomerDetDto ValidateAcNo(AccountNoValidateRequestDto request)
        {
            var response = new CustomerDetDto();
            _command = new OdbcCommand("call sp_get_account_det(?,?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Add(new OdbcParameter("@as_ac_no", request.AccountNo));
            _command.Parameters.Add(new OdbcParameter("@ai_ac_type", request.AcHeadId));
            _command.Parameters.Add(new OdbcParameter("@as_app_id", request.AppId));
            _command.Parameters.Add(new OdbcParameter("@ai_agent_id", request.AgentId));
            _command.Parameters.Add(new OdbcParameter("@as_read_method", request.AgentId));
            _command.Parameters.Add(new OdbcParameter("@as_qr_data", request.AgentId));
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.LedgerId = int.Parse(reader["ledger_id"]?.ToString());
                response.CustomerMemberId = int.Parse(reader["customer_member_id"]?.ToString());
                response.CustomerName = reader["customer_name"]?.ToString();
                response.CustomerAddress = reader["customer_address"]?.ToString();
                response.CustomerMobile = reader["customer_mobile_no"]?.ToString();
                response.CustomerEmail = reader["customer_email_id"]?.ToString();
                response.Remarks = reader["remarks"]?.ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        internal SMSContentModel SubmitCollection(CollectionSubmitRequestDto request)
        {
            var response = new SMSContentModel();
            _command = new OdbcCommand("call sp_net_banking_Collection_transfer(?,?,?,?,?,?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Add(new OdbcParameter("ai_agent_id", request.AgentId));
            _command.Parameters.Add(new OdbcParameter("ai_ledger_id", request.AccountId));
            _command.Parameters.Add(new OdbcParameter("ai_ac_type_id", request.AcHeadId));
            _command.Parameters.Add(new OdbcParameter("as_otp_pin", request.OTP));
            _command.Parameters.Add(new OdbcParameter("ad_amount", request.Amount));
            _command.Parameters.Add(new OdbcParameter("as_refno", request.RefNo));
            _command.Parameters.Add(new OdbcParameter("ai_app_id", request.AppId));
            _command.Parameters.Add(new OdbcParameter("as_device_id", request.DeviceId));
            _command.Parameters.Add(new OdbcParameter("as_session_tocken", request.SessionToken));
            _command.Parameters.Add(new OdbcParameter("as_service_type", request.Service));
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                response.status = reader["transfer_status"]?.ToString();
                response.CustomerMessage = reader["message_customer"]?.ToString();
                response.TraderMessage = reader["message_trader"]?.ToString();
                response.CustomerMobile = reader["mobile_no_customer"]?.ToString();
                response.CustomerEmail = reader["email_customer"]?.ToString();
                response.TraderMobile = reader["mobile_no_trader"]?.ToString();
                response.TraderEmail = reader["email_trader"]?.ToString();
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        internal List<CustomerDetDto> GetCollectionAcList(RequestBase request)
        {
            CustomerDetDto response = new CustomerDetDto();
            _command = new OdbcCommand("call sp_collection_ac_list(?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_agent_id", request.AgentId);
            _command.Parameters.AddWithValue("@as_agent_code", request.UserId);
            _command.Parameters.AddWithValue("@as_app_id", request.AppId);
            _command.Parameters.AddWithValue("@as_device_id", request.DeviceId);
            var reader = _command.ExecuteReader();
            List<CustomerDetDto> collectionAccountList = new List<CustomerDetDto>();
            while (reader.Read())
            {
                CustomerDetDto customerAcData = new CustomerDetDto();
                customerAcData.LedgerId = int.Parse(reader["ledger_id"].ToString());
                customerAcData.AccountNo = reader["account_no"].ToString();
                customerAcData.AcHeadId = int.Parse(reader["account_type_id"].ToString());
                customerAcData.AcHeadName = reader["account_name"].ToString();
                customerAcData.CustomerAddress = reader["customer_address"].ToString();
                customerAcData.CustomerName = reader["customer_name"].ToString();
                customerAcData.CustomerMemberId = int.Parse(reader["customer_member_id"].ToString());
                collectionAccountList.Add(customerAcData);

            }
            ConnectionHelper.close_conn(_conn);
            return collectionAccountList;
        }

        public List<CollectionStmtResponseDto> GetCollectionStatement(CollectionStmtRequestDto request)
        {
            var response = new List<CollectionStmtResponseDto>();
            string errorMessage = "Error while generating statement. Please contact your home branch";
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_get_collectionstmt_web(?,?,?,?,?)", _conn);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.AddWithValue("@ai_agent_id", request.AgentId);
            _command.Parameters.AddWithValue("@ai_account_type_id", request.AcHeadId);
            _command.Parameters.AddWithValue("@ad_from_date", request.FromDate);
            _command.Parameters.AddWithValue("@ad_to_date", request.ToDate);
            _command.Parameters.AddWithValue("@as_account_no", request.AccountNo);
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                CollectionStmtResponseDto colStmt = new CollectionStmtResponseDto();
                colStmt.AccountNo = reader["account_no"].ToString();
                colStmt.AcHeadName = reader["ac_head_name"].ToString();
                colStmt.CustomerName = reader["customer_name"].ToString();
                colStmt.Amount = decimal.Parse(reader["amount"].ToString());
                colStmt.Date = DateTime.Parse(reader["collection_date"].ToString()).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                response.Add(colStmt);
            }
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        public AgentProfile GetAgentAccountDetails(RequestBase request)
        {
             OdbcCommand _command2;
            MemberModel agent = new MemberModel();
            List<AgentAcSummaryDto> acSummaryList = new List<AgentAcSummaryDto>();
            AgentProfile response = new AgentProfile();
            ConnectionHelper.Reset(_conn);
            _command = new OdbcCommand("call sp_get_agent_personal_data(?,?,?)", _conn);
            _command.Parameters.AddWithValue("@ai_agent_id", request.AgentId);
            _command.Parameters.AddWithValue("@as_service_type", request.Service);
            _command.Parameters.AddWithValue("@as_app_id", request.AppId);
            var reader = _command.ExecuteReader();
            while (reader.Read())
            {
                agent.Name = reader["name"].ToString();
                agent.Address = reader["address"].ToString();
                agent.MembershipNo = reader["member_no"].ToString();
                agent.PhoneNumber = reader["mobile_no"].ToString();
            }
            response.AgentPersonalaData = agent;

            _command2 = new OdbcCommand("call sp_get_agent_account_summery(?,?,?)", _conn);
            _command2.CommandType = CommandType.StoredProcedure;
            _command2.Parameters.AddWithValue("@ai_agentid", request.AgentId);
            _command2.Parameters.AddWithValue("@as_service_type", request.Service);
            _command2.Parameters.AddWithValue("@as_app_id", request.AppId);
            var reader2 = _command2.ExecuteReader();
            while (reader2.Read())
            {
                AgentAcSummaryDto acSummary = new AgentAcSummaryDto();
                acSummary.AccountNo = reader2["account_no"].ToString();
                acSummary.AcHeadName = reader2["ac_head_name"].ToString();
                acSummary.Balance = decimal.Parse(reader2["current_balance"].ToString());
                acSummaryList.Add(acSummary);
            }
            response.AgentAccountSummary = acSummaryList;
            ConnectionHelper.close_conn(_conn);
            return response;
        }

        #endregion
    }

}