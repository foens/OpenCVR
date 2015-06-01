'use strict';

angular.module('openCvrApp')
  .controller('MainCtrl', function ($scope, $http) {
    var self = this;
	self.search = '';
	self.command = '';
	self.output = '';
	
	function canonicalize(url) {
      var div = document.createElement('div');
      div.innerHTML = '<a></a>';
      div.firstChild.href = url; // Ensures that the href is properly escaped
      div.innerHTML = div.innerHTML; // Run the current innerHTML back through the parser
      return div.firstChild.href;
    }
	
	self.performSearch = function() {
	  var url = '/api/v1/search/' + encodeURIComponent(self.search);
	  self.command = 'wget ' + decodeURIComponent(canonicalize(url));
      $http({
        url: url,
        method: 'GET'
      }).success(function(response){
        self.output = response;
		
      });
  };
});
