using GestionTareaAero.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using static GestionTareaAero.Models.toDoList;

namespace GestionTareaAero.Service
{
    public class ToDoListService : IToDoList
    {
        private static ConcurrentDictionary<string, string> MimeTypeToExtension = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> ExtensionToMimeType = new ConcurrentDictionary<string, string>();
        private WebConfig WC = new WebConfig();
        private const string DATA = @"{""object"":{""name"":""Name""}}";

        public IEnumerable<string[]> GetToDoList(ConjuntoParametros param, string Abogado)
        {
            if (param.Parametros.Count > 0 && param.Parametros[0].concepto != null && param.Parametros[0].concepto == "")
            {
                param = null;
            }
            List<string[]> miLista = new List<string[]>();
            List<Item[]> a = new List<Item[]>();
            a = Conexion(param, Abogado);
            if (a != null)
            {
                foreach (Item[] b in a)
                {
                    if (b.Length > 0)
                    {
                        toDoList tarea = new toDoList();
                        tarea = EvaluarItems(b);
                        tarea = limpiar(tarea);
                        miLista.Add(ConvertirToDoList(tarea));
                    }
                }
            }
            return miLista;
        }

        public string GetUsuario()
        {
            return WC.Logado;
        }

        public IEnumerable<string[]> GetCombo()
        {
            List<ItemCombo> a = new List<ItemCombo>();
            List<string[]> miCombo = new List<string[]>();
            List<string> miComboDes = new List<string>();
            List<string> miComboVal = new List<string>();
            a = ConexionCombo();

            foreach (ItemCombo b in a)
            {
                miComboDes.Add(b.descripcion);
            }
            foreach (ItemCombo b in a)
            {
                miComboVal.Add(b.valor);
            }
            miCombo.Add(miComboDes.ToArray());
            miCombo.Add(miComboVal.ToArray());

            return miCombo;
        }
        public void BloquearDesbloquearTarea(ParametroDocumento param, string Abogado, bool Bloquear)
        {
            if (Bloquear == false)
            {
                Abogado = "";
            }
            string formato = "{%22conceptoValor%22:{%22concepto%22:%22" + WC.BloqueoDesbloqueoTarea + "%22,%20%22descripcion%22:%22" + Abogado + "%22,%20%22indice%22:%22" + "0" + "%22}}";
            string URL = WC.urlWS + "/F_GESPDM_GestValorClaveExt_C/1.0?requestData={%22gestionarValoresListaConcepCExt%22:{%22entrada%22:{%22empresa%22:%22" + WC.Empresa + "%22,%20%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%20%22claveExterna%22:%22" + param.CasoId + "%22,%20%22listaValoresConcepto%22:[" + formato + "]}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));


        }
        public IEnumerable<string[]> GetDetalles(ParametroDocumento param, string Abogado)
        {
            List<Item[]> a = new List<Item[]>();
            List<string[]> miLista = new List<string[]>();
            a = ConexionDetalle(param.CasoId, Abogado);

            if (a != null)
            {
                foreach (Item[] b in a)
                {
                    if (b.Length > 0)
                    {
                        toDoList tarea = new toDoList();
                        tarea = EvaluarItems(b);
                        tarea = limpiar(tarea);
                        miLista.Add(ConvertirToDoListDetalle(tarea));
                    }
                }
            }
            return miLista;
        }

        public string SetDocumento(ParametroDocumento Documento)
        {
            XmlDocument doc = new XmlDocument();
            string xml = WC.WSSubir;
            xml = xml.Replace("[USERNAME]", WC.usuarioServidor);
            xml = xml.Replace("[PASSWORD]", WC.passServidor);
            xml = xml.Replace("[DOCUMENTO]", Documento.Documento);
            xml = xml.Replace("[CASOID]", Documento.CasoId);
            xml = xml.Replace("[TIPODOC]", WC.TipoDocumental);
            xml = xml.Replace("[MIME]", Documento.Tipo);
            xml = xml.Replace("[SGL]", Documento.SGL);
            xml = xml.Replace("[RUT]", Documento.Rut);
            xml = xml.Replace("[IDSERVIDOR]", WC.IDServidor);
            xml = xml.Replace("[FECHAACTUAL]", Math.Truncate((DateTime.Now - new DateTime(1970, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)).TotalMilliseconds).ToString());
            xml = xml.Replace("[NOMBRESINEXTENSION]", Documento.Nombre);
            xml = xml.Replace("[FECHAACTUALMAS]", Math.Truncate(((DateTime.Now.AddYears(Convert.ToInt32(WC.YearM)) - new DateTime(1970, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))).TotalMilliseconds).ToString());
            doc.LoadXml(xml);
            XmlDocument salida = LLamarServicio(doc
           , WC.urlWSDoc, "POST");
            if (salida != null)
            {
                XmlNodeList xnList = salida.SelectNodes("//" + "gdId");
                foreach (XmlNode xNode in xnList)
                {
                    return xNode.InnerText;
                }
            }
            return null;
        }

        public IEnumerable<string[]> GetDocumentos(ParametroDocumento documento)
        {
            List<ItemDocumento> a = new List<ItemDocumento>();
            List<string[]> documentos = new List<string[]>();
            a = ConexionDocumentos(documento.CasoId);
            foreach (ItemDocumento Cada in a)
            {
                Documento Eldocumento = new Documento();

                Eldocumento.Id = Cada.idDocumento;
                Eldocumento.Descripcion = Cada.descTipo;
                Eldocumento.Titulo = Cada.titulo.Trim();
                Eldocumento.Fecha = Cada.fecha;
                Eldocumento.Usuario = Cada.usuario;
                documentos.Add(ConvertirADocumentos(Eldocumento));
            }
            return documentos;
        }

        public string ConseguirDocumento(string IdDoc, string Tipo, string Usuario)
        {
            XmlDocument doc = new XmlDocument();
            string xml = WC.WS;
            xml = xml.Replace("[USERNAME]", WC.usuarioServidor);
            xml = xml.Replace("[PASSWORD]", WC.passServidor);
            xml = xml.Replace("[IDDOCRECUPERAR]", IdDoc);
            xml = xml.Replace("[TIPODOC]", Tipo);
            xml = xml.Replace("[IDSERVIDOR]", WC.IDServidor);
            doc.LoadXml(xml);
            XmlDocument salida = LLamarServicio(doc
, WC.urlWSDoc, "POST");
            if (salida != null)
            {
                XmlNodeList xnList = salida.SelectNodes("//" + "documento");
                foreach (XmlNode xNode in xnList)
                {
                    return xNode.InnerText;
                }
            }
            return null;
        }

        public string ConseguirTipo(string IdDoc, string Tipo, string Usuario)
        {
            XmlDocument doc = new XmlDocument();
            string xml = WC.WS;
            xml = xml.Replace("[USERNAME]", WC.usuarioServidor);
            xml = xml.Replace("[PASSWORD]", WC.passServidor);
            xml = xml.Replace("[IDDOCRECUPERAR]", IdDoc);
            xml = xml.Replace("[TIPODOC]", Tipo);
            xml = xml.Replace("[IDSERVIDOR]", WC.IDServidor);
            doc.LoadXml(xml);
            XmlDocument salida = LLamarServicio(doc
, WC.urlWSDoc, "POST");
            if (salida != null)
            {
                XmlNodeList xnList = salida.SelectNodes("//" + "tipoMime");
                foreach (XmlNode xNode in xnList)
                {
                    return xNode.InnerText;
                }
            }
            return null;
        }

        private XmlDocument LLamarServicio(XmlDocument entrada, string uri, string metodo)
        {
            WebRequest request = null;
            Stream requestStream = null;
            Stream responseStream = null;
            StreamReader reader = null;
            XmlDocument salida = null;

            try
            {
                request = WebRequest.Create(uri);
                request.Method = metodo;
                byte[] bytes = Encoding.UTF8.GetBytes(entrada.InnerXml);
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml; charset=utf-8";
                //request.Proxy = WebRequest.GetSystemWebProxy();
                requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Flush();
                responseStream = request.GetResponse().GetResponseStream();
                reader = new StreamReader(responseStream);
                string docu = reader.ReadToEnd();
                salida = new XmlDocument();
                salida.LoadXml(docu);
            }
            catch (Exception e)
            {
                //new LogWriter().WriteLog("Se ha producido un error en Llamar Servicio:" + e.Message.ToString() + "//" + e.Source.ToString());
                //ExceptionPolicy.HandleException(e, "InterfazGenerico Exception Policy");
            }
            finally
            {
                if (requestStream != null)
                    requestStream.Close();
                if (reader != null)
                    reader.Close();
                if (responseStream != null)
                    responseStream.Close();
            }
            return salida;
        }

        public IEnumerable<string[]> GetNotas(ParametroDocumento IdCaso)
        {
            List<string[]> prueba = new List<string[]>();
            List<Nota> ListaIndividual = new List<Nota>();
            Nota[] Item = new Nota[9];
            List<Nota[]> ListaCompleta = new List<Nota[]>();
            List<ItemNotas> a = new List<ItemNotas>();
            List<string[]> Notas = new List<string[]>();
            a = ConexionNotas(IdCaso.CasoId);
            string[] IndiceTotal = new string[a.Count];
            int MaxIndice = 0;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].concepto == "0186")
                {
                    if (MaxIndice < Convert.ToInt32(a[i].indice))
                    {
                        MaxIndice = Convert.ToInt32(a[i].indice);
                    }
                }
            }

            String[] Indices = new String[MaxIndice];

            foreach (ItemNotas Cada in a)
            {
                Nota nota = new Nota();
                for (int e = 0; e <= MaxIndice; e++)
                {
                    if (Cada.concepto == "0186" && e.ToString() == Cada.indice)
                    {
                        nota.Titulo = Cada.descripcion;
                        nota.Indice = Cada.indice;
                    }
                }
                if (nota.Titulo != null)
                {
                    ListaIndividual.Add(nota);
                }
            }

            foreach (Nota LaNota in ListaIndividual)
            {
                foreach (ItemNotas Cada in a)
                {
                    if (Cada.concepto == "0187" && Cada.indice == LaNota.Indice)
                    {
                        LaNota.Descripcion = Cada.descripcion;
                    }

                    if (Cada.concepto == "0188" && Cada.indice == LaNota.Indice)
                    {
                        LaNota.Fecha = Cada.descripcion;
                    }
                }
            }

            foreach (Nota LaNota in ListaIndividual)
            {
                Notas.Add(ConvertirANotas(LaNota));
            }

            return Notas;
        }

        public IEnumerable<string[]> GetHistorico(ParametroDocumento param)
        {
            List<string[]> historico = new List<string[]>();
            List<ItemHistorico> a = new List<ItemHistorico>();
            a = ConexionHistorico(param.CasoId);
            foreach (ItemHistorico Cada in a)
            {
                Historial historial = new Historial();
                historial.Fecha = Cada.fecha;
                historial.Cuerpo = Cada.descripcion;
                historial.Estado = Cada.codigo;
                historico.Add(ConvertirAHistorial(historial));
            }
            return historico;
        }

        public IEnumerable<string[]> GetAbogado()
        {
            List<ItemAbogado> Datos = new List<ItemAbogado>();
            List<string[]> Abogados = new List<string[]>();

            Datos = ConexionAbogado();

            foreach (ItemAbogado Cada in Datos)
            {
                Abogado abogado = new Abogado();
                abogado.Concepto = Cada.concepto;
                abogado.Indice = Cada.indice;
                abogado.Valor = Cada.valor;
                abogado.Codigo = Cada.codigo;
                abogado.ClaveExterna = Cada.claveExterna;
                Abogados.Add(ConvertirAAbogado(abogado));
            }

            return Abogados;
        }

        public IEnumerable<string[]> GetTerritorios(ParametroAbog IdAbogado)
        {
            List<ItemTerr> a = new List<ItemTerr>();
            List<string[]> miCombo = new List<string[]>();
            List<string> miComboId = new List<string>();
            List<string> miComboDesc = new List<string>();
            List<string> miComboIndice = new List<string>();
            List<string> miComboIdS = new List<string>();
            List<string> miComboDescS = new List<string>();
            a = ConexionTerritorios(IdAbogado.idAbogado);

            foreach (ItemTerr b in a)
            {
                if (b.concepto == "0141")
                {
                    bool igual = false;
                    int indice = 0;
                    int i = 0;
                    foreach (string Valor in miComboId)
                    {
                        if (Valor.Split('-')[0] == b.valor)
                        {
                            igual = true;
                            indice = i;
                        }
                        i++;
                    }
                    if (igual == false)
                    {
                        miComboId.Add(b.valor + "-" + b.indice);
                    }
                    else
                    {
                        miComboId[indice] = miComboId[indice] + "," + b.valor + "-" + b.indice;
                    }
                }
            }
            foreach (ItemTerr b in a)
            {
                if (b.concepto == "0169")
                {
                    if (miComboDesc.Contains(b.valor) == false)
                    {
                        miComboDesc.Add(b.valor);
                    }
                }
            }


            miCombo.Add(miComboId.ToArray());
            miCombo.Add(miComboDesc.ToArray());


            foreach (ItemTerr b in a)
            {
                if (b.concepto == "0138")
                {
                    miComboIdS.Add(b.valor + "-" + b.indice);
                }
            }
            foreach (ItemTerr b in a)
            {
                if (b.concepto == "0168")
                {
                    miComboDescS.Add(b.valor);
                }
            }

            miCombo.Add(miComboIdS.ToArray());
            miCombo.Add(miComboDescS.ToArray());
            return miCombo;
        }

        private toDoList EvaluarItems(Item[] b)
        {
            toDoList tarea = new toDoList();
            foreach (Item Cada in b)
            {
                if (Cada != null)
                {
                    if (Cada.concepto == ToDescriptionString(Concepto.Sla))
                    {
                        tarea.Sla = Cada.valor;
                    }
                    if (Cada.concepto == ToDescriptionString(Concepto.UsuarioBloqueo))
                    {
                        tarea.UsuarioBloqueo = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.EstadoSolicitud))
                    {
                        tarea.Etapa = Cada.valor;
                    }
                    if (Cada.concepto == ToDescriptionString(Concepto.Materia))
                    {
                        tarea.Materia = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.Rut))
                    {
                        tarea.RUT = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.NombreCliente))
                    {
                        tarea.NombreCliente = Codificar(Cada.valor);
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.CodigoCentto))
                    {
                        tarea.NumeroSucursal = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.NombreSucursalAltoCaso))
                    {
                        tarea.NombreSucursal = Codificar(Cada.valor);
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.ClaveInterna))
                    {
                        tarea.CasoId = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.NumeroSgl))
                    {
                        tarea.NumeroSgl = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.FechaCreacion))
                    {
                        tarea.Inicio = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.FechaVencimientoTarea))
                    {
                        tarea.Vencimiento = Cada.valor;
                    }

                    if (Cada.concepto == ToDescriptionString(Concepto.HoraVencimientoTarea))
                    {
                        tarea.HoraVencimiento = Cada.valor;
                    }
                    if (Cada.concepto == ToDescriptionString(Concepto.UsuarioAsignado))
                    {
                        tarea.HoraVencimiento = Cada.valor;
                    }
                }
            }

            return tarea;
        }

        public JsonTextReader SetReparo(string idCaso, string tipo)
        {
            return ConexionReparo(idCaso, tipo);
        }

        public void SetAsignarDoc(string Id, string Documento, string Nombre)
        {
            ConexionAsignarDoc(Id, Documento, Nombre);
        }

        public void SetNota(string titulo, string contenido, string indice, string IdCaso)
        {
            ConexionAgregoNota(titulo, contenido, indice, IdCaso);
        }

        private List<Item[]> Conexion(ConjuntoParametros param, string Abogado)
        {
            List<Item> a = new List<Item>();
            string formato = string.Empty;
            if (param != null && param.Parametros != null && param.Parametros.Count > 0)
            {
                foreach (Parametro ElParametro in param.Parametros)
                {
                    formato += "{%22conceptoValor%22:{%22concepto%22:%22" + ElParametro.concepto + "%22,%20%22valor%22:%22" + ElParametro.valor + "%22,%20%22signo%22:%22" + ElParametro.signo + "%22}},";
                }
            }

            formato += "{%22conceptoValor%22:{%22concepto%22:%22" + WC.Concepto3 + "%22,%20%22valor%22:%22" + Abogado + "%22,%20%22signo%22:%22" + "" + "%22}},";
            formato += "{%22conceptoValor%22:{%22concepto%22:%22" + WC.Concepto2 + "%22,%20%22valor%22:%22" + WC.Estado + "%22,%20%22signo%22:%22" + "" + "%22}}";
            string autentificacion = "&authenticationType=userPassword&usuario=" + WC.Usuario + "&password=" + WC.Pass;
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%22busquedaCaso%22:{%22entrada%22:{%22empresa%22:%22" + WC.Empresa + "%22,%20%22caso%22:%22%22,%20%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%20%22tipoProceso%22:%22" + WC.TipoProceso + "%22,%20%22pagina%22:%22" + WC.Pagina + "%22,%20%22canal%22:%22" + WC.Canal + "%22,%20%22idioma%22:{" + WC.Idioma + "},%20%22listaConceptoValor%22:[" + formato + "]}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));
            return Leer(reader);
        }

        private List<ItemCombo> ConexionCombo()
        {
            List<string> a = new List<string>();
            string formato = "'identificadorCampo': '" + WC.IdentificadorCampo + "', 'valorRelacionado': '" + WC.ValorRelacionado + "', 'conceptoRelacionado': '" + WC.ConceptoRelacionado + "'";
            string URL = WC.urlWS + "/F_GESPDM_Parametrizaciones_C/1.0?requestData={'consultaTablaParametrosEnlazada':{'entrada':{'empresa':'0022', 'idioma':{" + WC.Idioma + "}, 'listaTablaParametrosEnlazadaIn':[{'tablaParametrosEnlazadaIn':{" + formato + "}}]}}}" + WC.Autentificacion;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));

            return LeerCombo(reader);
        }

        private void ConexionAgregoNota(string titulo, string contenido, string indice, string idCaso)
        {
            string formatoConcepto = "{%20%22ConceptoValor%22:{%20%22concepto%22:%22" + WC.TituloNota + "%22,%20%22indice%22:%22" + indice + "%22,%20%22descripcion%22:%22" + titulo + "%22%20}%20},";
            formatoConcepto += "{%20%22ConceptoValor%22:{%20%22concepto%22:%22" + WC.CuerpoNota + "%22,%20%22indice%22:%22" + indice + "%22,%20%22descripcion%22:%22" + contenido + "%22%20}%20},";
            formatoConcepto += "{%20%22ConceptoValor%22:{%20%22concepto%22:%22" + WC.FechaNota + "%22,%20%22indice%22:%22" + indice + "%22,%20%22descripcion%22:%22" + DateTime.Now.ToString("dd-MM-yyyy") + "%22%20}%20}";
            string URL = WC.urlWS + "/F_GESPDM_GestValorClaveExt_C/1.0?requestData={%20%22gestionarValoresListaConcepCExt%22:{%20%22entrada%22:{%20%22empresa%22:%22" + WC.Empresa + "%22,%20%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%20%22claveExterna%22:%22" + idCaso + "%22,%20%22listaValoresConcepto%22:[" + formatoConcepto + "]%20}%20}%20}" + WC.Autentificacion;
            var decodedUrl = HttpUtility.UrlDecode(URL, Encoding.ASCII);
            var SplitUrl = decodedUrl.Split('?');
            string ParametrosSplit = SplitUrl[1];
            string UrlSplit = SplitUrl[0];
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(UrlSplit, ParametrosSplit);
            }
        }

        private JsonTextReader ConexionReparo(string idCaso, string tipo)
        {
            string DescAlfa = string.Empty;

            if (tipo == "1")
            {
                DescAlfa = "0022%20003A";
            }

            if (tipo == "2")
            {
                DescAlfa = "0022%20005A";
            }

            string Usuario = string.Empty;
            string formato = "{%20%22EMPRESA%22:%22" + WC.Empresa + "%22,%20%22PROCESO_DE_NEGOCIO%22:%22" + WC.ProcesoNegocio + "%22%20},%20%22estadoEvento%22:{%20%22EMPRESA%22:%22" + WC.Empresa + "%22,%20%22CODIGO_ALFANUM_3%22:%22" + WC.CodigoAlfa3 + "%22%20}";
            string formatoConcepto = "{%20%22ConceptoValor%22:{%20%22concepto%22:%22" + WC.ConceptoReparo + "%22,%20%22indice%22:%220%22,%20%22descripcion%22:%22" + DescAlfa + "%22%20}%20},";
            formatoConcepto += "{%22conceptoValor%22:{%22concepto%22:%22" + WC.BloqueoDesbloqueoTarea + "%22,%20%22descripcion%22:%22" + Usuario + "%22,%20%22indice%22:%22" + "0" + "%22}},";
            formatoConcepto += "{%20%22ConceptoValor%22:{%20%22concepto%22:%22" + WC.ConceptoReparo2 + "%22,%20%22indice%22:%220%22,%20%22descripcion%22:%22" + WC.CodigoAlfa3 + "%22%20}%20}";
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%20%22cancelarSLA%22:{%20%22entrada%22:{%20%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%20%22canal%22:%22" + WC.Canal + "%22,%20%22idioma%22:{" + WC.Idioma + "},%20%22claveExterna%22:%22" + idCaso + "%22,%20%22procesoNegocio%22:" + formato + ",%20%22listConceptosValor%22:[" + formatoConcepto + "]%20}%20}%20}" + WC.Autentificacion;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));
            return reader;
        }

        private void ConexionAsignarDoc(string idCaso, string Documento, string Nombre)
        {
            string formatoDocumento = "{%20%22documento%22:{%20%22tipoDocumental%22:%22" + WC.TipoDocumental + "%22,%20%22idDocumento%22:%22" + Documento + "%22,%20%22titulo%22:%22" + Nombre + "%22%20}%20}";
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%20%22insertarDocumentosList%22:{%20%22entrada%22:{%20%22empresaOrigen%22:%22" + WC.Empresa + "%22,%20%22tipoClavExt%22:%22" + WC.ClaveExterna + "%22,%20%22clavExt%22:%22" + idCaso + "%22,%20%22documentoList%22:" + formatoDocumento + "%20}%20}%20}" + WC.Autentificacion;
            var decodedUrl = HttpUtility.UrlDecode(URL,Encoding.ASCII);
            var SplitUrl = decodedUrl.Split('?');
            string ParametrosSplit = SplitUrl[1];
            string UrlSplit = SplitUrl[0];
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(UrlSplit, ParametrosSplit);
            }
        }

        private List<ItemAbogado> ConexionAbogado()
        {
            string formato = "{%20%22conceptoValor%22:%20{%20%22concepto%22:%20%22" + WC.FechaAbog + "%22,%20%22valor%22:%20%22" + DateTime.Now.ToString("yyyy-MM-dd") + "%22,%20%22signo%22:%20%22%20%3E%22%20}%20}%20,%20";
            formato += "{%20%22conceptoValor%22:%20{%20%22concepto%22:%20%22" + WC.ConceptoRelacionado + "%22,%20%22valor%22:%20%22" + WC.TipoProcesoAbog + "%22,%20%22signo%22:%20%22%22%20}%20}%20,%20";
            formato += "{%20%22conceptoValor%22:%20{%20%22concepto%22:%20%22" + WC.ConceptoAbog1 + "%22,%20%22valor%22:%20%22" + WC.Empresa + "%22,%20%22signo%22:%20%22%22%20}%20}%20,%20";
            formato += "{%20%22conceptoValor%22:%20{%20%22concepto%22:%20%22" + WC.ConceptoAbog2 + "%22,%20%22valor%22:%20%22" + WC.EmpresaAbog + "%22,%20%22signo%22:%20%22%22%20}%20}%20,%20";
            formato += "{%20%22conceptoValor%22:%20{%20%22concepto%22:%20%22" + WC.Concepto1 + "%22,%20%22valor%22:%20%22" + WC.Logado + "%22,%20%22signo%22:%20%22%22%20";
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%20%22busquedaCaso%22:%20{%20%22entrada%22:%20{%20%22empresa%22:%22" + WC.Empresa + "%22,%20%22caso%22:%22%22,%20%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%20%22tipoProceso%22:%22" + WC.TipoProcesoAbog + "%22,%20%22pagina%22:%22" + WC.PaginaAbog + "%22,%20%22canal%22:%22" + WC.Canal + "%22,%20%22idioma%22:%20{" + WC.IdiomaAbog + "}%20,%20%22listaConceptoValor%22:[%20" + formato + "}%20}%20]%20}%20}%20}" + WC.Autentificacion;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));

            return LeerAbogado(reader);
        }

        private List<ItemTerr> ConexionTerritorios(string IdAbogado)
        {
            List<string> a = new List<string>();
            string formato = "{%22claveExterna%22:%20%22" + IdAbogado + "%22,%20%22tipoClaveExterna%22:%20%22" + WC.ClaveExterna + "%22}";
            string URL = WC.urlWS + "/F_GESPDM_GestValorClaveExt_C/1.0?requestData={%20%22obtenerValoresPagina%22:%20{%22entrada%22:{%20%22empresa%22:%22" + WC.Empresa + "%22,%20%22tipoProceso%22:%22" + WC.TipoProcesoAbog + "%22,%20%22pagina%22:%22" + WC.PaginaTerr + "%22,%20%22valoresPaginaList%22:{%22valoresPagina%22:[" + formato + "]},%20%22channel%22:%22" + WC.Canal + "%22,%20%22language%22:{" + WC.IdiomaTerr + "}}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));

            return LeerTerritorios(reader);
        }

        private List<Item[]> ConexionDetalle(string CasoId, string Abogado)
        {
            string formato = "{%22conceptoValor%22:{%22concepto%22:%22" + WC.Concepto3 + "%22,%20%22valor%22:%22" + Abogado + "%22,%20%22signo%22:%22" + "" + "%22}},";
            formato += "{%22conceptoValor%22:{%22concepto%22:%22" + WC.Concepto2 + "%22,%20%22valor%22:%22" + WC.Estado + "%22,%20%22signo%22:%22" + "" + "%22}},";
            formato += "{%22conceptoValor%22:{%22concepto%22:%22" + WC.CodIdCaso + "%22,%20%22valor%22:%22" + CasoId + "%22,%20%22signo%22:%22" + "" + "%22}}";
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%22busquedaCaso%22:{%22entrada%22:{%22empresa%22:%22" + WC.Empresa + "%22,%20%22caso%22:%22%22,%20%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%20%22tipoProceso%22:%22" + WC.TipoProceso + "%22,%20%22pagina%22:%22" + WC.Pagina + "%22,%20%22canal%22:%22" + WC.Canal + "%22,%20%22idioma%22:{" + WC.Idioma + "},%20%22listaConceptoValor%22:[" + formato + "]}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));
            return Leer(reader);
        }

        private List<ItemNotas> ConexionNotas(string IdCaso)
        {
            string autentificacion = "&authenticationType=userPassword&usuario=" + WC.Usuario + "&password=" + WC.Pass;
            string URL = WC.urlWS + "/F_GESPDM_GestValorClaveExt_C/1.0?requestData={%22obtenerValoresPagina%22:{%22entrada%22:{%22empresa%22:%22" + WC.Empresa + "%22,%22tipoProceso%22:%22" + WC.TipoProceso + "%22,%22pagina%22:%22" + WC.PaginaNot + "%22,%22valoresPaginaList%22:{ %22valoresPagina%22:[{%22claveExterna%22:%22" + IdCaso + "%22,%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22}]},%22channel%22:%22" + WC.Canal + "%22,%22language%22:{" + WC.Idioma + "}}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));
            return LeerNota(reader);
        }

        private List<ItemHistorico> ConexionHistorico(string IdCaso)
        {
            string autentificacion = "&authenticationType=userPassword&usuario=" + WC.Usuario + "&password=" + WC.Pass;
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%22recuperarLogActividades%22:{%22entrada%22:{%22empresa%22:%22" + WC.Empresa + "%22,%22tipoProceso%22:%22" + WC.TipoProceso + "%22,%22pagina%22:%22" + WC.PaginaHist + "%22,%22tipoClaveExterna%22:%22" + WC.ClaveExterna + "%22,%22clavExt%22:%22" + IdCaso + "%22,%22canal%22:%22" + WC.Canal + "%22,%22idioma%22:{" + WC.Idioma + "},%22identifModo%22:%22BR%22}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));
            return LeerHistorico(reader, IdCaso);
        }

        private List<ItemDocumento> ConexionDocumentos(string IdCaso)
        {
            string URL = WC.urlWS + "/F_GESPRC_CommonSvc_C/1.0?requestData={%22obtenerDocumentosRelacionados%22:{%22entrada%22:{%22empresa%22:%22" + WC.Empresa + "%22,%22tipoclavext%22:%22" + WC.ClaveExterna + "%22,%22clavExt%22:%22" + IdCaso + "%22,%22tipoProceso%22:%22" + WC.TipoProceso + "%22,%22pagina%22:%22" + WC.PaginaDoc + "%22,%22canal%22:%22OFI%22,%22idioma%22:{" + WC.Idioma + "}}}}" + WC.Autentificacion;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();
            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            JsonTextReader reader = new JsonTextReader(new StreamReader(webStream));
            return LeerDetalle(reader);
        }

        private List<Item[]> Leer(JsonTextReader reader)
        {
            List<Item> ListaIndividual = new List<Item>();
            Item[] Item = new Item[12];
            List<Item[]> ListaCompleta = new List<Item[]>();

            int i = 0;
            string concepto = string.Empty;
            string indice = string.Empty;
            string valor = string.Empty;
            string codigo = string.Empty;
            bool empezar = false;
            bool primero = true;
            bool empiezeElemento = false;

            while (reader.Read())
            {
                if (reader.Value != null || empiezeElemento == true)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    if (reader.Value != null)
                    {
                        if (primero == false && reader.Value.ToString().Equals("buscarCasosList"))
                        {
                            empezar = true;
                            ListaIndividual.CopyTo(Item);
                            ListaCompleta.Add(Item);
                            ListaIndividual.Clear();
                            Item = new Item[12];
                        }
                    }
                    if (empezar == true && (reader.Value == null || reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer")))
                    {
                        if (i == 0)
                        {
                            concepto = reader.Value.ToString();
                        }
                        if (i == 1)
                        {

                            indice = reader.Value.ToString();
                            empiezeElemento = true;

                        }
                        if (i == 2)
                        {
                            if (reader.Value != null)
                            {
                                valor = reader.Value.ToString();
                            }
                            else
                            {
                                valor = string.Empty;
                            }



                        }
                        if (i == 3)
                        {
                            if (reader.Value != null)
                            {
                                codigo = reader.Value.ToString();
                            }
                            else
                            {
                                codigo = string.Empty;
                            }

                            empiezeElemento = false;

                        }
                        i++;
                        if (i == 4)
                        {
                            Item b = new Item();
                            b.concepto = concepto;
                            b.indice = indice;
                            b.valor = valor;
                            b.codigo = codigo;
                            ListaIndividual.Add(b);
                            i = 0;
                        }
                    }
                    if (reader.Value != null)
                    {
                        if (reader.Value.ToString().Equals("concepto"))
                        {
                            empezar = true;
                            primero = false;
                        }
                        else if (reader.Value.ToString().Equals("buscarCasosList"))
                        {
                            empezar = false;
                            primero = true;
                        }
                    }
                }
            }
            if (ListaIndividual.Count > 0)
            {
                ListaIndividual.CopyTo(Item);
                ListaCompleta.Add(Item);
            }

            return ListaCompleta;
        }

        private List<ItemCombo> LeerCombo(JsonTextReader reader)
        {
            List<ItemCombo> a = new List<ItemCombo>();
            int i = 0;
            string identificadorCampo = string.Empty;
            string concepto = string.Empty;
            string descripcion = string.Empty;
            string indice = string.Empty;
            string valor = string.Empty;
            bool empezar = false;

            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    empezar |= reader.Value.ToString().Equals("tablaParametrosEnlazadosOut");

                    if (empezar == true && (reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer")))
                    {
                        if (i == 0)
                        {
                            identificadorCampo = reader.Value.ToString();
                        }
                        if (i == 1)
                        {
                            concepto = reader.Value.ToString();
                        }
                        if (i == 2)
                        {
                            descripcion = reader.Value.ToString();
                        }
                        if (i == 3)
                        {
                            indice = reader.Value.ToString();
                        }
                        if (i == 4)
                        {
                            valor = reader.Value.ToString().Trim();
                        }
                        i++;
                        if (i == 5)
                        {
                            ItemCombo b = new ItemCombo();
                            b.identificadorCampo = identificadorCampo;
                            b.concepto = concepto;
                            b.descripcion = descripcion;
                            b.indice = indice;
                            b.valor = valor;
                            a.Add(b);
                            i = 0;
                        }
                    }
                }
            }
            return a;
        }

        private List<ItemAbogado> LeerAbogado(JsonTextReader reader)
        {
            bool empezar = false;
            bool ObtenerClave = false;

            List<ItemAbogado> ListaIndividual = new List<ItemAbogado>();
            ItemAbogado[] Item = new ItemAbogado[1];
            List<ItemAbogado[]> ListaCompleta = new List<ItemAbogado[]>();

            int i = 0;
            string ClaveExterna = string.Empty;
            string concepto = string.Empty;
            string Indice = string.Empty;
            string Codigo = string.Empty;
            string Valor = string.Empty;

            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);

                    if (ObtenerClave == true && (reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer")))
                    {
                        if (ClaveExterna == string.Empty)
                        {
                            ClaveExterna = reader.Value.ToString();
                        }
                    }
                    if (empezar == true && (reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer")))
                    {
                        if (i == 0)
                        {
                            concepto = reader.Value.ToString();
                        }
                        if (i == 1)
                        {
                            Indice = reader.Value.ToString();
                        }
                        if (i == 2)
                        {
                            Valor = reader.Value.ToString();
                        }
                        if (i == 3)
                        {
                            Codigo = reader.Value.ToString();
                        }
                        i++;
                        if (i == 4)
                        {
                            ItemAbogado b = new ItemAbogado();
                            b.concepto = concepto;
                            b.indice = Indice;
                            b.valor = Valor;
                            b.codigo = Codigo;
                            b.claveExterna = ClaveExterna;
                            ListaIndividual.Add(b);
                            i = 0;
                        }
                    }

                    if (reader.Value.ToString().Equals("concepto"))
                    {
                        empezar = true;
                        ObtenerClave = false;
                    }
                    ObtenerClave |= reader.Value.ToString().Equals("claveExterna");
                }
            }
            return ListaIndividual;
        }

        private List<ItemDocumento> LeerDetalle(JsonTextReader reader)
        {
            List<ItemDocumento> ListaIndividual = new List<ItemDocumento>();
            ItemDocumento[] Item = new ItemDocumento[1];
            List<ItemDocumento[]> ListaCompleta = new List<ItemDocumento[]>();

            int i = 0;
            string idDocumento = string.Empty;
            string titulo = string.Empty;
            string descripcion = string.Empty;
            string fecha = string.Empty;
            string usuario = string.Empty;
            bool TipoDoc = false;
            string Eltipo = string.Empty;
            bool empezar = false;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    if (TipoDoc == true)
                    {
                        Eltipo = reader.Value.ToString();
                        TipoDoc = false;
                    }
                    if (empezar == true && (reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer") || reader.TokenType.ToString().Equals("Date")))
                    {
                        if (i == 0)
                        {
                            idDocumento = reader.Value.ToString();
                        }
                        if (i == 1)
                        {

                            titulo = reader.Value.ToString();

                        }
                        if (i == 2)
                        {
                            descripcion = Eltipo;
                        }
                        if (i == 3)
                        {
                            fecha = reader.Value.ToString();
                        }
                        if (i == 4)
                        {
                            usuario = reader.Value.ToString().Trim();
                        }
                        i++;
                        if (i == 5)
                        {
                            ItemDocumento b = new ItemDocumento();
                            b.idDocumento = idDocumento;
                            b.titulo = titulo;
                            b.descTipo = descripcion;
                            b.fecha = fecha;
                            b.usuario = usuario;
                            ListaIndividual.Add(b);
                            empezar = false;
                            i = 0;
                        }
                    }
                    empezar |= reader.Value.ToString().Equals("idDocumento");
                    TipoDoc |= reader.Value.ToString().Equals("CODIGO_ALFANUM");
                }
            }
            return ListaIndividual;
        }

        private List<ItemNotas> LeerNota(JsonTextReader reader)
        {
            List<ItemNotas> ListaIndividual = new List<ItemNotas>();
            ItemNotas[] Item = new ItemNotas[1];
            List<ItemNotas[]> ListaCompleta = new List<ItemNotas[]>();

            int i = 0;
            string Indice = string.Empty;
            string concepto = string.Empty;
            string Traducible = string.Empty;
            string descripcion = string.Empty;
            string Valor = string.Empty;
            bool empezar = false;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);

                    if (empezar == true && (reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer")))
                    {
                        if (i == 0)
                        {
                            concepto = reader.Value.ToString();
                        }
                        if (i == 1)
                        {
                            Indice = reader.Value.ToString();
                        }
                        if (i == 2)
                        {
                            descripcion = reader.Value.ToString();
                        }
                        /* if (i == 3)
                         {
                             Traducible = reader.Value.ToString();
                         }*/
                        if (i == 3)
                        {
                            Valor = reader.Value.ToString();
                        }
                        i++;
                        if (i == 4)
                        {
                            ItemNotas b = new ItemNotas();
                            b.concepto = concepto;
                            b.indice = Indice;
                            b.descripcion = descripcion;
                            b.traducible = Traducible;
                            b.valor = Valor;
                            ListaIndividual.Add(b);
                            i = 0;
                        }
                    }
                    empezar |= reader.Value.ToString().Equals("concepto");
                }
            }

            return ListaIndividual;
        }

        private List<ItemHistorico> LeerHistorico(JsonTextReader reader, string IdCaso)
        {
            List<ItemHistorico> ListaIndividual = new List<ItemHistorico>();
            ItemHistorico[] Item = new ItemHistorico[1];
            List<ItemHistorico[]> ListaCompleta = new List<ItemHistorico[]>();
            int i = 0;
            string Indice = string.Empty;
            string concepto = string.Empty;
            string codigo = string.Empty;
            string descripcion = string.Empty;
            string fecha = string.Empty;
            bool empezar = false;
            bool comentarios = false;
            while (reader.Read())
            {
                if (comentarios == true || reader.Value != null)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);

                    if (empezar == true && (reader.Value == null || reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer") || reader.TokenType.ToString().Equals("Date")))
                    {
                        if (i == 2)
                        {
                            fecha = reader.Value.ToString();
                        }

                        if (i == 5)
                        {
                            codigo = reader.Value.ToString().Trim();
                        }

                        if (i == 7)
                        {
                            descripcion = reader.Value.ToString();
                            comentarios = true;
                        }
                        if (i == 8)
                        {
                            if (reader.Value == null)
                            {
                                concepto = string.Empty;
                            }
                            else
                            {
                                concepto = reader.Value.ToString();
                            }
                            comentarios = false;
                        }
                        i++;
                        if (i == 9)
                        {
                            ItemHistorico b = new ItemHistorico();
                            b.codigo = codigo;
                            if (concepto == string.Empty)
                            {
                                b.descripcion = descripcion;
                            }
                            else
                            {
                                b.descripcion = descripcion + "-" + concepto;
                            }
                            b.claveExt = IdCaso;
                            b.fecha = fecha;
                            ListaIndividual.Add(b);
                            empezar = false;
                            i = 0;
                            empezar = false;
                        }
                    }
                    if (reader.Value != null)
                    {
                        empezar |= reader.Value.ToString().Equals("LogActividades");
                    }
                }
            }
            return ListaIndividual;
        }

        private List<ItemTerr> LeerTerritorios(JsonTextReader reader)
        {
            List<ItemTerr> a = new List<ItemTerr>();
            int i = 0;
            string identificadorCampo = string.Empty;
            string concepto = string.Empty;
            string descripcion = string.Empty;
            string indice = string.Empty;
            string traducible = string.Empty;
            string valor = string.Empty;
            bool empezar = false;
            bool empiezeElemento = false;

            while (reader.Read())
            {
                if (reader.Value != null || empiezeElemento == true)
                {
                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    if (reader.Value != null)
                    {
                        empezar |= reader.Value.ToString().Equals("datoConceptoValor");
                    }

                    if (empezar == true && (reader.TokenType.ToString().Equals("String") || reader.TokenType.ToString().Equals("Integer") || reader.Value == null))
                    {
                        if (i == 0)
                        {
                            concepto = reader.Value.ToString();
                            empiezeElemento = true;
                        }
                        if (i == 1)
                        {
                            indice = reader.Value.ToString();
                        }
                        if (i == 2)
                        {
                            if (reader.Value != null)
                            {
                                descripcion = reader.Value.ToString();
                            }
                            else
                            {
                                descripcion = string.Empty;
                            }
                        }
                        if (i == 3)
                        {
                            if (reader.Value != null)
                            {
                                traducible = reader.Value.ToString();
                            }
                            else
                            {
                                traducible = string.Empty;
                            }
                        }
                        if (i == 4)
                        {
                            if (reader.Value != null)
                            {
                                valor = reader.Value.ToString().Trim();
                            }
                            else
                            {
                                valor = string.Empty;
                            }
                        }

                        i++;
                        if (i == 5)
                        {
                            ItemTerr b = new ItemTerr();
                            b.concepto = concepto;
                            b.indice = indice;
                            b.descripcion = descripcion;
                            b.traducible = traducible;
                            b.valor = valor;
                            a.Add(b);
                            empiezeElemento = false;
                            i = 0;
                        }
                    }
                }
            }

            return a;
        }

        public string[] ConvertirToDoList(toDoList Elemento)
        {
            List<string> listString = new List<string>();
            listString.Add("< a href = 'LANPRO_Task.html' >< i class='material-icons' data-uk-tooltip title = 'Finalizar' > &#xE899;</i><i class='material-icons' data-uk-tooltip title='Ir a la tarea'>&#xE154;</i></a>");
            listString.Add(Elemento.UsuarioBloqueo);
            listString.Add(Elemento.Etapa);
            listString.Add(Elemento.Materia);
            listString.Add(Elemento.RUT);
            listString.Add(Elemento.NombreCliente);
            listString.Add(Elemento.NumeroSucursal);
            listString.Add(Elemento.NombreSucursal);
            listString.Add(Elemento.CasoId);
            listString.Add(Elemento.NumeroSgl);
            listString.Add(Elemento.Inicio);
            listString.Add(Elemento.Sla);
            return listString.ToArray();
        }

        private string[] ConvertirToDoListDetalle(toDoList Elemento)
        {
            List<string> listString = new List<string>();
            listString.Add(Elemento.NombreSucursal);
            listString.Add(Elemento.NumeroSucursal);
            listString.Add(Elemento.CasoId);
            listString.Add(Elemento.Inicio);
            listString.Add(Elemento.Sla);
            listString.Add(Elemento.NumeroSgl);
            listString.Add(Elemento.RUT);
            listString.Add(Elemento.UsuarioAsignado);
            return listString.ToArray();
        }

        private string[] ConvertirADocumentos(Documento MiDocumento)
        {
            List<string> listString = new List<string>();
            listString.Add(MiDocumento.Id);
            listString.Add(MiDocumento.Titulo);
            listString.Add(MiDocumento.Fecha);
            listString.Add(MiDocumento.Descripcion);
            listString.Add(MiDocumento.Usuario);
            return listString.ToArray();
        }

        private string[] ConvertirANotas(Nota Elemento)
        {
            List<string> listString = new List<string>();
            listString.Add(Elemento.Titulo);
            listString.Add(Elemento.Descripcion);
            listString.Add(Elemento.Indice);
            listString.Add(Elemento.Fecha);
            return listString.ToArray();
        }

        private string[] ConvertirAAbogado(Abogado Elemento)
        {
            List<string> listString = new List<string>();
            listString.Add(Elemento.Codigo);
            listString.Add(Elemento.Valor);
            listString.Add(Elemento.Indice);
            listString.Add(Elemento.ClaveExterna);
            return listString.ToArray();
        }

        private string[] ConvertirAHistorial(Historial Elemento)
        {
            List<string> Historial = new List<string>();
            Historial.Add(Elemento.Fecha.ToString());
            Historial.Add(Elemento.Estado);
            Historial.Add(Elemento.Cuerpo);
            return Historial.ToArray();
        }

        private string Codificar(string valor)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(valor);
            byte[] transformado = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("windows-1252"), encodedBytes);
            return Encoding.UTF8.GetString(transformado);
        }

        private toDoList limpiar(toDoList tarea)
        {
            Type t = tarea.GetType();
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo p in properties)
            {
                if (p.PropertyType.Name == "String" && p.CanWrite == true)
                {
                    if (p.GetValue(tarea) == null)
                    {
                        p.SetValue(tarea, "");
                    }
                }
            }
            return tarea;
        }

        private string ToDescriptionString(Concepto val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public  string ConvertMimeTypeToExtension(string mimeType)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            { }

            string key = string.Format(@"MIME\Database\Content Type\{0}", mimeType);
            string result;
            if (MimeTypeToExtension.TryGetValue(key, out result))
                return result;

            RegistryKey regKey;
            object value;

            regKey = Registry.ClassesRoot.OpenSubKey(key, false);
            value = regKey != null ? regKey.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            MimeTypeToExtension[key] = result;
            return result;
        }

        public  string ConvertExtensionToMimeType(string extension)
        {

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentNullException("extension");

            if (!extension.StartsWith("."))
                extension = "." + extension;

            string result;
            if (ExtensionToMimeType.TryGetValue(extension, out result))
                return result;

            RegistryKey regKey;
            object value;

            regKey = Registry.ClassesRoot.OpenSubKey(extension, false);
            value = regKey != null ? regKey.GetValue("Content Type", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            ExtensionToMimeType[extension] = result;
            return result;
        }
    }
}