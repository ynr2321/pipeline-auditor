using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using HtmlAgilityPack;
using pipelineAuditor.Models;

namespace pipelineAuditor
{
    public class ResponseParser
    {
        public ResponseParser()
        {
        }

        public List<string> GetPipelineIds(string rawResponseBody)
        {
            List<string> PipelineIds = new List<string>();

            var parsedResponse = JsonConvert.DeserializeObject<ListPipelineResponse>(rawResponseBody)!;

            foreach (var pipeline in parsedResponse.value)
            {
                PipelineIds.Add(pipeline.id.ToString());
            }

            return PipelineIds;
        }


        public List<string> GetBuildIds(List<string> ListOf_ListRuns_Responses)
        {
            List<string> BuildIds = new List<string>();

            foreach (string response in ListOf_ListRuns_Responses)
            {
                RunList runList = JsonConvert.DeserializeObject<RunList>(response); // deserializing
               
                if (runList.count == 0 || runList.count == null)
                {
                    string buildId = "NoRunsExistYet";
                    BuildIds.Add(buildId);
                }
                else
                {
                  string buildId = runList.value[0].id.ToString(); // extracting buildId for first pipeline in list
                  BuildIds.Add(buildId);
                }
                
            }

            return BuildIds;
        }

        public List<Object> GetRunResponses(List<string> RunInfoResponses)
        {
            List<Object> parsedResponses = new List<Object>();

            foreach (string response in RunInfoResponses)
            {
                if (response != "NoRunsExistYet")
                {
                    try
                    {
                        Run run = JsonConvert.DeserializeObject<Run>(response);
                        parsedResponses.Add(run);
                    }
                    catch (Exception ex)
                    {
                        string message = $"Could not deserialize response to GET run api call: {ex.Message}";
                        parsedResponses.Add(message);
                    }
        
                }
                else
                {
                    string run = "NoRunsExistYet";
                    parsedResponses.Add(run);
                }
            }

            return parsedResponses;
        }
      

        public object GetIssuesDict(string rawResponseBody)
        {
            var issuesDict = new Dictionary<string, string>();

            Run parsedResponse = JsonConvert.DeserializeObject<Run>(rawResponseBody)!;
            // Extracting the records property from the Run
            var recordsList = parsedResponse.Records;
            // Extracting the task name and warning message for tasks with warnings
            foreach (var record in recordsList)
            {
                if (record.warningCount > 0)
                {
                    issuesDict.Add(record.name, record.issues[0].Message.ToString());
                }
            }

            return issuesDict;
        }


        public List<Object> GetAllIssues(List<string> RunInfoResponses)
        {
            List<Object> List = new List<Object>();

            foreach (string response in RunInfoResponses)
            {
                if (response != "NoRunsExistYet")
                {
                    try
                    {
                        object dict = GetIssuesDict(response);
                        List.Add(dict);
                    }
                    catch (Exception ex)
                    {
                        string message = $"Could not deserialize response to GET run api call: {ex.Message}";
                        List.Add(message);
                    }

                }
                else
                {
                    string run = "NoRunsExistYet";
                    List.Add(run);
                }
            }

            return List;
        }
    }
}












// Filtering properties of the 'run' object to select 'records' (of tasks) that contain issues
//var issues = parsedResponse.Records
//    .Where(r => r.Issues != null)
//    .Select(r => r.Issues!.Select(i => new
//    {
//        TaskName = r.Name,
//        Type = i.Type,
//        Message = i.Message
//    }))
//    .SelectMany(r => r)
//    .ToList();