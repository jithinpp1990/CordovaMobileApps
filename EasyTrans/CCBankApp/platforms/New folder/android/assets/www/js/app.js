// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
var starter = angular.module('starter', ['ionic', 'ngCordova'])

.run(function ($ionicPlatform, $rootScope) {
    $ionicPlatform.ready(function () {
        if (cordova.platformId === 'ios' && window.cordova && window.cordova.plugins.Keyboard) {
            // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
            // for form inputs)
            cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);

            // Don't remove this line unless you know what you are doing. It stops the viewport
            // from snapping when text inputs are focused. Ionic handles this internally for
            // a much nicer keyboard experience.
            cordova.plugins.Keyboard.disableScroll(true);
            $rootScope.$apply(function () {
                $rootScope.test = "CC";
            });

        }

        if (window.StatusBar) {
            StatusBar.styleDefault();
        }
    });
})
.factory('sessionFactory', function () {
    var session = {
        username: '',
        sessiontoken: '',
        userid: '',
        memberid: '',
        appid:''
    };
    return {
        setSession: function (username, token, userid, memberid,appid) {
            session.username = username;
            session.sessiontoken = token;
            session.userid = userid;
            session.memberid = memberid;
            session.appid = appid;
            return session;
        },
        getSession: function () {
            return session;
        }
    }
})
.factory('depositIdForStmt', function () {
    var depositId = 0;
    return {
        setDepositId: function (depositid) {
            depositId = depositid;
        },
        getDepositId: function () {
            return depositId;
        }
    }
})
    .factory('userStatus', function () {
        var userStatus = null;
        return {
            setUserStatus: function (userstatus) {
                userStatus = userstatus;
            },
            getUserStatus: function () {
                return userStatus;
            }
        }
    })
.factory('beneficiatryIdForEdit', function () {
    var beneficiaryId = 0;
    return {
        setBeneficiaryId: function (beneficiaryid) {
            beneficiaryId = beneficiaryid;
        },
        getBeneficiaryId: function () {
            return beneficiaryId;
        }
    }
});
