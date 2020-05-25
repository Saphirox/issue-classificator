namespace November

module Config =
    
    [<CLIMutable>]
    type IssueApiConfig = {
        Endpoint: string
    }