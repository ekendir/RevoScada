using Newtonsoft.Json;
using NUnit.Framework;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ServiceProcess;
using Revo.Core.Data;

namespace RevoScada.DesktopApplication.Test
{

    [TestFixture]
    public class DesktopAppGeneral
    {
        CacheManager _mainCacheManager;

        [SetUp]
        public void Init()
        {
    //         ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\Application.rsconfig", true);


          
        }



        class A
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        [Test]
        public void DecimalTests()
        {
            var parsedDecimal=NumericManipulation.ParseDecimalNumber(1.98765432m, 8);
            int integralPart = parsedDecimal.IntegralPart;
            int decimalPart = parsedDecimal.DecimalPart;
          

            float f = float.MinValue;
            decimal d = decimal.MinValue;
            System.Diagnostics.Debug.WriteLine("The Minimum Range of the Decimal Data " + "Type is : {0} ", Decimal.MaxValue);
            System.Diagnostics.Debug.WriteLine("The Minimum Range of the Float Data " + "Type is : {0} ", Single.MaxValue);
            System.Diagnostics.Debug.WriteLine("The Minimum Range of the Decimal Data " + "Type is : {0} ", Double.MaxValue);
            System.Diagnostics.Debug.WriteLine("Exponent Form : The Minimum Range of Decimal " + "Data Type  is : {0:E}", Decimal.MaxValue);
            System.Diagnostics.Debug.WriteLine("Exponent Form : The Minimum Range of Float " + "Data Type  is : {0:E}", Single.MaxValue);
            System.Diagnostics.Debug.WriteLine("Exponent Form : The Minimum Range of Double " + "Data Type  is : {0:E}", Double.MaxValue);

            d = Convert.ToDecimal(f);


        }
        [Test]
        [TestCase("2")]
        public void Get_furnace_load_number(string nextLoadNumberValue)
        {
            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            string loadNumberFromDB = applicationPropertyService.GetByName("LastLoadNumber").Value;

            string furnaceLoadNumber = $"{ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceName}-{loadNumberFromDB}";

            string furnaceLoadNumberTestCase = $"{ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceName}-{nextLoadNumberValue}";

            Assert.IsTrue(furnaceLoadNumber == furnaceLoadNumberTestCase);

        }

        [Test]
        public void Save_furnace_load_number_next()
        {

            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var entity = applicationPropertyService.GetByName("LastLoadNumber");


            int nextValue = Convert.ToInt32(entity.Value) + 1;


            entity.Value = nextValue.ToString();


            applicationPropertyService.Update(entity);


            entity = applicationPropertyService.GetByName("LastLoadNumber");


            Assert.IsTrue(Convert.ToInt32(entity.Value) == nextValue);

        }
        
        [Test]
        public void UI_states()
        {
            var removeCurrentButtonVisibilityCacheResult=    _mainCacheManager.GetString("UI_EnterParts_RemoveCurrentButton");
            BatchCurrentState removeCurrentButtonVisibility = (BatchCurrentState)Convert.ToInt32(removeCurrentButtonVisibilityCacheResult ?? "0");
            _mainCacheManager.Set("UI_EnterParts_RemoveCurrentButton", (int)BatchCurrentState.Finished);
        }

        [Test]
        public void Stop_service()
        {

 
            // Check whether the Alerter service is started.
            ServiceController sc = new ServiceController();
            sc.ServiceName = "Redis";
            Console.WriteLine("The Alerter service status is currently set to {0}",
            sc.Status.ToString());

            sc.Stop();

            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                // Start the service if the current status is stopped.
                Console.WriteLine("Starting the Alerter service...");
                try
                {
                    // Start the service, and wait until its status is "Running".
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);

                    // Display the current service status.
                    Console.WriteLine("The Alerter service status is now set to {0}.",
                                       sc.Status.ToString());
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Could not start the Alerter service.");
                }
            }



        }


      


        

    }
}

