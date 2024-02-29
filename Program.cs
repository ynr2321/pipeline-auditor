using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Security.AccessControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pipelineAuditor.Models;


namespace pipelineAuditor
{
    public class Program
    {
        // Fields ========================================================================================================================================
        private static string PersonalAccessToken = ConfigurationManager.AppSettings["ApiKey"]; // read-only token created for the pipeline-auditor
        private static string Org = ConfigurationManager.AppSettings["Organisation"];
        private static string Project = ConfigurationManager.AppSettings["Project"];

        // Constructing endpoint for LIST pipelines
        private static string pipelineListEndpoint =
            $"https://dev.azure.com/{Org}/{Project}/_apis/pipelines?api-version=7.1";
     
        // MAIN ========================================================================================================================================
        public static void Main(string[] args)
        {
            // Check if any command-line arguments are provided
            int numSelected = new int();
            if (args.Length > 0)
            {
                // The first argument is treated as the name
                int num = int.Parse(args[0]);
                Console.WriteLine($"The script is about to be run with input: {num}");
                numSelected = num;
            }
            else
            {
                Console.WriteLine("Please provide a number as a command-line argument:");
                Console.WriteLine("< 0 > for latest pipelines");
                Console.WriteLine("< -1 > for all pipelines");
                Console.WriteLine("< n > for latest up to the nth pipeline"); ;
                Console.WriteLine();
                Console.WriteLine("RUNNING DEFAULT CASE: Audit latest builds");
                numSelected = 0;
            }

            if (numSelected == 1)
            {
                numSelected = 0;
            }

            // Tool classes
            ResponseParser Parser = new ResponseParser();
            RestApiClient Client = new RestApiClient(PersonalAccessToken);
            EndpointGenerator EPG = new EndpointGenerator();

            // Initialising space to store responses
            List<string> RunMetaDataResponses = new List<string>();
            var ResultsList = new List<Object>();
            Dictionary<String, Object> ResultsDict = new Dictionary<String, Object>();


            // getting list of pipelines
            var listpipelineResponse = Client.GetResponse(pipelineListEndpoint).Result;
            List<string> pipelineList = Parser.GetPipelineIds(listpipelineResponse); // list of Ids
            List<string> pipelineNames= Parser.GetPipelineNames(listpipelineResponse); // list of Names

            // getting endpoint to list runs for each pipeline
            List<string> RunMetaDataEpList = EPG.GetListRunsEps(pipelineList);


            // getting metadata response for each pipeline
            foreach (var EndPoint in RunMetaDataEpList)
            {
                RunMetaDataResponses.Add(Client.GetResponse(EndPoint).Result);
                Console.WriteLine(".");
            }

        
            if (numSelected > 1)
            {
                foreach (int num in Enumerable.Range(0, numSelected))
                {
                    Console.WriteLine($"\nGathering warnings for run {num} \n");
                    // extracting buildIds for latest runs
                    var BuildIds = Parser.GetBuildIds(RunMetaDataResponses, num);

                    // getting end points to list records (task info) for each run
                    var RunInfoEndPoints = EPG.GetRunInfoEps(BuildIds);

                    // getting run info response (containing count and records) for each run
                    List<string> RunInfoResponses = new List<string>();
                    foreach (var EndPoint in RunInfoEndPoints)
                    {
                        if (EndPoint != "NoRunsExistYet")
                        {
                            RunInfoResponses.Add(Client.GetResponse(EndPoint).Result);
                            Console.WriteLine($"Made API call to endpoint: {EndPoint}");
                        }
                        else
                        {
                            RunInfoResponses.Add("NoRunsExistYet");
                        }
                    }

                    var LatestRunsParsed = Parser.GetRunResponses(RunInfoResponses);

                    var AllWarnings = Parser.GetAllWarnings(RunInfoResponses);

                    ResultsList.Add(AllWarnings);

                }

                // POST PROCESSING
                // for each nth run of each pipeline, add a dictionary of pipeline name and results (dict of task names: warnings)
                foreach (List<Object> result in ResultsList)
                {
                    int index = ResultsList.IndexOf(result);
                    
                    for (int i = 0; i < pipelineNames.Count; i++)
                    {
                        ResultsDict.Add(pipelineNames[i] + " (Run " + index.ToString() + ")", result[i]);
                        Console.WriteLine($"Adding {pipelineNames[i] + "Run" + index.ToString()}: Result ");
                    }
                }

            }
            else if (numSelected == 1 || numSelected == 0)
            {
                Console.WriteLine($"\nGathering warnings for run latest pipeline runs \n");
                // extracting buildIds for latest runs
                var BuildIds = Parser.GetBuildIds(RunMetaDataResponses, 0);

                // getting end points to list records (task info) for each run
                var RunInfoEndPoints = EPG.GetRunInfoEps(BuildIds);

                // getting run info response (containing count and records) for each run
                List<string> RunInfoResponses = new List<string>();
                foreach (var EndPoint in RunInfoEndPoints)
                {
                    if (EndPoint != "NoRunsExistYet")
                    {
                        RunInfoResponses.Add(Client.GetResponse(EndPoint).Result);
                        Console.WriteLine($"Made API call to endpoint: {EndPoint}");
                    }
                    else
                    {
                        RunInfoResponses.Add("NoRunsExistYet");
                    }
                }

                var LatestRunsParsed = Parser.GetRunResponses(RunInfoResponses);

                var AllWarnings = Parser.GetAllWarnings(RunInfoResponses);

                for (int i = 0; i < pipelineNames.Count; i++)
                {
                    ResultsDict.Add(pipelineNames[i] + " (Run 0)", AllWarnings[i]);
                    Console.WriteLine($"Adding {pipelineNames[i] + "_Run_0"}: Result ");
                }

            }
   
            Console.WriteLine("Done");
        }
    }
} 