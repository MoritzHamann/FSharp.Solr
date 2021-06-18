# Solr Accessor for F#

:warning: This is an alpha version! At this point no authentication is supported and
there is not a lot of HTTP connection settings missing.


## Client
To create a new Solr client instance, simply provide the base url endpoint:
```fsharp
open FSharp.Solr

let client = Client("http://localhost:8983/solr/")
```
The client class implements `FSharp.Solr.IClient` if any dependency injection is needed.

## Search
In order to search the Solr collection a `FSharp.Solr.Query.t` needs to be created. This is a
simple record, which can be build via a query builder
```fsharp
let query =
    Query.New
    |> Query.Query "*:*"                    // fills in the `q` parameter, alias: Query.Q, 
    |> Query.Filter "someField:someValue"   // fills in the `fq` parameter, alias: Query.Fq,
    |> Query.Field "someFieldName"          // fills in the `fl` parameter, alias: Query.Fl,
    |> Query.Rows 10u                       // fils in the `rows` parameter
    |> QUery.Custom "key" "value"           // adds a key=value to the query

let result = client.Search(query, Async.DefaultCancellationToken)        // return a Task<ClientResult>
let result = client.SearchAsync(query, Async.DefaultCancellationToken)   // return a Async<ClientResult>
```

The return value of `client.Search()` is a `ClientResult`, defined as `Result<QueryResult, ClientError>`.
In case of a success, a `QueryResult` is returned, which is (at this point) defined as
```fsharp
type QueryResult = {
    Raw: string;    // raw respone returned by SOLR
}
```

In case of an error, a `ClientError` record is returned. This record is defined as 
```fsharp
type ClientError = {
    Status: HttpStatusCode;     // HTTP status code
    Message: string;            // HTTP response content
}
```