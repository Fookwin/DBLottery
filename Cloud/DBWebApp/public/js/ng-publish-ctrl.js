angular.module('ng-index-app').controller('ng-publish-release-data-ctrl', function ($scope, $rootScope, $timeout, $http, $location) {       

    // data for root scope
    $rootScope.selectedNavIndex = 2;

    if (!$rootScope.originalReleaseContent) {
        // initialize the original data from cloud.
        _syncToCloud();
    }
    
    $scope.onReleaseDataChanged = function () {
        $scope.isReleaseDataChanged = true;
    }

    $scope.syncToCloud = function() {
        _syncToCloud();
    }

    $scope.syncToOffical = function() {
        if ($scope.isSyncingToOffical)
            return;

        $scope.isSyncingToOffical = true;

        $http.get('/offical/?issue=' + $rootScope.originalReleaseContent.lottery.issue).success(function (res) {
            
            if (res.data) {
                $rootScope.releaseContent.lottery = res.data;

                // correct the data format
                $rootScope.releaseContent.lottery.date = new Date(changeDateFormat(res.data.date));
                $scope.isReleaseDataChanged = true;
            }

            $scope.isSyncingToOffical = false;
        });
    }

    $scope.addNewRelease = function () {
        if ($scope.isAddingNew)
            return;

        $scope.isAddingNew = true;

        $rootScope.releaseContent = {
            lottery: {
                issue: $rootScope.originalReleaseContent.next.issue,
                date: $rootScope.originalReleaseContent.next.date,
                scheme:'xx xx xx xx xx xx+xx',
                pool:0,
                bet:0,
                bonus: [
                    0, 0, 0, 0, 0, 3000, 0, 200, 0, 10, 0, 5
                ],
                details: '',
            },
            next: {},
            recommendation: {
                redExcludes: [],
                redIncludes: [],
                blueIncludes: [],
                blueExcludes: []
            },
            dataVersion: angular.copy($rootScope.originalReleaseContent.dataVersion)
        }

        // apply radom recommendation
        $scope.RandomReds();
        $scope.RandomBlues();

        $http.get('/offical/?issue=' + $rootScope.originalReleaseContent.lottery.issue).success(function (res) {
            
            if (res.data) {
                $rootScope.releaseContent.lottery = res.data;

                // correct the data format
                $rootScope.releaseContent.lottery.date = new Date(changeDateFormat(res.data.date));
            }

            $scope.isAddingNew = false;
        });

        $scope.isReleaseDataChanged = true;
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

    $scope.RandomReds = function() {
        
        var nums = [],
            count = 8;
        while (nums.length < count) {
            var rN = random(33);
            if (!nums.find(function (num) { return num === rN } )) {
                nums.push(rN);
            }
        }

        $rootScope.releaseContent.recommendation.redIncludes = nums.slice(0, 2).sort(function (a, b) { return a > b; });
        $rootScope.releaseContent.recommendation.redExcludes = nums.slice(2).sort(function (a, b) { return a > b; });
        $scope.isReleaseDataChanged = true;
    }

    $scope.RandomBlues = function() {
        
        var nums = [],
            count = 4;
        while (nums.length < count) {
            var rN = random(16);
            if (!nums.find(function (num) { return num === rN } )) {
                nums.push(rN);
            }
        }

        $rootScope.releaseContent.recommendation.blueIncludes = nums.slice(0, 1).sort(function (a, b) { return a > b; });
        $rootScope.releaseContent.recommendation.blueExcludes = nums.slice(1).sort(function (a, b) { return a > b; });
        $scope.isReleaseDataChanged = true;
    }

    function random(max) {
        return Math.ceil(Math.ceil(Math.random() * max * 100) / 100);
    }

    function _syncToCloud() {
        if ($scope.isSyncingToCloud)
            return;
            
        $scope.isSyncingToCloud = true;

        $http.get('/release').success(function (res) {
            $rootScope.originalReleaseContent = res.data;

            // correct the data format
            $rootScope.originalReleaseContent.next.date = new Date(changeDateFormat(res.data.next.cutOffTime));
            $rootScope.originalReleaseContent.next.cutOffTime = new Date(changeDateFormat(res.data.next.cutOffTime));
            $rootScope.originalReleaseContent.next.lotteryTime = new Date(changeDateFormat(res.data.next.lotteryTime));
            $rootScope.originalReleaseContent.lottery.date = new Date(changeDateFormat(res.data.lottery.date));

            $rootScope.releaseContent = angular.copy($rootScope.originalReleaseContent);
            $scope.isSyncingToCloud = false;
        });
    }

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