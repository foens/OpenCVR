'use strict';

angular.module('openCvrApp')
  .controller('MainCtrl', function ($scope, $http) {
    this.search = '';
	this.output = '';
	
	$scope.$watch('this.search',function(){
    $http({
        url: '/api/search',
        method: 'GET',
        param: {
            q: this.search
        }
    }).success(function(response){
        console.log(response);
     });
    });
});
