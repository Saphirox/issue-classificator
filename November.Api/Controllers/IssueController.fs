namespace November.Api.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open November
open November.Api.Models.View
open November.Api.Services
open November.Models.Domain

[<ApiController>]
[<Route("issues")>]
type IssueController(logger: ILogger<IssueController>, tokenizeService: TokenizeService, issueService: IssueService) =
    inherit ControllerBase()
    
    [<HttpPost>]
    [<Route("classify")>]
    member this.Classify(view: UserContentView): Async<ActionResult> = async {
        let pipeline =
            view.Text
            |> tokenizeService.Tokenize
            |> ModelParameters.from
            |> issueService.Classify
          
        match! pipeline with
        | Ok result -> return this.Ok(result) :> _
        | Error NoResult ->
            logger.LogInformation("Service does not return any result")
            return this.BadRequest "Service has returned no result" :> _
        | Error(ServiceError ex) ->
            logger.LogError(ex, "Exception occured")
            return this.Problem() :> _
    }