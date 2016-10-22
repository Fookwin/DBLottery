angular.module('ng-index-app').controller('ng-publish-release-data-ctrl', function ($scope, $rootScope, $http, $location, util) {       

    // data for root scope
    $rootScope.selectedNavIndex = 2;

    $scope.syncToCloud = function() {
        if ($scope.isSyncingToCloud)
            return;
            
        $scope.isSyncingToCloud = true;

        util.syncReleaseDateToCloud(function () {
            $scope.isSyncingToCloud = false;
        });
    }

    if (!$rootScope.originalReleaseContent) {
        // initialize the original data from cloud.
        $scope.syncToCloud();
    }
    
    $scope.onReleaseDataChanged = function () {
        $scope.isReleaseDataChanged = true;
    }

    $scope.syncToOffical = function() {
        if ($scope.isSyncingToOffical)
            return;

        $scope.isSyncingToOffical = true;

        $http.get('/offical/?issue=' + $rootScope.originalReleaseContent.lottery.issue).success(function (res) {
            
            if (res.data) {
                $rootScope.releaseContent.lottery = res.data;

                // correct the data format
                $rootScope.releaseContent.lottery.date = new Date(util.correctMSDateString(res.data.date));
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
                $rootScope.releaseContent.lottery.date = new Date(util.correctMSDateString(res.data.date));
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
        
        var nums = util.getRandomNumbers(8, 33);
        $rootScope.releaseContent.recommendation.redIncludes = nums.slice(0, 2).sort(function (a, b) { return a > b; });
        $rootScope.releaseContent.recommendation.redExcludes = nums.slice(2).sort(function (a, b) { return a > b; });
        $scope.isReleaseDataChanged = true;
    }

    $scope.RandomBlues = function() {
        
        var nums = util.getRandomNumbers(4, 16);
        $rootScope.releaseContent.recommendation.blueIncludes = nums.slice(0, 1).sort(function (a, b) { return a > b; });
        $rootScope.releaseContent.recommendation.blueExcludes = nums.slice(1).sort(function (a, b) { return a > b; });
        $scope.isReleaseDataChanged = true;
    }
});

angular.module('ng-index-app').controller('ng-publish-notification-ctrl', function ($scope, $rootScope, $http, $location, util) {       
    $rootScope.selectedNavIndex = 2;

    // call init to make sure the content is there.
    if (!$rootScope.releaseContent) {
        util.syncReleaseDateToCloud(function () {
            init();
        });
    } else {
        init();
    }

    function init() {
        var releaseLot = $rootScope.releaseContent.lottery;

        $scope.templateList = [
                {
                    name: '开奖公告', 
                    notification: {
                        title: "第" + releaseLot.issue + "期" + " 开奖啦！", 
                        message: "红: " + releaseLot.scheme.slice(0, -3) + " 蓝: " + releaseLot.scheme.slice(-2)
                    }
                } ,
                {
                    name: '开奖详情', 
                    notification: {
                        title: "一等奖中出 " + releaseLot.bonus[0] + " 注 " + util.getMoneyFormat(releaseLot.bonus[1]), 
                        message: "奖池 " + util.getMoneyFormat(releaseLot.pool) + ", 够开出 " + Math.ceil(releaseLot.pool / 5000000) + " 个五百万！"
                    }
                },
                {
                    name: '下期推荐', 
                    notification: {
                        title: "一等奖中出 " + releaseLot.bonus[0] + " 注 " + util.getMoneyFormat(releaseLot.bonus[1]), 
                        message: "奖池 " + util.getMoneyFormat(releaseLot.pool) + ", 够开出 " + Math.ceil(releaseLot.pool / 5000000) + " 个五百万！"
                    }
                }
        ];

        $scope.selectedTemplate = $scope.templateList[0];
        $scope.notification = $scope.selectedTemplate.notification;

        $scope.notify = function () {
            $http.post('/notify', { platforms: [0,1,2], msg: $scope.notification }).then(function SuccessCallback(res) {
                alert(res.data.data);
            }, function errCallback(res) {
                alert(res.data.err);
            });
        }
    }    
});