module Structures

open System

type Team(name:string) = 
    member val Name = name
    member val Wins = 0 with get, set
    member val Losses = 0 with get, set
    member val EstimatedSkill = 25. with get, set
    member val SkillVariance = 10. with get, set
    

type Game = {
    Date: DateTime;
    Team1Name: string;
    Team2Name: string;
    Team1Score: int;
    Team2Score: int;
}