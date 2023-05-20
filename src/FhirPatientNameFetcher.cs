using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace FHIRTest;

public class FhirPatientNameFetcher
{
    private FhirClient fhirClient;
    public FhirPatientNameFetcher(string serverUrl)
    {
        fhirClient = GetFhirClient(serverUrl);
    }

    private FhirClient GetFhirClient(string serverUrl)
    {
        FhirClientSettings fhirClientSettings = new FhirClientSettings
        {
            PreferredFormat = ResourceFormat.Json,
            Timeout = 10_000,
            VerifyFhirVersion = true
        };
        
        return new FhirClient(serverUrl, fhirClientSettings);
    }

    public async Task<HashSet<string>> FetchNamesAsync()
    {
        HashSet<string> uniqueGivenNames = new HashSet<string>();
        
        Bundle? patientBundle = await fhirClient.SearchAsync<Patient>();

        while( patientBundle != null )
        {
            foreach(Bundle.EntryComponent entry in patientBundle.Entry)
            {
                Patient patient = (Patient) entry.Resource;

                foreach(HumanName humanName in patient.Name)
                {
                    uniqueGivenNames.UnionWith(humanName.Given);
                }
            }

            patientBundle = await fhirClient.ContinueAsync(patientBundle);
        }

        return uniqueGivenNames;
    }
}