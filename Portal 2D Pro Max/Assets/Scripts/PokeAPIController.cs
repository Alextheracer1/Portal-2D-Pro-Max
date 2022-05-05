using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class PokeAPIController : MonoBehaviour
{
    
    public TextMeshProUGUI pokeNameText, pokeNumText;
  

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    private void Start()
    {
       

        pokeNameText.text = "";
        pokeNumText.text = "";
        
    }

    public void OnButtonRandomPokemon()
    {
        int randomPokeIndex = Random.Range(1, 808); // Min: inclusive, Max: exclusive

        pokeNameText.text = "Loading...";
        pokeNumText.text = "#" + randomPokeIndex;

     
        StartCoroutine(GetPokemonAtIndex(randomPokeIndex));
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        // Get Pokemon Info

        string pokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();
        // Example URL: https://pokeapi.co/api/v2/pokemon/151

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string[pokeTypes.Count];

        for (int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        // Get Pokemon Sprite

        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }
            
        // Set UI Objects
        
        pokeNameText.text = CapitalizeFirstLetter(pokeName);

 
        
    }

    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
