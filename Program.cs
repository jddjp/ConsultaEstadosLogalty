using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ApiBack.Models;
using ConsultaEstadosLogalty.Models;
using LGT_SDK_NETCORE.Client;
using LGT_SDK_NETCORE.Entities;
using LGT_SDK_NETCORE.Util;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimuladorBackOffice.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ConsultaEstadosLogalty
{
    class Program
    {

      
        static void Main(string[] args)
        {
            var datos = resultado();
            
            //datos.ForEach(item =>item.guid)

            ContratacionLogalty example = new ContratacionLogalty();
            object[] guids = new object[datos.Count];
            int count = 0;
            datos.ForEach(
             
                   datos1 => guids[count++] = datos1.guid 
                );
            //datos1 => Console.WriteLine("\n" + datos1.guid + "\n"),
            //guids[0] = "001001-9996-000000001175792.par"; //input

            //            var fibNumbers = new List<int> { 0, 1, 1, 2, 3, 5, 8, 13 };
            //            int count1= 0;
            //            foreach (int element in fibNumbers)
            //            {
            //                count++;
            //1                Console.WriteLine($"Element #{count1}: {element}");
            //            }
            //            Console.WriteLine($"Number of elements: {count1}");
            var _context = new ApiBckContext();
            WSBusData WSData = new WSBusData();
            XmlDocument xmlRequest = WSData.DataStateRequestDocumentBuilder(guids);

            XMLSign utlSignXml = new XMLSign();
            XmlDocument xmlSigned = utlSignXml.BuildSigned(xmlRequest, example.Certificado);
            LGT_SDK_NETCORE.Entities.DataResponse dataResponse = WSData.PostRequest(xmlSigned, example.Certificado, example.ConnectSetting);

            var guardardatos = resultadoConsulta(dataResponse.ResponseXml);
            Console.WriteLine(("Response: \n"
                            + (dataResponse.ResponseXml)));
            Console.WriteLine(("State: "
                            + (dataResponse.Estado)));

            Console.WriteLine(guardardatos);

            Console.WriteLine("Press <Enter> to exit SimpleStatesExample");
            Environment.Exit(0);
            //Console.ReadLine();

        }

        public static List<ResultadoFeedback> resultado()
        {
            ContratacionLogalty example = new ContratacionLogalty();

            var _context = new ApiBckContext();
            var r = _context.ResultadoFeedback.FromSqlRaw<ResultadoFeedback>("EXEC firma_BuscarGuidFinalizadosSinFirma").ToList();
             _context.SaveChanges();
            return r;
        }

        public static List<State>resultadoConsulta(Object parameters)
        {
            ContratacionLogalty example = new ContratacionLogalty( );
            var _context = new ApiBckContext();
            var Respuestaxml = new SqlParameter("@Xmlrespuesta", parameters);

            var r = _context.State.FromSqlRaw<State>("EXEC GuardarConsultaED @Xmlrespuesta",
              Respuestaxml ).ToList();
            _context.SaveChanges();
            return r;
        }

    }
}
