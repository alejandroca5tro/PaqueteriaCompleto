using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paqueteria.Model;
using Paqueteria.Service;
using Paqueteria.Repository;
using Moq;
namespace PaqueteriaTest
{

    [TestClass]
    public class PaqueteServiceTest
    {
        private PaqueteService sut;
        private Mock<IPaqueteRepository> mockPaqueteRepo;

        [TestInitialize]
        public void Inicializa()
        {
            mockPaqueteRepo = new Mock<IPaqueteRepository>();

            sut = new PaqueteService(mockPaqueteRepo.Object);

        }


        [TestMethod]
        public void CreatePaqueteTest()
        {
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "rojo", EnvioId = 1, Peso = 10, PaqueteId = 1, Tamanyo = 100 };
            mockPaqueteRepo.Setup(PaqueteRepo => PaqueteRepo.Create(paquete1)).Returns(paquete1);
            PaqueteDTO resultado = sut.Create(paquete1);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void ListPaqueteTest()
        {
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "rojo", EnvioId = 1, Peso = 10, PaqueteId = 1, Tamanyo = 100 };
            mockPaqueteRepo.Setup(PaqueteRepo => PaqueteRepo.List())
                .Returns(
                () =>
                {
                    IList<PaqueteDTO> listado = new List<PaqueteDTO>();
                    listado.Add(new PaqueteDTO { Color = "rojo", EnvioId = 1, Peso = 10, PaqueteId = 1, Tamanyo = 100 });
                    listado.Add(new PaqueteDTO { Color = "rojo", EnvioId = 1, Peso = 10, PaqueteId = 1, Tamanyo = 100 });
                    return listado;
                }
             );
            IList<PaqueteDTO> lista = sut.List();
            Assert.AreEqual(2, lista.Count);

        }
        [TestMethod]
        public void ReadPaqueteTest()
        {
            PaqueteDTO paquete3 = new PaqueteDTO { Color = "Azul", EnvioId = 3, Peso = 20, PaqueteId = 3, Tamanyo = 300 };
            mockPaqueteRepo.Setup(PaqueteRepo => PaqueteRepo.Read(paquete3.PaqueteId)).Returns(paquete3);
            PaqueteDTO resultado = sut.Read(3);
            Assert.IsNotNull(resultado);

        }
        [TestMethod]
        public void UpdatePaqueteTest()
        {
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "rojo", EnvioId = 1, Peso = 10, PaqueteId = 1, Tamanyo = 100 };
            mockPaqueteRepo.Setup(PaqueteRepo => PaqueteRepo.Update(paquete1)).Returns(paquete1);
            PaqueteDTO resultado = sut.Update(paquete1);
            Assert.IsNotNull(resultado);
        }
        [TestMethod]
        public void DeletePaqueteTest()
        {
            PaqueteDTO paquete1 = new PaqueteDTO { Color = "Negro", EnvioId = 3, Peso = 160, PaqueteId = 5, Tamanyo = 10 };
            mockPaqueteRepo.Setup(PaqueteRepo => PaqueteRepo.Delete(paquete1.PaqueteId)).Returns(paquete1);
            PaqueteDTO resultado = sut.Delete(paquete1.PaqueteId);
            Assert.IsNotNull(resultado);
        }
    }
}
