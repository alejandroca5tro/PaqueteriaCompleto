using GestionTareaAero.Models;
using System.Collections.Generic;

namespace GestionTareaAero.Service
{
    public interface IToDoList
    {
        IEnumerable<string[]> GetToDoList(ConjuntoParametros param,string Abogado);

        IEnumerable<string[]> GetCombo();

        string GetUsuario();

        string ConseguirDocumento(string IdDoc, string Tipo, string Usuario);

        IEnumerable<string[]> GetDetalles(ParametroDocumento param, string Abogado);

        IEnumerable<string[]> GetNotas(ParametroDocumento CasoId);

        IEnumerable<string[]> GetDocumentos(ParametroDocumento param);

        IEnumerable<string[]> GetHistorico(ParametroDocumento param);

        IEnumerable<string[]> GetAbogado();
    }
}