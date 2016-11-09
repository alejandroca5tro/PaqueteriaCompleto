using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paqueteria;
using Paqueteria.Conversores;
using Paqueteria.Model;
using Paqueteria.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaqueteriaTest
{
    [TestClass]
    public class EnvioRepositoryTest
    {
        private IEnvioRepository sut;
        private IPaqDBFactory dbFactory;
        private IList<long> listado;

        [TestInitialize]
        public void TestInicializa()
        {
            dbFactory = new PaqDropCreateDBFactory();
            sut = new EnvioRepository(new EnvioConversor(new PaqueteConversor()), dbFactory);
            using (var ctx = dbFactory.GetInstance())
            {
                ctx.Database.Initialize(true);
            }
            listado = new List<long>();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            using (var ctx = dbFactory.GetInstance())
            {
                foreach (long id in listado)
                {
                    ctx.Database.ExecuteSqlCommand("DELETE from Envios WHERE EnvioId = @id", new SqlParameter("@id", id));
                }
            }
        }

        [TestMethod]
        public void CreateEnvioTest()
        {
            EnvioDTO envio1 = new EnvioDTO { DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO creado = sut.Create(envio1);
            listado.Add(creado.EnvioId);
            Assert.IsNotNull(creado);
            Assert.AreNotEqual(-1, creado.EnvioId);
        }

        [TestMethod]
        public void ReadEnvioTest()
        {
            EnvioDTO envio1 = new EnvioDTO { DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO creado = sut.Create(envio1);
            listado.Add(creado.EnvioId);
            EnvioDTO leido = sut.Read(creado.EnvioId);
            Assert.IsNotNull(leido);
            Assert.AreEqual(leido.EnvioId, creado.EnvioId);
        }

        [TestMethod]
        public void ListEnvioTest()
        {
            int cuentaActual = sut.List().Count;
            EnvioDTO envio1 = new EnvioDTO { DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO create = sut.Create(envio1);
            EnvioDTO create2 = sut.Create(envio1);
            EnvioDTO create3 = sut.Create(envio1);
            listado.Add(create.EnvioId);
            listado.Add(create2.EnvioId);
            listado.Add(create3.EnvioId);
            IList<EnvioDTO> lista = sut.List();
            Assert.AreEqual(cuentaActual+3, lista.Count);
        }

        [TestMethod]
        public void UpdateEnvioTest()
        {
            EnvioDTO envio1 = new EnvioDTO { DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO creado = sut.Create(envio1);
            listado.Add(creado.EnvioId);
            creado.DestinatarioId = 2;
            creado.Estado = 1;
            EnvioDTO actualizado = sut.Update(creado);
            Assert.IsNotNull(actualizado);
            Assert.AreNotEqual(envio1.DestinatarioId, actualizado.DestinatarioId);
            Assert.AreNotEqual(envio1.Estado, actualizado.Estado);
        }
        [TestMethod]
        public void DeleteEnvioTest()
        {
            EnvioDTO envio1 = new EnvioDTO { DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO creado = sut.Create(envio1);
            listado.Add(creado.EnvioId);
            EnvioDTO borrado = sut.Delete(creado.EnvioId);
            Assert.AreEqual(creado.EnvioId, borrado.EnvioId);
            Assert.AreEqual(2, borrado.Estado);
            EnvioDTO leido = sut.Read(borrado.EnvioId);
            Assert.IsNull(leido);

        }
    }
}
