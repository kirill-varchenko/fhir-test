using System;
using System.Collections.Generic;

namespace FHIRTest;

public static class Program
{
    private const string defaultFhirServer = "https://spark.incendi.no/fhir/";

    private static string GenerateFilenameFromUrl(string url)
    {
        Uri uri = new Uri(defaultFhirServer);
        return $"{uri.Host}.txt";
    }

    private static async Task WriteNameSetAsync(string fileName, HashSet<string> names)
    {
        List<string> namesList = names.ToList();
        namesList.Sort();
        await File.WriteAllLinesAsync(fileName, namesList);
    }

    static async Task Main()
    {
        Console.WriteLine($"FHIR server: {defaultFhirServer}");
        FhirPatientNameFetcher patientNameFetcher = new FhirPatientNameFetcher(defaultFhirServer); 
        try
        {
            HashSet<string> uniqueGivenNames = await patientNameFetcher.FetchNamesAsync();   
            string fileName = GenerateFilenameFromUrl(defaultFhirServer);
            await WriteNameSetAsync(fileName, uniqueGivenNames);
            Console.WriteLine($"Names are written to: {fileName}");
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error during REST operations:");
            Console.WriteLine(ex.ToString());
        }
    }
}