using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

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
        /// <summary>
        /// Main entry point for the programm
        /// </summary>
        /// <param name="args"></param>

        static int Main(string[] args)
        {
            FhirClient client = new FhirClient(fhirServer)
            {
                Settings =
                {
                    PreferredFormat = ResourceFormat.Json,
                    PreferredReturn = Prefer.ReturnRepresentation
                }
            };
            CreatePatient(client, "Santos", "Orlando", 2004, 02, 26);
            List <Patient> patients = GetPatients(client);
            Console.WriteLine($"Found: {patients.Count} patients!!");

            //delete all the patients in expetion the one we created
            string? firstId = null;
            foreach (Patient patient in patients)
            {
                if (string.IsNullOrEmpty(firstId))
                {
                    firstId = patient.Id;
                    continue;
                }
                    DeletPatient(client, patient.Id);
            }

            Patient firstPatient = ReadPatient(client, firstId);
            Console.WriteLine($"Read Back Patient: {firstPatient.Name[0].ToString()}");

            Patient updated = UpdatePatient(client, firstPatient);
            Patient ReadFinal = ReadPatient(client, firstId);
            


            return 0;
        }

        /// <summary>
        /// Get a list of patients matching a specified criteria
        /// </summary>
        /// <param name="c"></param>
        /// <param name="patientCriteria"></param>
        /// <param name="max"> The maximum number of patients to return (default:20)</param>
        /// <param name="onlywithEncounters">Flag to only return patient With Enconters(default:false)</param>
        /// <returns></returns>
    
        static List<Patient> GetPatients(FhirClient c, string[]? patientCriteria = null, int max = 20, bool onlywithEncounters = false)
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

                        Console.WriteLine($"Url:{entry.FullUrl}");
                        Console.WriteLine($"Id:{patient.Id}");

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

        /// <summary>
        /// crate a patient with the specified name and birthdate
        /// </summary>
        /// <param name="c"></param>
        /// <param name="FamilyName"></param>
        /// <param name="GivenName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>

        static void CreatePatient(FhirClient c, string FamilyName, string GivenName, int year, int month, int day)
        {
            Patient toCreate = new Patient()
            {
                Name = new List<HumanName>()
                {
                  new HumanName(){
                    Family=FamilyName,
                    Given=new List<string>(){
                        GivenName,
                    },
                  }
                },
                BirthDateElement = new Date(year, month, day),
            };
            Patient created = c.Create<Patient>(toCreate);
            Console.WriteLine($"Created Patient: {created.Id}");
        }
        static void DeletPatient(FhirClient c, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            Console.WriteLine($"Deleting Patient: {id}");
            c.Delete($"Patient/{id}");
        }
        /// <summary>
        /// delete a patient especified by an ID
        /// </summary>
        /// <param name="patientsBundle"></param>

        static Patient ReadPatient(FhirClient c, string id)
        {
           if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            return c.Read<Patient>($"Patient/{id}");
        }

        static Patient UpdatePatient(FhirClient c, Patient patient)
        {
            patient.Telecom.Add(new ContactPoint()
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = "555 555 555",
                Use = ContactPoint.ContactPointUse.Home,
            });

            patient.Gender = AdministrativeGender.Unknown;
            return c.Update<Patient>(patient);
        }
      
        public static void countEntries(Bundle patientsBundle)
        {
            Console.WriteLine($"Total Entries: {patientsBundle.Total}");
        }
        public static void countPatiensInBundle(Bundle patientsBundle)
        {
            Console.WriteLine($"Total Patients in Bundle: {patientsBundle.Entry.Count}\n");
        }
        public static void countPatients(List<Patient> l)
        {
            Console.WriteLine($"Found: {l.Count} patients!!");
        }
    }
}
