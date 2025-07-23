using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;

namespace Project01
{
    public static class FhirProject
    {
        private static readonly Dictionary<string, string> fhir_Servers = new Dictionary<string, string>()
        {
            {"PublicHapi","http://hapi.fhir.org/baseDstu3"},
            {"Local","http://localhost:8080/fhir"},
        };
        private static readonly string fhirServer = fhir_Servers["Local"];
        static void Main(string[] args)
        {
            FhirClient client = new FhirClient(fhirServer)
            {
                Settings =
                {
                    PreferredFormat = ResourceFormat.Json,
                    PreferredReturn = Prefer.ReturnRepresentation
                }
            };
            
            }
        }
    }
