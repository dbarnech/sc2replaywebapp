import json
import sys
import mpyq

archive = mpyq.MPQArchive(sys.argv[1])

metadata = archive.read_file("replay.gamemetadata.json")

metadataJson = metadata.decode("utf-8")
game_data = json.loads(metadataJson)

duration_seconds = int(game_data["Duration"] / 1.4)

winner = "Unknown"

players = []

for player in game_data["Players"]:

    won = player["Result"] == "Win"

    if won:
        winner = f"Player {player['PlayerID']}"

    players.append({
        "PlayerID": int(player['PlayerID']),
        "Name": f"Player {player['PlayerID']}",
        "APM": float(player["APM"]),
        "Result": player["Result"],
        "SelectedRace": player["SelectedRace"],
        "AssignedRace": player["AssignedRace"],
        "Won": won
    })


#print(f"{json.dumps(metadataJson)}")

result = {
    "Title": game_data["Title"],
    "GameVersion": game_data["GameVersion"],
    "DataBuild": game_data["DataBuild"],
    "DataVersion": game_data["DataVersion"],
    "BaseBuild": game_data["BaseBuild"],
    "Duration": int(game_data["Duration"]),
    "Winner": winner,
    "DurationSeconds": duration_seconds,
    "Players": players
}

print(json.dumps(result))

