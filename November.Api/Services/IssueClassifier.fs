namespace November

open System.Net
open System.Net.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Newtonsoft.Json
open November.Config
open November.IssueApi
open November.Models.Domain

type IssueServiceError =
    | NoResult
    | ServiceError of exn

type IssueService(httpFactory: IHttpClientFactory, config: IOptions<IssueApiConfig>, logger: ILogger<IssueService>) =
    
    let readModelPredictions (response: HttpResponseMessage) = async {
        let! prediction =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask
            |> Async.map JsonConvert.DeserializeObject<PredictionResponse>
             
        match response.StatusCode with
        | HttpStatusCode.OK ->
            match prediction.ClassifiedSentences with
            | [ ] ->
                return Error NoResult
            | _ ->
                return Ok prediction
        | _ ->
            return int response.StatusCode
                   |> sprintf "Service returned unsuccessful result: %i"
                   |> exn
                   |> ServiceError
                   |> Error
    }
    
    member __.Classify(parameters: ModelParameters) =
        let content = new StringContent(JsonConvert.SerializeObject(parameters.Sentences),
                                        System.Text.Encoding.UTF8,
                                        "application/json")
    
        httpFactory.CreateClient().PostAsync(config.Value.Endpoint, content)
        |> Async.AwaitTask
        |> AsyncResult.tryExecute
        |> AsyncResult.mapError ServiceError
        |> AsyncResult.bind readModelPredictions
        |> AsyncResult.map ClassificationReport.from
    
    
    
        