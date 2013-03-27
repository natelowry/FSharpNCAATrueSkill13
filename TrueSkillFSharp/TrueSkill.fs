module TrueSkill

open Structures


open MicrosoftResearch.Infer.Fun.FSharp.Syntax
open MicrosoftResearch.Infer.Fun.FSharp.Inference

open MicrosoftResearch.Infer
open MicrosoftResearch.Infer.Models
open MicrosoftResearch.Infer.Distributions
open MicrosoftResearch.Infer.Maths

let beta = 2.0
let ie = InferenceEngine()
ie.ShowProgress <- false

let inferTeamRankings (games:seq<Game>) (teams:List<Team>) =
    games 
    |> Seq.sortBy (fun g -> g.Date)
    |> Seq.iteri
        (fun i game -> 
            System.Diagnostics.Debug.Assert (game.Team1Score > game.Team2Score)
            let winningTeam = teams |> List.find (fun t -> t.Name = game.Team1Name)
            let losingTeam = teams |> List.find (fun t -> t.Name = game.Team2Name)

            let winningTeamPreviousSkill = Variable.GaussianFromMeanAndVariance(winningTeam.EstimatedSkill, winningTeam.SkillVariance)
            let losingTeamPreviousSkill = Variable.GaussianFromMeanAndVariance(losingTeam.EstimatedSkill, losingTeam.SkillVariance)

            let winningTeamPerformance = Variable.GaussianFromMeanAndVariance(winningTeamPreviousSkill,beta)
            let losingTeamPerformance = Variable.GaussianFromMeanAndVariance(losingTeamPreviousSkill,beta)

            do Variable.ConstrainPositive(winningTeamPerformance - losingTeamPerformance)
            
            let winningTeamInferredPerformance = ie.Infer<Gaussian> winningTeamPerformance
            let losingTeamInferredPerformance = ie.Infer<Gaussian> losingTeamPerformance

            winningTeam.EstimatedSkill <- winningTeamInferredPerformance.GetMean()
            winningTeam.SkillVariance <- winningTeamInferredPerformance.GetVariance()
            
            losingTeam.EstimatedSkill <- losingTeamInferredPerformance.GetMean()
            losingTeam.SkillVariance <- losingTeamInferredPerformance.GetVariance()

            winningTeam.Wins <- winningTeam.Wins + 1
            losingTeam.Losses <- losingTeam.Losses + 1

            if i % 200 = 0 then printfn "Done with %A" i
        )