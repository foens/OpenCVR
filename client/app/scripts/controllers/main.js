'use strict';

angular.module('openCvrApp')
  .controller('MainCtrl', function ($scope, $http) {
    var self = this;
	self.search = '';
	self.command = '';
	self.output = '';
	
	self.performSearch = function() {
      $http({
        url: '/api/1/search',
        method: 'GET',
        params: {
            q: self.search
        }
      }).success(function(response){
        self.output = response;
		self.command = 'wget http://opencvr.dk/api/1/search/' + self.search;
      });
  };
});
