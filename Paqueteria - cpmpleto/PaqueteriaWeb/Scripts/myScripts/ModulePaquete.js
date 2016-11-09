var gestionPaquete = angular.module('gestionPaquete', ['ngRoute']);

gestionPaquete.config(function ($routeProvider) {
    $routeProvider.when('/', {
        templateUrl: '../Content/myHtml/listadoPaquete.html',
        controller: 'listadoPaquete',
    }).when('/paquete/:paqueteId', {
        templateUrl: '../Content/myHtml/formularioVerPaquete.html',
        controller: 'GestionPaqueteController',
    }).otherwise({
        redirectTo: '/',
    });
});
gestionPaquete.factory('factoriaPaquete', function ($http) {
    return {
        list: function (callback) {
            $http.get('/api/paquete').success(callback);
        },
        get: function (id, callback) {
            $http.get('/api/paquete/' + id).success(callback);
        },
    };
});

