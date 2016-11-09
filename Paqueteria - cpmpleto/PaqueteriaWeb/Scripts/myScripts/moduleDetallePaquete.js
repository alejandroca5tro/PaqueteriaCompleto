var gestionaDetallePaquete = angular.module('gestionaDetallePaquete', ['ngRoute']);
gestionaDetallePaquete.config(function ($routeProvider) {
    $routeProvider.when('/', function () {

    }).when('/:paqueteId', function () {
    }).otherwise(function () {
    })
});
gestionaDetallePaquete.factory('factoriaDetalles', function ($http) {
    return {
        list: function (callback) {
            $http.get('/api/paquete').success(callback);
        },
        get: function (id, callback) {
            $http.get('/api/paquete/' + id).success(callback);
        },
    };
});

gestionaDetallePaquete.controller('ControllerDetalles', function ($scope, factoriaDetalles) {
    factoriaDetalles.list(function (data) {
        $scope.detalles = data;
    });
});

gestionaDetallePaquete.controller('GestionDetallesController', function ($scope,$routeParams, factoriaDetalles) {

    factoriaDetalles.get($routeParams.paqueteId, function (data) {
        $scope.model = data;
    })


});

