namespace UrbanBank.Helpers
{
    internal static class QueryBuilder
    {
        #region Easy Trans Region  
        public static string GetMemberForAuthenitcation()
        {
            return "select distinct  name,login_attempts,device_id,member_password,member_id,(select product_version from cc_about where application_id=99) appid from members where member_user_id = :username";
        }

        public static string ValidateSessionToken()
        {
            return "select count(*) from members where member_user_id = :username and session_token = :token and device_id = :deviceid";
        }

        public static string UpdateMembersWithAttemptsAndToken()
        {
            return "UPDATE members SET login_attempts =0,session_token=:token WHERE  member_user_id= :username";
        }

        public static string AllDepositsOfaMember()
        {
            return "select ac.account_name,d.deposit_id,d.deposit_no from deposits d, ac_heads ac where d.member_id = :id and ac.account_id = d.deposit_type_id";
        }
        public static string GetMemberDetailsById()
        {
            return "select distinct * from members where member_id = :id";
        }

        public static string InsertNewChequeRequest()
        {
            return "insert into ChequeBookRequest  (deposit_id,requested_on,issued_on) values ( :depositid ,GETDATE(),null)";
        }

        public static string GetMemberDetailsForStatment()
        {
            return "select dep.deposit_no,acc.account_id,mem.name,mem.address,mem.present_address,(SELECT CC.sf_get_deposit_operators(:depositId,':dateTo')) operated_by,(select branch_name from company_levels where level_id in(select company_id from deposits where deposit_id = :depositId )) as branch_name,acc.account_name,depmode.operation_mode,dep.deposit_date from deposits dep,ac_heads acc,members mem,deposit_modes depmode where mem.member_id = :memberid  and mem.member_id = dep.member_id and dep.deposit_type_id = acc.account_id and dep.mode_of_operation = depmode.mode_id and dep.deposit_id = :depositId";
        }

        public static string GetStatementTransactionDetails()
        {
            return "select scroll_id,scroll_date,particulars,amount_in,amount_out,account_id,(select entered_at from scroll_book where scroll_id = deposit_inout.scroll_id) entered_at from deposit_inout  where deposit_id = :depositid  and scroll_date > :datefrom and scroll_date < :dateto  order by scroll_date,entered_at";
        }

        public static string GetOpeningBalance()
        {
            return "select sum(amount_in-amount_out) as openingbalance from deposit_inout where deposit_id = :depositid  and scroll_date < :datefrom";
        }

        public static string LockCustomer()
        {
            return "update members set login_attempts = :attemptCount where member_user_id = :username ";
        }
        public static string GetMemberById()
        {
            return "select email_no,member_user_id,mobile_no from members where member_user_id = :customerId";
        }
        public static string GetMemberByMemberId()
        {
            return "select email_no,member_user_id,mobile_no from members where member_id = :customermemberid";
        }
        public static string UpdateOTP()
        {
            return "update members set OTP = :otp, OTP_time = :otptime where member_user_id=:customerId";
        }
        public static string GetMemberForOTPValidation()
        {
            return "select OTP_time,OTP from members where member_user_id= :id";
        }

        public static string UpdatePassword()
        {
            return "update members set member_password= :password,device_id= :deviceid where member_user_id= :id";
        }
        #endregion

        #region Business Correpondent
        public static string GetAgentForAuthenitcation()
        {
            return "select distinct  agent_name,login_attempts,device_id,agent_password,agent_id,(select product_version from cc_about where application_id=99) appid from agents where agent_code = :username";
        }
        public static string UpdateAgentWithAttemptsAndToken()
        {
            return "UPDATE agents SET login_attempts =0,session_token=:token WHERE  agent_code= :username";
        }

        public static string GetAgentByCode()
        {
            return "select email_no,agent_code,mobile_no from agents where agent_code = :agentCode";
        }

        public static string UpdateAgentOTP()
        {
            return "update agents set OTP = :otp, OTP_time = :otptime where agent_code=:agentCode";
        }

        public static string GetAgentForOTPValidation()
        {
            return "select OTP_time,OTP from agents where agent_code= :id";
        }

      
        #endregion
    }
}