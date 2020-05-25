namespace November.Models

open System
open November.IssueApi

module Domain =
    type ModelName =
        | BERT
        | LogisticRegression
        | All
    
    type Approach =
        | RuleBased
        | MachineLearning
        | DeepLearning
    
    [<CLIMutable>] 
    type ModelParameters = {
        Sentences: string list
    }
    with
        static member from(sentences: string list) =
            {
                Sentences = sentences
            } 
    
    [<CLIMutable>]
    type ClassifiedSentence = {
        Approach: Approach
        Content: string
        IsIssue: bool
        Likelihood: float
    }
    with
        static member from(result: ClassificationResult): ClassifiedSentence =
            {
                Approach =
                    match result.ClassifiedBy with
                    | "RuleBased" -> RuleBased
                    | "MachineLearning" -> MachineLearning
                    | "DeepLearning" -> DeepLearning
                    | _ -> "" |> exn |> raise
                Content = result.Sentence
                IsIssue = result.IsIssue
                Likelihood = result.Likelihood
            }
    
    [<CLIMutable>]
    type TimeClassificationReport = {
        PreprocessTime: DateTimeOffset
        FullTime: DateTimeOffset
    }
    with
        static member from(predictionResult: PredictionResponse) =
            {
                PreprocessTime = predictionResult.PreprocessTimeUnix |> DateTimeOffset.FromUnixTimeSeconds
                FullTime = predictionResult.FullTimeUnix |> DateTimeOffset.FromUnixTimeSeconds
            }
            
    [<CLIMutable>]
    type ClassificationReport = {
       ModelName: ModelName
       Sentences: ClassifiedSentence list
       TimeReport: TimeClassificationReport
    }
    with

        static member from(predictionResult: PredictionResponse): ClassificationReport =
            {
                ModelName =
                    match predictionResult.ModelName with
                    | "BERT" -> BERT
                    | "LogisticRegression" -> LogisticRegression
                    | "All" -> All
                    | _ -> "" |> exn |> raise
                
                Sentences = predictionResult.ClassifiedSentences |> List.map ClassifiedSentence.from
                TimeReport = TimeClassificationReport.from predictionResult
            }
