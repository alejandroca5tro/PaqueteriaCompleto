           var init = function() {


               $(".enroller").click(onFiltrosContainerClick);
           };



           var onFiltrosContainerClick = function() {

               var newHeight = "0px";

               if ($(this).prev().css("height").slice(0, -2) < 13) {
                   newHeight = "600px";
                   $(this).prev().show();
               }

               $(this).prev().animate({
                   height: newHeight
               }, 200, function() {
                   if (newHeight == "0px") $(this).hide();
               });
           };

           var LimpiarFiltro = function() {
               var filtro = [];
               $("#filterNexpedicion").val("");
               $("#filterPoblacion").val("");
               $("#filterIncidencias").val("");
               $("#filterCautonoma").val("");
               $("#filterCiudad").val("");
               $("#filterPoblacion").val("");
               $("#filterAgencia").val("");






               var $miSelect = $('#filtroEstado');
               $miSelect.val($miSelect.children('option:first').val());
               //$("#filter_Supervisado option:selected").val("-1");
               // $("#filter_Observaciones").val("");

               // $("#filter_Zona").val("");
               //showLoadingWindow();
               // getListData(renderListData, filtro);
           };
           var calendario = function() {
               var meses = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
               var diasShort = ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"];
               $.datepicker.setDefaults({
                   dateFormat: "dd/mm/yy",
                   firstDay: 1,
                   dayNamesMin: diasShort,
                   monthNames: meses
               });
               // $("#txt_fecha").datepicker();
               // $("#txt_fechaRecepcion").datepicker();
               $("#FechaEntrega").datepicker();
               // $("#filter_FechaHasta").datepicker();
           };

           var gestionEnvios = angular.module('gestionEnvios', ['ngRoute', 'ui.bootstrap']);




           gestionEnvios.directive('ngReallyClick', [function() {
               return {
                   restrict: 'A',
                   link: function(scope, element, attrs) {
                       element.bind('click', function() {
                           var message = attrs.ngReallyMessage;
                           if (message && confirm(message)) {
                               scope.$apply(attrs.ngReallyClick);
                           }
                       });
                   }
               }
           }]);
           gestionEnvios.config(function($routeProvider) {
               $routeProvider.when('/', {
                       templateUrl: '/Content/myHTML/listadoEnvios.html',
                       controller: 'listaEnvios',
                   }).when('/envio/create', {
                       templateUrl: '/Content/myHTML/listadoPaquete.html',
                       controller: 'crearEnvio',
                   }).when('/envio/:envioId', {
                       templateUrl: '/Content/myHTML/listadoPaquete.html',
                       controller: 'listadoPaquete'
                   }) //.when('/envio/incidencias/:envioId', {
                   // templateUrl: '/Content/myHTML/Incidencias.html',
                   // controller: 'Incidencias'
                   // })
                   .when('/envio/:envioId/paquete/create', {
                       templateUrl: '/Content/myHTML/Incidencias.html',
                       controller: 'crearPaquete',
                   }).when('/paquete/:paqueteId', {
                       templateUrl: '/Content/myHTML/Incidencias.html',
                       controller: 'GestionPaqueteController',
                   }).when('/paquete/:paqueteId/editar', {
                       templateUrl: '/Content/myHTML/Incidencias.html',
                       controller: 'editarPaquete'
                   }).when('/envio/:envioId/editar', {
                       templateUrl: '/Content/myHTML/listadoPaquete.html',
                       controller: 'editarEnvio'
                   }).otherwise({
                       redirectTo: '/',
                   });
           });
           gestionEnvios.factory('factoria', function($http) {
               return {
                   list: function(callback) {
                       $http.get('/api/envio').success(callback);
                   },
                   get: function(id, callback) {
                       $http.get('/api/envio/' + id).success(callback);
                   },
                   Incidencias: function(id, callback) {
                       $http.get('/api/envio/incidencias/' + id).success(callback);
                   },
                   listPaquete: function(id, callback) {
                       $http.get('/api/envio/' + id + '/paquete').success(callback);
                   },
                   getPaquete: function(id, callback) {
                       $http.get('/api/paquete/' + id).success(callback);
                   },
                   Filtro: function(Filtro, Tipo, callback) {
                       $http.post('/api/envio/filtro/' + Filtro + '/' + Tipo).success(callback);
                   },
                   createEnvio: function(data, callback) {
                       $http.post('/api/envio', data).success(callback);
                   },
                   createPaquete: function(data, callback) {
                       $http.post('/api/paquete', data).success(callback);
                   },
                   borrarenvio: function(id, callback) {
                       $http.delete('/api/envio/' + id).success(callback);
                   },
                   borrarpaquete: function(id, callback) {
                       $http.delete('/api/paquete/' + id).success(callback);
                   },
                   createenvio: function(data, callback) {
                       $http.post('/api/envio/', data).success(callback);
                   },
                   editaenvio: function(id, data, callback) {
                       $http.put('/api/envio/' + id, data).success(callback);
                   },
                   editapaquete: function(id, data, callback) {
                       $http.put('/api/paquete/' + id, data).success(callback);
                   }
               };
           });


           gestionEnvios.controller('listadoPaquete', function($scope, $routeParams, factoria) {
               $scope.form = {
                   EnvioId: true,
                   NBulto: true,
                   FechaEntrega: true,
                   estado: true,
                   tablePaquetes: true,
                   NExpedicion: true,
                   Volumen: true,
                   Kilos: true,
                   Fecha: true,
                   TipoPorte: true,
                   Remitente: true,
                   Observaciones: true,
                   Destinatario: true,
                   Incidencias: true,
                   Ciudad: true,
                   Cautonoma: true,
                   Poblacion: true,
                   Agencia: true,

               };
               $scope.LinkButtons = {
                   volver: true,
                   editar: true,
                   borrar: true,
                   crearEnvio: false,
                   editarEnvio: false,
                   anadirPaquete: false,
                   borrarPaquete: true,
               }
               $scope.detailButtons = {

               };
               $scope.listadoPaquetes = function() {
                   factoria.listPaquete($routeParams.envioId, function(data) {
                       $scope.paquetes = data;
                   });
               }
               $scope.borrarenvio = function(id) {
                   factoria.borrarenvio(id, function(data) {
                       alert("Se a borrado el envio " + data.EnvioId);
                       window.location = "#/";
                   });
               }
               $scope.borrarPaquete = function(id) {
                   factoria.borrarpaquete(id, function(data) {
                       $scope.listadoPaquetes();
                       alert("Se a borrado el paquete " + data.PaqueteId);
                       window.location = "#/envio/" + data.EnvioId;
                   });
               }
               factoria.get($routeParams.envioId, function(data) {
                   $scope.model = data;
               });
               $scope.listadoPaquetes();
           });

           gestionEnvios.controller('listaEnvios', function($scope, $routeParams, factoria) {
               $(".enroller").click(onFiltrosContainerClick);
               //tablePaquetes $("#btnNuevoRegistro .btnSuperior").click(NuevoRegistro);
               $("#btn_limpiarFiltro").click(LimpiarFiltro);

               $scope.PaginadorFiltro = function(envios) {
                   $scope.envio = [];
                   $scope.envios = [];

                   $scope.currentPage = 1;
                   $scope.numPerPage = 3;
                   $scope.maxSize = 5;


                   makeTodosFiltro(envios);



                   ///////

                   function updateFilteredItems() {
                       var begin = (($scope.currentPage - 1) * $scope.numPerPage),
                           end = begin + $scope.numPerPage;

                       $scope.envios = $scope.envio.slice(begin, end);
                   }

                   function makeTodosFiltro(envios) {
                       $scope.envio = [];
                       $scope.envio = envios;

                       // var cl = this;
                       angular.forEach(envios, function(element) {
                           if (element.Estado == 0) {
                               element.Estado = "Pendiente";
                           } else if (element.Estado == 1) {
                               element.Estado = "Entregado";
                           } else if (element.Estado == 2) {
                               element.Estado = "En Reparto";
                           }

                       });
                       $scope.$watch('currentPage + numPerPage', updateFilteredItems);


                   }

               }

               $scope.Paginador = function() {
                   $scope.envio = [];
                   $scope.envios = [];

                   $scope.currentPage = 1;
                   $scope.numPerPage = 3;
                   $scope.maxSize = 5;


                   makeTodos();



                   ///////

                   function updateFilteredItems() {
                       var begin = (($scope.currentPage - 1) * $scope.numPerPage),
                           end = begin + $scope.numPerPage;

                       $scope.envios = $scope.envio.slice(begin, end);
                   }

                   function makeTodos() {
                       $scope.envio = [];
                       factoria.list(function(envios) {
                           $scope.envio = envios;

                           // var cl = this;
                           angular.forEach(envios, function(element) {
                               if (element.Estado == 0) {
                                   element.Estado = "Pendiente";
                               } else if (element.Estado == 1) {
                                   element.Estado = "Entregado";
                               } else if (element.Estado == 2) {
                                   element.Estado = "En Reparto";
                               }
                               if (element.FechaEntrega != null) {
                                              var Prueba=  element.FechaEntrega
                                   element.FechaEntrega =  Prueba.substring(0, 10);
                                   //$("#FechaEntrega").datepicker({dateFormat: 'dd-mm-yy'}).val();
                                 
                               }

                           });
                           $scope.$watch('currentPage + numPerPage', updateFilteredItems);
                       });

                   }

               }

               $scope.RefrescarLista = function() {
                   factoria.list(function(envios) {
                       $scope.envios = envios;
                       // var cl = this;
                       angular.forEach(envios, function(element) {
                           if (element.Estado == 0) {
                               element.Estado = "Pendiente";
                           } else if (element.Estado == 1) {
                               element.Estado = "Entregado";
                           } else if (element.Estado == 2) {
                               element.Estado = "En Reparto";
                           }

                       });
                   });

               }


               // $scope.RefrescarLista();

               $scope.Filtro = function() {
                   var Filtro;
                   var Tipo;
                   if ($("#filterNexpedicion").val().trim() != "") {
                       Filtro = $("#filterNexpedicion").val().trim();
                       Tipo = 'Nexpedicion';
                   } else if ($("#filterAgencia").val().trim() != "") {
                       Filtro = $("#filterAgencia").val().trim();
                       Tipo = 'Agencia';
                   } else if ($("#filterPoblacion").val().trim() != "") {
                       Filtro = $("#filterPoblacion").val().trim();
                       Tipo = 'Poblacion';
                   } else if ($("#filterCautonoma").val().trim() != "") {
                       Filtro = $("#filterCautonoma").val().trim();
                       Tipo = 'Cautonoma';
                   } else if ($("#filterCiudad").val().trim() != "") {
                       Filtro = $("#filterCiudad").val().trim();
                       Tipo = 'Ciudad';
                   } else if ($("#filterIncidencias").val().trim() != "") {
                       Filtro = $("#filterIncidencias").val().trim();
                       Tipo = 'Incidencias';
                   }

                   factoria.Filtro(Filtro, Tipo, function(envios) {

                       //   $scope.envios = $scope.modelPrueba;
                       //   $scope.envios = envios;
                       $scope.PaginadorFiltro(envios);
                       //factoria.get(envios.EnvioId, function (data) {

                       // window.location = "#/envio/" + $scope.model.EnvioId;

                   });
               };

               //$scope.Filtro();
               $scope.Paginador();
               // $scope.PruebaPagina();
               $scope.borrarenvio = function(id) {
                   factoria.borrarenvio(id, function(data) {
                       alert("Se a borrado el Pedido");
                       $scope.Paginador();
                   });
               }

           });
           gestionEnvios.controller('GestionPaqueteController', function($scope, $routeParams, factoria) {
               $scope.form = {
                   IncidenciaId: true,
                   FMercancia: true,
                   RMercancia: true,
                   DMercancia: true,
                   AMercancia: true,
                   NExpedicion: true,
                   EnvioId: true

               };
               $scope.LinkButtons = {
                   volver: true,
                   borrarPaquete: true,
                   crearPaquete: false,
                   editarPaquete: true,
                   anadirPaquete: false,
                   editaPaquete: true,


               }
               $scope.detailButtons = {

               };



               $scope.editpaquete = function() {
                   if ($("#FMercancia").is(':checked')) {
                       $scope.model.FMercancia = 1;
                   } else {
                       $scope.model.FMercancia = 0;
                   }
                   if ($("#RMercancia").is(':checked')) {
                       $scope.model.RMercancia = 1;
                   } else {
                       $scope.model.RMercancia = 0;
                   }
                   if ($("#DMercancia").is(':checked')) {
                       $scope.model.DMercancia = 1;
                   } else {
                       $scope.model.DMercancia = 0;
                   }
                   if ($("#AMercancia").is(':checked')) {
                       $scope.model.AMercancia = 1;
                   } else {
                       $scope.model.AMercancia = 0;
                   }
                   factoria.editapaquete($scope.model.IncidenciaId, $scope.model, function(data) {
                       alert("La incidencia se ha modificado correctamente");
                       window.location = "#/paquete/" + $scope.model.IncidenciaId;
                   })
               }
               $scope.agregarIncidencia = function(dato) {
                   if (dato == true) {
                       factoria.get($scope.model.EnvioId, function(data) {
                           $scope.modelIncidencia = data;
                           $scope.modelIncidencia.Incidencias = '';
                           factoria.editaenvio($scope.model.EnvioId, $scope.modelIncidencia, function(data) {
                               // alert("El envio " + $scope.model.EnvioId + " se ha modificado correctamente");
                               // window.location = "#/envio/" + $scope.model.EnvioId;
                           })
                       });
                   }

               }
               $scope.getpaquete = function() {
                   factoria.getPaquete($routeParams.paqueteId, function(data) {
                       $scope.model = data;
                       // angular.forEach(data, function (element) {
                       if (data.FMercancia == 0) {

                           $("#FMercancia").attr('checked', false);
                       } else {
                           $("#FMercancia").attr('checked', true);
                       }
                       if (data.RMercancia == 0) {
                           $("#RMercancia").attr('checked', false);
                       } else {
                           $("#RMercancia").attr('checked', true);
                       }

                       if (data.DMercancia == 0) {
                           $("#DMercancia").attr('checked', false);
                       } else {
                           $("#DMercancia").attr('checked', true);
                       }
                       if (data.AMercancia == 0) {
                           $("#AMercancia").attr('checked', false);
                       } else {
                           $("#AMercancia").attr('checked', true);
                       }


                   });
               }
               $scope.getpaquete();
               $scope.borrarPaquete = function(id) {
                   factoria.borrarpaquete(id, function(data) {

                       alert("Se a borrado la Incidencia ");
                       $scope.agregarIncidencia(true);
                       window.location = "#/envio/" + data.EnvioId;
                   });

               }

           });
           gestionEnvios.controller('crearPaquete', function($scope, $routeParams, factoria) {
               $scope.form = {
                   IncidenciaId: '',
                   FMercancia: '',
                   RMercancia: '',
                   DMercancia: '',
                   AMercancia: '',
                   NExpedicion: '',

               };
               $scope.LinkButtons = {
                   volver: true,
                   editar: false,
                   borrar: false,
                   crearEnvio: false,
                   editarEnvio: false,
                   anadirPaquete: true,
                   borrarPaquete: false,
                   crearIncidencias: true,


               }
               $scope.detailButtons = {

               };

               $scope.modelIncidencia = {
                   EnvioId: '',
                   DestinatarioId: '',
                   FechaEntrega: '',
                   Estado: '',
                   TipoPortes: '',
                   Incidencias: '',
                   NExpedicion: '',
                   Volumen: '',
                   NBulto: '',
                   TipoPorte: ''


               }
               $scope.agregarIncidencia = function(dato) {
                   if (dato == true) {
                       factoria.get($scope.model.EnvioId, function(data) {
                           $scope.modelIncidencia = data;
                           $scope.modelIncidencia.Incidencias = 'SI';
                           factoria.editaenvio($scope.model.EnvioId, $scope.modelIncidencia, function(data) {
                               // alert("El envio " + $scope.model.EnvioId + " se ha modificado correctamente");
                               // window.location = "#/envio/" + $scope.model.EnvioId;
                           })
                       });
                   }

               }
               $scope.model = {
                   IncidenciaId: '',
                   FMercancia: '',
                   RMercancia: '',
                   DMercancia: '',
                   AMercancia: '',
                   NExpedicion: '',
                   EnvioId: $routeParams.envioId,
               }
               $scope.datosPaquetes = {
                   IncidenciaId: '',
                   FMercancia: '',
                   RMercancia: '',
                   DMercancia: '',
                   AMercancia: '',
                   NExpedicion: '',
                   EnvioId: $routeParams.envioId,
               }
               $scope.createpaquete = function(alta) {
                   // var datosPaquetes = {};
                   factoria.listPaquete($routeParams.envioId, function(data) {
                       datosPaquetes = data;

                       if (datosPaquetes[0] != null) {

                           window.location = "#/paquete/" + datosPaquetes[0].IncidenciaId;
                       } else if (alta == true) {
                           if ($("#FMercancia").is(':checked')) {
                               $scope.model.FMercancia = 1;
                           }
                           if ($("#RMercancia").is(':checked')) {
                               $scope.model.RMercancia = 1;
                           }
                           if ($("#DMercancia").is(':checked')) {
                               $scope.model.DMercancia = 1;
                           }
                           if ($("#AMercancia").is(':checked')) {
                               $scope.model.AMercancia = 1;
                           }
                           factoria.createPaquete($scope.model, function(data) {
                               window.location = "#/paquete/" + data.IncidenciaId;
                               $scope.agregarIncidencia(true);
                           });
                       }
                   });
               }
               $scope.createpaquete(false);
           });



           gestionEnvios.controller('editarEnvio', function($scope, $routeParams, factoria) {
               factoria.get($routeParams.envioId, function(data) {
                   $scope.model = data;
               });
               $scope.form = {
                   envioId: true,
                   destinatarioId: false,
                   fechaEntrega: false,
                   estado: false,
                   tablePaquetes: false,
                   Nexpedicion: false,
                   Volumen: false,
                   Kilos: false,
                   Fecha: false,
                   TipoPortes: false,
                   NBulto: false,
                   Incidencias: false
               };
               $scope.LinkButtons = {
                   volver: true,
                   editar: false,
                   borrar: false,
                   crearEnvio: false,
                   editarEnvio: true,
                   anadirPaquete: false,

               }
               $scope.model = {
                   EnvioId: '',
                   DestinatarioId: '',
                   FechaEntrega: '',
                   Estado: '',
                   TipoPortes: '',
                   Incidencias: '',
                   NExpedicion: '',
                   Volumen: '',
                   NBulto: '',
                   TipoPorte: ''


               }
               $scope.modelIncidencia = {
                   EnvioId: '',
                   DestinatarioId: '',
                   FechaEntrega: '',
                   Estado: '',
                   TipoPortes: '',
                   Incidencias: 'SI',
                   NExpedicion: '',
                   Volumen: '',
                   NBulto: '',
                   TipoPorte: ''


               }
               $scope.editEnvio = function(dato) {
                   if (dato == true) {

                       factoria.editaenvio($scope.model.EnvioId, $scope.modelIncidencia, function(data) {
                           //alert("El envio " + $scope.model.EnvioId + " se ha modificado correctamente");
                           window.location = "#/envio/" + $scope.model.EnvioId;
                       })
                   } else {
                       factoria.editaenvio($scope.model.EnvioId, $scope.model, function(data) {
                           alert("El pedido  se ha modificado correctamente");
                           window.location = "#/envio/" + $scope.model.EnvioId;
                       })
                   }
               }
           });

           gestionEnvios.controller('editarPaquete', function($scope, $routeParams, factoria) {
               factoria.getPaquete($routeParams.paqueteId, function(data) {
                   $scope.model = data;
               });
               $scope.form = {
                   FMercancia: true,
                   RMercancia: false,
                   Peso: false,

               };
               $scope.LinkButtons = {
                   volver: true,
                   borrarPaquete: false,
                   crearPaquete: false,
                   editarPaquete: false,
                   editaPaquete: true,
                   anadirPaquete: false,

               }

               $scope.model = {
                   IncidenciaId: '',
                   FMercancia: '',
                   RMercancia: '',
                   DMercancia: '',
                   AMercancia: '',
                   NExpedicion: '',
                   EnvioId: $routeParams.envioId,
               }


               $scope.editpaquete = function() {
                   factoria.editapaquete($scope.model.IncidenciaId, $scope.model, function(data) {
                       alert("El Paquete " + $scope.model.PaqueteId + " se ha modificado correctamente");
                       window.location = "#/paquete/" + $scope.model.PaqueteId;
                   })
               }
           });
           gestionEnvios.controller('crearEnvio', function($scope, $routeParams, factoria) {
               $("#FechaEntrega").on("focus", calendario);
               $("#FechaEntrega").click(calendario);
               $scope.form = {
                   EnvioId: false,
                   destinatarioId: false,
                   FechaEntrega: false,
                   estado: false,
                   tablePaquetes: false,
                   Nexpedicion: false,
                   Volumen: false,
                   Kilos: false,
                   Fecha: false,
                   TipoPorte: false,
                   NBulto: false,
                   Incidencias: false
               };
               $scope.LinkButtons = {
                   volver: true,
                   editar: false,
                   borrar: false,
                   crearEnvio: true,
                   editarEnvio: false,
                   anadirPaquete: false,

               }
               $scope.detailButtons = {

               };
               var FechaPrueba = $("#FechaEntrega").val().trim() + " 00:00:00";
               $scope.model = {
                   EnvioId: '',
                   DestinatarioId: '',
                   FechaEntrega: FechaPrueba,
                   Estado: '',
                   TipoPortes: '',
                   Incidencias: '',
                   NExpedicion: '',
                   Volumen: '',
                   NBulto: '',
                   TipoPorte: ''
               }

               $scope.createenvio = function() {
                   factoria.createEnvio($scope.model, function(data) {
                       window.location = "#/envio/" + data.EnvioId;
                   });
               }
           });
