using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Content;
using System.IO;

/*
 * Form : https://goo.gl/forms/jrC9oKh8EPMMdO2l2
 * 
 * Post URL: <form action="https://docs.google.com/forms/u/1/d/e/1FAIpQLSfiFPuQOTXJI54LI-WNvn1K6qCkM5xErxJdUUJRhCZthaIqcA/formResponse" target="_self" method="POST" id="mG61Hd">
 *
 */

class PublishedGameStats
{
    /* Google Form is waiting for the following datas 
     * 
     *    Scenario ID/name (automatic)
     *    Victory (manual)
     *    Your rating for this scenario (1-10)  (manual)
     *    Optional: Comments / issue report / suggestion (manual)
     *    Game duration (automatic)
     *    Number of players (automatic)
     *    List of investigators (automatic)
     *    List of Events activated (automatic)
     */

    public string scenario_name="";
    public string victory = "";
    public string rating = "";
    public string comments = "";
    public int duration = 0;
    public int players_count = 0;
    public string investigators_list = "";
    public string language_selected = "";
    public string events_list = "";


    public void Reset()
    {
        scenario_name = "";
        victory = "";
        rating = "";
        comments = "";
        duration = 0;
        players_count = 0;
        investigators_list = "";
        language_selected = "";
        events_list = "";
    }

}


class ScenarioStats
{
    /* We gather the following information for each scenario:
     *    scenario_name
     *    scenario_play_count
     *    scenario_avg_rating
     *    scenario_avg_duration
     *    scenario_avg_win_ratio 
     */

}

class GoogleFormPostman : MonoBehaviour
{
    private Uri uri = null;
    private WWWForm formFields = null;

    public void AddFormField(string name, string value)
    {
       if (formFields == null) formFields = new WWWForm();

        Debug.Log("AddFormField"+ name+":"+value);

        formFields.AddField(name, value);
    }

    public void SetURL(string url)
    {
        uri = new Uri(url);
    }

    public void PostFormAsync()
    {
        StartCoroutine(PostForm());
    }

    private IEnumerator PostForm()
    {
        UnityWebRequest www = UnityWebRequest.Post(uri, formFields);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            // Most probably a connection error
            Debug.Log("Error sending stats : most probably a connectivity issue (please check your internet connection)");
        }
        else if (www.isHttpError)
        {
            // Most probably a connection error
            Debug.Log("Error sending stats : most probably a connection error");
        }
        else
        {
            Debug.Log("stats sent");
            // success! thank you
        }
    }
}

// This class provides functions to load and save games.
class StatsManager
{
    static PublishedGameStats gameStats = null;
    private StringKey QUEST_NAME = new StringKey("val", "game.quest");


    public void PrepareStats(string victory, int rating, string comments)
    {
        if (gameStats==null) gameStats = new PublishedGameStats();

        gameStats.Reset();

        gameStats.victory = victory;

        if (rating == 0)
            gameStats.rating = "not set";
        else
            gameStats.rating = rating.ToString();

        gameStats.comments = comments;

        Game game = Game.Get();

        // quest filename is the unique id
        gameStats.scenario_name = Path.GetFileName(game.quest.questPath);

        // language is required to see the quality of translations
        gameStats.language_selected = game.currentLang;

        // Get number of heroes
        foreach (Quest.Hero h in game.quest.heroes)
        {
            if (h.heroData != null)
            {
                gameStats.players_count++;
                // remove leading 'hero' before hero name
                gameStats.investigators_list += h.heroData.sectionName.Remove(0,4) + ";";
            }
        }

        // Get the list of events
        if (game.quest.eventList != null)
        {
            foreach (string event_name in game.quest.eventList)
            {
                gameStats.events_list += event_name + ";";
            }
        }
        else
        {
            gameStats.events_list = "no event (old save?)";
        }

        // max cell size of Google sheet is 50k characters
        if (gameStats.events_list.Length > 50000)
        {
            gameStats.events_list.Remove( 88 + (gameStats.events_list.Length - 50000), 50000);
            gameStats.events_list = "---Beginning of event list is not included to avoid exceeding google sheet max size---" + gameStats.events_list;
        }

        // not available yet
        gameStats.duration = 0;
    }

    public void PublishData()
    {
        //Use WebClient Class to submit a new entry
        GameObject network = new GameObject("StatsManager");
        network.tag = Game.ENDGAME;
        //network.transform.SetParent(Game.Get().uICanvas.transform);
        GoogleFormPostman post_client = network.AddComponent<GoogleFormPostman>();

        post_client.AddFormField("entry.1875990408", gameStats.scenario_name);
        post_client.AddFormField("entry.84574628",   gameStats.victory);
        post_client.AddFormField("entry.227102998",  gameStats.rating);
        post_client.AddFormField("entry.2125749314", gameStats.comments);
        post_client.AddFormField("entry.170795919",  gameStats.duration.ToString());
        post_client.AddFormField("entry.376629889",  gameStats.players_count.ToString());
        post_client.AddFormField("entry.1150567176", gameStats.investigators_list);
        post_client.AddFormField("entry.2106598722", gameStats.language_selected);
        post_client.AddFormField("entry.1047979960", gameStats.events_list);

        post_client.SetURL("https://docs.google.com/forms/u/1/d/e/1FAIpQLSfiFPuQOTXJI54LI-WNvn1K6qCkM5xErxJdUUJRhCZthaIqcA/formResponse?hl=en");
        //post_client.SetURL("https://docs.google.com/forms/  d/e/1FAIpQLSfiFPuQOTXJI54LI-WNvn1K6qCkM5xErxJdUUJRhCZthaIqcA/formResponse");
        
        // submit:
        post_client.PostFormAsync();
    }
}
