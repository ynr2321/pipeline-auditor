using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using pipelineAuditor.Models;
using System.Configuration;

namespace pipelineAuditor
{
    public class EndpointGenerator
    {
        private static string Org = ConfigurationManager.AppSettings["Organisation"];
        private static string Project = ConfigurationManager.AppSettings["Project"];

        public EndpointGenerator()
        {
        }

        public List<string> GetListRunsEps(List<string> pipelineIds)
        // for a list of pipelineIds, returns list of endpoints to list runs for each pipeline
        {
            List<string> Endpoints = new List<string>();

            foreach (var pipelineId in pipelineIds)
            {
                Endpoints.Add($"https://dev.azure.com/{Org}/{Project}/_apis/pipelines/{pipelineId.ToString()}/runs?api-version=7.1");
            }

            return Endpoints;
        }
        public List<string> GetRunInfoEps(List<string> buildIds)
        {
            List<string> Endpoints = new List<string>();

            foreach (var buildId in buildIds)
            {
                if (buildId != "NoRunsExistYet")
                {
                    Endpoints.Add($"https://dev.azure.com/{Org}/{Project}/_apis/build/builds/{buildId.ToString()}/timeline?api-version=7.1");
                }
                else
                {
                    Endpoints.Add("NoRunsExistYet");
                }
    
            }

            return Endpoints;
        }

    }
}