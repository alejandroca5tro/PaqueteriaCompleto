using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using Paqueteria;
using PaqueteriaTest;


namespace SeleniumTests
{
    [TestClass]
    public class CreateEnvioTest
    {



        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private static IPaqDBFactory dbFactory;
        private string baseURL;
        private bool acceptNextAlert = true;
        private Process _iisProcess;
        private string CurrentPath;
        private string AppLocation = @"\PaqueteriaWeb\obj\Debug\Package\PackageTmp";
        private int Port = 10400;

        [ClassInitialize]
        public static void TestClass(TestContext testctx)
        {
            dbFactory = new PaqDropCreateDBFactory();
            using (var ctx = dbFactory.GetInstance())
            {
                ctx.Database.Initialize(true);
            }
        }

        [TestInitialize]
        public void SetupTest()
        {
            var thread = new Thread(StartIisExpress) { IsBackground = true };
            thread.Start();
            Thread.Sleep(2000);
            driver = new FirefoxDriver();
            baseURL = "http://localhost:10400/";
            verificationErrors = new StringBuilder();
            Thread.Sleep(2000);
        }

        [TestCleanup]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
                _iisProcess.Kill();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [TestMethod]
        public void TheCreateEnvioTest()
        {
            driver.Navigate().GoToUrl(baseURL + "/Index.html#/");
            Thread.Sleep(2000);
            driver.FindElement(By.LinkText("Añadir Envio")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.Id("DestinatarioId")).Clear();
            driver.FindElement(By.Id("DestinatarioId")).SendKeys("1");
            driver.FindElement(By.Id("FechaEntrega")).Clear();
            driver.FindElement(By.Id("FechaEntrega")).SendKeys("12/12/2014");
            new SelectElement(driver.FindElement(By.Id("Estado"))).SelectByText("Pendiente");
            driver.FindElement(By.XPath("//div[@id='main']/div/button")).Click();
            Thread.Sleep(20000);
            Assert.AreEqual("Editar", driver.FindElement(By.LinkText("Editar")).Text);
        }

        [TestMethod]
        public void TheBorrarEnvioTest()
        {
            driver.Navigate().GoToUrl(baseURL + "/Index.html#/");
            Thread.Sleep(2000);
            driver.FindElement(By.LinkText("Añadir Envio")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.Id("DestinatarioId")).Clear();
            driver.FindElement(By.Id("DestinatarioId")).SendKeys("1");
            driver.FindElement(By.Id("FechaEntrega")).Clear();
            driver.FindElement(By.Id("FechaEntrega")).SendKeys("12/12/2014");
            new SelectElement(driver.FindElement(By.Id("Estado"))).SelectByText("Pendiente");
            driver.FindElement(By.XPath("//div[@id='main']/div/button")).Click();
            Thread.Sleep(4000);
            driver.FindElement(By.LinkText("Editar")).Click();
            Thread.Sleep(4000);
            String idCreado = driver.FindElement(By.Id("idenvio")).GetAttribute("value");
            driver.FindElement(By.Id("DestinatarioId")).Clear();
            driver.FindElement(By.Id("DestinatarioId")).SendKeys("2");
            driver.FindElement(By.Id("FechaEntrega")).Clear();
            driver.FindElement(By.Id("FechaEntrega")).SendKeys("2014-11-12T00:00:00");
            driver.FindElement(By.XPath("//div[@id='main']/div/button[2]")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("El envio "+idCreado+" se ha modificado correctamente", CloseAlertAndGetItsText());
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
        private void StartIisExpress()
        {
            CurrentPath = Directory.GetCurrentDirectory();
            CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf("\\"));
            CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf("\\"));
            CurrentPath = CurrentPath.Substring(0, CurrentPath.LastIndexOf("\\"));
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardInput = true,
                Arguments = string.Format("/path:\"{0}\" /port:{1}", CurrentPath + AppLocation, Port)
            };
            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"])
                              ? startInfo.EnvironmentVariables["programfiles(x86)"]
                              : startInfo.EnvironmentVariables["programfiles"];

            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

            try
            {
                _iisProcess = new Process { StartInfo = startInfo };
                _iisProcess.Start();

                _iisProcess.WaitForExit();
                return;
            }
            catch (Exception e)
            {
                e = null;
                //_iisProcess.CloseMainWindow();
                //_iisProcess.Dispose();
            }
        }
    }
}