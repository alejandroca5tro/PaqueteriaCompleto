using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paqueteria.Repository;
using Paqueteria.Model;
using Paqueteria;
using Paqueteria.Conversores;
using System.Data.SqlClient;
namespace PaqueteriaTest
{

    [TestClass]
    public class PaqueteriaRepositoryTest
    {
        private PaqueteRepository sut;
        private static IPaqDBFactory dbFactory = new PaqDropCreateDBFactory();
        private IList<long> Listado = new List<long>();
        private static long EnvioIdTest = -1;
        [ClassInitialize]
        public static void TestClass(TestContext ctx)
        {
            using (var context = dbFactory.GetInstance())
            {
                Envio forId = context.Envios.Add(new Envio { DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now });
                context.SaveChanges();
                EnvioIdTest = forId.EnvioId;
            }
        }
        [TestInitialize]
        public void TestInicializa()
        {
            sut = new PaqueteRepository(new PaqueteConversor(), dbFactory);
            using (var ctx = dbFactory.GetInstance())
            {
                ctx.Database.Initialize(true);
            }
        }


        [TestCleanup]
        public void TestCleanUp()
        {
            using (var ctx = dbFactory.GetInstance())
            {
                foreach (long ctxId in Listado)
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM Paquetes where PaqueteId = @id ", new SqlParameter("@id", ctxId));
                }
                ctx.SaveChanges();
            }
        }
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            using (var ctx = dbFactory.GetInstance())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM Envios where EnvioId = @id ", new SqlParameter("@id", EnvioIdTest));
                ctx.SaveChanges();
            }
        }

        [TestMethod]
        public void CreatePaqueteTest()
        {
            //EnvioDTO envio1 = new EnvioDTO { DestinatarioId = 1, FechaEntrega = DateTime.Now,  Estado=1, EnvioId=1 };
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "rojo", EnvioId = EnvioIdTest, Peso = 10, Tamanyo = 100 };
            paquete1 = sut.Create(paquete1);
            Listado.Add(paquete1.PaqueteId);
            Assert.IsNotNull(paquete1);
            Assert.AreNotEqual(-1, paquete1.PaqueteId);
        }

        [TestMethod]
        public void ListPaqueteTest()
        {
            int cuentaActual = sut.List().Count;
            // PaqueteDTO paquete1 = new PaqueteDTO { Color = "rojo", EnvioId = 1, Peso = 10, PaqueteId = 1, Tamanyo = 100 };
            IList<PaqueteDTO> listado = sut.List();
            listado.Add(new PaqueteDTO { Color = "rojo", EnvioId = EnvioIdTest, Peso = 10, PaqueteId = 1, Tamanyo = 100 });
            listado.Add(new PaqueteDTO { Color = "rojo", EnvioId = EnvioIdTest, Peso = 10, PaqueteId = 1, Tamanyo = 100 });
            Assert.AreEqual(cuentaActual+2, listado.Count);
        }
        [TestMethod]
        public void ReadPaqueteTest()
        {
            PaqueteDTO paquete3 = new PaqueteDTO { Color = "Azul", EnvioId = EnvioIdTest, Peso = 20, Tamanyo = 300 };
            PaqueteDTO resultado = sut.Create(paquete3);
            Listado.Add(resultado.PaqueteId);
            paquete3 = sut.Read(resultado.PaqueteId);
            Assert.IsNotNull(paquete3);

        }
        [TestMethod]
        public void UpdatePaqueteTest()
        {
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "rojo", EnvioId = EnvioIdTest, Peso = 10, Tamanyo = 100 };
            PaqueteDTO resultado = sut.Create(paquete1);
            Listado.Add(resultado.PaqueteId);
            paquete1 = null;
            resultado.Peso = 50;
            resultado.Color = "negro";
            paquete1 = sut.Update(resultado);
            Assert.IsNotNull(paquete1);
            Assert.AreEqual(paquete1.PaqueteId, resultado.PaqueteId);
            Assert.AreEqual("negro", paquete1.Color);
            Assert.AreEqual(50, paquete1.Peso);

        }
        [TestMethod]
        public void DeletePaqueteTest()
        {
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "Negro", EnvioId = EnvioIdTest, Peso = 160, PaqueteId = 1, Tamanyo = 10 };
            PaqueteDTO paquetecreate = sut.Create(paquete1);
            Listado.Add(paquetecreate.PaqueteId);
            PaqueteDTO resultado = sut.Delete(paquete1.PaqueteId);
            Assert.IsNull(resultado);
        }

    }
}
