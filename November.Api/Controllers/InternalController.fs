namespace November.Api.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open November
open November.Api.Services

[<ApiController>]
type InternalController(logger: ILogger<InternalController>, tokenizeService: TokenizeService, issueService: IssueService) =
    inherit ControllerBase()
    
    [<HttpGet>]
    [<Route("")>]
    member this.Index() = async {
        return this.Ok("It works") 
    }
