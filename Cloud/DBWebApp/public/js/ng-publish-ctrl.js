angular.module('ng-index-app').controller('ng-publish-release-data-ctrl', function ($scope, $rootScope, $timeout, $http, $location) {       

    // data for root scope
    $rootScope.selectedNavIndex = 2;
    
    $scope.onReleaseDataChanged = function () {
        $scope.isReleaseDataChanged = true;
    }

    $scope.refresh = function() {
        $scope.isRefreshing = true;

        $http.get('/release').success(function (res) {
            $rootScope.originalReleaseContent = res.data;

            // correct the data format
            $rootScope.originalReleaseContent.nextReleaseTime = new Date(changeDateFormat(res.data.nextReleaseTime));
            $rootScope.originalReleaseContent.sellOffTime = new Date(changeDateFormat(res.data.sellOffTime));
            $rootScope.originalReleaseContent.lottery.date = new Date(changeDateFormat(res.data.lottery.date));

            $rootScope.releaseContent = angular.copy($rootScope.originalReleaseContent);
            $scope.isRefreshing = false;
        });
    }

    $scope.resetReleaseData = function () {
        $rootScope.releaseContent = angular.copy($rootScope.originalReleaseContent);
        $scope.isReleaseDataChanged = false;
    };

    $scope.leaveReleaseData = function () {
        if ($scope.isReleaseDataChanged){
            $('#submitReleaseDataModal').modal('show') 
        } else {
            $location.url('/publish/notification');
        }
    }

    $scope.saveReleaseData = function () {
        alert('done');
        $scope.isReleaseDataChanged = false;
        $('#submitReleaseDataModal').modal('hide');
        $('#submitReleaseDataModal').on('hidden.bs.modal', function (e) {
            $location.url('/publish/notification');
        });
    };

    // Get a formated date string from "\/Date(1476720000000+0800)\/".
    function changeDateFormat(jsondate) {     
        jsondate = jsondate.replace("/Date(", "").replace(")/", "");     
        if (jsondate.indexOf("+") > 0) {    
            jsondate = jsondate.substring(0, jsondate.indexOf("+"));     
        }     
        else if (jsondate.indexOf("-") > 0) {    
            jsondate = jsondate.substring(0, jsondate.indexOf("-"));     
        }     
        
        var date = new Date(parseInt(jsondate, 10));   
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;    
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();    
        return date.getFullYear() + "-" + month + "-" + currentDate;    
    }  
});

angular.module('ng-index-app').controller('ng-publish-notification-ctrl', function ($scope, $rootScope, $timeout, $http, $location) {       
    $scope.templateList = [
            {name: '开奖公告', content: '开奖公告'},
            {name: '开奖详情', content: '开奖详情'},
            {name: '下期推荐', content: '下期推荐'}
            ];

    $scope.selectedTemplate = $scope.templateList[0];
    $scope.content = $scope.selectedTemplate.content;

    $scope.notify = function () {
        $http.post('/notify', { platforms: [0,1,2], msg: $scope.content }).then(function SuccessCallback(res) {
            alert(res.data.data);
        }, function errCallback(res) {
            alert(res.data.err);
        });
    }
});