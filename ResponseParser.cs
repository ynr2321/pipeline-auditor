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

        // PIPELINES  ------------------------------------------------------------------------------------------------
        public List<string> GetPipelineIds(string rawResponseBody)
        // Input: raw response body from 'LIST pipelines' api call
        {
            List<string> PipelineIds = new List<string>();

            var parsedResponse = JsonConvert.DeserializeObject<ListPipelineResponse>(rawResponseBody)!;

            foreach (var pipeline in parsedResponse.value)
            {
                PipelineIds.Add(pipeline.id.ToString());
            }

            return PipelineIds;
        }
        public List<string> GetPipelineNames(string rawResponseBody)
            // Input: raw response body from 'LIST pipelines' api call
        {
            List<string> PipelineNames = new List<string>();

            var parsedResponse = JsonConvert.DeserializeObject<ListPipelineResponse>(rawResponseBody)!;

            foreach (var pipeline in parsedResponse.value)
            {
                PipelineNames.Add(pipeline.name.ToString());
            }

            return PipelineNames;
        }


        // RUNS ---------------------------------------------------------------------------------------------------
        public List<string> GetBuildIds(List<string> ListOf_ListRuns_Responses, int nthRun)
        // Input: a list of raw response bodies from the 'LIST runs' api call made for each pipeline
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
                    try
                    {
                        string buildId =
                            runList.value[nthRun].id.ToString(); // extracting buildId for nth pipeline in list
                        BuildIds.Add(buildId);
                    }
                    catch (System.ArgumentOutOfRangeException ex)
                    {
                        // use NoRunsExistYet for now as the buildId processing function recognises this
                        string buildId = "NoRunsExistYet";
                        BuildIds.Add(buildId);
                    }
                    catch (Exception ex)
                    {
                        string buildId = "NoRunsExistYet";
                        BuildIds.Add(buildId);
                    }
                  
                }
                
            }

            return BuildIds;
        }


        public List<Object> GetRunResponses(List<string> RunInfoResponses)
        // Input: list of 'GET' responses for specific runs
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
        // Input: rawResponse body from 'GET Run' api call
        {
            var issuesDict = new Dictionary<string, string>();

            Run parsedResponse = JsonConvert.DeserializeObject<Run>(rawResponseBody)!;
            // Extracting the records property from the Run
            List<Record> recordsList = new List<Record>();
            try
            {
                if (parsedResponse != null)
                {
                    recordsList = parsedResponse.Records;
                }
                else
                {
                    Record fakeRecord = new Record();
                    fakeRecord.warningCount = 0;
                    List<Record> fakeRecordsList = new List<Record>();
                    fakeRecordsList.Add(fakeRecord);

                    recordsList = fakeRecordsList;
                }
            }
            catch (System.NullReferenceException ex)
            {
                Record fakeRecord = new Record();
                fakeRecord.warningCount = 0;
                List<Record> fakeRecordsList = new List<Record>();
                fakeRecordsList.Add(fakeRecord);

                recordsList = fakeRecordsList;
            }

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


        public List<Object> GetAllWarnings(List<string> RunInfoResponses)
        
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






