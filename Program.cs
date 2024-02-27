using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pipelineAuditor.Models;

// NEW: https://elektasoftwarefactory.visualstudio.com/TSM/_build/results?buildId=111281&view=results
// example pipelineId = 1608 (PackageChecker)
// example buildId = 111281


namespace pipelineAuditor
{
    public class Program
    {
        // Fields ========================================================================================================================================
        private static string PersonalAccessToken = ConfigurationManager.AppSettings["ApiKey"]; // read-only token created for the pipeline-auditor
        private static string Org = ConfigurationManager.AppSettings["Organisation"];
        private static string Project = ConfigurationManager.AppSettings["Project"];

        private static string runInfoEndpoint =
            "https://dev.azure.com/elektasoftwarefactory/TSM/_apis/build/builds/111281/timeline?api-version=7.1";

        private static string runListEndpoint =
            "https://dev.azure.com/elektasoftwarefactory/TSM/_apis/pipelines/1608/runs?api-version=7.1";

        private static string pipelineListEndpoint =
            "https://dev.azure.com/elektasoftwarefactory/TSM/_apis/pipelines?api-version=7.1";

        // MAIN ========================================================================================================================================
        public static void Main(string[] args)
        {
            // Tool classes
            ResponseParser Parser = new ResponseParser();
            RestApiClient Client = new RestApiClient(PersonalAccessToken);
            EndpointGenerator EPG = new EndpointGenerator();

            // Initialising space to store responses
            List<string> RunMetaDataResponses = new List<string>();
            List<string> RunInfoResponses = new List<string>();


            // getting list of pipelines
            var listpipelineResponse = Client.GetResponse(pipelineListEndpoint).Result;
            List<string> pipelineList = Parser.GetPipelineIds(listpipelineResponse);

            // getting endpoint to list runs for each pipeline
            List<string> RunMetaDataEpList = EPG.GetListRunsEps(pipelineList);

            // getting metadata response for each pipeline's latest run
            foreach (var EndPoint in RunMetaDataEpList)
            {
                RunMetaDataResponses.Add(Client.GetResponse(EndPoint).Result);
            }

            // extracting buildIds for latest runs
            var BuildIds = Parser.GetBuildIds(RunMetaDataResponses);

            // getting end points to list records (task info) for each run
            var RunInfoEndPoints = EPG.GetRunInfoEps(BuildIds);

            // getting run info response (containing count and records) for each run
            foreach (var EndPoint in RunInfoEndPoints)
            {
                if (EndPoint != "NoRunsExistYet")
                {
                    RunInfoResponses.Add(Client.GetResponse(EndPoint).Result);
                }
                else
                {
                    RunInfoResponses.Add("NoRunsExistYet");
                }
            }

            var LatestRunsParsed = Parser.GetRunResponses(RunInfoResponses);

            var AllWarnings = Parser.GetAllWarnings(RunInfoResponses);

            Console.WriteLine("Done");



           // // TESTING ON PACKAGE CHECKER PIPELINE

           // // Getting warnings
           // var response_string = Client.GetResponse(runInfoEndpoint).Result;
           //// var parsedResponse = JsonConvert.DeserializeObject<Run>(response_string);
           // var issues_dict = Parser.GetIssuesDict(response_string);

        }
    }
} 