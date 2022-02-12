# The Defender
A mobile location-based AR shooting game with a Pokémon Go style map.

Demo video & details are available at: https://guizichen.wixsite.com/portfolio/ar-game


## Code Samples
(Detailed code explanations are included in the comments)

1. [**HealthMultiplayer.cs**](https://github.com/Gavin-Guiii/The_Defender/blob/main/Multiplayer/HealthMultiplayer.cs) <br>It is the base Health class in the multiplayer environment. 
   <br>[**EnemyMultiplayer**](https://github.com/Gavin-Guiii/The_Defender/blob/main/Multiplayer/EnemyMultiplayer.cs) and [**PlayerHealthMultiplayer**](https://github.com/Gavin-Guiii/The_Defender/blob/main/Multiplayer/PlayerHealthMultiplayer.cs) inherit from it.



2. [**PlaceData.cs**](https://github.com/Gavin-Guiii/The_Defender/blob/main/POI/PlaceData.cs) <br> Places of interest (POIs) are represented by a serializable class so that it can be stored in a JSON file. POI data includes both game-related parameters and real-world coordinates so that they can be pinned on the map.
   <br>[**POIDataHelper**](https://github.com/Gavin-Guiii/The_Defender/blob/main/POI/POIDataHelper.cs) handles reading/writing of the JSON file, and [**FactoryListItem**](https://github.com/Gavin-Guiii/The_Defender/blob/main/POI/FactoryListItem.cs) displays the list of all POIs.



