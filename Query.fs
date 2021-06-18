namespace FSharp.Solr

open System

module Query =
    type t = {
        q: string;
        fq: string list;
        fl: string list;
        rows: uint;
        custom: Map<string, string>
    }

    let New = {
        q = "*:*";
        fq = [];
        fl = [];
        rows = 10u;
        custom = Map.empty;
    }

    let Query scoreQuery query = {query with q = scoreQuery}
    let Q scoreQuery query = Query scoreQuery query

    let Filter filter query = {query with fq = filter::query.fq}
    let Fq fq t = Filter fq t

    let Field field query = {query with fl = field::query.fl}
    let Fl field query = Field field query

    let Rows rows query = {query with rows = rows}

    let Custom field value query = {query with custom = Map.add field value query.custom}

    let ToString t =
        let content = Web.HttpUtility.ParseQueryString("")
        content.["q"] <- t.q
        for filter in t.fq do
            content.["fq"] <- filter
        if List.length t.fl > 0 then
            content.["fl"] <- String.Join(",", t.fl)
        content.["rows"] <- t.rows.ToString()

        t.custom |> Map.iter (fun key value -> content.[key] <- value)

        content.ToString()
