import json
import sys
import mpyq

archive = mpyq.MPQArchive(sys.argv[1])

metadata = archive.read_file("replay.gamemetadata.json")

game_data = json.loads(metadata.decode("utf-8"))

duration_seconds = int(game_data["Duration"] / 1.4)

winner = "Unknown"

players = []

for player in game_data["Players"]:

    won = player["Result"] == "Win"

    if won:
        winner = f"Player {player['PlayerID']}"

    players.append({
        "Name": f"Player {player['PlayerID']}",
        "Race": player["AssignedRace"],
        "Won": won
    })

result = {
    "MapName": game_data["Title"],
    "Winner": winner,
    "DurationSeconds": duration_seconds,
    "RawDuration": int(game_data["Duration"]),
    "Players": players
}

print(json.dumps(result))