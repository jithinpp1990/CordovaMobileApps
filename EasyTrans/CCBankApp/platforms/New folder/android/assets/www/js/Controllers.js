
starter.controller("preloginCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$ionicSlideBoxDelegate", "$cordovaFileTransfer", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $ionicSlideBoxDelegate, $cordovaFileTransfer, $cordovaDevice) {
    $scope.ImageModelList = {};

    $scope.updateSlider = function () {
        $ionicSlideBoxDelegate.update();
    };
    $scope.GetImages = function () {
        var url = APIUrl + "Authentication/GetImages/";
        var request = {};
        $http.post(url, request).then(function (data, status) {
            $scope.ImageModelList = data.data;
            $ionicSlideBoxDelegate.autoPlay();
            $ionicSlideBoxDelegate.enableSlide();
            $ionicSlideBoxDelegate.start();
            return false;
        }
        , function errorcallback(data, status) {
            $scope.ShowLoader = false;
            alert("Could Not Connect To Server.");
        });
    };
    $scope.Login = function () {
        $state.go("login");
        return false;
    };
    $scope.Register = function () {
        $state.go("verifycustomer");
        return false;
    };
    $scope.GetImages();
}]);

starter.controller("loginCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$ionicSlideBoxDelegate","$ionicPopup", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $ionicSlideBoxDelegate,$ionicPopup) {
    $scope.model = {
        username: '',
        password: ''
    };
    $scope.pauseTime = null;
    $scope.resumeTime = null;
    $scope.onLoad = function () {
        document.addEventListener("deviceready", document.onDeviceReady, false);
    };
    document.onDeviceReady = function () {
        document.addEventListener("pause", document.onPause, false);
        document.addEventListener("resume", document.onResume, false);
        // Add similar listeners for other events
    };
    document.onPause = function () {
        // Handle the pause event
        $scope.resumeTime = null;
        $scope.pauseTime = new Date();
        return false;
    }
    document.onResume = function () {
        // Handle the resume event     
        $scope.resumeTime = null;
        $scope.resumeTime = new Date();
        var dif = $scope.resumeTime - $scope.pauseTime;
        if (dif > 120000) {
            alert("Session Timeout Please Re-Login!")
            sessionFactory.setSession(null, null, null, null, null, null, null, null);
            $state.go("login");
            return false;
        }
        return false;
    }
    $scope.ShowLoader = false;
    $scope.Login = function () {
        if ($scope.model.username.trim() == "" || $scope.model.password.trim() == "") {
            alert("Invalid username, password");
            $scope.ClearAll();
            return false;
        }
        var deviceId = $cordovaDevice.getUUID();
        var url = APIUrl + "Authentication/AuthenticateUser/";
        var name = $scope.model.username;
        var request = {
            UserrName: $scope.model.username,
            Password: $scope.model.password,
            DeviceId: deviceId,
            Service: "login"
        };
        $scope.ShowLoader = true;
        $http.post(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (!data.data.AuthenticationSuccess) {
                alert(data.data.ErrorMessage);
                $scope.ClearAll();
                return false;
            }
            else {
                sessionFactory.setSession(data.data.SessionToken.Username, data.data.SessionToken.SessionToken, data.data.SessionToken.UserId, data.data.SessionToken.MemberId, data.data.SessionToken.AppId, data.data.SessionToken.MobileNo, data.data.SessionToken.Name, data.data.SessionToken.BankId);
                var session = sessionFactory.getSession();
                $scope.onLoad();
                $state.go("dashboard");
            }
        }, function errorcallback(data, status) {
            $scope.ShowLoader = false;
            alert("Could Not Connect To Server");
            $scope.ClearAll();
        });
    }
    $scope.ClearAll = function () {
        var elements = document.getElementsByTagName("input");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text" || elements[ii].type == "password") {
                elements[ii].value = "";
            }
        }
    };
}]);

starter.controller("branchdetailCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$cordovaDatePicker", "$cordovaLaunchNavigator", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $cordovaDatePicker, $cordovaLaunchNavigator) {
    $scope.ShowLoader = false;
    $scope.BranchList = {};

    $scope.launchNavigator = function (latitude, longitude) {
        //var destination = [9.948294, 76.347777];
        var destination = [latitude, longitude];
        console.log(latitude + " : " + longitude);
        var start = "Peoples Urban Co-operative Bank";
        $cordovaLaunchNavigator.navigate(destination);
    }
    $scope.Back = function () {
        var userid = sessionFactory.getSession().userid;
        if (userid == null || userid == "")
            $state.go("login");
        else
            $state.go("dashboard");
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Authentication/GetBranchList/";
        $http({
            method: "GET",
            url: url
        }).then(function mySucces(data) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.BranchList = data.data.BranchDetailsList;
                return false;
            }
        }, function myError(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.GetInitialData();
}]);

starter.controller("managementdetailCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$cordovaDatePicker", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $cordovaDatePicker) {
    $scope.ShowLoader = false;
    $scope.ManagementList = {};
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Authentication/GetManagementDetails/";
        $http({
            method: "GET",
            url: url
        }).then(function mySucces(data) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.ManagementList = data.data.ManagementDetailsList;
                return false;
            }
        }, function myError(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.Back = function () {
        var userid = sessionFactory.getSession().userid;
        if (userid == null || userid == "")
            $state.go("login");
        else
            $state.go("dashboard");
    };
    $scope.GetInitialData();
}]);

starter.controller("dashboardCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.Username = sessionFactory.getSession().username;

    $scope.ShowLoader = false;
    $scope.ServiceStatus = {};

    $scope.Redirect = function (target) {
        switch (target) {
            case "CheckBookRequest":
                $state.go("checkbookrequest");
                break;
            case "AccountStatement":
                $state.go("accountstatement");
                break;
            case "AccountSummary":
                $state.go("accountsummary");
                break;
            case "ChangePassword":
                $state.go("verifycustomer");
                break;
            case "AccountDetails":
                $state.go("accountdetails");
                break;
            case "Notifications":
                $state.go("notifications");
                break;
            case "Documents":
                $state.go("documents");
                break;
            case "FundTransfer":
                $state.go("fundtransfer");
                break;
            case "BankDet":
                $state.go("branchdetail");
                break;
            case "QuickBalance":
                $state.go("quickbalance");
                break;
            case "EazyTrade":
                $state.go("tradedashboard");
                break;
        }
    }
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.Back = function () {
        $state.go("dashboard");
    };
   
}]);

starter.controller("checkbookrequestCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.depositId = 0;
    $scope.options = {};
    $scope.Username = sessionFactory.getSession().username;

    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        AppId: sessionToken.appid,
        Service: "chequebookrequest",
        DepositId: 0,
        SelectedOption: {
            Id: 0
        }
    };

    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountHeads/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.options = data.data.ModeOfAccount;
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }

    $scope.RequestChequeBook = function () {
        $scope.request.DepositId = $scope.request.SelectedOption.Id;
        if ($scope.request.DepositId == 0) {
            alert("Please Select an Account.");
            return false;
        }
        url = APIUrl + "Service/RequestCheckBook/";
        $scope.ShowLoader = true;
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (!data.data.IsChequeRequestSuccesful) {
                alert(data.data.ErrorMessage);
                return false;
            }
            else {
                alert("Cheque book request submitted succesfully.");
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.Back = function () {
        $state.go("dashboard");
    };
    $scope.GetInitialData();

}]);

starter.controller("accountstatementCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$cordovaDatePicker", "depositIdForStmt", "$cordovaPrinter", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $cordovaDatePicker, depositIdForStmt, $cordovaPrinter) {
    $scope.ShowLoader = false;
    $scope.Receipt = "<Html></Html>";
    $scope.options = {};

    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        DateFrom: new Date(),
        DateTo: new Date(),
        DepositId: 0,
        AccountType: 0,
        AppId: sessionToken.appid,
        Service: "accountstatement",
        SelectedOption: { Id: 0, Caption: '', AccountType: 0 }
    };

    $scope.ShowStatement = false;
    $scope.StatementModel = null;

    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountHeads/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.Receipt = data.data.ReceiptHtml;
                $scope.options = data.data.ModeOfAccount;
                if (depositIdForStmt.getDepositId() != 0 && data.data.ModeOfAccount.length != 0) {
                    for (i = 0; i < data.data.ModeOfAccount.length; i++) {
                        console.log(data.data.ModeOfAccount[i].Id == depositIdForStmt.getDepositId());
                        if (data.data.ModeOfAccount[i].Id == depositIdForStmt.getDepositId()) {
                            $scope.request.SelectedOption = { Id: data.data.ModeOfAccount[i].Id, Caption: data.data.ModeOfAccount[i].Caption, AccountType: data.data.ModeOfAccount[i].accountType };
                            console.log($scope.request.SelectedOption);
                        }
                    }
                }

                var dt = new Date();
                dt.setDate($scope.request.DateFrom.getDate() - data.data.StmtFromDateDiff);
                $scope.request.DateFrom = dt;
                depositIdForStmt.setDepositId(0);
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }

    $scope.RequestStatement = function () {
        if ($scope.request.SelectedOption.Id == 0) {
            alert("Please Select Account Type")
            return false;
        }
        $scope.ShowLoader = true;
        $scope.request.DepositId = $scope.request.SelectedOption.Id;
        $scope.request.AccountType = $scope.request.SelectedOption.AccountType;
        var url = APIUrl + "Service/RequestStatement/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.ResponseStatement.StatementDetails.length > 0) {
                $scope.ShowStatement = data.data.ResponseStatement.StatementDetails.length > 0;
                $scope.StatementModel = data.data.ResponseStatement.StatementDetails;
            }
            else {
                $scope.ShowStatement = false;
                $scope.StatementModel = null;
                alert("No Data To Display!!!");
            }

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }

    $scope.Back = function () {
        $state.go("dashboard");
    };

    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.print = function () {
        //get service call to receive the receipt html
        if ($cordovaPrinter.isAvailable()) {

            $cordovaPrinter.print($scope.Receipt, { bounds: '10, 10, 0, 0' });

        } else {
            alert("Printing is not available on device");
        }
    }
    $scope.GetInitialData();
}]);

starter.controller("accountsummaryCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "depositIdForStmt", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, depositIdForStmt) {
    $scope.ShowLoader = false;
    $scope.Member = null;
    $scope.AccountSummaryList = null;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    var request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        AppId: sessionToken.appid,
        Service: "accountsummery",
        MemberId: sessionToken.memberid

    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountSummary/";
        $http.post(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            //$scope.Member = data.data.MemberDetails;
            $scope.AccountSummaryList = data.data.AccountSummaryList;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.RedirectToStatement = function (depositid) {
        depositIdForStmt.setDepositId(depositid);
        $state.go("accountstatement");
    };

    $scope.Back = function () {
        $state.go("dashboard");
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.GetInitialData();
}]);

starter.controller("verifycustomerCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$ionicPopup", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $ionicPopup) {
    $scope.request = {
        CustomerId: '',
        UserId: '',
        OTP: ''
    };
    $scope.ShowLoader = false;
    $scope.Back = function () {

        var CustomerId = sessionFactory.getSession().CustomerId;
        console.log(CustomerId);
        if (CustomerId == null || CustomerId == "")
            $state.go("login");
        else
            $state.go("dashboard");
    };
    $scope.VerifyCustomer = function () {
        if ($scope.request.CustomerId.trim() == "") {
            alert("User id should not be empty.");
            return false;
        }
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/VerifyCustomer/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.ValidCustomer) {
                sessionFactory.setSession(null, null, $scope.request.CustomerId, null,null,null,null,null);
                $scope.showPopup();
                return false;
            }
            else {
                alert(data.data.ErrorMessage);
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 412) {
                alert("Invalid Customer id");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.showPopup = function () {
        $scope.data = {};
        var myPopup = $ionicPopup.show({
            template: '<input type="password" ng-model="data.otp">',
            title: 'Enter OTP',
            subTitle: 'Please enter OTP received on your mobile.',
            scope: $scope,
            buttons: [
              { text: 'Cancel' },
              {
                  text: '<b>Ok</b>',
                  type: 'button-positive',
                  onTap: function (e) {
                      if (!$scope.data.otp) {
                          e.preventDefault();
                      } else {
                          return $scope.data.otp;
                      }
                  }
              }
            ]
        });

        myPopup.then(function (res) {
            $scope.request.OTP = res;
            $scope.IsSentOTP = true;
            $scope.VerifyOTP();
        });

        $timeout(function () {
            myPopup.close(); //close the popup after 3 seconds for some reason
        }, 200000);
    };
    $scope.VerifyOTP = function () {
        $scope.request.CustomerId = sessionFactory.getSession().userid;
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/VerifyOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.ValidCustomer) {
                $state.go("resetpassword");
                return false;
            }
            else {
                alert(data.data.ErrorMessage);
                $state.go("verifycustomer");
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 412) {
                alert("Invalid OTP");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Back = function () {
        var CustomerId = sessionFactory.getSession().CustomerId;
        if (CustomerId == null || CustomerId == "")
            $state.go("login");
        else
            $state.go("login");
    };
}]);

starter.controller("verifyotpCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.request = {
        OTP: '',
        CustomerId: ''
    };
    $scope.ShowLoader = false;
    $scope.VerifyOTP = function () {
        if ($scope.request.OTP.trim() == "") {
            alert("Please enter a valid OTP.");
            return false;
        }
        $scope.request.CustomerId = sessionFactory.getSession().userid;
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/VerifyOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.ValidCustomer) {
                $state.go("resetpassword");
                return false;
            }
            else {
                alert(data.data.ErrorMessage);
                $state.go("verifycustomer");
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 412) {
                alert("Invalid OTP");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Back = function () {
        var userid = sessionFactory.getSession().userid;
        if (userid == null || userid == "")
            $state.go("login");
        else
            $state.go("login");
    };
}]);

starter.controller("resetpasswordCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {

    $scope.PassHide = false;
    $scope.PinHide = true;
    $scope.request = {
        OldPassword: '',
        Password: '',
        ConfirmPassword: '',
        CustomerId: '',
        Type: "PAS"
    };
    $scope.Back = function () {
        var userid = sessionFactory.getSession().userid;
        if (userid == null || userid == "")
            $state.go("login");
        else
            $state.go("login");
    };
    $scope.ShowLoader = false;
    $scope.updatepassword = function () {
        if ($scope.request.Type == "PAS") {
            if ($scope.request.Password.trim() == "" || $scope.request.ConfirmPassword.trim() == "") {
                alert("Please fill password");
                return false;
            }
        }
        else {
            if ($scope.request.Password.trim() == "" || $scope.request.ConfirmPassword.trim() == "") {
                alert("Please fill PIN");
                return false;
            }
        }
        if ($scope.request.Type == "PIN" && $scope.request.OldPassword.trim() == "") {
            alert("Please Enter Old PIN");
            return false;
        }
        if ($scope.request.Password != $scope.request.ConfirmPassword) {
            if ($scope.request.Type == "PAS") {

                alert("Passwords mismatch!!!");
            }
            else {
                alert("PIN mismatch!!!");
            }
            return false;
        }
        $scope.request.DeviceId = $cordovaDevice.getUUID();
        $scope.request.CustomerId = sessionFactory.getSession().userid;
        $scope.ShowLoader = true;
        if ($scope.request.Type == "PAS") {
            var url = APIUrl + "Service/UpdatePassword/";
            $http.post(url, $scope.request).then(function (data, status) {
                $scope.ShowLoader = false;
                if (data.data.ValidCustomer) {
                    alert("Password Changed Successfully ");
                    $state.go("login");
                    return false;
                }
                else {
                    alert("Password Change Failed");
                }
            }, function errorcallback(error, status) {
                $scope.ShowLoader = false;
                if (error.status == 412) {
                    alert("Password should contain one letter(a-z) one number(1-9) and atleast 8 digit length.");
                    return false;
                }
                alert("An unexpected error occured. Please contact the administrator.");
            });
        }
        else if ($scope.request.Type == "PIN") {
            var url = APIUrl + "Service/UpdatePIN/";
            $http.post(url, $scope.request).then(function (data, status) {
                $scope.ShowLoader = false;
                if (data.data.ValidCustomer) {
                    alert("PIN Changed Successfully");
                    $state.go("login");
                    return false;
                }
                else {
                    alert("PIN Change Failed!!!");
                }
            }, function errorcallback(error, status) {
                $scope.ShowLoader = false;
                if (error.status == 412) {
                    alert("PIN Change Failed!!!");
                    return false;
                }
                alert("An unexpected error occured. Please contact the administrator.");
            });
        }
    };
    $scope.SetType = function () {
        if ($scope.request.Type == "PIN") {
            $scope.PassHide = true;
            $scope.PinHide = false;
        }
        else if ($scope.request.Type == "PAS") {
            $scope.PassHide = false;
            $scope.PinHide = true;
        }
    };
}]);

starter.controller("accountdetailsCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    $scope.AccountDetails = {};
    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    var request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        AppId: sessionToken.appid,
        Service: "memberdetails",
        MemberId: sessionToken.memberid
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountDetails/";
        $http.post(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.AccountDetails = data.data;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.Back = function () {
        $state.go("dashboard");
    };
    $scope.GetInitialData();
}]);

starter.controller("notificationsCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.Notifications = {};
    var sessionToken = sessionFactory.getSession();
    var request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        AppId: sessionToken.appid,
        Service: "notification",
        MemberId: sessionToken.memberid
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetNotifications/";
        $http.post(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.Notifications.length > 0) {
                $scope.Notifications = data.data.Notifications;
            }
            else {
                alert("No Notification To Display!!!");
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.Back = function () {
        $state.go("dashboard");
    };
    $scope.GetInitialData();
}]);

starter.controller("documentCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.Documents = {};
    var sessionToken = sessionFactory.getSession();
    var request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        AppId: sessionToken.appid,
        Service: "documents",
        MemberId: sessionToken.memberid
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Content/GetDocumentNames/";
        $http.get(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.Documents = data.data;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
        document.addEventListener('deviceready', function () {

            var url = encodeURI("http://www.gstatic.com/webp/gallery/2.jpg");
            alert("cordova.file : " + cordova.file);
            alert('cordova.file.externalDataDirectory: ' + cordova.file.externalDataDirectory);
            myFsRootDirectory1 = 'file:///storage/emulated/0/'; // path for tablet
            myFsRootDirectory2 = 'file:///storage/sdcard0/'; // path for phone
            fileTransferDir = cordova.file.externalDataDirectory;
            //if (fileTransferDir.indexOf(myFsRootDirectory1) === 0) {
            //    fileDir = fileTransferDir.replace(myFsRootDirectory1, '');
            //}
            //if (fileTransferDir.indexOf(myFsRootDirectory2) === 0) {
            //    fileDir = fileTransferDir.replace(myFsRootDirectory2, '');
            //}
            //alert('Android FILETRANSFERDIR: ' + fileTransferDir);
            //alert('Android FILEDIR: ' + fileDir);
            var trustHosts = true;
            var options = {};

            $cordovaFileTransfer.download(url, fileTransferDir + "Myimage.jpg", options, trustHosts)
              .then(function (result) {
                  alert("success : " + result);
                  // Success!
              }, function (err) {
                  // Error
                  alert("Error : " + err.code);
                  alert("Error object : " + JSON.stringify(err))
              }, function (progress) {
                  $timeout(function () {
                      $scope.downloadProgress = (progress.loaded / progress.total) * 100;
                  });
              });

        }, false);
    };


    $scope.Download = function (documentName) {
        $scope.ShowLoader = true;
        var url = APIUrl + "Content/GetDocument?documentName=" + documentName;
        $http.get(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.Documents = data.data;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.Back = function () {
        $state.go("dashboard");
    };
    $scope.GetInitialData();
}]);

starter.controller("fundtransferCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {

    $scope.IsATrader = false;
    $scope.Username = sessionFactory.getSession().username;
    $scope.request = {
        SessionToken: sessionFactory.getSession().sessiontoken,
        UserId: sessionFactory.getSession().userid,
        DeviceId: sessionFactory.getSession().deviceId,
        MemberId: sessionFactory.getSession().memberid,
        AppId: sessionFactory.getSession().appid,
        TradeCode: null
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    };


    //$scope.GetInitialData = function () {
    //    $scope.ShowLoader = true;
    //    var url = APIUrl + "Service/ValidateTrader/";
    //    console.log(url + " -- " + $scope.request);
    //    $http.post(url, $scope.request).then(function (data, status) {
    //        $scope.ShowLoader = false;
    //        if (data.data.TraderName == null || data.data.TraderName == "") {
    //            $scope.IsATrader = true;
    //        }
    //        else {
    //            $scope.IsATrader = false;
    //        }
    //    }, function errorcallback(error, status) {
    //        $scope.ShowLoader = false;
    //        if (error.status == 401) {
    //            alert("Unauthorized access. Login again.");
    //            sessionFactory.setSession(null, null, null, null, null,null,null,null);
    //            $state.go("login");
    //            return false;
    //        }
    //        alert("An unexpected error occured. Please contact the administrator.");
    //    });
    //};


    $scope.Redirect = function (type) {
        switch (type) {
            case 'INTERNAL': {
                $state.go('ownaccounttransfer');
                break;
            }
            case 'OTHER': {
                $state.go('otheraccounttransfer');
                break;
            }
            case 'IMPS': {
                $state.go('impsdashboard');
                break;
            }
            case 'AddBeneficiary': {
                $state.go('addbeneficiarydashboard');
                break;
            }
            case 'BeneficiaryList': {
                $state.go('beneficiarylist');
                break;
            }
            case 'EazyTrade': {
                $state.go('eazytrade');
                break;
            }
            case 'EazyTradeTrader': {
                $state.go('eazytradetrader');
                break;
            }
            case 'TradeStatement': {
                $state.go('accountstatementtrade');
                break;
            }
        }
    };

    $scope.Back = function () {
        $state.go("dashboard");
    };
    // $scope.GetInitialData();

}]);

starter.controller("easytradedashboardCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = true;
    $scope.IsATrader = false;
    $scope.Username = sessionFactory.getSession().username;
    $scope.request = {
        SessionToken: sessionFactory.getSession().sessiontoken,
        UserId: sessionFactory.getSession().userid,
        DeviceId: sessionFactory.getSession().deviceId,
        MemberId: sessionFactory.getSession().memberid,
        AppId: sessionFactory.getSession().appid,
        TradeCode: null
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    };


    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/ValidateTrader/";
        console.log(url + " -- " + $scope.request);
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.TraderName == null || data.data.TraderName == "") {
                $scope.IsATrader = true;
            }
            else {
                $scope.IsATrader = false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };


    $scope.Redirect = function (type) {
        switch (type) {
            case 'EazyTrade': {
                $state.go('eazytrade');
                break;
            }
            case 'EazyTradeTrader': {
                $state.go('eazytradetrader');
                break;
            }
            case 'TradeStatement': {
                $state.go('accountstatementtrade');
                break;
            }
        }
    };

    $scope.Back = function () {
        $state.go("dashboard");
    };
    $scope.GetInitialData();

}]);

starter.controller("ownaccounttransferCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.FromAccounts = {};
    $scope.ToAccounts = {};
    $scope.IsSentOTP = false;
    $scope.IsReSendOTP = true;
    $scope.PaymentType = "";

    $scope.Service = "ownbanktransfer";
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        AppId: sessionToken.appid,
        VerificationMethod: "OTP",
        OTP: "",
        Amount: 0.00,
        Service: "ownbanktransfer"
    };
    $scope.GetInitialData = function () {
        console.log("initial data");
        $scope.request.TransferType = 'WithInBank';
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetTransferDetails/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.FromAccounts = data.data.FromAccounts;
            $scope.ToAccounts = data.data.ToAccounts;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.SubmitTransfer = function () {
        if ($scope.request.Amount == 0 || $scope.request.Amount == "") {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.OTP.trim() == "") {
            alert("Invalid OTP/PIN");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Please Select Your Account!!!");
            return false;
        }
        if ($scope.request.ToAccount == null) {
            alert("Invalid Beneficiary Account");
            return false;
        }
        $scope.ShowLoader = true;
        $scope.request.TransferType = $scope.PaymentType;
        $scope.request.OTP = $scope.request.VerificationMethod + $scope.request.OTP;
        var url = APIUrl + "Service/SubmitTransfer/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = false;
            alert(data.data);
            $scope.ClearAll();
            $state.go('fundtransfer');

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            $state.go('fundtransfer');
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.SetStatus = function () {
        $scope.request.OTP = "";
        if ($scope.request.VerificationMethod == "PIN") {
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
            $scope.InputType = "password"
        }
        else if ($scope.request.VerificationMethod == "OTP") {
            $scope.IsSentOTP = false;
            $scope.IsReSendOTP = false;
            $scope.InputType = "text"
        }
    };
    $scope.SendOTP = function () {
        if ($scope.request.Amount == 0 || $scope.request.Amount == "") {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.request.ToAccount == null) {
            alert("Invalid Beneficiary Account");
            return false;
        }
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/SendOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.ClearAll = function () {
        var elements = document.getElementsByTagName("input");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text") {
                elements[ii].value = "";
            }
            if (elements[ii].type == "number") {
                elements[ii].value = 0;
            }
        }

    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.Back = function () {
        $state.go("fundtransfer");
    }
    $scope.GetInitialData();
}]);

starter.controller("otheraccounttransferCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.FromAccounts = {};
    $scope.ToAccounts = {};
    $scope.IsSentOTP = false;
    $scope.IsReSendOTP = true;
    $scope.PaymentType = "";
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        AppId: sessionToken.appid,
        Amount: 0.00,
        VerificationMethod: "OTP",
        Service: "oterbanktransfer"
    };

    $scope.GetInitialData = function () {
        $scope.request.TransferType = 'OtherBank';
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetTransferDetails/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.FromAccounts = data.data.FromAccounts;
            $scope.ToAccounts = data.data.ToAccounts;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.SubmitTransfer = function () {
        if ($scope.request.Amount == 0) {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.OTP.trim() == "") {
            alert("Invalid OTP/PIN");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.request.ToAccount == null) {
            alert("Invalid Beneficiary Account");
            return false;
        }

        $scope.ShowLoader = true;
        $scope.request.TransferType = $scope.PaymentType;
        var url = APIUrl + "Service/SubmitTransfer/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = false;
            alert(data.data);
            $state.go("transfer");
            $scope.ClearAll();

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.SendOTP = function () {
        if ($scope.request.Amount == 0) {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.TransferType == "" || $scope.request.TransferType == null) {
            alert("Invalid Transfer Type");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.request.ToAccount == null) {
            alert("Invalid Beneficiary Account");
            return false;
        }
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/SendOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.SetStatus = function () {
        if ($scope.request.VerificationMethod == "PIN") {
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
            $scope.InputType = "password"
        }
        else if ($scope.request.VerificationMethod == "OTP") {
            $scope.IsSentOTP = false;
            $scope.IsReSendOTP = false;
            $scope.InputType = "text"
        }
    };
    $scope.ClearAll = function () {
        var elements = document.getElementsByTagName("input");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text" || elements[ii].type == "number") {
                elements[ii].value = "";
            }
        }
    };
    $scope.Back = function () {
        $state.go("fundtransfer");
    };

    $scope.GetInitialData();
}]);

starter.controller("addbeneficiarydashboardCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {

    $scope.IsATrader = false;
    $scope.Username = sessionFactory.getSession().username;
    $scope.request = {
        SessionToken: sessionFactory.getSession().sessiontoken,
        UserId: sessionFactory.getSession().userid,
        DeviceId: sessionFactory.getSession().deviceId,
        MemberId: sessionFactory.getSession().memberid,
        AppId: sessionFactory.getSession().appid,
        TradeCode: null
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    };
    $scope.Redirect = function (type) {
        switch (type) {
            case 'Inter': {
                $state.go('addbeneficiaryinter');
                break;
            }
            case 'Intra': {
                $state.go('addbeneficiaryintra');
                break;
            }
            case 'Imps': {
                $state.go('addbeneficiaryimps');
                break;
            }
        }
    };

    $scope.Back = function () {
        $state.go("fundtransfer");
    }

}]);

starter.controller("addbeneficiaryCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$ionicPopup", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $ionicPopup, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.MinLimit = 100;
    $scope.MaxLimit = 100000;
    $scope.IsSentOTP = false;
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        IFSC: "",
        BeneficiaryName: "",
        AccountNumber: "",
        ConfirmAccountNumber: "",
        Mmid: "",
        ConfirmMmid: "",
        MobileNumber: "",
        AddressLine1: "",
        AddressLine2: "",
        AddressLine3: "",
        MaxLimit: 100,
        OTP: "",
        BankName: "",
        BankBranch: "",
        AppId: sessionToken.appid,
        Service: "addbeneficiary",
        BeneficiaryType: ""
    };
    $scope.SendOTP = function () {
        if ($scope.request.BeneficiaryType == "Imps") {
            if ($scope.request.BeneficiaryName == "") {
                alert("Invalid Beneficiary Name");
                return false;
            }

            if ($scope.request.Mmid.trim() == "" || $scope.request.ConfirmMmid.trim() == "") {
                alert("Invalid MMID");
                return false;
            }

            if ($scope.request.Mmid.trim() != $scope.request.ConfirmMmid.trim()) {
                alert("MMID Does not match");
                return false;
            }

            if ($scope.request.MobileNumber == "" || $scope.request.MobileNumber.toString().length != 10) {
                alert("Invalid Mobile number");
                return false;
            }
            if ($scope.request.TransferLimit == 0) {
                alert("Invalid transfer limit");
                return false;
            }
        }
        else if ($scope.request.BeneficiaryType == "Intra") {
            if ($scope.request.BeneficiaryName == "") {
                alert("Invalid Beneficiary Name");
                return false;
            }
            if ($scope.request.AccountNumber.trim() == "" || $scope.request.ConfirmAccountNumber.trim() == "") {
                alert("Invalid Ac. No.");
                return false;
            }
            if ($scope.request.AccountNumber.trim() != $scope.request.ConfirmAccountNumber.trim()) {
                alert("Ac. No. Does not match");
                return false;
            }
            if ($scope.request.TransferLimit == 0) {
                alert("Invalid transfer limit");
                return false;
            }
        }
        else if ($scope.request.BeneficiaryType == "Inter") {
            if ($scope.request.BeneficiaryName == "") {
                alert("Invalid Beneficiary Name");
                return false;
            }
            if ($scope.request.AccountNumber.trim() == "" || $scope.request.ConfirmAccountNumber.trim() == "") {
                alert("Invalid Ac. No.");
                return false;
            }
            if ($scope.request.AccountNumber.trim() != $scope.request.ConfirmAccountNumber.trim()) {
                alert("Ac. No. Does not match");
                return false;
            }
            if ($scope.request.TransferLimit == 0) {
                alert("Invalid transfer limit");
                return false;
            }
            if ($scope.request.BankName == "") {
                alert("Invalid IFSC Code");
                return false;
            }


        }
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/SendOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.showPopup();
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.ValidateIFSC = function () {
        if ($scope.request.IFSC == "") {
            alert("Invalid IFSC Code");
            return false;
        }
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/ValidateIFSC/";
        $http.post(url, $scope.request).then(function (data, status) {
            if (data.data == null) {
                alert("Invalid IFSC code");
                return false;
            }
            $scope.request.BankBranch = data.data.BankBranch;
            $scope.request.BankName = data.data.BankName;
            $scope.ShowLoader = false;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.AddBeneficiary = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/AddBeneficiary/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.IsSentOTP = false;
            $scope.ClearAll();
            $scope.ShowLoader = false;
            alert(data.data);
            $state.go('addbeneficiarydashboard');
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.Back = function () {
        $state.go("addbeneficiarydashboard");
    };
    $scope.ClearAll = function () {
        var elements = document.getElementsByClassName("input-pub");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text") {
                elements[ii].value = null;
            }
        }
    };
    $scope.showPopup = function () {
        $scope.data = {};
        var myPopup = $ionicPopup.show({
            template: '<input type="password" ng-model="data.otp">',
            title: 'Enter OTP',
            subTitle: 'Please enter OTP received on your mobile.',
            scope: $scope,
            buttons: [
              { text: 'Cancel' },
              {
                  text: '<b>Ok</b>',
                  type: 'button-positive',
                  onTap: function (e) {
                      if (!$scope.data.otp) {
                          e.preventDefault();
                      } else {
                          return $scope.data.otp;
                      }
                  }
              }
            ]
        });

        myPopup.then(function (res) {
            $scope.request.OTP = res;
            $scope.IsSentOTP = true;
            $scope.AddBeneficiary();
        });

        $timeout(function () {
            myPopup.close(); //close the popup after 3 seconds for some reason
        }, 200000);
    };
    $scope.ChangeBeneficiaryType = function (type) {
        $scope.BeneficiaryType = type;
    };
}]);

starter.controller("beneficiarylistCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "beneficiatryIdForEdit", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, beneficiatryIdForEdit) {
    $scope.ShowLoader = false;
    $scope.Member = null;
    $scope.BenificiaryList = null;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    var request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        Service: "beneficiarylist",
        AppId: sessionToken.appid,
        MemberId: sessionToken.memberid
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetBeneficiaryList/";
        $http.post(url, request).then(function (data, status) {
            $scope.ShowLoader = false;
            //$scope.Member = data.data.MemberDetails;
            $scope.BenificiaryList = data.data;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.RedirectToBenificiary = function (beneficiaryid) {
        beneficiatryIdForEdit.setBeneficiaryId(beneficiaryid);
        $state.go("beneficiaryedit");
    };

    $scope.Back = function () {
        $state.go("fundtransfer");
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.GetInitialData();
}]);

starter.controller("beneficiaryeditCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "beneficiatryIdForEdit", "$ionicPopup", "$timeout", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, beneficiatryIdForEdit, $ionicPopup, $timeout) {
    $scope.ShowLoader = false;
    $scope.beneficiaryDetails = {}
    $scope.Member = null;
    $scope.BankName = '';
    $scope.Back = '';
    $scope.IsAddBeneficiary = false;
    $scope.Username = sessionFactory.getSession().username;

    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        BeneficiaryId: beneficiatryIdForEdit.getBeneficiaryId(),
        IFSC: "",
        AppId: sessionToken.appid,
        Service: "beneficiaryedit"
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetBeneficiary/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            beneficiatryIdForEdit.setBeneficiaryId(0);
            if (data.data != null) {
                $scope.beneficiaryDetails = data.data;
                $scope.BankName = data.data.BankName;
                $scope.BankBranch = data.data.BankBranch;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            beneficiatryIdForEdit.setBeneficiaryId(0);
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.ValidateIFSC = function () {
        $scope.ShowLoader = true;

        $scope.request.IFSC = $scope.beneficiaryDetails.IFSC;
        var url = APIUrl + "Service/ValidateIFSC/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data == null) {
                alert("Invalid IFSC code");
                return false;
            }
            $scope.BankName = data.data.BankName;
            $scope.BankBranch = data.data.BankBranch;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.Back = function () {
        $state.go("beneficiarylist");
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.Delete = function () {
        $scope.request.Service = "deletebeneficiary";
        $scope.IsAddBeneficiary = false;
        $scope.SendOTP();
    };
    $scope.UpdateDet = function () {
        $scope.IsAddBeneficiary = true;
        $scope.SendOTP();
    };

    $scope.SendOTP = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/SendOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.showPopup();
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.DeleteBeneficiary = function () {
        $scope.ShowLoader = true;
        $scope.request.Service = "deletebeneficiary";
        var url = APIUrl + "Service/DeleteBeneficiary/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.showAlert(data.data);
            $state.go("beneficiarylist");
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });

    };

    $scope.AddBeneficiary = function () {
        $scope.request.SessionToken = sessionToken.sessiontoken;
        $scope.request.Service = "updatebeneficiary";
        $scope.request.UserId = sessionToken.userid;
        $scope.request.DeviceId = deviceId;
        $scope.request.MemberId = sessionToken.memberid;
        $scope.request.AppId = sessionToken.appid;

        console.log($scope.request);
        //if ($scope.request.Name == "") {
        //    alert("Invalid Name");
        //    return false;
        //}
        if ($scope.request.OTP.trim() == "") {
            alert("Invalid OTP");
            return false;
        }
        if ($scope.request.TransferLimit == 0) {
            alert("Invalid transfer limit");
            return false;
        }
        $scope.ShowLoader = true;

        var url = APIUrl + "Service/AddBeneficiary/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.showAlert(data.data);
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });

    };

    $scope.showPopup = function () {
        $scope.data = {};
        var myPopup = $ionicPopup.show({
            template: '<input type="password" ng-model="data.otp">',
            title: 'Enter OTP',
            subTitle: 'Please enter OTP received on your mobile.',
            scope: $scope,
            buttons: [
              { text: 'Cancel' },
              {
                  text: '<b>Validate</b>',
                  type: 'button-positive',
                  onTap: function (e) {
                      if (!$scope.data.otp) {
                          e.preventDefault();
                      } else {
                          return $scope.data.otp;
                      }
                  }
              }
            ]
        });

        myPopup.then(function (res) {
            $scope.request = $scope.beneficiaryDetails;
            $scope.request.OTP = res;
            if ($scope.IsAddBeneficiary)
                $scope.AddBeneficiary();
            else
                $scope.DeleteBeneficiary();
        });

        $timeout(function () {
            myPopup.close(); //close the popup after 3 seconds for some reason
        }, 30000);
    };

    $scope.showAlert = function (message) {
        var alertPopup = $ionicPopup.alert({
            title: 'Beneficiary List Update',
            template: message
        });

        alertPopup.then(function (res) {
            $scope.go('dashboard');
        });
    };

    $scope.GetInitialData();
    //$scope.showPopup();
}]);

starter.controller("quickbalanceCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$cordovaDatePicker", "depositIdForStmt", "$ionicPopup", "$timeout", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $cordovaDatePicker, depositIdForStmt, $ionicPopup, $timeout) {
    $scope.ShowLoader = false;
    $scope.options = {};
    $scope.CurrentBalance = "0";
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        DepositId: 0,
        AccountType: 0,
        AppId: sessionToken.appid,
        Service: "quickbalance",
        SelectedOption: { Id: 0, Caption: '', AccountType: 0 }
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountHeads/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.options = data.data.ModeOfAccount;
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.RequestBalance = function () {
        $scope.ShowLoader = true;
        $scope.request.DepositId = $scope.request.SelectedOption.Id;
        $scope.request.AccountType = $scope.request.SelectedOption.AccountType;
        console.log($scope.request.SelectedOption);
        console.log($scope.request.SelectedOption.Id);
        console.log($scope.request.SelectedOption.AccountType);
        if ($scope.request.SelectedOption.Id == 0) {
            alert("Please Select Account Type")
            $scope.ShowLoader = false;
            return false;
        }
        var url = APIUrl + "Service/RequestBalance/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.CurrentBalance = "Rs. " + data.data.ResponseStatement.BalanceAmt;
            $scope.showAlert();

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }

    $scope.Back = function () {
        $state.go("dashboard");
    };
    // An alert dialog
    $scope.showAlert = function () {
        var alertPopup = $ionicPopup.alert({
            title: 'Available Balance :',
            template: $scope.CurrentBalance
        });

        alertPopup.then(function (res) {
            console.log('Thank you for not eating my delicious ice cream cone');
        });
    };

    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.GetInitialData();
}]);

starter.controller("eazytradeCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$cordovaBarcodeScanner", "$cordovaPrinter", "$ionicPopup", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $cordovaBarcodeScanner, $cordovaPrinter, $ionicPopup) {
    $scope.ShowLoader = false;
    $scope.ShowTrader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.FromAccounts = {};
    $scope.ToAccounts = {};
    $scope.IsSentOTP = false;
    $scope.IsReSendOTP = true;
    $scope.checkbox_status = null;
    $scope.PaymentType = "";
    $scope.InputType = "text";
    $scope.Receipt = "<Html></Html>";
    $scope.IsPrint = false;
    $scope.IsQR = false;
    $scope.IsNormal = true;
    var sessionToken = sessionFactory.getSession();

    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        AppId: sessionToken.appid,
        Service: "eazytrade",
        TraderAddress: "",
        TraderName: "",
        TradeCode: "",
        TraderId: "",
        RefNo: "",
        OTP: "",
        BarcodeData: "",
        CustomerMemberId: null,
        VerificationMethod: "OTP",
        Amount: 0.00,
        ReadMethod: 'N'
    };

    $scope.GetInitialData = function () {
        $scope.request.TransferType = 'OwnBank';
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountHeads/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.FromAccounts = data.data.ModeOfAccount;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.SubmitTransfer = function () {
        if ($scope.request.Amount == 0) {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.OTP.trim() == "") {
            alert("Invalid OTP/PIN");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.ShowTrader == false) {
            alert("Invalid Trade Code");
            return false;
        }


        $scope.ShowLoader = true;
        $scope.request.TransferType = $scope.PaymentType;
        var url = APIUrl + "Service/SubmitTradeTran/";
        $scope.request.OTP = $scope.request.VerificationMethod + $scope.request.OTP;
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = false;
            $scope.ShowTrader = false;
            //$scope.OTP = null;
            //$scope.Amount = null;
            //$scope.request.RefNo= "";
            //$scope.request.OTP = "";
            //$scope.request.TradeCode = "";
            $scope.ClearAll();
            $scope.SetStatus();
            alert(data.data.CustomerMessage);
            if (data.data.status == "S") {
                $scope.IsPrint = true;
                $scope.Receipt = data.data.ReceiptHtml;
                $scope.showConfirm();
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");

        });
    };

    $scope.SendOTP = function () {
        if ($scope.request.Amount == 0) {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.ShowTrader == false && $scope.request.Service == "TI") {
            alert("Invalid Customer Code");
            return false;
        }
        if ($scope.ShowTrader == false && $scope.request.Service == "CI") {
            alert("Invalid Trader Code");
            return false;
        }
        $scope.ShowLoader = true;
        if ($scope.request.Service == "TI") {
            var url = APIUrl + "Service/SendOTPCustomer/";
            $http.post(url, $scope.request).then(function (data, status) {
                $scope.ShowLoader = false;
                $scope.IsSentOTP = true;
            }, function errorcallback(error, status) {
                $scope.ShowLoader = false;
                if (error.status == 401) {
                    alert("Unauthorized access. Login again.");
                    sessionFactory.setSession(null, null, null, null, null, null, null, null);
                    $state.go("login");
                    return false;
                }

                alert("An unexpected error occured. Please contact the administrator.");
            });
        }
        else {
            var url = APIUrl + "Service/SendOTP/";
            $http.post(url, $scope.request).then(function (data, status) {
                $scope.ShowLoader = false;
                $scope.IsSentOTP = true;
            }, function errorcallback(error, status) {
                $scope.ShowLoader = false;
                if (error.status == 401) {
                    alert("Unauthorized access. Login again.");
                    sessionFactory.setSession(null, null, null, null, null, null, null, null);
                    $state.go("login");
                    return false;
                }

                alert("An unexpected error occured. Please contact the administrator.");
            });
        }
    };
    $scope.ValidateTRCode = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/ValidateTrader/";
        console.log(url + " -- " + $scope.request);
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowTrader = true;
            if (data.data.TraderName == null || data.data.TraderName == "") {
                $scope.ShowTrader = false;
                alert("Invalid Trade Code");
                $scope.ShowLoader = false
                return false;
            }
            $scope.request.TraderAddress = data.data.TraderAddress;
            $scope.request.TraderName = data.data.TraderName;
            $scope.request.TradeCode = data.data.TradeCode;
            $scope.request.TraderId = data.data.TraderId;
            if ($scope.request.Service == "TI") {
                $scope.request.CustomerMemberId = data.data.TraderId;
            }
            $scope.ShowLoader = false;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false; weds
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.SetStatus = function () {
        $scope.request.OTP = "";
        if ($scope.request.VerificationMethod == "PIN") {
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
            $scope.InputType = "password"
        }
        else if ($scope.request.VerificationMethod == "OTP") {
            $scope.IsSentOTP = false;
            $scope.IsReSendOTP = false;
            $scope.InputType = "text"
        }
    };
    $scope.SetReader = function () {
        if ($scope.request.ReadMethod == "Q") {
            $scope.IsQR = true;
            $scope.IsNormal = false;
        }
        else if ($scope.request.ReadMethod == "N") {
            $scope.IsQR = false;
            $scope.IsNormal = true;
        }
    };
    $scope.Back = function () {
        $state.go("tradedashboard");
    };
    $scope.scanBarcode = function () {
        $cordovaBarcodeScanner.scan().then(function (imageData) {
            //alert(imageData.text);
            //$scope.ShowTrader = true;
            $scope.request.BarcodeData = imageData.text;
            console.log("Barcode Format -> " + imageData.format);
            console.log("Cancelled -> " + imageData.cancelled);
            if ($scope.request.BarcodeData != null && $scope.request.BarcodeData != "") {
                $scope.ValidateTRCode();
            }
        }, function (error) {
            console.log("An error happened -> ");
        });
    };
    $scope.showConfirm = function () {
        var confirmPopup = $ionicPopup.confirm({
            title: 'Recipt',
            template: 'Print a Receipt of Transaction...'
        });

        confirmPopup.then(function (res) {
            if (res) {
                $scope.print();
            } else {
                console.log('You are not sure');
            }
        });
    };
    $scope.ClearAll = function () {
        var elements = document.getElementsByTagName("input");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text" || elements[ii].type == "number") {
                elements[ii].value = "";
            }
        }
    };
    $scope.print = function () {
        //get service call to receive the receipt html
        if ($cordovaPrinter.isAvailable()) {

            $cordovaPrinter.print($scope.Receipt, { bounds: '10, 10, 0, 0' });

        } else {
            alert("Printing is not available on device");
        }
    }
    $scope.GetInitialData();
    $scope.SetStatus();
    $scope.SetReader();
}]);

starter.controller("accountstatementtradeCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", "$cordovaDatePicker", "depositIdForStmt", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice, $cordovaDatePicker, depositIdForStmt) {
    $scope.ShowLoader = false;
    $scope.options = {};
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        DateFrom: new Date(),
        DateTo: new Date(),
        DepositId: 0,
        AccountType: 0,
        AppId: sessionToken.appid,
        Service: "accountstatementtrade",
        SelectedOption: { Id: 0, Caption: '', AccountType: 0 }
    };

    $scope.ShowStatement = false;
    $scope.StatementModel = null;

    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountHeads/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.options = data.data.ModeOfAccount;
                if (depositIdForStmt.getDepositId() != 0 && data.data.ModeOfAccount.length != 0) {
                    for (i = 0; i < data.data.ModeOfAccount.length; i++) {
                        console.log(data.data.ModeOfAccount[i].Id == depositIdForStmt.getDepositId());
                        if (data.data.ModeOfAccount[i].Id == depositIdForStmt.getDepositId()) {
                            $scope.request.SelectedOption = { Id: data.data.ModeOfAccount[i].Id, Caption: data.data.ModeOfAccount[i].Caption, AccountType: data.data.ModeOfAccount.accountType };
                            console.log($scope.request.SelectedOption);
                        }
                    }
                }
                var dt = new Date();
                dt.setDate($scope.request.DateFrom.getDate() - data.data.StmtFromDateDiff);
                $scope.request.DateFrom = dt;
                depositIdForStmt.setDepositId(0);
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }

    $scope.RequestStatement = function () {
        $scope.ShowLoader = true;
        $scope.request.DepositId = $scope.request.SelectedOption.Id;
        $scope.request.AccountType = $scope.request.SelectedOption.accountType;
        console.log($scope.request.SelectedOption);
        console.log($scope.request.SelectedOption.Id);
        console.log($scope.request.SelectedOption.AccountType);

        var url = APIUrl + "Service/RequestStatement/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.ShowStatement = data.data.ResponseStatement.StatementDetails.length > 0;
            $scope.StatementModel = data.data.ResponseStatement.StatementDetails;

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }

    $scope.Back = function () {
        $state.go("tradedashboard");
    };

    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.GetInitialData();
}]);

starter.controller("impsdashboardCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    //$scope.ShowLoader = true;
    $scope.IsATrader = false;
    $scope.Username = sessionFactory.getSession().username;
    $scope.request = {
        SessionToken: sessionFactory.getSession().sessiontoken,
        UserId: sessionFactory.getSession().userid,
        DeviceId: sessionFactory.getSession().deviceId,
        MemberId: sessionFactory.getSession().memberid,
        AppId: sessionFactory.getSession().appid,
        TradeCode: null
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    };
    $scope.Redirect = function (type) {
        switch (type) {
            case 'GenMMID': {
                $state.go('generatemmid');
                break;
            }
            case 'CancelMMID': {
                $state.go('cancelmmid');
                break;
            }
            case 'FTP2P': {
                $state.go('p2ptransfer');
                break;
            }
            case 'FTP2A': {
                $state.go('p2atransfer');
                break;
            }
            case 'FTP2U': {
                $state.go('p2utransfer');
                break;
            }
            case 'TopUp': {
                $state.go('topuprecharge');
                break;
            }
            case 'BillPay': {
                $state.go('billpay');
                break;
            }
        }
    };

    $scope.Back = function () {
        $state.go("fundtransfer");
    };

}]);

starter.controller("generatemmidCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.depositId = 0;
    $scope.options = {};
    $scope.Username = sessionFactory.getSession().username;

    var deviceId = $cordovaDevice.getUUID();
    var sessionToken = sessionFactory.getSession();
    $scope.Response = {
        Mmid: '',
        ActCode: '',
        ActCodeDec: '',
        TransRefNo: ''
    }
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        DepositId: 0,
        AccountType: 0,
        AccountNumber: null,
        AppId: sessionToken.appid,
        Service: "generatemmid",
        SelectedOption: { Id: 0, Caption: '', AccountType: 0, AccountNumber: '', ValidationData: '' },
        ReqXml: {
            OriginatingChannel: "Mobile",
            LocalTxnDtTime: new Date(),
            Stan: "123456",
            RemitterMobNo: sessionToken.mobileno,
            RemitterAccNo: null,
            ValidationData: null,
            RemitterName: sessionToken.name,
            InstitutionID: sessionToken.bankid
        }
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetAccountHeads/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data != null) {
                $scope.options = data.data.ModeOfAccount;
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.GenerateMMID = function () {
        $scope.request.AccountType = $scope.request.SelectedOption.AccountType;
        $scope.request.Id = $scope.request.SelectedOption.Id;
        $scope.request.ReqXml.RemitterAccNo = $scope.request.AccountNumber = $scope.request.SelectedOption.AccountNumber;
        $scope.request.ReqXml.ValidationData = $scope.request.SelectedOption.ValidationData;
        if ($scope.request.Id == 0) {
            alert("Please Select an Ac Type");
            return false;
        }

        url = APIUrl + "Service/GenerateMMID/";
        $scope.ShowLoader = true;
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.ResponseCode != "000") {
                alert(data.data.ResponseMessage);
                return false;
            }
            else {
                $scope.Response.Mmid = data.data.XmlData.Mmid;
                $scope.Response.ActCode = data.data.XmlData.ActCode;
                $scope.Response.ActCodeDesc = data.data.XmlData.ActCodeDesc;
                $scope.Response.TransRefNo = data.data.XmlData.TransRefNo;
                alert("Your MMID for A/c No. " + $scope.request.AccountNumber + " is " + $scope.Response.Mmid)
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.CancelMMID = function () {
        $scope.request.Service = "cancelmmid";
        $scope.request.AccountType = $scope.request.SelectedOption.AccountType;
        $scope.request.Id = $scope.request.SelectedOption.Id;
        $scope.request.ReqXml.RemitterAccNo = $scope.request.AccountNumber = $scope.request.SelectedOption.AccountNumber;
        $scope.request.ReqXml.ValidationData = $scope.request.SelectedOption.ValidationData;

        if ($scope.request.Id == 0) {
            alert("Please Select an Ac Number.");
            return false;
        }
        url = APIUrl + "Service/CancelMMID/";
        $scope.ShowLoader = true;
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            if (data.data.ResponseCode != "000") {
                alert(data.data.ResponseMessage);
                return false;
            }
            else {
                $scope.Response.ActCode = data.data.XmlData.ActCode;
                $scope.Response.ActCodeDesc = data.data.XmlData.ActCodeDesc;
                $scope.Response.TransRefNo = data.data.XmlData.TransRefNo;
                alert("Your MMID for A/c No. " + $scope.request.AccountNumber + " is Canceled")
                return false;
            }
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    }
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.Back = function () {
        $state.go("impsdashboard");
    };
    $scope.GetInitialData();

}]);

starter.controller("impsfundtranferCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$ionicPopup", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $ionicPopup, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.FromAccounts = {};
    $scope.Beneficiaries = {};
    $scope.TransferModes = {};
    $scope.IsSentOTP = false;
    $scope.IsReSendOTP = true;
    $scope.PaymentType = "";
    $scope.ReqXml = {
        MessageType: "",
        ProcCode: "",
        OriginatingChannel: "",
        LocalTxnDtTime: "",
        Stan: "",
        RemitterMobNo: "",
        RemitterAccNo: "",
        RemitterName: "",
        BeneMobileNo: "",
        BeneAccNo: "",
        BeneMMID: "",
        BeneIFSC: "",
        BeneAadharNo: "",
        TranAmount: "",
        Remark: "",
        InstitutionID: ""
    };
    $scope.ResXml = {
        //MessageType: "",
        //ProcCode: "",
        //OriginatingChannel: "",
        //LocalTxnDtTime: "",
        //Stan: "",
        //ActCode: "",
        //ActCodeDec: "",
        //BeneName: "",
        //TransRefNo: ""
    };
    var sessionToken = sessionFactory.getSession();
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        AppId: sessionToken.appid,
        Amount: 0.00,
        VerificationMethod: "OTP",
        Service: "",
        Nerration: "",
        Amount: "",
        SelectedBeneficiary: {
            Id: 0,
            BeneficiaryName: "",
            AccountNumber: "",
            ConfirmAccountNumber: "",
            AccountType: 0,
            BankBranch: "",
            BankName: "",
            MaxLimit: 0,
            IFSC: "",
            TransferType: "",
            RequestMode: "",
            BeneficiaryType: "",
            Mmid: "",
            Uid: "",
            ConfirmMmid: "",
            MobileNumber: ""
        },
        SelectedFromAc: {
            id: 0,
            AccountType: 0,
            AccountNumber: ""
        },
        TransferMode: {
            Type: "IMPS"
        },
        ReqXml: {},
        ResXml: {}
    };
    $scope.GetInitialData = function () {
        $scope.ShowLoader = true;
        $scope.request.Service = "ImpsTransInit";
        var url = APIUrl + "Service/GetImpsTransferInitData/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.FromAccounts = data.data.FromAccounts;
            $scope.Beneficiaries = data.data.Beneficiaries;
            $scope.TransferModes = [{ Type: "IMPS" }, { Type: "NEFT" }, { Type: "RTGS" }];
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.SubmitTransfer = function () {
        var url = "";
        $scope.ShowLoader = true;
        if ($scope.request.Service == "p2p") {
            url = APIUrl + "Service/P2PRequest/";
            $scope.ReqXml.ProcCode = "111004";
        }
        else if ($scope.request.Service == "p2a") {
            url = APIUrl + "Service/P2ARequest/";
            $scope.ReqXml.ProcCode = "111009";
        }
        else {
            url = APIUrl + "Service/P2URequest/";
            $scope.ReqXml.ProcCode = "111007";
        }
        $scope.ReqXml.MessageType = "1200";
        $scope.ReqXml.OriginatingChannel = "Mobile";
        $scope.ReqXml.LocalTxnDtTime = new Date();
        $scope.ReqXml.Stan = "123456";
        $scope.ReqXml.RemitterMobNo = sessionToken.mobileno;
        $scope.ReqXml.RemitterAccNo = $scope.request.SelectedFromAc.AccountNumber;
        $scope.ReqXml.RemitterName = sessionToken.name;
        $scope.ReqXml.BeneMobileNo = $scope.request.SelectedBeneficiary.MobileNumber;
        $scope.ReqXml.BeneAccNo = $scope.request.SelectedBeneficiary.AccountNumber;
        $scope.ReqXml.BeneMMID = $scope.request.SelectedBeneficiary.Mmid;
        $scope.ReqXml.BeneIFSC = $scope.request.SelectedBeneficiary.IFSC;
        $scope.ReqXml.BeneAadharNo = $scope.request.SelectedBeneficiary.Uid
        $scope.ReqXml.TranAmount = $scope.request.Amount;
        $scope.ReqXml.Remark = $scope.request.Nerration;
        $scope.ReqXml.InstitutionID = sessionToken.bankid;
        $scope.request.TransferType = $scope.request.TransferMode.Type;
        $scope.request.ReqXml = $scope.ReqXml;
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = false;
            $scope.ResXml = data.data;
            if ($scope.ResXml.XmlData.ActCodeDesc == "Success") {
                alert("Fund Transfer Success. Reference No: " + $scope.ResXml.XmlData.TransRefNo);
            }
            $state.go("impsdashboard");
            $scope.ClearAll();

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.SendOTP = function () {
        if ($scope.request.SelectedFromAc.Id == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.request.SelectedBeneficiary.Id == 0) {
            alert("Invalid Beneficiary");
            return false;
        }
        if ($scope.request.TransferMode.Type == "" && $scope.request.Service == "p2a") {
            alert("Invalid Transfer Mode");
            return false;
        }
        if ($scope.request.Amount == 0) {
            alert("Invalid amount");
            return false;
        }
        //if ($scope.request.OTP.trim() == "") {
        //    alert("Invalid OTP/PIN");
        //    return false;
        //}
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/SendOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.showPopup();
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }
    $scope.SetStatus = function () {
        if ($scope.request.VerificationMethod == "PIN") {
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
            $scope.InputType = "password"
        }
        else if ($scope.request.VerificationMethod == "OTP") {
            $scope.IsSentOTP = false;
            $scope.IsReSendOTP = false;
            $scope.InputType = "text"
        }
    };
    $scope.ClearAll = function () {
        var elements = document.getElementsByTagName("input");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text" || elements[ii].type == "number") {
                elements[ii].value = "";
            }
        }
    };
    $scope.Back = function () {
        $state.go("impsdashboard");
    };
    $scope.showPopup = function () {
        $scope.data = {};
        var myPopup = $ionicPopup.show({
            template: '<input type="password" ng-model="data.otp">',
            title: 'Enter OTP',
            subTitle: 'Please enter OTP received on your mobile.',
            scope: $scope,
            buttons: [
              { text: 'Cancel' },
              {
                  text: '<b>Ok</b>',
                  type: 'button-positive',
                  onTap: function (e) {
                      if (!$scope.data.otp) {
                          e.preventDefault();
                      } else {
                          return $scope.data.otp;
                      }
                  }
              }
            ]
        });

        myPopup.then(function (res) {
            $scope.request.OTP = res;
            $scope.IsSentOTP = true;
            $scope.SubmitTransfer();
        });

        $timeout(function () {
            myPopup.close(); //close the popup after 3 seconds for some reason
        }, 200000);
    };
    $scope.GetInitialData();
}]);

starter.controller("topuprechargeCtrl", ["$scope", "$state", "APIUrl", "sessionFactory", "$http", "$cordovaDevice", function ($scope, $state, APIUrl, sessionFactory, $http, $cordovaDevice) {
    $scope.ShowLoader = false;
    $scope.Username = sessionFactory.getSession().username;
    var deviceId = $cordovaDevice.getUUID();
    $scope.FromAccounts = {};
    $scope.ToAccounts = {};
    $scope.IsSentOTP = false;
    $scope.IsReSendOTP = true;
    $scope.PaymentType = "";
    var sessionToken = sessionFactory.getSession();
    $scope.beneDetails = {
        Name: "",
        Mobile: "",
        AcNo: ""
    }
    $scope.request = {
        SessionToken: sessionToken.sessiontoken,
        UserId: sessionToken.userid,
        DeviceId: deviceId,
        MemberId: sessionToken.memberid,
        AppId: sessionToken.appid,
        Amount: 0.00,
        VerificationMethod: "OTP",
        Service: ""
    };

    $scope.GetInitialData = function () {
        $scope.request.TransferType = 'OtherBank';
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/GetTransferDetails/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.FromAccounts = data.data.FromAccounts;
            $scope.ToAccounts = data.data.ToAccounts;
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.SubmitTransfer = function () {
        $scope.ShowLoader = true;
        $scope.request.TransferType = $scope.PaymentType;
        var url = APIUrl + "Service/SubmitTransfer/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.IsSentOTP = false;
            alert(data.data);
            $state.go("impsdashboard");
            $scope.ClearAll();

        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };

    $scope.SendOTP = function () {
        if ($scope.request.Amount == 0) {
            alert("Invalid amount");
            return false;
        }
        if ($scope.request.TransferType == "" || $scope.request.TransferType == null) {
            alert("Invalid Transfer Type");
            return false;
        }
        if ($scope.request.FromAccount == null) {
            alert("Invalid From Account");
            return false;
        }
        if ($scope.request.ToAccount == null) {
            alert("Invalid Beneficiary Account");
            return false;
        }
        $scope.ShowLoader = true;
        var url = APIUrl + "Service/SendOTP/";
        $http.post(url, $scope.request).then(function (data, status) {
            $scope.ShowLoader = false;
            $scope.showPopup();
        }, function errorcallback(error, status) {
            $scope.ShowLoader = false;
            if (error.status == 401) {
                alert("Unauthorized access. Login again.");
                sessionFactory.setSession(null, null, null, null, null, null, null, null);
                $state.go("login");
                return false;
            }
            alert("An unexpected error occured. Please contact the administrator.");
        });
    };
    $scope.Logout = function () {
        sessionFactory.setSession(null, null, null, null, null, null, null, null);
        $state.go("login");
        return false;
    }

    $scope.SetStatus = function () {
        if ($scope.request.VerificationMethod == "PIN") {
            $scope.IsSentOTP = true;
            $scope.IsReSendOTP = true;
            $scope.InputType = "password"
        }
        else if ($scope.request.VerificationMethod == "OTP") {
            $scope.IsSentOTP = false;
            $scope.IsReSendOTP = false;
            $scope.InputType = "text"
        }
    };
    $scope.ClearAll = function () {
        var elements = document.getElementsByTagName("input");
        for (var ii = 0; ii < elements.length; ii++) {
            if (elements[ii].type == "text" || elements[ii].type == "number") {
                elements[ii].value = "";
            }
        }
    };
    $scope.Back = function () {
        $state.go("fundtransfer");
    };

    $scope.GetInitialData();
}]);

