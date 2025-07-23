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
        private static readonly string fhirServer = fhir_Servers["PublicHapi"];
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
            var parameters = new string[] { "_summary=count" };
            //condition to search for patients with the name test
            var parameters2 = new string[] { "name=test" };
            Bundle patientBundle = client.Search<Patient>(parameters);
            Bundle patientBundleE = client.Search<Patient>(null);
            Bundle patientBundleS = client.Search<Patient>(parameters2);


            Console.WriteLine($"Total:{patientBundle.Total}");
            Console.WriteLine($"Entry:{patientBundleE.Entry.Count}\n");

            int entrycount = 1;
            bool foundPatients = false;

            while (patientBundleS != null && !foundPatients)
            {
                foreach (Bundle.EntryComponent entry in patientBundleS.Entry)
                {
                    if (entry.Resource is Patient patient)
                    {
                        foundPatients = true; // break outer loop after this batch

                        Console.WriteLine($"Entry: {entrycount}\nID: {patient.Id}");

                        if (patient.Name.Count > 0)
                        {
                            Console.WriteLine($"Name: {patient.Name[0]}\nUrl: {entry.FullUrl}");
                        }

                        var encounterBundle = client.Search<Encounter>(new string[] { $"patient=Patient/{patient.Id}" });
                        Console.WriteLine($"EncountersTotal: {encounterBundle.Total}\n");
                        entrycount++;
                    }
                }
                patientBundleS = client.Continue(patientBundleS);
            }
        }
    }
}