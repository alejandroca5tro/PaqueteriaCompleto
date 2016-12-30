using System;
using System.Collections.Generic;
using System.Web.Http;
using GestionTareaAero.Models;
using GestionTareaAero.Service;
using System.Net;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using static GestionTareaAero.Models.toDoList;
using System.Text;
using System.Web.Mvc;

namespace GestionTareaAero.Controllers
{
    public class ToDoListController : ApiController
    {
        private readonly ToDoListService service = new ToDoListService();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/GetToDoList/{param}/{Abogado}")]
    public IEnumerable<string[]> GetToDoList(ConjuntoParametros param,string Abogado)
        {
            return service.GetToDoList(param,Abogado);
        }

        [System.Web.Http.Route("~/api/todolist/load")]
        public IEnumerable<string[]> GetCombo()
        {
            return service.GetCombo();
        }

        [System.Web.Http.Route("~/api/todolist/loadAbog")]
        public IEnumerable<string[]> GetAbogado()
        {
            return service.GetAbogado();
        }


        [System.Web.Http.Route("~/api/todolist/getDocumento/{IdDoc}/{Tipo}/{Usuario}")]
       public IEnumerable<byte> ConseguirDocumento(string IdDoc,string Tipo,string Usuario)
        {
           string  documento = service.ConseguirDocumento(IdDoc, Tipo, Usuario);
            byte[] pru = Convert.FromBase64String(documento);
            List<byte> file = new List<byte>();

            foreach(  byte a   in pru)
            {
                file.Add(a);
            }
            return file;
        }

        [System.Web.Http.Route("~/api/todolist/getTipo/{IdDoc}/{Tipo}/{Usuario}")]
        public string ConseguirTipo(string IdDoc, string Tipo, string Usuario)
        {
            string documento = service.ConseguirTipo(IdDoc, Tipo, Usuario);
            documento = documento +","+service.ConvertMimeTypeToExtension(documento);
            return documento;
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/loadTerr")]
        public IEnumerable<string[]> GetTerritorios(ParametroAbog idAbogado)
        {
            return service.GetTerritorios(idAbogado);
        }

        [System.Web.Http.Route("~/api/todolist/user")]
        public string GetUsuario()
        {
            return service.GetUsuario();
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/loadDocumentos")]
        public IEnumerable<string[]> GetDocumentos(ParametroDocumento documento)
        {
            return service.GetDocumentos(documento);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/setReparo/{idCaso}/{tipo}")]
        public JsonTextReader SetReparo(string idCaso, string tipo)
        {
            return service.SetReparo(idCaso,tipo);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/SetAsignarDoc/{Id}/{Documento}/{Nombre}")]
        public void SetAsignarDoc(string Id, string Documento,string Nombre)
        {
            service.SetAsignarDoc(Id, Documento,Nombre);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/setNota/{titulo}/{contenido}/{indice}/{idCaso}")]
        public void SetNota(string titulo, string contenido,string indice,string idCaso)
        {
            service.SetNota(titulo, contenido,indice,idCaso);
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/setDocumento/")]
        public string SetDocumento(ParametroDocumento Documento)
        {
           return  service.SetDocumento(Documento);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/loadNotas")]
        public IEnumerable<string[]> GetNotas(ParametroDocumento CasoId)
        {
            return service.GetNotas(CasoId);
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/loadDetalle/{CasoId}/{Abogado}")]
        public IEnumerable<string[]> GetDetalles(ParametroDocumento CasoId,string Abogado)
        {
            return service.GetDetalles(CasoId,Abogado);
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/BloquearDesbloquearTarea/{CasoId}/{Abogado}/{Bloquear}")]
        public void BloquearDesbloquearTarea(ParametroDocumento CasoId, string Abogado,bool Bloquear)
        {
             service.BloquearDesbloquearTarea(CasoId, Abogado, Bloquear);
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("~/api/todolist/loadHistorico")]
        public IEnumerable<string[]> GetHistorico(ParametroDocumento CasoId)
        {
            return service.GetHistorico(CasoId);
        }
    }
}
