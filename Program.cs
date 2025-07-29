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
            List<Patient> patients = GetPatients(client);
            Console.WriteLine($"Found: {patients.Count} patients!!");
           
        }

        static List<Patient> GetPatients(FhirClient c, string[] patientCriteria = null, int max = 20, bool onlywithEncounters = false)
        {

            List<Patient> patients = new List<Patient>();
            Bundle patientBundle;
            Bundle patientBundle2 = c.Search<Patient>(new string[] { "_summary=count" });

            if (patientCriteria == null || patientCriteria.Length == 0)
            {
                patientBundle = c.Search<Patient>();
            }
            else
            {
                patientBundle = c.Search<Patient>(patientCriteria);
            }

            List<string> pWithEncouter = new List<string>();

            while (patientBundle != null)
            {
                countEntries(patientBundle2);
                countPatiensInBundle(patientBundle);
                foreach (Bundle.EntryComponent entry in patientBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        Patient patient = (Patient)entry.Resource;
                        Bundle encounterBundle = c.Search<Encounter>(new string[]{
                                $"patient=Patient/{patient.Id}",
                        });

                        if (onlywithEncounters && (encounterBundle.Total == 0))
                        {
                            continue;
                        }
                        patients.Add(patient);

                        Console.WriteLine($"{patients.Count}\n{entry.FullUrl}\n");
                        Console.WriteLine($"Id: {patient.Id}\n");

                        if (patient.Name.Count > 0)
                        {
                            Console.WriteLine($"Name: {patient.Name[0].ToString()}\n");
                        }

                        if (encounterBundle.Total > 0)
                        {
                            Console.WriteLine($"Entry Count: {encounterBundle.Entry.Count} Encounter Total: {encounterBundle.Total}");
                        }
                    }

                    if (patients.Count >= max)
                    {
                        break;
                    }
                }
                if (patients.Count >= max)
                {
                    break;
                }
                patientBundle = c.Continue(patientBundle);
            }
            return patients;
        }
        public static void countEntries(Bundle patientsBundle)
        {
            Console.WriteLine($"Total Entries: {patientsBundle.Total}");
        }
        public static void countPatiensInBundle(Bundle patientsBundle)
        {
            Console.WriteLine($"Total Patients in Bundle: {patientsBundle.Entry.Count}\n");
        }
        public static void countPatients(List <Patient> l)
        {
            Console.WriteLine($"Found: {l.Count} patients!!");
        }
    }
}
