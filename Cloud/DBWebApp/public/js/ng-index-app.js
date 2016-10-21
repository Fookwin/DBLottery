var app = angular.module('ng-index-app', ['ngRoute']);

app.config(['$routeProvider', function($routeProvider) {
    $routeProvider
        .when('/', {templateUrl: '/templates/home.html'})
        .when('/users', {templateUrl: '/templates/user-picker-view.html'})
        .when('/publish', {redirectTo:'/publish/release'})
        .when('/publish/release', {templateUrl: '/templates/publish-release-data-view.html'})
        .when('/publish/notification', {templateUrl: '/templates/publish-notification-view.html'})
        .otherwise({redirectTo:'/'});
}]);

app.directive("ngGeneralHeaderDirective", function () {
    return {
        restrict : 'EAC',
        controller: 'ng-index-header-ctrl',
        templateUrl: '/templates/header.html'
    };
});

app.controller('ng-index-header-ctrl', function ($scope, $rootScope, $interval) {
   $rootScope.title = 'Lottery Data Management';
   $rootScope.selectedNavIndex = -1;

   $interval(function(){
       $rootScope.time = new Date().toLocaleString();   
   }, 1000);

   $scope.navButtons = [
        { title: 'HOME', href: '#/' },
        { title: 'USERS', href: '#/users' },
        { title: 'PUBLISH', href: '#/publish' },
   ];

   $scope.navigateTo = function (index) {
       $scope.selectedNavIndex = index;
   };
});

app.controller('ng-index-footer-ctrl', function ($scope, $http) {
    $http.get('/templates/footer.html').then(function(res) {
         $scope.footer = res.data;
    });
});

app.directive("ngUserPickerView", function () {
    return {
        restrict : 'EAC',
        controller: 'ng-user-picker-ctrl',
        templateUrl: '/templates/user-picker-view.html'
    };
});

