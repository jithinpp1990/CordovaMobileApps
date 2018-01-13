starter.config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
    .state("login", {
        url: "/login",
        templateUrl: "templates/loginView.html",
        controller: "loginCtrl"
    })
    .state("prelogin", {
        url: "/prelogin",
        templateUrl: "templates/PreLogin.html",
        controller: "preloginCtrl"
    })
    .state("dashboard", {
        url: "/dashboard",
        templateUrl: "templates/dashboardView.html",
        controller: "dashboardCtrl"
    })
    .state("collection", {
        url: "/collection",
        templateUrl: "templates/collectionView.html",
        controller: "collectionCtrl"
    })
     .state("payment", {
         url: "/payment",
         templateUrl: "templates/paymentView.html",
         controller: "paymentCtrl"
     })
    .state("collectionaccountlist", {
        url: "/collectionaccountlist",
        templateUrl: "templates/collectionaccountlistView.html",
        controller: "collectionaccountlistCtrl"
    })
    .state("collectionstatement", {
         url: "/collectionstatement",
         templateUrl: "templates/collectionstatementView.html",
         controller: "collectionstatementCtrl"
    })
    .state("agentprofile", {
        url: "/agentprofile",
        templateUrl: "templates/agentprofileView.html",
        controller: "agentprofileCtrl"
    })


    .state("checkbookrequest", {
        url: "/checkbookrequest",
        templateUrl: "templates/checkbookrequestView.html",
        controller: "checkbookrequestCtrl"
    })
    .state("accountstatement", {
        url: "/accountstatement",
        templateUrl: "templates/accountstatementView.html",
        controller: "accountstatementCtrl"
    })
    .state("accountsummary", {
        url: "/accountsummary",
        templateUrl: "templates/accountsummaryView.html",
        controller: "accountsummaryCtrl"
    })
    .state("verifyagent", {
        url: "/verifyagent",
        templateUrl: "templates/VerifyAgent.html",
        controller: "verifyagentCtrl"
    })
    .state("verifyotp", {
        url: "/verifyotp",
        templateUrl: "templates/VerifyOTP.html",
        controller: "verifyotpCtrl"
    })
    .state("resetpassword", {
        url: "/resetpassword",
        templateUrl: "templates/PasswordReset.html",
        controller: "resetpasswordCtrl"
    })
    .state("accountdetails", {
         url: "/accountdetails",
         templateUrl: "templates/accountdetailsView.html",
         controller: "accountdetailsCtrl"
     })
    .state("notifications", {
        url: "/notifications",
        templateUrl: "templates/notificationsView.html",
        controller: "notificationsCtrl"
    })
    .state("branchdetail", {
        url: "/branchdetail",
        templateUrl: "templates/branchdetailView.html",
        controller: "branchdetailCtrl"
    })
    .state("managementdetail", {
        url: "/managementdetail",
        templateUrl: "templates/managementdetailView.html",
        controller: "managementdetailCtrl"
    })
    .state("documents", {
        url: "/documents",
        templateUrl: "templates/documentView.html",
        controller: "documentCtrl"
    })
    .state("fundtransfer", {
        url: "/fundtransfer",
        templateUrl: "templates/fundtransferView.html",
        controller: "fundtransferCtrl"
    })
    .state("ownaccounttransfer", {
        url: "/transfer",
        templateUrl: "templates/OwnBankTransferView.html",
        controller: "ownaccounttransferCtrl"
    })
    .state("otheraccounttransfer", {
       url: "/transfer",
       templateUrl: "templates/OtherBankTransferView.html",
       controller: "otheraccounttransferCtrl"
   })
    .state("addbeneficiary", {
       url: "/addbeneficiary",
       templateUrl: "templates/addbeneficiaryView.html",
       controller: "addbeneficiaryCtrl"
   })
    .state("beneficiaryedit", {
       url: "/beneficiaryedit",
       templateUrl: "templates/beneficiaryeditView.html",
       controller: "beneficiaryeditCtrl"
   })
    .state("beneficiarylist", {
         url: "/beneficiarylist",
         templateUrl: "templates/beneficiarylistView.html",
         controller: "beneficiarylistCtrl"
     })
    .state("quickbalance", {
         url: "/quickbalance",
         templateUrl: "templates/quickBalanceView.html",
         controller: "quickbalanceCtrl"
     })
    .state("eazytrade", {
        url: "/eazytrade",
        templateUrl: "templates/eazytradeView.html",
        controller: "eazytradeCtrl"
    })
    .state("eazytradetrader", {
             url: "/eazytradetrader",
             templateUrl: "templates/eazytradetraderView.html",
             controller: "eazytradeCtrl"
         })
    .state("accountstatementtrade", {
         url: "/accountstatementtrade",
         templateUrl: "templates/accountstatementtradeView.html",
         controller: "accountstatementtradeCtrl"
     })
    .state("tradedashboard", {
         url: "/tradedashboard",
         templateUrl: "templates/tradedashboardView.html",
         controller: "fundtransferCtrl"
     })
     .state("receipttrade", {
         url: "/receipttrade",
         templateUrl: "templates/receipttradeView.html",
         controller: ""
     })
    $urlRouterProvider.otherwise('/prelogin');
})
;