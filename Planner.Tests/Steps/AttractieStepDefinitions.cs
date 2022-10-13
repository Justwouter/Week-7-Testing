using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Xunit;
using TechTalk.SpecFlow;
using Planner.Tests.Hooks;
using Planner.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;

namespace Planner.Tests.Steps;

[Binding]
public sealed class AttractieStepDefinitions
{
    private readonly RestClient _client;
    private readonly DatabaseData _databaseData;
    private RestResponse? response;

    public AttractieStepDefinitions(DatabaseData databaseData)
    {
        _databaseData = databaseData;
        _client = new RestClient("http://localhost:5001/");

        // Het HTTPS certificaat hoeft niet gevalideerd te worden, dus return true
        ServicePointManager.ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) => true;
    }

    [Given("attractie (.*) bestaat")]
    public async Task AttractieBestaat(string naam)
    {
        await _databaseData.Context.Attractie.AddAsync(new Attractie {Naam = naam });
        await _databaseData.Context.SaveChangesAsync();
        Xunit.Assert.Equal(1, await _databaseData.Context.Attractie.CountAsync());
    }

    [Given("attractie (.*) exists")]
    public async Task AttractieBestaat(string[] args)
    {
        await _databaseData.Context.Attractie.AddAsync(new Attractie {Naam = args[0], Id = Convert.ToInt32(args[1]) });
        await _databaseData.Context.SaveChangesAsync();
        Xunit.Assert.Equal(1, await _databaseData.Context.Attractie.CountAsync());
    }

    [When("attractie (.*) wordt toegevoegd")]
    public async Task AttractieToevoegen(string naam)
    {
        var request = new RestRequest("api/Attracties").AddJsonBody(new { Naam = naam, Reserveringen = new List<string>() });
        response = await _client.ExecutePostAsync(request);
    }

    [Then("moet er een error (.*) komen")]
    public void Error(int httpCode)
    {
        Assert.Equal(httpCode, (int)response!.StatusCode);
    }

    [Given("attractie (.*) bestaat niet")]
    public async Task AttractieBestaatNiet(string naam)
    {
        var DBArray = await _databaseData.Context.Attractie.ToArrayAsync();
        for(int i=0; i< await _databaseData.Context.Attractie.CountAsync<Attractie>(); i++){
            if(DBArray[i].Naam == naam){
                _databaseData.Context.Attractie.Remove(DBArray[i]);
            }
        }
    }

    [When("attractie (.*) wordt verwijderd")]
    public async Task AttractieVerwijderd(string naam)
    {
        var DBArray = await _databaseData.Context.Attractie.ToArrayAsync();
        for(int i=0; i< await _databaseData.Context.Attractie.CountAsync<Attractie>(); i++){
            if(DBArray[i].Naam == naam){
                Xunit.Assert.Equal(DBArray[i].Naam,naam);
                var request = new RestRequest("api/Attracties/"+DBArray[i].Id);
                response = await _client.DeleteAsync(request);
                break;
            }
        }
        //Xunit.Assert.Equal("1",naam);
        response = await _client.DeleteAsync(new RestRequest("api/Attracties/"+ await _databaseData.Context.Attractie.CountAsync()+1));        
        
    }

    [When("attractie (.*) is deleted")]
    public async Task AttractieDeleted(string[] args)
    {
        var DBArray = await _databaseData.Context.Attractie.ToArrayAsync();
        var request = new RestRequest("api/Attracties/"+args[1]);
        response = await _client.DeleteAsync(request);   
        await _databaseData.Context.SaveChangesAsync();     
        // try{
        //     response = null;
        //     int counter = 0;
        //     for(int i=0; i< await _databaseData.Context.Attractie.CountAsync<Attractie>(); i++){
        //         if(DBArray[i].Naam == args[0]){
        //             Xunit.Assert.Equal(DBArray[i].Naam,args[0]);
        //             var request = new RestRequest("api/Attracties/"+args[1]);
        //             response = await _client.DeleteAsync(request);
        //             Assert.Equal(await _databaseData.Context.Attractie.CountAsync(), 0);
        //             Assert.Equal(200, (int)response!.StatusCode);
        //             //Xunit.Assert.NotEqual(response,response);
        //             break;   
        //         }
        //      counter++;
        //     }
        //     if(response == null){
        //         throw new Exception(counter.ToString());
        //     }

        // }
        // catch{
        //     response = await _client.DeleteAsync(new RestRequest("api/Attracties/"+ await _databaseData.Context.Attractie.CountAsync()+1));        
        //     throw new Exception();
        // }
    }

    [Then("moet er een (.*) code komen")]
    public void httpCode(int httpCode)
    {
        Debug.WriteLine(httpCode);
        Debug.WriteLine((int)response!.StatusCode);
        Console.WriteLine(httpCode);
        Assert.Equal(httpCode, (int)response!.StatusCode);
    }


    [StepArgumentTransformation]
    public string[] ConvertStringToArray(string argument)
    {
        return argument.Split(",");
    } 
    
}

class AttractieToegevoegd
{
    public int Id { get; set; }
}