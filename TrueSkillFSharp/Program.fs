// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open TrueSkill
open Parser
open Structures

let prettyPrintGame game = 
    printfn "%A played %A on %A (%A-%A)" game.Team1Name game.Team2Name game.Date.ToShortDateString game.Team1Score game.Team2Score

let prettyPrintTeam (team:Team) =
    printfn "%A (%A %A) (%A %A)" team.Name team.Wins team.Losses team.EstimatedSkill team.SkillVariance

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    let games = parseGamesList "2015-2016_games.csv"

  //  games
    //|> Seq.iter prettyPrintGame

    let mutable teams =
        games 
        |> Seq.collect (fun y -> seq {yield y.Team1Name; yield y.Team2Name})
        |> Seq.distinct
        |> Seq.map (fun n -> new Team(n))
        |> Seq.toList

    inferTeamRankings games teams

    teams 
        |> Seq.sortBy (fun t -> -t.EstimatedSkill)
        |> Seq.iter prettyPrintTeam


    ignore (System.IO.Directory.CreateDirectory(@"C:\temp\trueskillNCAA\"))

    System.IO.File.WriteAllLines( @"C:\temp\trueskillNCAA\fu.txt", (
    teams 
        |> Seq.sortBy (fun t -> -t.EstimatedSkill)
        |> Seq.map (fun team -> team.Name + " (" + team.Wins.ToString() + "-" + team.Losses.ToString() + ") " + team.EstimatedSkill.ToString() + "," + team.SkillVariance.ToString())
        |> Seq.toArray)
       )

//    let c = 
//        teams 
//        |> Seq.toArray
//        |> Array.length
    
    0 // return an integer exit code
