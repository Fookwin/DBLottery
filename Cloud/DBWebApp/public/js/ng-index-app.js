var app = angular.module('ng-index-app', ['ngRoute']);

app.config(['$routeProvider', function($routeProvider) {
    $routeProvider
        .when('/', {template: 'not supported'})
        .when('/users', {templateUrl: '/templates/user-manage.html'})
        .when('/release', {templateUrl: '/templates/release-manage.html'})
        .otherwise({redirectTo:'/'});
}]);

app.directive("ngGeneralHeaderDirective", function () {
    return {
        restrict : 'EAC',
        controller: 'ng-index-header-ctrl',
        templateUrl: '/templates/header.html'
    };
});

app.controller('ng-index-header-ctrl', function ($scope, $interval) {
   $scope.title = 'Lottery Data Management';
   $interval(function(){
       $scope.time = new Date().toLocaleString();   
   }, 1000);

   $scope.navButtons = [
        { title: 'HOME', href: '#/' },
        { title: 'USERS', href: '#/users' },
        { title: 'PUBLISH', href: '#/release' },
   ];

   $scope.selectedNavIndex = 0;
   $scope.navigateTo = function (index) {
       $scope.selectedNavIndex = index;
   };
});

// app.service('userNameExtractor', function() {
//     this.extract = function(address) {
//         return address.substring(0, address.indexOf("@"));
//     }
// });

// app.filter('addressFilter', ['userNameExtractor', function (userNameExtractor) {
//     return function(address) {
//         return userNameExtractor.extract(address);
//     };
// }]);

app.controller('ng-index-footer-ctrl', function ($scope, $http) {
    $http.get('/templates/footer.html').then(function(res) {
         $scope.footer = res.data;
    });
});

