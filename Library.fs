namespace FSharp.Solr

open System
open System.Net
open System.Threading
open FSharp.Control.Tasks.V2.ContextInsensitive


type ClientError = {
    Status: HttpStatusCode;
    Message: string;
}
type QueryResult = {
    Raw: string;
}
type ClientResult = Result<QueryResult, ClientError>

type IClient = 
    abstract member Search: Query.t * CancellationToken -> Tasks.Task<ClientResult>
    abstract member SearchAsync: Query.t * CancellationToken -> Async<ClientResult>

type Client(url: string) =
    let client = new Http.HttpClient()
    let baseUrl = Uri(url.TrimEnd('/') + "/")

    // Helper function to enable interface
    member this.Search(query, token) = (this :> IClient).Search(query, token)
    member this.SearchAsync(query, token) = (this :> IClient).SearchAsync(query, token)

    interface IClient with
        member this.Search(query: Query.t, token: CancellationToken) =
            let endpoint = UriBuilder(Uri(baseUrl, "select"))
            endpoint.Query <- Query.ToString query
            task {
                let! resp = client.GetAsync(endpoint.Uri, token)
                let! content = resp.Content.ReadAsStringAsync()
                match resp.IsSuccessStatusCode with
                | false ->
                    return Error {
                        Status = resp.StatusCode;
                        Message = content;
                    }
                | true -> return Ok { Raw = content }
            }
            

        member this.SearchAsync(query: Query.t,  token: CancellationToken) =
            this.Search(query, token) |> Async.AwaitTask