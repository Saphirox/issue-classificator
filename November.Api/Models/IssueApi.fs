namespace November

module IssueApi =
    
    [<CLIMutable>]
    type PredictionRequest = {
        ModelName: string
        Sentences: string array
    }
    
    [<CLIMutable>]
    type PredictionResponse = {
        ModelName: string
        ClassifiedSentences: ClassificationResult list
        PreprocessTimeUnix: int64
        FullTimeUnix: int64
    }
    and [<CLIMutable>] ClassificationResult = {
        Sentence: string
        IsIssue: bool
        Likelihood: float
        ClassifiedBy: string
    }
    
