module Parser

open Structures
open System.IO
open System

let parseSingleGame (line: string) = 
    let values = line.Split [|','|]
    {Date = DateTime.Parse(values.[0]); Team1Name = values.[1]; Team2Name = values.[2]; Team1Score = Int32.Parse(values.[3]); Team2Score = Int32.Parse(values.[4]) }

let parseGamesList fileName =
    File.ReadAllLines fileName 
    |> Seq.skip 1
    |> Seq.map parseSingleGame
