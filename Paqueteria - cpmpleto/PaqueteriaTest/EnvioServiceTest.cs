using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Paqueteria.Model;
using Paqueteria.Repository;
using Paqueteria.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaqueteriaTest
{
    [TestClass]
    public class EnvioServiceTest
    {
        private Mock<IEnvioRepository> _envioRepository;
        private IEnvioService sut;
        [TestInitialize]
        public void TestInicializa()
        {
            _envioRepository = new Mock<IEnvioRepository>();
            sut = new EnvioService(_envioRepository.Object);
        }
        [TestMethod]
        public void CreateEnvioTest()
        {
            _envioRepository.Setup(repo => repo.Create(It.IsAny<EnvioDTO>()))
                .Returns<EnvioDTO>(dto => { dto.EnvioId = 1; return dto; });
            EnvioDTO envio = new EnvioDTO { EnvioId = -1, DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO creado = sut.Create(envio);
            Assert.IsNotNull(creado);
            _envioRepository.Verify(repo => repo.Create(It.IsAny<EnvioDTO>()), Times.Once());
        }
        [TestMethod]
        public void ReadEnvioTest()
        {
            _envioRepository.Setup(repo => repo.Read(It.IsAny<long>()))
                .Returns((long id) => { return new EnvioDTO { EnvioId = id, Estado = 0, FechaEntrega = DateTime.Now, DestinatarioId = 1 }; });
            EnvioDTO leido = sut.Read(1);
            Assert.IsNotNull(leido);
            _envioRepository.Verify(repo => repo.Read(It.IsAny<long>()), Times.Once());
        }
        [TestMethod]
        public void ListEnvioTest()
        {
            _envioRepository.Setup(repo => repo.List())
                .Returns(
                () =>
                {
                    IList<EnvioDTO> listado = new List<EnvioDTO>();
                    listado.Add(new EnvioDTO { EnvioId = 1, Estado = 0, FechaEntrega = DateTime.Now, DestinatarioId = 1 });
                    listado.Add(new EnvioDTO { EnvioId = 2, Estado = 0, FechaEntrega = DateTime.Now, DestinatarioId = 1 });
                    return listado;
                }
            );
            IList<EnvioDTO> lista = sut.List();
            Assert.AreEqual(2, lista.Count);
            _envioRepository.Verify(repo => repo.List(), Times.Once());
        }
        [TestMethod]
        public void UpdateEnvioTest()
        {
            _envioRepository.Setup(repo => repo.Update(It.IsAny<EnvioDTO>())).Returns<EnvioDTO>(edto => edto);
            EnvioDTO update = new EnvioDTO { EnvioId = 1, DestinatarioId = 1, Estado = 0, FechaEntrega = DateTime.Now };
            EnvioDTO updated = sut.Update(update);
            Assert.IsNotNull(updated);
            _envioRepository.Verify(repo => repo.Update(It.IsAny<EnvioDTO>()), Times.Once());
        }
        [TestMethod]
        public void DeleteEnvioTest()
        {
            _envioRepository.Setup(repo => repo.Delete(It.IsAny<long>())).Returns((long id) => { return new EnvioDTO { EnvioId = id, DestinatarioId = 1, Estado = 2, FechaEntrega = DateTime.Now }; });
            EnvioDTO deleted = sut.Delete(1);
            Assert.IsNotNull(deleted);
            _envioRepository.Verify(repo => repo.Delete(It.IsAny<long>()), Times.Once());
        }
    }
}
